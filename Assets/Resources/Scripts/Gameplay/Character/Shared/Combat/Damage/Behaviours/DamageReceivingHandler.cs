using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(KnockbackReceivingHandler))]
    public class DamageReceivingHandler : CharacterBehaviourControllerBase, IStatusEffectFlagConsumer
    {
        // --- States ---
        //  1. References
        private HPController HPController => this.CharacterRootController.HPController;

        [Header("References - Knockback")]
        [SerializeField] private KnockbackReceivingHandler _knockbackHandler;
        private KnockbackReceivingHandler KnockbackHandler => _knockbackHandler;

        [Header("References - Hit Invulnerability")]
        [SerializeField] private MonoBehaviour _hitInvulnerabilityProviderSource;

        private ICharacterHitInvulnerabilityProvider _hitInvulnerabilityProvider;
        private MonoBehaviour HitInvulnerabilityProviderSource => _hitInvulnerabilityProviderSource;
        private ICharacterHitInvulnerabilityProvider HitInvulnerabilityProvider => _hitInvulnerabilityProvider;

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

        // --- Events ---
        public event Action<DamageAppliedContext> OnDamageApplied;

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

            // Hit invulnerability check (e.g., dodging)
            // IMPORTANT:
            // This must happen before TryRegisterHitTarget and NotifyHitConfirmed.
            // Otherwise dodge would still consume the hitbox and trigger on-hit status effects.
            if (this.ShouldAvoidIncomingHit())
            {
                return;
            }

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

            // Apply damage 
            if (!this.IgnoreDamageReceived)
            {
                CharacterDeathCauseEntry deathCause = CharacterDeathCauseGenerationUtility.FromDamageSource
                (
                    victim: this.CharacterRootController,
                    sourceCharacter: damager.SourceCharacterRootController,
                    sourceObject: other.gameObject,
                    reason: "Direct damage"
                );

                int damageAmount = damager.EffectiveDamageAmount;
                targetDamageable.TakeDamage(damageAmount, deathCause);

                this.OnDamageApplied?.Invoke
                (
                    new DamageAppliedContext
                    (
                        attacker: damager.SourceCharacterRootController,
                        victim: this.CharacterRootController,
                        damageAmount: damageAmount
                    )
                );
            }

            // Apply knockback
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
        private bool ShouldAvoidIncomingHit()
        {
            return this.HitInvulnerabilityProvider != null &&
                   this.HitInvulnerabilityProvider.IsHitInvulnerable;
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (!this.KnockbackHandler) _knockbackHandler = this.GetComponent<KnockbackReceivingHandler>();

            if (this.HitInvulnerabilityProvider == null)
            {
                if (this.HitInvulnerabilityProviderSource == null)
                {
                    _hitInvulnerabilityProviderSource = this.CharacterRootController.DodgeController;
                }

                if (this.HitInvulnerabilityProviderSource != null)
                {
                    _hitInvulnerabilityProvider = this.HitInvulnerabilityProviderSource as ICharacterHitInvulnerabilityProvider;
                }

                if (this.HitInvulnerabilityProvider == null)
                {
                    _hitInvulnerabilityProvider = this.GetComponentInParent<ICharacterHitInvulnerabilityProvider>();
                }

                if (this.HitInvulnerabilityProvider == null)
                {
                    _hitInvulnerabilityProvider = this.GetComponentInChildren<ICharacterHitInvulnerabilityProvider>(true);
                }
            }
        }
    }
}
