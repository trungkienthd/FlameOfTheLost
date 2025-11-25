using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class StatusEffectOverTimeHandler : MonoBehaviour
    {
        [SerializeField] private StatusEffectSystemConfig _systemConfig;
        private StatusEffectSystemConfig SystemConfig
        {
            get => _systemConfig;
            set => _systemConfig = value;
        }

        private readonly List<RuntimeStatusEffect> _active = new();
        private List<RuntimeStatusEffect> ActiveStatusEffects => _active;

        // Per-instance fractional accumulator so we can output ints to HealthController
        // Value meaning: positive = pending heal, negative = pending damage
        private readonly Dictionary<uint, float> _pendingHpByEffect = new();
        private Dictionary<uint, float> PendingHPByEffect => _pendingHpByEffect;

        // External system 
        private HealthController _health;
        private HealthController HealthController
        {
            get => _health;
            set => _health = value;
        }

        internal void Initialize(HealthController healthController, StatusEffectSystemConfig systemConfig)
        {
            this.HealthController = healthController;
            this.SystemConfig = systemConfig;
        }

        internal void SetActiveStatusEffects(List<RuntimeStatusEffect> list)
        {
            this.ActiveStatusEffects.Clear();
            this.ActiveStatusEffects.AddRange(list);

            // Ensure we have accumulators for all currently active status effects
            // and drop any stale ones
            this.PendingHPByEffect.Clear();
            for (int i = 0; i < this.ActiveStatusEffects.Count; i++)
            {
                var statusEffect = this.ActiveStatusEffects[i];
                if (!this.PendingHPByEffect.ContainsKey(statusEffect.RuntimeID))
                {
                    this.PendingHPByEffect[statusEffect.RuntimeID] = 0f;
                }
            }
        }

        private void Update()
        {
            if (this.ActiveStatusEffects.Count == 0 || this.HealthController == null) return;

            float dt = Time.deltaTime;

            for (int i = 0; i < this.ActiveStatusEffects.Count; i++)
            {
                var statusEffect = this.ActiveStatusEffects[i];
                var periodic = statusEffect.ScaledPeriodic;
                if (!periodic.IsEnabled) continue;

                if (!this.PendingHPByEffect.ContainsKey(statusEffect.RuntimeID))
                {
                    this.PendingHPByEffect[statusEffect.RuntimeID] = 0f;
                }

                statusEffect.TickTimer += dt;

                // Process ticks; accumulate fractional amounts
                while (statusEffect.TickTimer >= periodic.TickInterval)
                {
                    statusEffect.TickTimer -= periodic.TickInterval;

                    float tickAmount = periodic.TickAmount;
                    if (Mathf.Approximately(tickAmount, 0f)) continue;

                    if (tickAmount < 0f)        // Damage over time
                    {
                        bool bypass = (this.SystemConfig == null) ? true : this.SystemConfig.DamageOverTimeBypassesDamageTaken;
                        bool treatAsIncoming = periodic.TreatAsIncomingDamage;

                        if (!bypass || treatAsIncoming)
                        {

                        }
                    }

                    // Accumulate into this effect’s running pool
                    this.PendingHPByEffect[statusEffect.RuntimeID] += tickAmount;
                }

                // After processing ticks, extract whole interger amounts from the accumulator
                float pendingAmount = this.PendingHPByEffect[statusEffect.RuntimeID];

                if (pendingAmount >= 1f)        // Apply Heal
                {
                    int healAmount = Mathf.FloorToInt(pendingAmount);
                    if (healAmount > 0)
                    {
                        this.HealthController.Heal(healAmount);
                        this.PendingHPByEffect[statusEffect.RuntimeID] -= healAmount;
                    }
                }
                else if (pendingAmount <= -1f)  // Apply Damage
                {
                    int damageAmount = Mathf.FloorToInt(-pendingAmount);
                    if (damageAmount > 0)
                    {
                        this.HealthController.TakeDamage(damageAmount, Vector2.zero, 0f);   // Over-time damage: zeroed hit direction, no knockback
                        this.PendingHPByEffect[statusEffect.RuntimeID] += damageAmount;     // Pending amount is negative
                    }
                }
            }
        }
    }
}
