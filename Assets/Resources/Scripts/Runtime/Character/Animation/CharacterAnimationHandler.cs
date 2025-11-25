using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimationHandler : MonoBehaviour, IFacingProvider
    {
        [Header("Refs")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _flipGroup;
        [SerializeField] private CharacterCombatLoadoutController _combatLoadoutController;
        public SpriteRenderer SpriteRenderer
        {
            get => _spriteRenderer;
            private set => _spriteRenderer = value;
        }
        public Animator Animator
        {
            get => _animator;
            private set => _animator = value;
        }
        public Transform FlipGroup
        {
            get => _flipGroup;
            private set => _flipGroup = value;
        }
        public CharacterCombatLoadoutController CombatLoadoutController
        {
            get => _combatLoadoutController;
            private set => _combatLoadoutController = value;
        }

        [Header("Tuning")]
        [Tooltip("World units per second that map to Speed=1 in the Locomotion blend trees.")]
        [SerializeField] private float _speedForOne = 10f;
        [Tooltip("Damp time for Speed parameter smoothing.")]
        [SerializeField] private float _speedDampTime = 0.05f;
        private float SpeedForOne => _speedForOne;
        private float SpeedDampTime => _speedDampTime;

        private Vector2 _lastDir = Vector2.down;
        public Vector2 LastDir
        {
            get => _lastDir;
            private set => _lastDir = value;
        }

        public int FacingSignX { get; private set; } = +1;

        public static readonly int SpeedId = Animator.StringToHash("Speed");
        public static readonly int DirXId = Animator.StringToHash("DirX");
        public static readonly int DirYId = Animator.StringToHash("DirY");
        public static readonly int FacingId = Animator.StringToHash("Facing");
        public static readonly int DeadId = Animator.StringToHash("Dead");
        public static readonly int MeleeAttackId = Animator.StringToHash("MeleeAttack");
        public static readonly int RangedAttackId = Animator.StringToHash("RangedAttack");
        public static readonly int HitId = Animator.StringToHash("Hit");
        public static readonly int MeleeSkillId = Animator.StringToHash("MeleeSkillId");
        public static readonly int RangedSkillId = Animator.StringToHash("RangedSkillId");

        // Example mappings — adapt to your parameters
        public FacingStatus FacingStatus
        {
            get
            {
                // If you store last move vector or have an int param:
                int facingInt = this.Animator.GetInteger("Facing"); // 0=Down,1=Side,2=Up
                return (FacingStatus)facingInt;

                //// Or infer from your DirX/DirY floats, etc.
                //float dx = this.Animator.GetFloat("DirX");
                //float dy = this.Animator.GetFloat("DirY");
                //if (Mathf.Abs(dy) > Mathf.Abs(dx))
                //    return (dy > 0f) ? FacingStatus.UP : FacingStatus.DOWN;
                //return FacingStatus.SIDE;
            }
        }

        public bool IsFacingRight
        {
            get
            {
                // Derive from DirX:
                float dx = this.Animator.GetFloat("DirX");
                if (Mathf.Abs(dx) > 0.001f) return dx > 0f;
                return true; // default to right if unknown
            }
        }

        private void Awake()
        {
            if (!this.SpriteRenderer) this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
            if (!this.Animator) this.Animator = this.GetComponent<Animator>();
            if (!this.FlipGroup) this.FlipGroup = this.transform.parent;

            this.CombatLoadoutController = this.GetComponentInParent<CharacterCombatLoadoutController>();
        }

        public void SetFacing(int facing)
        {
            this.Animator.SetInteger(CharacterAnimationHandler.FacingId, facing);
        }
        public void SetFacing(FacingStatus facing)
        {
            this.SetFacing((int)facing);
        }

        public void FeedMovement(Vector2 worldVel)
        {
            // Keep the last non-zero direction
            if (worldVel.sqrMagnitude > 0.0001f)
            {
                this.LastDir = worldVel.normalized;
            }

            // Smooth speed in [0..1] range
            float speed01 = (this.SpeedForOne > 0f) ? Mathf.Clamp01(worldVel.magnitude / 10f) : 0f; // Scale: Assuming 10 is the max speed
            this.Animator.SetFloat(CharacterAnimationHandler.SpeedId, speed01, this.SpeedDampTime, Time.deltaTime);

            // Direction vectors for aim/idle pose
            this.Animator.SetFloat(CharacterAnimationHandler.DirXId, this.LastDir.x);
            this.Animator.SetFloat(CharacterAnimationHandler.DirYId, this.LastDir.y);

            int facing;
            if (Mathf.Abs(this.LastDir.x) >= Mathf.Abs(this.LastDir.y))
            {
                // Horizontal movement is dominant
                facing = 1;
            }
            else if (this.LastDir.y > 0f)
            {
                // Vertical movement is dominant and going up
                facing = 2;
            }
            else
            {
                // Vertical movement is dominant and going down
                facing = 0;
            }

            this.SetFacing(facing);

            if (this.FlipGroup)
            {
                if (facing == 1)
                {
                    // Horizontal movement: flip the sprite
                    this.FacingSignX = (this.LastDir.x >= 0f) ? +1 : -1;
                    Vector3 ls = this.FlipGroup.localScale;
                    ls.x = Mathf.Abs(ls.x) * this.FacingSignX;
                    this.FlipGroup.localScale = ls;
                }
                else
                {
                    // Vertical movement: don't flip the sprite
                    this.FacingSignX = +1;
                }
            }

            if (this.SpriteRenderer)
            {
                // Flip sprite based on direction (If direction is horizontal)
                this.SpriteRenderer.flipX = false; // (facing == 1 && this.LastDir.x < 0f);
            }
        }

        // Triggers
        public void TriggerMeleeAttack(int skillId = 0)
        {
            // Debug.Log($"Hello: {skillIndex}");

            this.Animator.SetInteger(CharacterAnimationHandler.MeleeSkillId, Mathf.Max(0, skillId));
            this.Animator.SetTrigger(CharacterAnimationHandler.MeleeAttackId);
        }
        public void TriggerRangedAttack(int skillId = 0)
        {
            this.Animator.SetInteger(CharacterAnimationHandler.RangedSkillId, Mathf.Max(0, skillId));
            this.Animator.SetTrigger(CharacterAnimationHandler.RangedAttackId);
        }
        public void TriggerHit()
        {
            this.Animator.SetTrigger(CharacterAnimationHandler.HitId);
        }
        public void SetDead(bool isDead)
        {
            this.Animator.SetBool(CharacterAnimationHandler.DeadId, isDead);
        }

        public void ForceFacing(Vector2 dir)
        {
            if (dir.sqrMagnitude <= 0.0001f) return;
            this.LastDir = dir.normalized;
        }

        // Called from Animation Event
        public void AE_MeleeHitboxWindowStart()
        {
            this.CombatLoadoutController.MeleeCombatController.OnMeleeHitboxWindowStart(); 
        }
        public void AE_MeleeHitboxWindowEnd()
        {
            this.CombatLoadoutController.MeleeCombatController.OnMeleeHitboxWindowEnd();
        }

        public void AE_RangedAttackStart()
        {
            this.CombatLoadoutController.RangedCombatController.OnRangedFire();
        }
        public void AE_RangedAttackEnd()
        {
            this.CombatLoadoutController.RangedCombatController.OnRangedEnd();
        }

    }
}
