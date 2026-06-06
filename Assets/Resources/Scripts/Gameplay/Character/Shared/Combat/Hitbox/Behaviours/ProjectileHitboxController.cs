using UnityEngine;

namespace FlameOfTheLost
{
    public class ProjectileHitboxController : HitboxController
    {
        // --- State
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

        // --- Unity methods
        protected override void Awake()
        {
            base.Awake();
        }
        private void Start()
        {
            this.TimeAlive = 0f;
        }
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

        // --- Public APIs
        public override void EnableHitbox(HitboxDamagePayload payload, AttackSide attackSideType)
        {
            base.EnableHitbox(payload, attackSideType);

            // Reset states
            this.TimeAlive = 0f;
            if (this.ActiveRuntimeSkill.Profile is RangedSkillProfile rangedSkillProfile)
            {
                this.Lifetime = rangedSkillProfile.ProjectileLifetime;
                this.DestroyOnHit = rangedSkillProfile.DestroyProjectileOnHit;
            }
        }

        public override void NotifyHitConfirmed(CharacterRootController target)
        {
            base.NotifyHitConfirmed(target);

            if (this.DestroyOnHit)
            {
                this.DisableHitbox();
                UnityEngine.Object.Destroy(this.gameObject);
            }
        }
    }
}