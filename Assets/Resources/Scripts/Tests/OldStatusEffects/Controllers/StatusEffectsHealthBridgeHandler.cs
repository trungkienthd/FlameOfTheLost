using UnityEngine;

namespace FlameOfTheLost
{
    // Bridges StatusEffectController events to HealthController and KnockbackHandler.
    [RequireComponent(typeof(StatusEffectController))]
    [RequireComponent(typeof(HealthController))]
    [RequireComponent(typeof(KnockbackHandler))]
    public class StatusEffectsHealthBridgeHandler : MonoBehaviour
    {
        [Header("Targets")]
        [SerializeField] private HealthController _health;
        [SerializeField] private KnockbackHandler _knockback;
        private HealthController HealthController
        {
            get => _health;
            set => _health = value;
        }
        private KnockbackHandler KnockbackHandler
        {
            get => _knockback;
            set => _knockback = value;
        }

        private StatusEffectController _statusEffect;
        private StatusEffectController StatusEffectController
        {
            get => _statusEffect;
            set => _statusEffect = value;
        }

        private void Awake()
        {
            this.StatusEffectController = this.GetComponent<StatusEffectController>();
            if (!this.HealthController) this.HealthController = this.GetComponent<HealthController>();
            if (!this.KnockbackHandler) this.KnockbackHandler = this.GetComponent<KnockbackHandler>();
            
            this.StatusEffectController.OnDamageOverTimeTick.AddListener(this.HandleDamageOverTime);
            this.StatusEffectController.OnHealOverTimeTick.AddListener(this.HandleHealOverTime);
            this.StatusEffectController.OnStatusEffectAggregatesChanged.AddListener(this.HandleStatusEffectAggregatesChanged);
        }

        private void Start()
        {
            if (this.StatusEffectController != null)
            {
                this.HandleStatusEffectAggregatesChanged(this.StatusEffectController.CurrentStatusEffectAggregatesSnapshot);
            }
        }

        private void HandleDamageOverTime(int amount)
        {
            // Adjust signature if the HealthController differs
            this.HealthController?.TakeDamage(amount, Vector2.zero, 0f);
        }

        private void HandleHealOverTime(int amount)
        {
            this.HealthController?.Heal(amount);
        }

        private void HandleStatusEffectAggregatesChanged(StatusEffectAggregates statusEffectAggregates)
        {
            // Add these tiny setters if not present yet.
            this.HealthController.IncomingDamageMultiplier      = statusEffectAggregates.DamageTakenMultiplier;
            this.KnockbackHandler.IncomingKnockbackMultiplier   = statusEffectAggregates.KnockbackTakenMultiplier;
        }

        private void OnDestroy()
        {
            if (!this.StatusEffectController) return;

            this.StatusEffectController.OnDamageOverTimeTick.RemoveListener(this.HandleDamageOverTime);
            this.StatusEffectController.OnHealOverTimeTick.RemoveListener(this.HandleHealOverTime);
            this.StatusEffectController.OnStatusEffectAggregatesChanged.RemoveListener(this.HandleStatusEffectAggregatesChanged);
        }
    }
}