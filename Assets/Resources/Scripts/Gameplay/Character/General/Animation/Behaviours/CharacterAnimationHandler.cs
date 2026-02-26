using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimationHandler : MonoBehaviour, IFacingProvider
    {
        // --- References
        [Header("References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _flipGroup;
        [SerializeField] private CharacterCombatAttackController _combatAttackController;
        private SpriteRenderer SpriteRenderer
        {
            get => _spriteRenderer;
            set => _spriteRenderer = value;
        }
        private Animator Animator
        {
            get => _animator;
            set => _animator = value;
        }
        private Transform CharacterFlipGroup
        {
            get => _flipGroup;
            set => _flipGroup = value;
        }
        private CharacterCombatAttackController CombatAttackController
        {
            get => _combatAttackController;
            set => _combatAttackController = value;
        }

        // --- Settings
        [Header("Settings")]
        [SerializeField] private float _speedForOne = 10f;      // World units per second that map to Speed=1 in the Locomotion blend trees
        [SerializeField] private float _speedDampTime = 0.05f;  // Damp time for Speed parameter smoothing
        private float SpeedForOne => _speedForOne;
        private float SpeedDampTime => _speedDampTime;

        private Vector2 _lastDir = Vector2.down;
        public Vector2 LastDir
        {
            get => _lastDir;
            private set => _lastDir = value;
        }
        public int FacingSignX { get; private set; } = +1;

        // --- Cache per-skill override to avoid rebuilding every attack
        private readonly Dictionary<int, AnimatorOverrideController> _overrideCache = new();
        private Dictionary<int, AnimatorOverrideController> OverrideCache => _overrideCache;

        // --- Attack animation clips (placeholders from base controller)
        [Header("Attack Placeholder Clips (from base controller)")]
        [SerializeField] private AnimationClip _placeholderAttackDown;
        [SerializeField] private AnimationClip _placeholderAttackSide;
        [SerializeField] private AnimationClip _placeholderAttackUp;
        public AnimationClip PlaceholderAttackDown => _placeholderAttackDown;
        public AnimationClip PlaceholderAttackSide => _placeholderAttackSide;
        public AnimationClip PlaceholderAttackUp => _placeholderAttackUp;

        // --- Animator Parameter IDs
        public static readonly int SpeedID = Animator.StringToHash("Speed");
        public static readonly int DirXID = Animator.StringToHash("DirX");
        public static readonly int DirYID = Animator.StringToHash("DirY");
        public static readonly int FacingID = Animator.StringToHash("Facing");
        public static readonly int DeadID = Animator.StringToHash("Dead");
        public static readonly int AttackID = Animator.StringToHash("Attack");
        public static readonly int HitID = Animator.StringToHash("Hit");

        // --- Facing states
        public FacingStatusType FacingStatus
        {
            get
            {
                // If you store last move vector or have an int param:
                int facingInt = this.Animator.GetInteger("Facing"); // 0=Down,1=Side,2=Up
                return (FacingStatusType)facingInt;

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

        // --- Unity methods
        private void Awake()
        {
            if (!this.SpriteRenderer) this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
            if (!this.Animator) this.Animator = this.GetComponent<Animator>();
            if (!this.CharacterFlipGroup) this.CharacterFlipGroup = this.transform.parent;

            this.CombatAttackController = this.GetComponentInParent<CharacterCombatAttackController>();
        }

        // --- Public APIs
        //  0. Setting facing direction directly
        public void SetFacing(int facing)
        {
            this.Animator.SetInteger(CharacterAnimationHandler.FacingID, facing);
        }
        public void SetFacing(FacingStatusType facing)
        {
            this.SetFacing((int)facing);
        }

        //  1. Feeding movement input
        public void FeedMovement(Vector2 worldVel)
        {
            // Keep the last non-zero direction
            if (worldVel.sqrMagnitude > 0.0001f)
            {
                this.LastDir = worldVel.normalized;
            }

            // Smooth speed in [0..1] range
            float speed01 = (this.SpeedForOne > 0f) ? Mathf.Clamp01(worldVel.magnitude / 10f) : 0f; // Scale: Assuming 10 is the max speed
            this.Animator.SetFloat(CharacterAnimationHandler.SpeedID, speed01, this.SpeedDampTime, Time.deltaTime);

            // Direction vectors for aim/idle pose
            this.Animator.SetFloat(CharacterAnimationHandler.DirXID, this.LastDir.x);
            this.Animator.SetFloat(CharacterAnimationHandler.DirYID, this.LastDir.y);

            FacingStatusType facing = FacingStatusType.Down;
            if (Mathf.Abs(this.LastDir.x) >= Mathf.Abs(this.LastDir.y))
            {
                // Horizontal movement is dominant
                facing = FacingStatusType.Side;
            }
            else if (this.LastDir.y > 0f)
            {
                // Vertical movement is dominant and going up
                facing = FacingStatusType.Up;
            }
            else
            {
                // Vertical movement is dominant and going down
                facing = FacingStatusType.Down;
            }

            this.SetFacing(facing);

            if (this.CharacterFlipGroup)
            {
                if (facing == FacingStatusType.Side)
                {
                    // Horizontal movement: flip the sprite
                    this.FacingSignX = (this.LastDir.x >= 0f) ? +1 : -1;
                    Vector3 ls = this.CharacterFlipGroup.localScale;
                    ls.x = Mathf.Abs(ls.x) * this.FacingSignX;
                    this.CharacterFlipGroup.localScale = ls;
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
                this.SpriteRenderer.flipX = false; // (facing == FacingStatusType.Side && this.LastDir.x < 0f);
            }
        }

        //  2. Combat animation triggers
        //      2.1. Trigger generic attack
        public void TriggerAttack(RuntimeSkill runtimeSkill)
        {
            if (runtimeSkill == null) return;
            int skillID = runtimeSkill.SkillID;

            // Apply correct animator override for this skill
            this.ApplySkillAttackOverride(runtimeSkill);

            this.Animator.SetTrigger(CharacterAnimationHandler.AttackID);
        }
        //      2.2. Trigger hit reaction
        public void TriggerHit()
        {
            this.Animator.SetTrigger(CharacterAnimationHandler.HitID);
        }
        //      2.3. Set dead/alive state
        public void SetDead(bool isDead)
        {
            this.Animator.SetBool(CharacterAnimationHandler.DeadID, isDead);
        }

        //  3. IFacingProvider implementation
        public void ForceFacing(Vector2 dir)
        {
            if (dir.sqrMagnitude <= 0.0001f) return;
            this.LastDir = dir.normalized;
        }

        //  4. Called from Animation Event
        //      4.1. Attack start frame
        public void AE_CharacterAttackWindowStart()
        {
            this.CombatAttackController?.OnAttackWindowStart();
        }
        //      4.2. Attack end frame
        public void AE_CharacterAttackWindowEnd()
        {
            this.CombatAttackController?.OnAttackWindowEnd();
        }

        // --- Private helpers
        private void ApplySkillAttackOverride(RuntimeSkill runtimeSkill)
        {
            if (this.OverrideCache.TryGetValue(runtimeSkill.SkillID, out AnimatorOverrideController overrideController))
            {
                // Use cached override
                this.Animator.runtimeAnimatorController = overrideController;
                return;
            }

            // Build an override controller from current base controller
            var baseAnimatorController = this.Animator.runtimeAnimatorController;
            var overrideAnimatorController = new AnimatorOverrideController(baseAnimatorController);

            // Validate clips exist
            var down    = runtimeSkill.AnimationProfile.AttackDown;
            var side    = runtimeSkill.AnimationProfile.AttackSide;
            var up      = runtimeSkill.AnimationProfile.AttackUp;

            if (down == null || side == null || up == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.CharacterAnimationHandler] One or more attack animation clips are missing for skill ID {runtimeSkill.SkillID}");
            }

            // Override placeholders -> skill clips
            if (this.PlaceholderAttackDown  != null && down != null) overrideAnimatorController[this.PlaceholderAttackDown] = down;
            if (this.PlaceholderAttackSide  != null && side != null) overrideAnimatorController[this.PlaceholderAttackSide] = side;
            if (this.PlaceholderAttackUp    != null && up   != null) overrideAnimatorController[this.PlaceholderAttackUp]   = up;

            this.OverrideCache.Add(runtimeSkill.SkillID, overrideAnimatorController);
            this.Animator.runtimeAnimatorController = overrideAnimatorController;
        }
    }
}
