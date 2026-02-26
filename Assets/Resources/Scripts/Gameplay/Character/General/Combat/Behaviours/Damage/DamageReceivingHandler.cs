using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(KnockbackReceivingHandler))]
    public class DamageReceivingHandler : MonoBehaviour, IStatusEffectFlagConsumer
    {
        // --- References
        [Header("References")]
        [SerializeField] private HPController _hp;
        [SerializeField] private KnockbackReceivingHandler _knockback;
        private HPController HPController
        {
            get => _hp;
            set => _hp = value;
        }
        private KnockbackReceivingHandler KnockbackHandler
        {
            get => _knockback;
            set => _knockback = value;
        }

        // --- Status Effects Flags (Runtime)
        [Header("Status Effects Flags (Runtime)")]
        [SerializeField] private bool _ignoreDamageReceived;
        [SerializeField] private bool _ignoreKnockbackReceived;
        public bool IgnoreDamageReceived
        {
            get => _ignoreDamageReceived;
            private set => _ignoreDamageReceived = value;
        }
        public bool IgnoreKnockbackReceived
        {
            get => _ignoreKnockbackReceived;
            private set => _ignoreKnockbackReceived = value;
        }

        // --- Internal States: Last Hit Event's attributes
        private int _lastHitFrame = -1;
        private IDamager _lastDamager = null;
        private int LastHitFrame
        {
            get => _lastHitFrame;
            set => _lastHitFrame = value;
        }
        private IDamager LastDamager
        {
            get => _lastDamager;
            set => _lastDamager = value;
        }

        // --- Unity methods
        private void Awake()
        {
            if (!this.HPController)     this.HPController       = this.GetComponentInParent<HPController>();
            if (!this.KnockbackHandler) this.KnockbackHandler   = this.GetComponentInParent<KnockbackReceivingHandler>();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (this.HPController == null || this.HPController.IsDead) return;
            if (!(this.HPController is IDamageable target)) return;
            if (!other.TryGetComponent<IDamager>(out var damagerObj)) return;

            // Root GameObject representing the target (usually the character root)
            GameObject targetGameObject = this.HPController != null
                ? this.HPController.gameObject
                : this.transform.root.gameObject;

            // If this collider is a hitbox, enforce "once per target per enable window"
            if (other.TryGetComponent<HitboxController>(out var hitboxController))
            {
                if (!hitboxController.TryRegisterHitTarget(targetGameObject))
                {
                    return; // already hit this target (or hitbox is single-target and already consumed)
                }
            }

            // Character Faction filter
            var damager = damagerObj;
            if (damager is DamageDealingHandler dd && !dd.CanHit(target)) return;

            // Check if already processed this attacker (This is not the main protection, which is done by HitboxController)
            if (Time.frameCount == this.LastHitFrame && System.Object.ReferenceEquals(this.LastDamager, damager)) return;

            this.LastHitFrame = Time.frameCount;
            this.LastDamager = damager;

            // Direction = from damager origin to this
            Vector2 direction = (this.transform.position - damager.OriginTransform.position).normalized;

            // Apply damage and knockback
            if (!this.IgnoreDamageReceived) target.TakeDamage(damager.EffectiveDamageAmount);
            if (!this.IgnoreKnockbackReceived) this.KnockbackHandler?.ApplyKnockback(direction, damager.EffectiveKnockbackForce);

            //Debug.Log($"[DRH] ENTER: other={other.name} (path={other.transform.GetHierarchyPath()}) " +
            //          $"this={this.name} (path={this.transform.GetHierarchyPath()})");

            // Notify resource-resolution & skill status effect application handlers on the ATTACK collider (melee/projectile hitbox)
            if (hitboxController != null)
            {
                hitboxController.NotifyHitConfirmed(targetGameObject);
            }

            // Notify damager
            if (damager is DamageDealingHandler dd2) dd2.OnSuccessfulHit();
        }

        // --- Public APIs
        //  Status Effect Flag consumption
        public void SetStatusEffectFlag(StatusEffectFlagType flagType, bool value)
        {
            switch (flagType)
            {
                case StatusEffectFlagType.IgnoreDamageReceived:
                    this.IgnoreDamageReceived = value;
                    return;

                case StatusEffectFlagType.IgnoreKnockbackReceived:
                    this.IgnoreKnockbackReceived = value;
                    return;

                default:
                    return;
            }
        }
    }
}
