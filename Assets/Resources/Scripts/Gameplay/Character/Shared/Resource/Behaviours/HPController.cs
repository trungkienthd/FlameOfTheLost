using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class HPController : CharacterResourcePoolController, IDamageable, IVirtualHPGrantedConsumer
    {
        // --- States ---
        //  1. References
        private Rigidbody2D Rigidbody2D => this.CharacterRootController.MovementHandler.Rigidbody2D; 
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

        //  3. HP-related States
        [Header("States")]
        [SerializeField] private bool _isDead = false;
        public bool IsDead
        {
            get => _isDead;
            private set => _isDead = value;
        }

        //  4. Character Faction
        public CharacterFaction CharacterFaction => this.CharacterRootController.IdentityController.Faction;

        //  5. Status Effect Stat Periodic (Type)
        public override StatusEffectStatPeriodicTargetType PeriodicTarget => StatusEffectStatPeriodicTargetType.HP;

        //  6. Events
        public event Action<int> OnDamageTaken;
        public event Action<int> OnHealed;
        public event Action OnDeath;
        public event Action<int, int> OnVirtualHPChanged;

        // --- Unity Callbacks ---
        private void Start()
        {
            this.IsDead = false;

            this.CurrentVirtualHP = Mathf.Max(0, this.CurrentVirtualHP);
            this.OnVirtualHPChanged?.Invoke(this.CurrentVirtualHP, this.MaxVirtualHP);
        }

        // --- Public APIs ---
        //  1. Primarily spend HP for damage taking (Virtual HP is considered first)
        public void TakeDamage(int damageAmount)
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

            // c. invoke damage taken event (Death/Hit)
            this.ApplyAnimationAfterDamage();
        }

        //  2. Heal HP directly
        public void Heal(int healAmount)
        {
            if (healAmount <= 0) return;

            this.Restore(healAmount);

            this.OnHealed?.Invoke(healAmount);
        }

        //  3. Kill will pass Virtual HP, and affect Current HP directly
        public void Kill()
        {
            if (this.IsDead) return;

            this.SetCurrentValue(0);
            this.IsDead = true;

            this.VisualController?.SetDead(true);
            if (this.Rigidbody2D != null) this.Rigidbody2D.linearVelocity = Vector2.zero; // Stop movement on death

            this.OnDeath?.Invoke();
        }

        //  4. TrySpend override not to account for Virtual HP, but only Current HP (Value)
        public override bool TrySpendCost(int baseCostValue)
        {
            if (baseCostValue < 0) return false;
            if (baseCostValue == 0) return true;
            if (!this.CanSpendCost(baseCostValue)) return false;

            int effectiveCostValue = this.ComputeEffectiveCostValue(baseCostValue);
            this.CurrentValue -= effectiveCostValue;

            // HP Cost may kill, but never trigger hit animation
            this.TryHandleDeath();

            return true;
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
        public override void SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTargetType target, float multiplier)
        {
            switch (target)
            {
                case StatusEffectStatModifierTargetType.DamageReceived:
                    this.DamageReceivedMultiplier = multiplier;
                    return;
                case StatusEffectStatModifierTargetType.HPCost:
                    this.CostMultiplier = multiplier;
                    return;
                default:
                    return;
            }
        }
        public override void SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTargetType target, float additiveFlat)
        {
            switch (target)
            {
                case StatusEffectStatModifierTargetType.DamageReceived:
                    this.DamageReceivedAdditiveFlat = additiveFlat;
                    return;
                case StatusEffectStatModifierTargetType.HPCost:
                    this.CostAdditiveFlat = additiveFlat;
                    return;
                default:
                    return;
            }
        }

        //  7. Refresh Pool for Respawning
        public override void RefreshPoolAfterRespawn(CharacterRespawnRefreshContext context)
        {
            base.RefreshPoolAfterRespawn(context);

            this.IsDead = false;

            this.MaxVirtualHP = 0;
            this.CurrentVirtualHP = 0;
            this.OnVirtualHPChanged?.Invoke(this.CurrentVirtualHP, this.MaxVirtualHP);

            this.VisualController?.SetDead(false);
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

        //  2. Death-only check. Does NOT trigger Hit. This is called after taking HP cost (TrySpend).
        private void TryHandleDeath()
        {
            if (this.IsDead) return;

            if (this.CurrentValue <= 0)
            {
                this.IsDead = true;
                this.VisualController?.SetDead(true);
                if (this.Rigidbody2D != null) this.Rigidbody2D.linearVelocity = Vector2.zero;

                this.OnDeath?.Invoke();
                Debug.Log($"[{this.CharacterName}.HPController] Entity is dead (self-cost or status effect periodic cost).");
            }
        }

        //  3. Damage feedback: if dead => death flow; else => trigger Hit. This is called after taking external damage.
        private void ApplyAnimationAfterDamage()
        {
            if (this.IsDead) return;

            if (this.CurrentValue <= 0)
            {
                this.IsDead = true;
                this.VisualController?.SetDead(true);
                if (this.Rigidbody2D != null) this.Rigidbody2D.linearVelocity = Vector2.zero;

                this.OnDeath?.Invoke();
                Debug.Log($"[{this.CharacterName}.HPController] Entity is dead.");
                return;
            }

            this.VisualController?.TriggerHit();
            Debug.Log($"[{this.CharacterName}.HPController] Current HP: {this.CurrentValue}");
        }

        //  4. Apply periodic damage according to policies
        private void ApplyPeriodicDamage(int amount, bool treatAsTrueDamage, bool bypassVirtualHP)
        {
            if (amount <= 0) return;

            // Case A: Not true damage => interpret as HP self-cost.
            // Virtual HP is NEVER considered for self-cost, so bypass flag is irrelevant here.
            if (!treatAsTrueDamage)
            {
                this.TrySpendCost(amount);     // HP cost only (modifiers apply)
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

            this.ApplyAnimationAfterDamage();
        }

        //  5. Status Effect Stat Periodic handler
        protected override void ApplyStatPeriodicTick(StatusEffectStatPeriodicContribution periodic, int tickAmount, RuntimeStatusEffect source)
        {
            if (periodic == null) return;
            if (tickAmount <= 0) return;
            if (this.IsDead) return;
            if (periodic.Target != StatusEffectStatPeriodicTargetType.HP) return;

            switch (periodic.Method)
            {
                case StatusEffectStatPeriodicMethodType.Damage:
                    this.ApplyPeriodicDamage(tickAmount, periodic.TreatHPDamageAsTrueDamage, periodic.TrueDamageBypassVirtualHP);
                    break;

                case StatusEffectStatPeriodicMethodType.Heal:
                    this.Heal(tickAmount);
                    break;

                // Not recommended for HP, but make it robust:
                case StatusEffectStatPeriodicMethodType.Drain:
                    // interpret as self-cost
                    this.TrySpendCost(tickAmount);
                    break;

                case StatusEffectStatPeriodicMethodType.Restore:
                    // interpret as heal
                    this.Heal(tickAmount);
                    break;
            }
        }
    }
}
