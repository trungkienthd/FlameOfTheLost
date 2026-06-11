using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(KnockbackReceivingHandler))]
    public class DamageReceivingHandler : CharacterBehaviourControllerBase, IStatusEffectFlagConsumer
    {
        // --- States ---
        //  1. References
        [Header("References (Knockback)")]
        [SerializeField] private KnockbackReceivingHandler _knockbackHandler;
        private KnockbackReceivingHandler KnockbackHandler
        {
            get => _knockbackHandler;
            set => _knockbackHandler = value;
        }
        private HPController HPController => this.CharacterRootController.HPController;

        //  2. Status Effects Flags (Runtime)
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

        //  3. Internal States: Last Hit Event's attributes
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

        // --- Unity Callbacks ---
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (this.HPController == null || this.HPController.IsDead) return;
            if (!(this.HPController is IDamageable targetDamageable)) return;
            if (!other.TryGetComponent<IDamager>(out var damagerObj)) return;

            // Root GameObject representing the target (usually the character root)
            CharacterRootController target = this.CharacterRootController;

            // Character Faction filter
            var damager = damagerObj;
            DamageDealingHandler damageDealingHandler = damager as DamageDealingHandler;
            if (damageDealingHandler != null && !damageDealingHandler.CanHit(targetDamageable)) return;

            // Check if already processed this attacker (This is not the main protection, which is done by HitboxController)
            if (Time.frameCount == this.LastHitFrame && System.Object.ReferenceEquals(this.LastDamager, damager)) return;

            // If this collider is a hitbox, enforce "once per target per enable window"
            if (other.TryGetComponent<HitboxController>(out var hitboxController))
            {
                if (!hitboxController.TryRegisterHitTarget(target))
                {
                    return; // already hit this target (or hitbox is single-target and already consumed)
                }
            }

            this.LastHitFrame = Time.frameCount;
            this.LastDamager = damager;

            // Direction = from damager origin to this
            Vector2 direction = damager.OriginTransform != null 
                ? (this.transform.position - damager.OriginTransform.position).normalized 
                : Vector2.zero;

            // Apply damage and knockback
            if (!this.IgnoreDamageReceived)
            {
                CharacterDeathCauseEntry deathCause = CharacterDeathCauseGenerationUtility.FromDamageSource
                (
                    victim: this.CharacterRootController,
                    sourceCharacter: damager.SourceCharacterRootController,
                    sourceObject: other.gameObject,
                    reason: "Direct damage"
                );

                targetDamageable.TakeDamage(damager.EffectiveDamageAmount, deathCause);
            }

            if (!this.IgnoreKnockbackReceived)
            {
                this.KnockbackHandler?.ApplyKnockback(direction, damager.EffectiveKnockbackForce);
            }

            //Debug.Log($"[DRH] ENTER: other={other.name} (path={other.transform.GetHierarchyPath()}) " +
            //          $"this={this.name} (path={this.transform.GetHierarchyPath()})");

            // Notify resource-resolution & skill status effect application handlers on the ATTACK collider (melee/projectile hitbox)
            if (hitboxController != null)
            {
                hitboxController.NotifyHitConfirmed(target);
            }

            // Notify damager
            damageDealingHandler?.OnSuccessfulHit();
        }

        // --- Public APIs ---
        //  1. IStatusEffectFlagConsumer implementation
        public void SetStatusEffectFlag(StatusEffectFlagTarget flagType, bool value)
        {
            switch (flagType)
            {
                case StatusEffectFlagTarget.IgnoreDamageReceived:
                    this.IgnoreDamageReceived = value;
                    return;

                case StatusEffectFlagTarget.IgnoreKnockbackReceived:
                    this.IgnoreKnockbackReceived = value;
                    return;

                default:
                    return;
            }
        }

        // --- Helpers ---
        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (!this.KnockbackHandler) _knockbackHandler = this.GetComponent<KnockbackReceivingHandler>();
        }
    }
}
