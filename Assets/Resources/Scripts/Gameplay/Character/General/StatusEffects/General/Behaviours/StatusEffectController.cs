using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(StatusEffectFlagHandler))]
    [RequireComponent(typeof(StatusEffectStatModifierHandler))]
    [RequireComponent(typeof(StatusEffectStatPeriodicHandler))]
    public class StatusEffectController : MonoBehaviour
    {
        // --- Guard Configuration
        [Header("Configuration")]
        [SerializeField] private StatusEffectGuardConfig _guardConfig;
        public StatusEffectGuardConfig GuardConfig => _guardConfig;

        // --- Handler References
        [Header("Handler References")]
        [SerializeField] private StatusEffectFlagHandler _flagHandler;
        [SerializeField] private StatusEffectStatModifierHandler _statModifierHandler;
        [SerializeField] private StatusEffectStatPeriodicHandler _statPeriodicHandler;
        private StatusEffectFlagHandler FlagHandler
        {
            get => _flagHandler;
            set => _flagHandler = value;
        }
        private StatusEffectStatModifierHandler StatModifierHandler
        {
            get => _statModifierHandler;
            set => _statModifierHandler = value;
        }
        private StatusEffectStatPeriodicHandler StatPeriodicHandler
        {
            get => _statPeriodicHandler;
            set => _statPeriodicHandler = value;
        }

        // --- Runtime state
        [Header("Runtime")]
        [SerializeField] private readonly List<RuntimeStatusEffect> _runtimeStatusEffects = new();
        public List<RuntimeStatusEffect> RuntimeStatusEffects => _runtimeStatusEffects;

        private uint _nextRuntimeID = 1;
        private uint NextRuntimeID
        {
            get => _nextRuntimeID;
            set => _nextRuntimeID = value;
        }

        // --- Events
        public event Action OnRuntimeListChanged;     // add/remove/clear/expire
        public event Action OnRuntimeValuesChanged;   // stacks/potency/duration refresh without list size change

        // --- Unity methods
        private void Awake()
        {
            if (!this.FlagHandler)          this.FlagHandler            = this.GetComponent<StatusEffectFlagHandler>();
            if (!this.StatModifierHandler)  this.StatModifierHandler    = this.GetComponent<StatusEffectStatModifierHandler>();
            if (!this.StatPeriodicHandler)  this.StatPeriodicHandler    = this.GetComponent<StatusEffectStatPeriodicHandler>();

            this.FlagHandler?.Bind(this);
            this.StatModifierHandler?.Bind(this);
            this.StatPeriodicHandler?.Bind(this);

            // Enforce immunity rules once on startup (in case pre-seeded runtime list exists)
            this.EnforceImmunityRulesAndPurge();
            this.ForceRecomputeAll();
        }
        private void Update()
        {
            float dt = Time.deltaTime;

            // Tick duration & remove expired status effects
            bool listIsChanged = false;
            for (int i = this.RuntimeStatusEffects.Count - 1; i >= 0; i--)
            {
                var runtimeStatusEffect = this.RuntimeStatusEffects[i];
                runtimeStatusEffect.RemainingDuration = Mathf.Max(0f, runtimeStatusEffect.RemainingDuration - dt);
                if (runtimeStatusEffect.RemainingDuration <= 0f)
                {
                    this.RuntimeStatusEffects.RemoveAt(i);
                    listIsChanged = true;
                }
            }

            // If immunity is active, purge anything that should not exist under it.
            // This matters when an immunity exists and something was added externally,
            // or you later change affinity classification, etc.
            if (this.EnforceImmunityRulesAndPurge()) listIsChanged = true;

            if (listIsChanged)
            {
                this.OnRuntimeListChanged?.Invoke();
                this.ForceRecomputeAll();
            }
        }

        // --- Public API ---
        //  1. Apply or refresh a status effect; returns runtime id
        public uint ApplyStatusEffect(StatusEffectProfile profile, GameObject source, float potency = 1f)
        {
            if (profile == null) return 0;

            // Block if cannot apply (immunity flags etc.)
            if (!this.CanApply(profile)) return 0;

            // Find similar existing status effect(s)
            int idx = this.RuntimeStatusEffects.FindIndex(se => se.Profile.ID == profile.ID && se.Source == source);
            if (idx >= 0)
            {
                var runtimeStatusEffect = this.RuntimeStatusEffects[idx];

                switch (profile.DurationRefreshType)
                {
                    case StatusEffectDurationRefreshType.Extend:
                        // Extend duration and add a stack (clamped)
                        runtimeStatusEffect.CurrentStacks = Mathf.Clamp(runtimeStatusEffect.CurrentStacks + 1, 1, Mathf.Max(1, profile.MaxStacks));

                        // Extend remaining time, but cap it (simple predictable bound)
                        float maxDuration = profile.Duration * Mathf.Max(1, profile.MaxStacks);
                        runtimeStatusEffect.RemainingDuration = Mathf.Clamp(runtimeStatusEffect.RemainingDuration + profile.Duration, 0f, maxDuration);

                        break;

                    case StatusEffectDurationRefreshType.Refresh:
                        runtimeStatusEffect.RemainingDuration = profile.Duration;
                        break;
                    case StatusEffectDurationRefreshType.Remain:
                        if (potency > runtimeStatusEffect.Potency)
                        {
                            runtimeStatusEffect.Potency = potency;
                            runtimeStatusEffect.RemainingDuration = profile.Duration;
                        }
                        break;
                }

                this.OnRuntimeValuesChanged?.Invoke();

                // If the refreshed effect grants immunity, it may need to purge others.
                bool purged = this.EnforceImmunityRulesAndPurge();
                if (purged) this.OnRuntimeListChanged?.Invoke();

                this.ForceRecomputeAll();
                return runtimeStatusEffect.RuntimeID;
            }

            // No existing status effect found; create new one
            var newStatusEffect = new RuntimeStatusEffect()
            {
                RuntimeID = this.NextRuntimeID++,
                Profile = profile,
                Source = source,
                Potency = Mathf.Max(0f, potency),
                CurrentStacks = 1,
                RemainingDuration = profile.Duration
            };

            this.RuntimeStatusEffects.Add(newStatusEffect);

            // New effect may be immunity => purge
            this.EnforceImmunityRulesAndPurge();

            this.OnRuntimeListChanged?.Invoke();
            this.ForceRecomputeAll();

            bool stillExists = this.RuntimeStatusEffects.Exists(se => se.RuntimeID == newStatusEffect.RuntimeID);
            return stillExists ? newStatusEffect.RuntimeID : 0;
        }

        public bool RemoveByRuntimeID(uint runtimeID)
        {
            int idx = this.RuntimeStatusEffects.FindIndex(se => se.RuntimeID == runtimeID);
            if (idx < 0) return false;

            this.RuntimeStatusEffects.RemoveAt(idx);
            this.OnRuntimeListChanged?.Invoke();
            this.ForceRecomputeAll();
            return true;
        }

        public bool RemoveAllFromSource(GameObject source)
        {
            int initialCount = this.RuntimeStatusEffects.Count;
            this.RuntimeStatusEffects.RemoveAll(se => se.Source == source);
            if (this.RuntimeStatusEffects.Count >= initialCount) return false;

            this.OnRuntimeListChanged?.Invoke();
            this.ForceRecomputeAll();
            return true;
        }

        public bool RemoveAll()
        {
            if (this.RuntimeStatusEffects.Count <= 0) return false;

            this.RuntimeStatusEffects.Clear();
            this.OnRuntimeListChanged?.Invoke();
            this.ForceRecomputeAll();
            return true;
        }

        public bool HasStatusEffect(int statusEffectID)
        {
            return this.RuntimeStatusEffects.Exists(se => se.Profile.ID == statusEffectID);
        }

        public bool CanApply(StatusEffectProfile profile)
        {
            if (profile == null) return false;

            // If the incoming effect itself grants an immunity flag, always allow it.
            bool incomingIsImmunity =
                this.ProfileHasFlag(profile, StatusEffectFlagType.IgnoreAllStatusEffects)    ||
                this.ProfileHasFlag(profile, StatusEffectFlagType.IgnoreDebuffStatusEffects) ||
                this.ProfileHasFlag(profile, StatusEffectFlagType.IgnoreBuffStatusEffects);

            if (incomingIsImmunity) return true;

            // Non-immunity status effects
            //  If IgnoreAll is active, block anything that is not an immunity.
            if (this.HasActiveFlag(StatusEffectFlagType.IgnoreAllStatusEffects)) return false;

            //  Debuff/Buff immunity blocks new incoming effects based on Affinity
            bool ignoreDebuff   = this.HasActiveFlag(StatusEffectFlagType.IgnoreDebuffStatusEffects);
            bool ignoreBuff     = this.HasActiveFlag(StatusEffectFlagType.IgnoreBuffStatusEffects);

            //  Treat Mixed as both (conservative & consistent with "immune to debuffs/buffs being applied")
            if (ignoreDebuff    && (profile.Affinity == StatusEffectAffinityType.Debuff || profile.Affinity == StatusEffectAffinityType.Mixed)) return false;
            if (ignoreBuff      && (profile.Affinity == StatusEffectAffinityType.Buff   || profile.Affinity == StatusEffectAffinityType.Mixed)) return false;

            return true;
        }


        // --- Internal mechanics ---
        public void ForceRecomputeAll()
        {
            this.FlagHandler?.Recompute();
            this.StatModifierHandler?.Recompute();
            this.StatPeriodicHandler?.Recompute();
        }

        private bool EnforceImmunityRulesAndPurge()
        {
            bool ignoreAll      = this.HasActiveFlag(StatusEffectFlagType.IgnoreAllStatusEffects);
            bool ignoreDebuff   = this.HasActiveFlag(StatusEffectFlagType.IgnoreDebuffStatusEffects);
            bool ignoreBuff     = this.HasActiveFlag(StatusEffectFlagType.IgnoreBuffStatusEffects);

            if (!ignoreAll && !ignoreDebuff && !ignoreBuff) return false;

            bool removedAny = false;
            for (int i = this.RuntimeStatusEffects.Count - 1; i >= 0; i--)
            {
                var runtimeStatusEffect = this.RuntimeStatusEffects[i];
                var profile = runtimeStatusEffect?.Profile;
                if (profile == null)
                {
                    this.RuntimeStatusEffects.RemoveAt(i);
                    removedAny = true;
                    continue;
                }

                // Never purge the immunity providers themselves.
                bool isImmunityProvider =
                    this.ProfileHasFlag(profile, StatusEffectFlagType.IgnoreAllStatusEffects)      ||
                    this.ProfileHasFlag(profile, StatusEffectFlagType.IgnoreDebuffStatusEffects)   ||
                    this.ProfileHasFlag(profile, StatusEffectFlagType.IgnoreBuffStatusEffects);

                if (isImmunityProvider) continue;

                if (ignoreAll)
                {
                    this.RuntimeStatusEffects.RemoveAt(i);
                    removedAny = true;
                    continue;
                }

                // Mixed is treated as both => purged by either side immunity.
                if (ignoreDebuff && (profile.Affinity == StatusEffectAffinityType.Debuff || profile.Affinity == StatusEffectAffinityType.Mixed))
                {
                    this.RuntimeStatusEffects.RemoveAt(i);
                    removedAny = true;
                    continue;
                }

                if (ignoreBuff && (profile.Affinity == StatusEffectAffinityType.Buff || profile.Affinity == StatusEffectAffinityType.Mixed))
                {
                    this.RuntimeStatusEffects.RemoveAt(i);
                    removedAny = true;
                    continue;
                }
            }    

            return removedAny;
        }


        private bool HasActiveFlag(StatusEffectFlagType flagType)
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

        private bool ProfileHasFlag(StatusEffectProfile profile, StatusEffectFlagType flagType)
        {
            if (profile == null) return false;

            var flags = profile.Flags;
            if (flags == null) return false;

            for (int i = 0; i < flags.Count; i++)
            {
                var f = flags[i];
                if (f == null) continue;

                // If your contribution field name differs, rename here.
                if (f.Flag == flagType)
                    return true;
            }

            return false;
        }
    }
}