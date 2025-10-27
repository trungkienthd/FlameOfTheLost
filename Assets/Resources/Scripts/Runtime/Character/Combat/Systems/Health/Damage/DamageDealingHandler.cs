using UnityEngine;

namespace FlameOfTheLost
{
    public class DamageDealingHandler : MonoBehaviour, IDamager, IDamageConfigurator, IHitFilter, IHitPostProcessor
    {
        [Header("Damage Payload")]
        [SerializeField] private int _damageAmount = 0;
        [SerializeField] private float _knockbackForce = 0f;
        public int DamageAmount
        {
            get => _damageAmount;
            private set => _damageAmount = value;
        }
        public float KnockbackForce
        {
            get => _knockbackForce;
            private set => _knockbackForce = value;
        }

        [Header("Source")]
        [SerializeField] private CharacterFaction _sourceCharacterFaction = CharacterFaction.Neutral;
        [SerializeField] private Transform _originTransform;
        public CharacterFaction SourceCharacterFaction
        {
            get => _sourceCharacterFaction;
            private set => _sourceCharacterFaction = value;
        }
        public Transform OriginTransform
        {
            get => _originTransform;
            private set => _originTransform = value;
        }

        [Header("Hit Rules")]
        [SerializeField] private bool _destroyOnHit = false;
        [SerializeField] private bool _ignoresSameFaction = true;
        private bool DestroyOnHit => _destroyOnHit;
        private bool IgnoresSameFaction => _ignoresSameFaction;

        private void Awake()
        {
            if (this.OriginTransform == null) this.OriginTransform = this.transform;
        }

        public bool CanHit(IDamageable target)
        {
            if (!this.IgnoresSameFaction) return true;
            return (target.CharacterFaction != this.SourceCharacterFaction && target.CharacterFaction != CharacterFaction.Neutral);
        }

        public void OnSuccessfulHit()
        {
            if (this.DestroyOnHit) Object.Destroy(this.gameObject);
        }

        // IDamageConfigurator
        public void SetDamageAmount(int value) 
        { 
            this.DamageAmount = Mathf.Max(0, value); 
        }
        public void SetKnockbackForce(float force) 
        { 
            this.KnockbackForce = Mathf.Max(0f, force); 
        }
        public void SetOrigin(Transform origin) 
        {
            if (origin) this.OriginTransform = origin; 
        }
        public void SetSourceCharacterFaction(CharacterFaction characterFaction)
        {
            this.SourceCharacterFaction = characterFaction;
        }
    }
}
