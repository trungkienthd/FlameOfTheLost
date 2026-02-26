using UnityEngine;

namespace FlameOfTheLost
{
    public class KnockbackReceivingHandler : MonoBehaviour, IStatusEffectStatModifierMultiplierConsumer, IStatusEffectStatModifierAdditiveConsumer
    {
        // --- References
        [Header("References")]
        [SerializeField] private Rigidbody2D _rigidbody2D;
        public Rigidbody2D Rigidbody2D
        {
            get => _rigidbody2D;
            private set => _rigidbody2D = value;
        }

        // --- Settings
        [Header("Knockback Settings")]
        [SerializeField] private float _massScale = 1f;
        public float MassScale
        {
            get => _massScale;
            private set => _massScale = value;
        }

        // --- Status Effects Modifiers
        [Header("Status Effects Modifiers")]
        [SerializeField] private float _knockbackTakenMultiplier = 1f;
        [SerializeField] private float _knockbackTakenAdditiveFlat = 0f;
        public float KnockbackTakenMultiplier
        {
            get => _knockbackTakenMultiplier;
            private set => _knockbackTakenMultiplier = Mathf.Max(0f, value);
        }
        public float KnockbackTakenAdditiveFlat
        {
            get => _knockbackTakenAdditiveFlat;
            private set => _knockbackTakenAdditiveFlat = value;
        }
        

        // --- Unity methods
        private void Awake()
        {
            this.Rigidbody2D = this.GetComponentInParent<Rigidbody2D>();
        }

        // --- Public APIs
        //  1. Apply knockback
        public void ApplyKnockback(Vector2 direction, float force)
        {
            if (this.Rigidbody2D == null) return;

            // Calculate effective force with status effect modifiers
            float effectiveForce = force * this.KnockbackTakenMultiplier + this.KnockbackTakenAdditiveFlat;

            // Calculate the knockback force based on the mass scale
            this.Rigidbody2D.linearVelocity = Vector2.zero;
            Vector2 knockbackForce = direction.normalized * effectiveForce * this.MassScale;

            // Apply the force to the Rigidbody2D
            this.Rigidbody2D.AddForce(knockbackForce, ForceMode2D.Impulse);
        }

        //  2. Status Effect Stat Modifier application
        public void SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTargetType target, float multiplier)
        {
            if (target == StatusEffectStatModifierTargetType.KnockbackReceived)
            {
                this.KnockbackTakenMultiplier = multiplier;
            }
        }
        public void SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTargetType target, float additiveFlat)
        {
            if (target == StatusEffectStatModifierTargetType.KnockbackReceived)
            {
                this.KnockbackTakenAdditiveFlat = additiveFlat;
            }
        }
    }
}