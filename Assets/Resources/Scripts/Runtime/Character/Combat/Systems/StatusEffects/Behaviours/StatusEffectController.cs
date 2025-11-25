using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class StatusEffectController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private StatusEffectSystemConfig _systemConfig;
        private StatusEffectSystemConfig SystemConfig => _systemConfig;

        [Header("References")]
        [SerializeField] private MovementHandler _movement;
        [SerializeField] private HealthController _health;
        [SerializeField] private CharacterResourcesController _resources;
        [SerializeField] private StatusEffectModifiersHandler _modifiersHandler;
        [SerializeField] private StatusEffectOverTimeHandler _overTimeHandler;
        private MovementHandler Movement
        {
            get => _movement;
            set => _movement = value;
        }
        private HealthController Health
        {
            get => _health;
            set => _health = value;
        }
        private CharacterResourcesController Resources
        {
            get => _resources;
            set => _resources = value;
        }
        private StatusEffectModifiersHandler ModifiersHandler
        {
            get => _modifiersHandler;
            set => _modifiersHandler = value;
        }
        private StatusEffectOverTimeHandler OverTimeHandler
        {
            get => _overTimeHandler;
            set => _overTimeHandler = value;
        }

        // Runtime state
        private readonly List<RuntimeStatusEffect> _active = new();
        private List<RuntimeStatusEffect> ActiveStatusEffects => _active;

        private uint _nextRuntimeID = 1;
        private uint NextRuntimeID
        {
            get => _nextRuntimeID;
            set => _nextRuntimeID = value;
        }

        private int _lastAppliedMaxHPDelta = 0;
        private int LastAppliedMaxHPDelta
        {
            get => _lastAppliedMaxHPDelta;
            set => _lastAppliedMaxHPDelta = value;
        }

        private void Awake()
        {
            if (this.Movement == null)  this.Movement   = this.GetComponent<MovementHandler>();
            if (this.Health == null)    this.Health     = this.GetComponent<HealthController>();
            if (this.Resources == null) this.Resources  = this.GetComponent<CharacterResourcesController>();

            if (this.ModifiersHandler == null) this.ModifiersHandler = this.gameObject.AddComponent<StatusEffectModifiersHandler>();
            if (this.OverTimeHandler == null) this.OverTimeHandler = this.gameObject.AddComponent<StatusEffectOverTimeHandler>();

            this.OverTimeHandler.Initialize(this.Health, this.SystemConfig);
            this.PushActiveListsToHandlers();
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            // Tick duration & remove expired status effects
            bool isChanged = false;
            for (int i = this.ActiveStatusEffects.Count - 1; i >= 0; i--)
            {
                var statusEffect = this.ActiveStatusEffects[i];
                statusEffect.RemainingDuration -= dt;
                if (statusEffect.RemainingDuration <= 0f)
                {
                    this.ActiveStatusEffects.RemoveAt(i);
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                this.Recompute();
            }
        }

        // --- Public API ---
        public uint ApplyStatusEffect(StatusEffectProfile profile, GameObject source, float potency = 1f)
        {
            if (profile == null) return 0;

            // Find similar existing status effect(s)
            int idx = this.ActiveStatusEffects.FindIndex(se => se.Profile.ID == profile.ID && se.Source == source);
            if (idx >= 0)
            {
                var existingStatusEffect = this.ActiveStatusEffects[idx];

                switch (profile.DurationRefreshType)
                {
                    case StatusEffectDurationRefreshType.Extend:
                        existingStatusEffect.CurrentStacks = Mathf.Min(existingStatusEffect.CurrentStacks + 1, Mathf.Max(1, profile.MaxStacks));
                        existingStatusEffect.InvalidateCache();
                        break;

                    case StatusEffectDurationRefreshType.Refresh:
                        existingStatusEffect.RemainingDuration = profile.Duration;
                        break;
                    case StatusEffectDurationRefreshType.DoNotChange:
                        if (potency > existingStatusEffect.Potency)
                        {
                            existingStatusEffect.Potency = potency;
                            existingStatusEffect.RemainingDuration = profile.Duration;
                            existingStatusEffect.InvalidateCache();
                        }
                        break;
                }

                this.Recompute();
                return existingStatusEffect.RuntimeID;
            }
            else
            {
                var newStatusEffect = new RuntimeStatusEffect()
                {
                    RuntimeID = this.NextRuntimeID++,
                    Profile = profile,
                    Source = source,
                    Potency = Mathf.Max(0f, potency),
                    CurrentStacks = 1,
                    RemainingDuration = profile.Duration
                };

                this.ActiveStatusEffects.Add(newStatusEffect);
                this.Recompute();
                return newStatusEffect.RuntimeID;
            }
        }

        public bool RemoveByRuntimeID(uint runtimeID)
        {
            int idx = this.ActiveStatusEffects.FindIndex(se => se.RuntimeID == runtimeID);
            if (idx >= 0)
            {
                this.ActiveStatusEffects.RemoveAt(idx);
                this.Recompute();
                return true;
            }
            return false;
        }

        public bool RemoveAllFromSource(GameObject source)
        {
            int initialCount = this.ActiveStatusEffects.Count;
            this.ActiveStatusEffects.RemoveAll(se => se.Source == source);
            if (this.ActiveStatusEffects.Count < initialCount)
            {
                this.Recompute();
                return true;
            }
            return false;
        }

        public bool ClearAll()
        {
            if (this.ActiveStatusEffects.Count > 0)
            {
                this.ActiveStatusEffects.Clear();
                this.Recompute();
                return true;
            }
            return false;
        }

        public bool HasStatusEffect(int statusEffectID)
        {
            return this.ActiveStatusEffects.Exists(se => se.Profile.ID == statusEffectID);
        }

        // --- Query API for other systems ---
        public float GetMultiplier(StatusEffectModifierTargetType target)
        {
            return this.ModifiersHandler != null ? this.ModifiersHandler.GetMultiplier(target) : 1f;
        }
        public float GetAdditiveFlat(StatusEffectModifierTargetType target)
        {
            return this.ModifiersHandler != null ? this.ModifiersHandler.GetAdditiveFlat(target) : 0f;
        }

        // --- Helpers ---
        // Attacker-side outbound damage adjustment
        public float ApplyOutgoingDamageModifiers(float baseDamage)
        {
            float additiveFlat  = this.GetAdditiveFlat  (StatusEffectModifierTargetType.DamageDealt);
            float multiplier    = this.GetMultiplier    (StatusEffectModifierTargetType.DamageDealt);
            return Mathf.Max(0f, (baseDamage + additiveFlat) * multiplier);
        }

        // Defender-side inbound damage adjustment
        public float ApplyIncomingDamageModifiers(float baseDamage)
        {
            float additiveFlat  = this.GetAdditiveFlat  (StatusEffectModifierTargetType.DamageTaken);
            float multiplier    = this.GetMultiplier    (StatusEffectModifierTargetType.DamageTaken);
            return Mathf.Max(0f, (baseDamage + additiveFlat) * multiplier);
        }

        public float ApplyKnockbackTaken(float baseKnockbackForce)
        {
            float additiveFlat  = this.GetAdditiveFlat  (StatusEffectModifierTargetType.KnockbackTaken);
            float multiplier    = this.GetMultiplier    (StatusEffectModifierTargetType.KnockbackTaken);
            return Mathf.Max(0f, (baseKnockbackForce + additiveFlat) * multiplier);
        }

        public float ApplyCooldownScale(float baseCooldown)
        {
            float multiplier = this.GetMultiplier(StatusEffectModifierTargetType.CooldownScale);
            float cooldown = baseCooldown * multiplier;
            return this.SystemConfig ? StatusEffectUtility.ClampCooldown(cooldown, this.SystemConfig.MinCooldownSeconds) : cooldown;
        }

        public float GetMovementSpeedScale()
        {
            float multiplier = this.GetMultiplier(StatusEffectModifierTargetType.MovementSpeedScale);
            return this.SystemConfig ? StatusEffectUtility.ClampMovementSpeedScale(multiplier, this.SystemConfig.MinMovementSpeedScale, this.SystemConfig.MaxMovementSpeedScale) : multiplier;
        }

        public float GetAbilityCostScaleStamina()
        {
            return this.GetMultiplier(StatusEffectModifierTargetType.AbilityCostScaleStamina);
        }
        public float GetAbilityCostScaleFP()
        {
            return this.GetMultiplier(StatusEffectModifierTargetType.AbilityCostScaleFP);
        }

        public float GetStaminaRegenRateDelta()
        {
            return this.GetAdditiveFlat(StatusEffectModifierTargetType.StaminaRegenRateDelta);
        }
        public float GetFPRegenRateDelta()
        {
            return this.GetAdditiveFlat(StatusEffectModifierTargetType.FPRegenRateDelta);
        }

        public float GetStaminaRegenDelayDelta()
        {
            return this.GetAdditiveFlat(StatusEffectModifierTargetType.StaminaRegenDelayDelta);
        }
        public float GetFPRegenDelayDelta()
        {
            return this.GetAdditiveFlat(StatusEffectModifierTargetType.FPRegenDelayDelta);
        }

        public float GetMaxHealthDelta()
        {
            return this.GetAdditiveFlat(StatusEffectModifierTargetType.MaxHealthDelta);
        }

        // --- Internal mechanics ---
        private void Recompute()
        {
            int previousMaxHPDelta = this.LastAppliedMaxHPDelta;
            this.PushActiveListsToHandlers();

            if (this.Health == null) return;

            int newMaxHPDelta = Mathf.FloorToInt(this.GetMaxHealthDelta());
            if (newMaxHPDelta == previousMaxHPDelta) return;

            int baseMaxHP = this.Health.MaxHP;
        }

        private void PushActiveListsToHandlers()
        {
            this.ModifiersHandler?.SetActiveStatusEffects(this.ActiveStatusEffects);
            this.OverTimeHandler?.SetActiveStatusEffects(this.ActiveStatusEffects);
        }
    }
}