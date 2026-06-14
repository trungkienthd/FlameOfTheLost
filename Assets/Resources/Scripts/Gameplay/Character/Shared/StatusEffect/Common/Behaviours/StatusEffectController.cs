using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(StatusEffectFlagHandler))]
    [RequireComponent(typeof(StatusEffectStatModifierHandler))]
    [RequireComponent(typeof(StatusEffectStatPeriodicHandler))]
    public class StatusEffectController : CharacterBehaviourControllerBase, ICharacterRefreshable
    {
        // --- States ---
        //  1. References
        //      1.1. Guard Configuration
        [Header("Data (SE Guard)")]
        [SerializeField] private StatusEffectGuardConfig _guardConfig;
        public StatusEffectGuardConfig GuardConfig => _guardConfig;

        //      1.2. Component Handlers
        private StatusEffectFlagHandler FlagHandler => this.CharacterRootController.StatusEffectFlagHandler;
        private StatusEffectStatModifierHandler StatModifierHandler => this.CharacterRootController.StatusEffectStatModifierHandler;
        private StatusEffectStatPeriodicHandler StatPeriodicHandler => this.CharacterRootController.StatusEffectStatPeriodicHandler;

        //      1.3. VFX References
        [Header("References (VFX)")]
        [SerializeField] private GameObject _statusEffectVFXInstancePrefab;
        private GameObject StatusEffectVFXInstancePrefab => _statusEffectVFXInstancePrefab;
        private Transform StatusEffectVFXHolder => this.CharacterRootController?.StatusEffectVFXHolder;

        private readonly Dictionary<uint, StatusEffectVFXHandler> _runtimeVFXMap = new();
        private IReadOnlyDictionary<uint, StatusEffectVFXHandler> RuntimeVFXMap => _runtimeVFXMap;

        //  2. Runtime state
        [Header("Runtime")]
        [SerializeField] private List<RuntimeStatusEffect> _runtimeStatusEffects = new();
        public IReadOnlyList<RuntimeStatusEffect> RuntimeStatusEffects => _runtimeStatusEffects;

        private uint _nextRuntimeID = 1;
        private uint NextRuntimeID
        {
            get => _nextRuntimeID;
            set => _nextRuntimeID = value;
        }


        // --- Events ---
        public event Action OnRuntimeListChanged;     // add/remove/clear/expire
        public event Action OnRuntimeValuesChanged;   // stacks/potency/duration refresh without list size change

        // --- Unity Callbacks ---
        protected override void Awake()
        {
            base.Awake();

            this.FlagHandler?.Bind(this);
            this.StatModifierHandler?.Bind(this);
            this.StatPeriodicHandler?.Bind(this);
        }
        private void Start()
        {
            this.SyncNextRuntimeID();

            // In case the runtime list was pre-seeded in editor/debug
            bool listChanged = this.EnforceImmunityRulesAndPurge();

            // Ensure VFX exists for any surviving pre-seeded runtime status effects
            for (int i = 0; i < this.RuntimeStatusEffects.Count; i++)
            {
                this.EnsureStatusEffectVFX(this.RuntimeStatusEffects[i]);
            }

            this.ForceRecomputeAll();

            if (listChanged)
            {
                this.OnRuntimeListChanged?.Invoke();
            }
        }
        private void Update()
        {
            float dt = Time.deltaTime;
            bool listIsChanged = false;

            // Tick duration & remove expired status effects
            for (int i = this.RuntimeStatusEffects.Count - 1; i >= 0; i--)
            {
                var runtimeStatusEffect = this.RuntimeStatusEffects[i];
                if (runtimeStatusEffect == null)
                {
                    if (this.RemoveRuntimeStatusEffectAt(i))
                    {
                        listIsChanged = true;
                    }
                    continue;
                }

                runtimeStatusEffect.RemainingDuration = Mathf.Max(0f, runtimeStatusEffect.RemainingDuration - dt);
                if (runtimeStatusEffect.RemainingDuration <= 0f)
                {
                    if (this.RemoveRuntimeStatusEffectAt(i))
                    {
                        listIsChanged = true;
                    }
                }
            }

            // If immunity is active, purge anything that should not exist under it
            if (this.EnforceImmunityRulesAndPurge())
            {
                listIsChanged = true;
            }

            if (listIsChanged)
            {
                this.OnRuntimeListChanged?.Invoke();
                this.ForceRecomputeAll();
            }
        }

        // --- Public APIs ---
        //  1. Apply or refresh a status effect; returns runtime id
        public uint ApplyStatusEffect(StatusEffectProfile profile, GameObject source, float potency = 1f)
        {
            if (profile == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.{nameof(StatusEffectController)}] Attempted to apply NULL profile.");
                return 0;
            }

            // Block if cannot apply (immunity flags etc.)
            if (!this.CanApply(profile))
            {
                Debug.Log($"[{this.CharacterName}.{nameof(StatusEffectController)}] BLOCKED - Cannot apply '{profile.name}' due to immunity or rules.");
                return 0;
            }

            // Find similar existing status effect(s)
            int idx = _runtimeStatusEffects.FindIndex(se => se != null && se.Profile != null && se.Profile.Id == profile.Id && se.Source == source);
            if (idx >= 0)
            {
                var runtimeStatusEffect = this.RuntimeStatusEffects[idx];
                if (runtimeStatusEffect == null)
                {
                    Debug.LogWarning($"[{this.CharacterName}.{nameof(StatusEffectController)}] Found index but runtimeStatusEffect is NULL.");
                    return 0;
                }

                switch (profile.DurationRefreshType)
                {
                    case StatusEffectDurationRefreshType.Extend:
                        // Extend duration and add a stack (clamped)
                        runtimeStatusEffect.CurrentStacks = Mathf.Clamp(runtimeStatusEffect.CurrentStacks + 1, 1, Mathf.Max(1, profile.MaxStacks));

                        // Extend remaining time, but cap it (simple predictable bound)
                        float maxDuration = profile.Duration * Mathf.Max(1, profile.MaxStacks);
                        runtimeStatusEffect.RemainingDuration = Mathf.Clamp(runtimeStatusEffect.RemainingDuration + profile.Duration, 0f, maxDuration);

                        //Debug.Log($"[{this.CharacterName}.{nameof(StatusEffectController)}] -> EXTEND applied | NewStacks={runtimeStatusEffect.CurrentStacks}, NewDuration={runtimeStatusEffect.RemainingDuration:F2}");
                        break;

                    case StatusEffectDurationRefreshType.Refresh:
                        runtimeStatusEffect.RemainingDuration = profile.Duration;

                        //Debug.Log($"[{this.CharacterName}.{nameof(StatusEffectController)}] -> REFRESH applied | Duration reset to {profile.Duration:F2}");
                        break;
                    case StatusEffectDurationRefreshType.Remain:
                        if (potency > runtimeStatusEffect.Potency)
                        {
                            runtimeStatusEffect.Potency = potency;
                            runtimeStatusEffect.RemainingDuration = profile.Duration;

                            //Debug.Log($"[{this.CharacterName}.{nameof(StatusEffectController)}] -> REMAIN (override) | NewPotency={potency:F2}, Duration reset");
                        }
                        else
                        {
                            //Debug.Log($"[{this.CharacterName}.{nameof(StatusEffectController)}] -> REMAIN (ignored) | Incoming potency {potency:F2} <= Current {runtimeStatusEffect.Potency:F2}");
                        }
                        break;
                }

                this.OnRuntimeValuesChanged?.Invoke();

                // If the refreshed effect grants immunity, it may need to purge others.
                bool purged = this.EnforceImmunityRulesAndPurge();
                if (purged)
                {
                    //Debug.Log($"[{this.CharacterName}.{nameof(StatusEffectController)}] Immunity purge triggered after update.");
                    this.OnRuntimeListChanged?.Invoke();
                }

                this.ForceRecomputeAll();

                bool stillExists = this.RuntimeStatusEffects.Any(se => se != null && se.RuntimeID == runtimeStatusEffect.RuntimeID);
                if (!stillExists)
                {
                    //Debug.Log($"[{this.CharacterName}.{nameof(StatusEffectController)}] REMOVED after recompute (RuntimeID={runtimeStatusEffect.RuntimeID})");
                    return 0;
                }

                this.EnsureStatusEffectVFX(runtimeStatusEffect);

                //Debug.Log
                //(
                //    $"[{this.CharacterName}.{nameof(StatusEffectController)}] FINAL (Updated EXISTING) '{profile.name}' | " +
                //    $"Stacks={runtimeStatusEffect.CurrentStacks} | " +
                //    $"Duration={runtimeStatusEffect.RemainingDuration:F2} | " +
                //    $"Potency={runtimeStatusEffect.Potency:F2}"
                //);

                return runtimeStatusEffect.RuntimeID;
            }

            // No existing status effect found; create new one
            RuntimeStatusEffect newStatusEffect = new RuntimeStatusEffect()
            {
                RuntimeID = this.NextRuntimeID++,
                Profile = profile,
                Source = source,
                Potency = Mathf.Max(0f, potency),
                CurrentStacks = 1,
                RemainingDuration = profile.Duration
            };
            _runtimeStatusEffects.Add(newStatusEffect);

            // New effect may be immunity => purge
            // New effect may be immunity => purge
            bool purgedNew = this.EnforceImmunityRulesAndPurge();
            if (purgedNew)
            {
                //Debug.Log($"[{this.CharacterName}.{nameof(StatusEffectController)}] Immunity purge triggered after NEW apply.");
            }

            this.OnRuntimeListChanged?.Invoke();
            this.ForceRecomputeAll();

            bool newStatusEffectStillExists = this.RuntimeStatusEffects.Any(se => se != null && se.RuntimeID == newStatusEffect.RuntimeID);
            if (!newStatusEffectStillExists)
            {
                //Debug.Log($"[{this.CharacterName}.{nameof(StatusEffectController)}] NEW effect immediately removed (RuntimeID={newStatusEffect.RuntimeID})");
                this.RemoveStatusEffectVFX(newStatusEffect.RuntimeID);
                return 0;
            }

            this.EnsureStatusEffectVFX(newStatusEffect);

            Debug.Log
            (
                $"[{this.CharacterName}.{nameof(StatusEffectController)}] FINAL (NEW applied) '{profile.name}' | " +
                $"(RuntimeID={newStatusEffect.RuntimeID}) | " +
                $"Stacks=1 | Duration={profile.Duration:F2} | Potency={newStatusEffect.Potency:F2}"
            );

            return newStatusEffect.RuntimeID;
        }

        //  2. Check for status effect by ID
        public bool HasStatusEffect(int statusEffectID)
        {
            return _runtimeStatusEffects.Exists(se => se != null && se.Profile != null && se.Profile.Id == statusEffectID);
        }

        //  3. Check if a status effect can be applied (immunity rules)
        public bool CanApply(StatusEffectProfile profile)
        {
            if (profile == null) return false;

            // If the incoming effect itself grants an immunity flag, always allow it.
            bool incomingIsImmunity =
                this.ProfileHasFlag(profile, StatusEffectFlagTarget.IgnoreAllStatusEffects)    ||
                this.ProfileHasFlag(profile, StatusEffectFlagTarget.IgnoreDebuffStatusEffects) ||
                this.ProfileHasFlag(profile, StatusEffectFlagTarget.IgnoreBuffStatusEffects);

            if (incomingIsImmunity) return true;

            // Non-immunity status effects
            //  If IgnoreAll is active, block anything that is not an immunity.
            if (this.HasActiveFlag(StatusEffectFlagTarget.IgnoreAllStatusEffects)) return false;

            //  Debuff/Buff immunity blocks new incoming effects based on Affinity
            bool ignoreDebuff   = this.HasActiveFlag(StatusEffectFlagTarget.IgnoreDebuffStatusEffects);
            bool ignoreBuff     = this.HasActiveFlag(StatusEffectFlagTarget.IgnoreBuffStatusEffects);

            //  Treat Mixed as both (conservative & consistent with "immune to debuffs/buffs being applied")
            if (ignoreDebuff    && (profile.Affinity == StatusEffectAffinityType.Debuff || profile.Affinity == StatusEffectAffinityType.Mixed)) return false;
            if (ignoreBuff      && (profile.Affinity == StatusEffectAffinityType.Buff   || profile.Affinity == StatusEffectAffinityType.Mixed)) return false;

            return true;
        }

        //  4. Remove Status Effect(s)
        public bool RemoveStatusEffectsFromSource(GameObject source)
        {
            if (source == null) return false;
            return this.RemoveAllFromSource(source);
        }

        //  5. ICharacterPlacementRefreshable implementation
        public int RefreshmentConfigurePriority => 20;
        public void RefreshCharacter(CharacterPlacementContext context)
        {
            this.RemoveAllStatusEffectVFXInstances();

            _runtimeStatusEffects.Clear();
            this.NextRuntimeID = 1;

            this.FlagHandler?.ClearAndApply();
            this.StatModifierHandler?.ClearAndApply();
            this.StatPeriodicHandler?.ClearAndApply();

            this.OnRuntimeListChanged?.Invoke();

            this.ForceRecomputeAll();
        }

        //  6. Expose consumer behaviours for accessing in handlers
        public MonoBehaviour[] GetStatusEffectConsumerBehaviours()
        {
            Transform searchRoot = this.CharacterRootController != null
                ? this.CharacterRootController.transform
                : this.transform.root;

            return searchRoot.GetComponentsInChildren<MonoBehaviour>(true);
        }

        // --- Helpers ---
        //  0. Runtime ID Sync (Start)
        private void SyncNextRuntimeID()
        {
            uint maxRuntimeID = 0;

            for (int i = 0; i < this.RuntimeStatusEffects.Count; i++)
            {
                RuntimeStatusEffect runtimeStatusEffect = this.RuntimeStatusEffects[i];
                if (runtimeStatusEffect == null) continue;

                if (runtimeStatusEffect.RuntimeID > maxRuntimeID)
                {
                    maxRuntimeID = runtimeStatusEffect.RuntimeID;
                }
            }

            this.NextRuntimeID = maxRuntimeID + 1;
        }

        //  1. Runtime SEs Removal
        private bool RemoveRuntimeStatusEffectAt(int index)
        {
            if (index < 0 || index >= this.RuntimeStatusEffects.Count) return false;

            RuntimeStatusEffect runtimeStatusEffect = this.RuntimeStatusEffects[index];
            uint runtimeID = runtimeStatusEffect != null ? runtimeStatusEffect.RuntimeID : 0;

            _runtimeStatusEffects.RemoveAt(index);

            if (runtimeID != 0)
            {
                this.RemoveStatusEffectVFX(runtimeID);
            }

            return true;
        }

        private bool RemoveByRuntimeID(uint runtimeID)
        {
            int idx = _runtimeStatusEffects.FindIndex(se => se.RuntimeID == runtimeID);
            if (idx < 0) return false;

            if (!this.RemoveRuntimeStatusEffectAt(idx)) return false;

            this.OnRuntimeListChanged?.Invoke();
            this.ForceRecomputeAll();
            return true;
        }

        private bool RemoveAllFromSource(GameObject source)
        {
            bool removedAny = false;

            for (int i = this.RuntimeStatusEffects.Count - 1; i >= 0; i--)
            {
                RuntimeStatusEffect runtimeStatusEffect = this.RuntimeStatusEffects[i];
                if (runtimeStatusEffect == null || runtimeStatusEffect.Source != source) continue;

                if (this.RemoveRuntimeStatusEffectAt(i))
                {
                    removedAny = true;
                }
            }

            if (!removedAny) return false;

            this.OnRuntimeListChanged?.Invoke();
            this.ForceRecomputeAll();
            return true;
        }

        private bool RemoveAll()
        {
            if (this.RuntimeStatusEffects.Count <= 0) return false;

            for (int i = this.RuntimeStatusEffects.Count - 1; i >= 0; i--)
            {
                this.RemoveRuntimeStatusEffectAt(i);
            }

            this.OnRuntimeListChanged?.Invoke();
            this.ForceRecomputeAll();
            return true;
        }

        //  2. Recompute
        private void ForceRecomputeAll()
        {
            this.FlagHandler?.Recompute();
            this.StatModifierHandler?.Recompute();
            this.StatPeriodicHandler?.Recompute();
        }

        //  3. SE Flag Immunity Rules
        private bool EnforceImmunityRulesAndPurge()
        {
            bool ignoreAll      = this.HasActiveFlag(StatusEffectFlagTarget.IgnoreAllStatusEffects);
            bool ignoreDebuff   = this.HasActiveFlag(StatusEffectFlagTarget.IgnoreDebuffStatusEffects);
            bool ignoreBuff     = this.HasActiveFlag(StatusEffectFlagTarget.IgnoreBuffStatusEffects);

            if (!ignoreAll && !ignoreDebuff && !ignoreBuff) return false;

            bool removedAny = false;
            for (int i = this.RuntimeStatusEffects.Count - 1; i >= 0; i--)
            {
                RuntimeStatusEffect runtimeStatusEffect = this.RuntimeStatusEffects[i];
                StatusEffectProfile profile = runtimeStatusEffect?.Profile;
                if (profile == null)
                {
                    if (this.RemoveRuntimeStatusEffectAt(i))
                    {
                        removedAny = true;
                    }
                    continue;
                }

                // Never purge the immunity providers themselves.
                bool isImmunityProvider =
                    this.ProfileHasFlag(profile, StatusEffectFlagTarget.IgnoreAllStatusEffects)      ||
                    this.ProfileHasFlag(profile, StatusEffectFlagTarget.IgnoreDebuffStatusEffects)   ||
                    this.ProfileHasFlag(profile, StatusEffectFlagTarget.IgnoreBuffStatusEffects);

                // Never purge the immunity providers themselves
                if (isImmunityProvider) continue;

                if (ignoreAll)
                {
                    if (this.RemoveRuntimeStatusEffectAt(i))
                    {
                        removedAny = true;
                    }
                    continue;
                }

                // Mixed is treated as both => purged by either side immunity.
                if (ignoreDebuff && (profile.Affinity == StatusEffectAffinityType.Debuff || profile.Affinity == StatusEffectAffinityType.Mixed))
                {
                    if (this.RemoveRuntimeStatusEffectAt(i))
                    {
                        removedAny = true;
                    }
                    continue;
                }

                if (ignoreBuff && (profile.Affinity == StatusEffectAffinityType.Buff || profile.Affinity == StatusEffectAffinityType.Mixed))
                {
                    if (this.RemoveRuntimeStatusEffectAt(i))
                    {
                        removedAny = true;
                    }
                    continue;
                }
            }    

            return removedAny;
        }

        private bool HasActiveFlag(StatusEffectFlagTarget flagType)
        {
            // Scan runtime list directly (no reliance on FlagHandler aggregation order).
            for (int i = 0; i < this.RuntimeStatusEffects.Count; i++)
            {
                var p = this.RuntimeStatusEffects[i]?.Profile;
                if (p == null) continue;

                if (this.ProfileHasFlag(p, flagType)) return true;
            }
            return false;
        }

        private bool ProfileHasFlag(StatusEffectProfile profile, StatusEffectFlagTarget flagType)
        {
            if (profile == null) return false;

            var flags = profile.Flags;
            if (flags == null) return false;

            for (int i = 0; i < flags.Count; i++)
            {
                var f = flags[i];
                if (f == null) continue;

                // If your contribution field name differs, rename here.
                if (f.Flag == flagType) return true;
            }

            return false;
        }

        //  4. SE VFX
        private void EnsureStatusEffectVFX(RuntimeStatusEffect runtimeStatusEffect)
        {
            if (runtimeStatusEffect == null || runtimeStatusEffect.Profile == null) return;
            if (this.StatusEffectVFXHolder == null || this.StatusEffectVFXInstancePrefab == null) return;

            if (this.RuntimeVFXMap.TryGetValue(runtimeStatusEffect.RuntimeID, out StatusEffectVFXHandler existingHandler))
            {
                if (existingHandler != null) return;
                _runtimeVFXMap.Remove(runtimeStatusEffect.RuntimeID);
            }

            GameObject instance = UnityEngine.Object.Instantiate(this.StatusEffectVFXInstancePrefab, this.StatusEffectVFXHolder);
            instance.name = $"StatusEffectVFX_{runtimeStatusEffect.Profile.name}_{runtimeStatusEffect.RuntimeID}";

            StatusEffectVFXHandler handler = instance.GetComponent<StatusEffectVFXHandler>();
            if (handler == null)
            {
                UnityEngine.Object.Destroy(instance);
                return;
            }

            handler.Initialize
            (
                runtimeStatusEffect.RuntimeID,
                runtimeStatusEffect.Profile,
                this.gameObject,
                this.CharacterRootController.VisualController.SpriteRenderer.sortingOrder
            );

            _runtimeVFXMap[runtimeStatusEffect.RuntimeID] = handler;
        }

        private void RemoveStatusEffectVFX(uint runtimeID)
        {
            if (!this.RuntimeVFXMap.TryGetValue(runtimeID, out StatusEffectVFXHandler handler)) return;

            if (handler != null)
            {
                UnityEngine.Object.Destroy(handler.gameObject);
            }

            _runtimeVFXMap.Remove(runtimeID);
        }

        private void RemoveAllStatusEffectVFXInstances()
        {
            foreach (var kvp in this.RuntimeVFXMap)
            {
                StatusEffectVFXHandler handler = kvp.Value;

                if (handler != null)
                {
                    UnityEngine.Object.Destroy(handler.gameObject);
                }
            }

            _runtimeVFXMap.Clear();
        }

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.StatusEffectVFXInstancePrefab == null) _statusEffectVFXInstancePrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/StatusEffectVFXInstance");
        }

        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.StatusEffectVFXHolder == null) Debug.LogError($"[{this.CharacterName}.{nameof(StatusEffectController)}] StatusEffectVFXHolder reference is missing. Status effect VFX will not be displayed.", this);
            if (this.StatusEffectVFXInstancePrefab == null) Debug.LogError($"[{this.CharacterName}.{nameof(StatusEffectController)}] StatusEffectVFXInstancePrefab reference is missing. Status effect VFX will not be displayed.", this);
        }
    }
}