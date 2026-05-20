using UnityEngine;

namespace FlameOfTheLost
{
    public class DamageDealingHandler : MonoBehaviour, IDamager, IDamageConfigurator, IHitFilter, IHitPostProcessor, IStatusEffectStatModifierMultiplierConsumer, IStatusEffectStatModifierAdditiveConsumer
    {
        // --- Data Fields (Base) ---
        [Header("Damage Payload (Base)")]
        [SerializeField] private int _baseDamageAmount = 0;
        [SerializeField] private float _baseKnockbackForce = 0f;
        public int BaseDamageAmount
        {
            get => _baseDamageAmount;
            private set => _baseDamageAmount = value;
        }
        public float BaseKnockbackForce
        {
            get => _baseKnockbackForce;
            private set => _baseKnockbackForce = value;
        }

        // --- Effective Payload ---
        [Header("Damage Payload (Final)")]
        [SerializeField] private int _damageAmount = 0;
        [SerializeField] private float _knockbackForce = 0f;
        public int EffectiveDamageAmount
        {
            get => _damageAmount;
            private set => _damageAmount = value;
        }
        public float EffectiveKnockbackForce
        {
            get => _knockbackForce;
            private set => _knockbackForce = value;
        }

        // --- Stat Modifiers ---
        [Header("Status Effects Stat Modifier (Runtime Channels)")]
        [SerializeField] private float _damageDealtMultiplier = 1f;
        [SerializeField] private float _damageDealtAddFlat = 0f;
        [SerializeField] private float _knockbackDealtMultiplier = 1f;
        [SerializeField] private float _knockbackDealtAddFlat = 0f;
        public float DamageDealtMultiplier
        {
            get => _damageDealtMultiplier;
            private set => _damageDealtMultiplier = Mathf.Max(0f, value);
        }
        public float DamageDealtAddFlat
        {
            get => _damageDealtAddFlat;
            private set => _damageDealtAddFlat = value;
        }
        public float KnockbackDealtMultiplier
        {
            get => _knockbackDealtMultiplier;
            private set => _knockbackDealtMultiplier = Mathf.Max(0f, value);
        }
        public float KnockbackDealtAddFlat
        {
            get => _knockbackDealtAddFlat;
            private set => _knockbackDealtAddFlat = value;
        }

        // --- Source ---
        [Header("Source")]
        [SerializeField] private CharacterFactionType _sourceCharacterFaction = CharacterFactionType.Neutral;
        [SerializeField] private Transform _originTransform;
        public CharacterFactionType SourceCharacterFaction
        {
            get => _sourceCharacterFaction;
            private set => _sourceCharacterFaction = value;
        }
        public Transform OriginTransform
        {
            get => _originTransform;
            private set => _originTransform = value;
        }

        // --- Hit Rules ---
        [Header("Hit Rules")]
        [SerializeField] private bool _destroyOnHit = false;
        [SerializeField] private bool _ignoresSameFaction = true;
        private bool DestroyOnHit => _destroyOnHit;
        private bool IgnoresSameFaction => _ignoresSameFaction;

        // --- Unity Methods ---
        private void Awake()
        {
            if (this.OriginTransform == null) this.OriginTransform = this.transform;

            this.RecomputePayload();
        }

        public bool CanHit(IDamageable target)
        {
            if (!this.IgnoresSameFaction) return true;
            return (target.CharacterFaction != this.SourceCharacterFaction);
        }

        public void OnSuccessfulHit()
        {
            if (this.DestroyOnHit) UnityEngine.Object.Destroy(this.gameObject);
        }

        // IDamageConfigurator
        public void SetDamageAmount(int value) 
        { 
            this.BaseDamageAmount = Mathf.Max(0, value); 
            this.RecomputePayload();
        }
        public void SetKnockbackForce(float force) 
        { 
            this.BaseKnockbackForce = Mathf.Max(0f, force); 
            this.RecomputePayload();
        }
        public void SetOrigin(Transform origin) 
        {
            if (origin) this.OriginTransform = origin; 
        }
        public void SetSourceCharacterFaction(CharacterFactionType characterFaction)
        {
            this.SourceCharacterFaction = characterFaction;
        }

        // Status Effect Stat Modifier consumers
        public void SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTargetType target, float multiplier)
        {
            switch (target)
            {
                case StatusEffectStatModifierTargetType.DamageDealt:
                    this.DamageDealtMultiplier = multiplier;
                    this.RecomputePayload();
                    break;
                case StatusEffectStatModifierTargetType.KnockbackDealt:
                    this.KnockbackDealtMultiplier = multiplier;
                    this.RecomputePayload();
                    break;
                default:
                    break;
            }
        }

        public void SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTargetType target, float addFlat)
        {
            switch (target)
            {
                case StatusEffectStatModifierTargetType.DamageDealt:
                    this.DamageDealtAddFlat = addFlat;
                    this.RecomputePayload();
                    break;
                case StatusEffectStatModifierTargetType.KnockbackDealt:
                    this.KnockbackDealtAddFlat = addFlat;
                    this.RecomputePayload();
                    break;
                default:
                    break;
            }
        }

        // --- Helpers
        private void RecomputePayload()
        {
            float dmg = ((float)this.BaseDamageAmount * this.DamageDealtMultiplier) + this.DamageDealtAddFlat;
            int finalDmg = Mathf.Max(0, Mathf.RoundToInt(dmg));

            float kb = (this.BaseKnockbackForce * this.KnockbackDealtMultiplier) + this.KnockbackDealtAddFlat;
            float finalKb = Mathf.Max(0f, kb);

            this.EffectiveDamageAmount = finalDmg;
            this.EffectiveKnockbackForce = finalKb;
        }
    }
}
