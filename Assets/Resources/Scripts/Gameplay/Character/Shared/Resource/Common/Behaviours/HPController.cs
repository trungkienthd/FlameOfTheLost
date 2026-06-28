using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class HPController : CharacterResourcePoolController, IDamageable, IVirtualHPGrantedConsumer
    {
        // --- States ---
        //  1. References
        private CharacterVisualController VisualController => this.CharacterRootController.VisualController;

        //  2. Status Effect Stat Modifier
        //      2.1. Damage Received Modifiers
        [Header("Damage Received Modifiers")]
        [SerializeField] private float _damageReceivedMultiplier = 1f;
        [SerializeField] private float _damageReceivedAdditiveFlat = 0f;
        public float DamageReceivedMultiplier
        {
            get => _damageReceivedMultiplier;
            private set => _damageReceivedMultiplier = Mathf.Max(0f, value);
        }
        public float DamageReceivedAdditiveFlat
        {
            get => _damageReceivedAdditiveFlat;
            private set => _damageReceivedAdditiveFlat = value;
        }

        //      2.2. Virtual HP: External damage affects this first before Current HP, however HP self-cost does not affect this.
        [Header("Virtual HP")]
        [SerializeField] private int _currentVirtualHP;
        [SerializeField] private int _maxVirtualHP;
        public int CurrentVirtualHP
        {
            get => _currentVirtualHP;
            private set => _currentVirtualHP = Mathf.Clamp(value, 0, this.MaxVirtualHP);
        }
        public int MaxVirtualHP
        {
            get => _maxVirtualHP;
            private set => _maxVirtualHP = Mathf.Clamp(value, 0, this.MaxValue);
        }

        //  3. Death-related States
        [Header("Death (Runtime)")]
        [SerializeField] private bool _isDead = false;
        [SerializeField] private CharacterDeathCauseEntry _lastDeathCause;
        public bool IsDead
        {
            get => _isDead;
            private set => _isDead = value;
        }
        public CharacterDeathCauseEntry LastDeathCause
        {
            get => _lastDeathCause;
            private set => _lastDeathCause = value;
        }

        //  4. Character Faction
        public CharacterFaction CharacterFaction => this.CharacterRootController.IdentityController.Faction;

        //  5. Status Effect Stat Periodic (Type)
        public override StatusEffectStatPeriodicTarget Target => StatusEffectStatPeriodicTarget.HP;

        // --- Events ---
        public event Action<int> OnDamageTaken;
        public event Action<int> OnHealed;
        public event Action<CharacterDeathCauseEntry> OnDeath;
        public event Action<int, int> OnVirtualHPChanged;

        // --- Public APIs ---
        //  1. Primarily spend HP for damage taking (Virtual HP is considered first)
        public void TakeDamage(int damageAmount, CharacterDeathCauseEntry deathCause)
        {
            if (this.IsDead) return;
            if (damageAmount <= 0) return;

            // Apply DamageReceived modifiers (external damage only)
            int effectiveDamage = this.ComputeEffectiveDamageReceived(damageAmount);
            if (effectiveDamage <= 0) return;

            int remainingDamageAmount = effectiveDamage;

            // a. Spend Virtual HP first
            if (this.CurrentVirtualHP > 0)
            {
                int absorbed = Mathf.Min(this.CurrentVirtualHP, remainingDamageAmount);
                this.CurrentVirtualHP -= absorbed;
                remainingDamageAmount -= absorbed;

                this.OnVirtualHPChanged?.Invoke(this.CurrentVirtualHP, this.MaxVirtualHP);
            }

            // b. apply remaining damage to real HP

            if (remainingDamageAmount > 0)
            {
                // Directly subtract because this is external damage, not self-cost.
                this.CurrentValue -= remainingDamageAmount;
                this.OnDamageTaken?.Invoke(remainingDamageAmount);
            }

            // c. death is calculated immediately, but visual still plays Hit first.
            bool died = this.TryMarkDead(deathCause, "External damage");

            this.VisualController?.PlayHitSequence(shouldPlayDieAfterHit: died);
        }

        //  2. Heal HP directly
        public void Heal(int healAmount)
        {
            if (this.IsDead) return;
            if (healAmount <= 0) return;

            this.Restore(healAmount);

            this.OnHealed?.Invoke(healAmount);
        }

        //  3. Kill will pass Virtual HP, and affect Current HP directly
        public void Kill(CharacterDeathCauseEntry deathCause)
        {
            if (this.IsDead) return;

            this.SetCurrentValue(0);

            bool died = this.TryMarkDead(deathCause, "Instant Kill");

            this.VisualController?.PlayHitSequence(shouldPlayDieAfterHit: died);
        }

        //  4. TrySpend override not to account for Virtual HP, but only Current HP (Value)
        public bool TrySpendCost(int baseCostValue, CharacterDeathCauseEntry deathCause)
        {
            if (this.IsDead) return false;
            if (this.IsDeathDisabled) return false;

            if (baseCostValue < 0) return false;
            if (baseCostValue == 0) return true;

            int effectiveCostValue = this.ComputeEffectiveCostValue(baseCostValue);
            this.CurrentValue -= effectiveCostValue;

            bool died = this.TryMarkDead(deathCause, "HP self-cost");

            this.VisualController?.PlaySelfCostSequence(shouldPlayDieAfterSelfCost: died);

            return true;
        }
        public override bool TrySpendCost(int baseCostValue)
        {
            if (!this.CanSpendCost(baseCostValue)) return false;

            return this.TrySpendCost
            (
                baseCostValue,
                CharacterDeathCauseGenerationUtility.Self
                (
                    selfCharacter: this.CharacterRootController,
                    sourceObject: this.gameObject,
                    reason: "HP self-cost"
                )
            );
        }

        //  5. Virtual HP handling
        //      5.1. Set Virtual HP granted from Status Effect Modifiers 
        public void SetVirtualHPGranted(int grantedMaxVirtualHP)
        {
            grantedMaxVirtualHP = Mathf.Max(0, grantedMaxVirtualHP);

            int oldMaxVirtualHP = this.MaxVirtualHP;
            int newMaxVirtualHP = grantedMaxVirtualHP;

            // Apply the same delta to remaining shield, regardless of sign
            int delta = newMaxVirtualHP - oldMaxVirtualHP;
            this.MaxVirtualHP = newMaxVirtualHP;

            // Important: if the max virtual value drops (SE expired), remaining (current) virtual value must change and clamp too
            this.CurrentVirtualHP = Mathf.Clamp(this.CurrentVirtualHP + delta, 0, this.MaxVirtualHP);

            this.OnVirtualHPChanged?.Invoke(this.CurrentVirtualHP, this.MaxVirtualHP);
        }

        //      5.2. Add virtual HP 
        public void RestoreVirtualHP(int amount)
        {
            if (amount <= 0) return;

            this.CurrentVirtualHP = Mathf.Clamp(this.CurrentVirtualHP + amount, 0, this.MaxVirtualHP);
            this.OnVirtualHPChanged?.Invoke(this.CurrentVirtualHP, this.MaxVirtualHP);
        }

        //  6. Status Efect Stat Modifiers hooks
        public override void SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTarget target, float multiplier)
        {
            switch (target)
            {
                case StatusEffectStatModifierTarget.DamageReceived:
                    this.DamageReceivedMultiplier = multiplier;
                    return;
                case StatusEffectStatModifierTarget.HPCost:
                    this.CostMultiplier = multiplier;
                    return;
                default:
                    return;
            }
        }
        public override void SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTarget target, float additiveFlat)
        {
            switch (target)
            {
                case StatusEffectStatModifierTarget.DamageReceived:
                    this.DamageReceivedAdditiveFlat = additiveFlat;
                    return;
                case StatusEffectStatModifierTarget.HPCost:
                    this.CostAdditiveFlat = additiveFlat;
                    return;
                default:
                    return;
            }
        }

        //  7. Refresh Pool for Placement
        public override void RefreshPoolAfterPlace(CharacterRefreshContext context)
        {
            base.RefreshPoolAfterPlace(context);

            this.IsDead = false;
            this.LastDeathCause = null;

            this.MaxVirtualHP = 0;
            this.CurrentVirtualHP = 0;

            this.OnVirtualHPChanged?.Invoke(this.CurrentVirtualHP, this.MaxVirtualHP);
        }

        // --- Helpers ---
        //  1. Compute the effective damage received (for TakeDamage only)
        private int ComputeEffectiveDamageReceived(int baseDamage)
        {
            if (baseDamage <= 0) return 0;

            float raw = baseDamage * this.DamageReceivedMultiplier + this.DamageReceivedAdditiveFlat;
            // clamp to non-negative, then round up (same style as cost)
            return Mathf.Max(0, Mathf.CeilToInt(raw));
        }

        private bool TryMarkDead(CharacterDeathCauseEntry deathCause, string fallbackReason = null)
        {
            if (this.IsDead) return true;
            if (this.CurrentValue > 0) return false;

            this.IsDead = true;

            this.LastDeathCause = deathCause ?? CharacterDeathCauseGenerationUtility.Unknown(fallbackReason ?? "Unknown death cause");

            this.OnDeath?.Invoke(this.LastDeathCause);

            //Debug.Log
            //(
            //    $"[{this.CharacterName}.{this.GetType().Name}] Entity is technically dead. " +
            //    $"CauseType: {this.LastDeathCause.CauseType}. " +
            //    $"SourceCharacter: {(this.LastDeathCause.SourceCharacter != null ? this.LastDeathCause.SourceCharacter.name : "None")}. " +
            //    $"Reason: {this.LastDeathCause.Reason}.",
            //    this
            //);

            return true;
        }

        //  4. Apply periodic damage according to policies
        private void ApplyPeriodicDamage(int amount, bool treatAsTrueDamage, bool bypassVirtualHP, RuntimeStatusEffect sourceStatusEffect)
        {
            if (amount <= 0) return;

            CharacterDeathCauseEntry deathCause = this.CreateDeathCauseFromStatusEffectSource(sourceStatusEffect);

            // Case A: Not true damage => interpret as HP self-cost.
            // Virtual HP is NEVER considered for self-cost, so bypass flag is irrelevant here.
            if (!treatAsTrueDamage)
            {
                this.TrySpendCost(amount, deathCause);     // HP cost only (modifiers apply)
                return;
            }

            // Case B: True damage => interpret as external damage.
            // Now bypass flag becomes meaningful.
            if (!bypassVirtualHP)
            {
                // True external damage, but still consumes Virtual HP first.
                int remaining = amount;

                if (this.CurrentVirtualHP > 0)
                {
                    int absorbed = Mathf.Min(this.CurrentVirtualHP, remaining);
                    this.CurrentVirtualHP -= absorbed;
                    remaining -= absorbed;

                    this.OnVirtualHPChanged?.Invoke(this.CurrentVirtualHP, this.MaxVirtualHP);
                }

                if (remaining > 0)
                {
                    // True damage ignores DamageTaken modifiers => subtract directly
                    this.CurrentValue -= remaining;
                    this.OnDamageTaken?.Invoke(remaining);
                }
            }
            else
            {
                // True external damage bypassing Virtual HP => subtract directly from HP
                this.CurrentValue -= amount;
                this.OnDamageTaken?.Invoke(amount);
            }

            bool died = this.TryMarkDead(deathCause, "Status effect periodic damage");

            this.VisualController?.PlayHitSequence(shouldPlayDieAfterHit: died);
        }

        //  5. Status Effect Stat Periodic handler
        protected override void ApplyStatPeriodicTick(StatusEffectStatPeriodicContributionEntry periodic, int tickAmount, RuntimeStatusEffect sourceStatusEffect)
        {
            if (periodic == null) return;
            if (tickAmount <= 0) return;
            if (this.IsDead) return;
            if (periodic.Target != StatusEffectStatPeriodicTarget.HP) return;

            switch (periodic.Method)
            {
                case StatusEffectStatPeriodicMethodType.Damage:
                    this.ApplyPeriodicDamage(tickAmount, periodic.TreatHPDamageAsTrueDamage, periodic.TrueDamageBypassVirtualHP, sourceStatusEffect);
                    break;

                case StatusEffectStatPeriodicMethodType.Heal:
                    this.Heal(tickAmount);
                    break;

                // Not recommended for HP, but make it robust:
                case StatusEffectStatPeriodicMethodType.Drain:
                    // interpret as cost
                    CharacterDeathCauseEntry deathCause = this.CreateDeathCauseFromStatusEffectSource(sourceStatusEffect);
                    this.TrySpendCost(tickAmount, deathCause);
                    break;

                case StatusEffectStatPeriodicMethodType.Restore:
                    // interpret as heal
                    this.Heal(tickAmount);
                    break;
            }
        }
        private CharacterDeathCauseEntry CreateDeathCauseFromStatusEffectSource(RuntimeStatusEffect source)
        {
            GameObject sourceObject = source != null ? source.Source : null;

            CharacterRootController sourceCharacter = sourceObject != null
                ? sourceObject.GetComponentInParent<CharacterRootController>()
                : null;

            return CharacterDeathCauseGenerationUtility.FromDamageSource
            (
                victim: this.CharacterRootController,
                sourceCharacter: sourceCharacter,
                sourceObject: sourceObject,
                reason: source?.Profile != null
                    ? $"Status effect: {source.Profile.name}"
                    : "Status effect periodic"
            );
        }

        //  6. FOTL Base Behaviour Overrides
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.IsDead = false;
            this.LastDeathCause = null;

            _maxVirtualHP = Mathf.Clamp(_maxVirtualHP, 0, this.MaxValue);
            _currentVirtualHP = Mathf.Clamp(_currentVirtualHP, 0, this.MaxVirtualHP);
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            this.OnVirtualHPChanged?.Invoke(this.CurrentVirtualHP, this.MaxVirtualHP);
        }
    }
}
