using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementHandler : MonoBehaviour
    {
        [Header("Tuning")]
        [SerializeField] private float _movementSpeed = 6f;
        [SerializeField] private float _accel = 60f;
        [SerializeField] private float _decel = 80f;
        private float MovementSpeed => _movementSpeed;
        private float Accel => _accel;
        private float Decel => _decel;

        [Header("Refs")]
        [SerializeField] private CharacterAnimationHandler _animationHandler;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        public CharacterAnimationHandler AnimationHandler
        {
            get => _animationHandler;
            set => _animationHandler = value;
        }
        public Rigidbody2D Rigidbody2D
        {
            get => _rigidbody2D;
            private set => _rigidbody2D = value;
        }

        private Vector2 _desired;
        private Vector2 Desired
        {
            get => _desired;
            set => _desired = value;
        }

        private Vector2 _vel;
        private Vector2 Vel
        {
            get => _vel;
            set => _vel = value;
        }

        private void Awake()
        {
            this.Rigidbody2D = this.GetComponent<Rigidbody2D>();
            if (!this.AnimationHandler) this.AnimationHandler = this.GetComponentInChildren<CharacterAnimationHandler>();
        }

        private void FixedUpdate()
        {
            // Simple x/y acceleration
            this.Vel = this.Rigidbody2D.linearVelocity;

            var vel = this.Vel;
            vel.x = Mathf.MoveTowards(this.Vel.x, this.Desired.x, (Mathf.Abs(this.Desired.x) > 0.01f ? this.Accel : this.Decel) * Time.fixedDeltaTime);
            vel.y = Mathf.MoveTowards(this.Vel.y, this.Desired.y, (Mathf.Abs(this.Desired.y) > 0.01f ? this.Accel : this.Decel) * Time.fixedDeltaTime);
            this.Vel = vel;

            this.Rigidbody2D.linearVelocity = this.Vel;

            // Feed animator (Speed uses world velocity magnitude)
            this.AnimationHandler?.FeedMovement(this.Rigidbody2D.linearVelocity);
        }

        public void SetMoveInput(Vector2 input)
        {
            this.Desired = Vector2.ClampMagnitude(input, 1f) * this.MovementSpeed;
        }
    }
}
