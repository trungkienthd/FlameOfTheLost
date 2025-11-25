using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class KnockbackHandler : MonoBehaviour
    {
        [Header("Knockback Settings")]
        [SerializeField] private float _massScale = 1f;
        public float MassScale
        {
            get => _massScale;
            private set => _massScale = value;
        }

        [Header("Status Effects Modifiers")]
        [SerializeField] private float _incomingKnockbackMultiplier = 1f;
        public float IncomingKnockbackMultiplier
        {
            get => _incomingKnockbackMultiplier;
            set => _incomingKnockbackMultiplier = Mathf.Max(0f, value);
        }

        [Header("References")]
        [SerializeField] private Rigidbody2D _rb;
        public Rigidbody2D Rigidbody2D
        {
            get => _rb;
            private set => _rb = value;
        }

        private void Awake()
        {
            this.Rigidbody2D = this.GetComponent<Rigidbody2D>();
        }

        public void ApplyKnockback(Vector2 direction, float force)
        {
            if (this.Rigidbody2D == null) return;

            // Calculate the knockback force based on the mass scale
            this.Rigidbody2D.linearVelocity = Vector2.zero;
            Vector2 knockbackForce = direction.normalized * force * this.MassScale * this.IncomingKnockbackMultiplier;
            // Apply the force to the Rigidbody2D
            this.Rigidbody2D.AddForce(knockbackForce, ForceMode2D.Impulse);
        }
    }
}