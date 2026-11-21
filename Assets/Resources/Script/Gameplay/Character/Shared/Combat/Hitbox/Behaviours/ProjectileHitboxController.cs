using UnityEngine;

namespace FlameOfTheLost
{
    public class ProjectileHitboxController : HitboxController
    {
        // --- States ---
        [Header("State")]
        [SerializeField] private float _lifetime = 1.5f;                    // in seconds
        [SerializeField] private bool _useUnscaledTime = false;
        [SerializeField] private bool _destroyOnHit = false;
        public float Lifetime
        {
            get => _lifetime;
            set => _lifetime = value;
        }
        public bool UseUnscaledTime => _useUnscaledTime;

        private float _timeAlive;
        private float TimeAlive
        {
            get => _timeAlive;
            set => _timeAlive = value;
        }
        public bool DestroyOnHit
        {
            get => _destroyOnHit;
            set => _destroyOnHit = value;
        }
        protected override bool AllowMultipleTargetsPerEnableWindow => !this.DestroyOnHit;

        // --- Unity Callbacks ---
        private void Update()
        {
            if (!this.IsActive) return;
            if (this.Lifetime <= 0f) return;

            this.TimeAlive += this.UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            if (this.TimeAlive >= this.Lifetime)
            {
                // lifetime ended: 
                this.DisableHitbox();
                UnityEngine.Object.Destroy(this.gameObject);        // destroy projectile
            }
        }

        // --- Public APIs ---
        public override void EnableHitbox(HitboxActivationContext payload, AttackSide attackSideType)
        {
            base.EnableHitbox(payload, attackSideType);

            // Reset states
            this.TimeAlive = 0f;

            if (this.ActiveRuntimeSkill != null && this.ActiveRuntimeSkill.Profile is RangedSkillProfile rangedSkillProfile)
            {
                this.Lifetime = rangedSkillProfile.ProjectileLifetime;
                this.DestroyOnHit = rangedSkillProfile.DestroyProjectileOnHit;
            }
        }

        public override void NotifyHitConfirmed(UnityEngine.Object target, CharacterRootController characterTarget)
        {
            if (!this.IsActive) return;

            base.NotifyHitConfirmed(target, characterTarget);

            if (!this.DestroyOnHit) return;

            this.DisableHitbox();
            UnityEngine.Object.Destroy(this.gameObject);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.TimeAlive = 0f;
        }
    }
}