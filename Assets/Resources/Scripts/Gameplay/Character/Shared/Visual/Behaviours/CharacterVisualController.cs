using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterVisualFacingHandler))]
    [RequireComponent(typeof(CharacterVisualLocomotionAnimationHandler))]
    [RequireComponent(typeof(CharacterVisualCombatAnimationHandler))]
    public class CharacterVisualController : CharacterBehaviourBaseController,
        IFacingProvider,
        ICharacterRespawnRefreshable
    {
        // --- States ---
        //  1. References
        [Header("References (Core)")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _characterFlipGroup;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public Animator Animator => _animator;
        public Transform CharacterFlipGroup => _characterFlipGroup;

        private CharacterVisualFacingHandler FacingHandler => this.CharacterComponentController.VisualFacingHandler;
        private CharacterVisualLocomotionAnimationHandler LocomotionAnimationHandler => this.CharacterComponentController.VisualLocomotionAnimationHandler;
        private CharacterVisualCombatAnimationHandler CombatAnimationHandler => this.CharacterComponentController.VisualCombatAnimationHandler;

        //  2. Animation Controller
        private RuntimeAnimatorController _baseAnimationController;
        private RuntimeAnimatorController BaseAnimationController => _baseAnimationController;

        private AnimatorOverrideController _runtimeOverrideController;
        public AnimatorOverrideController RuntimeOverrideController
        { 
            get => _runtimeOverrideController;
            private set => _runtimeOverrideController = value;
        }

        //  3. Animator Parameter IDs
        public static readonly int SpeedID = Animator.StringToHash("Speed");
        public static readonly int DirXID = Animator.StringToHash("DirX");
        public static readonly int DirYID = Animator.StringToHash("DirY");
        public static readonly int FacingID = Animator.StringToHash("Facing");
        public static readonly int DeadID = Animator.StringToHash("Dead");
        public static readonly int AttackID = Animator.StringToHash("Attack");
        public static readonly int HitID = Animator.StringToHash("Hit");

        // --- Unity Callbacks ---
        private void OnEnable()
        {
            this.InitializeHandlers();
            this.RebuildBaseAnimationOverride();
        }

        // --- Public APIs ---
        //  1. IFacingProvider implementation
        public CharacterFacingStatus FacingStatus => this.FacingHandler != null
            ? this.FacingHandler.FacingStatus
            : CharacterFacingStatus.Down;

        public bool IsFacingRight => this.FacingHandler == null || this.FacingHandler.IsFacingRight;

        //  2. ICharacterRespawnRefreshable implementation
        public int RespawnRefreshPriority => 50;
        public void RefreshAfterRespawn(CharacterRespawnRefreshContext context)
        {
            if (this.Animator == null) return;

            this.Animator.ResetTrigger(CharacterVisualController.AttackID);
            this.Animator.ResetTrigger(CharacterVisualController.HitID);
            this.SetDead(false);

            if (this.FacingHandler != null)
            {
                this.FacingHandler.SetRespawnFacing(context.RespawnFacing);
                this.FacingHandler.SetSpeedImmediate(0f);
            }

            this.RebuildBaseAnimationOverride();
        }

        //  3. Facing-related
        public void FeedMovement(Vector2 worldVelocity)
        {
            if (this.FacingHandler == null) return;

            this.FacingHandler.FeedMovement(worldVelocity);
        }

        public void SetFacingParameter(int facing)
        {
            if (this.FacingHandler == null) return;

            this.FacingHandler.SetFacingParameter(facing);
        }

        public void SetFacingParameter(CharacterFacingStatus facingStatus)
        {
            if (this.FacingHandler == null) return;

            this.FacingHandler.SetFacingParameter(facingStatus);
        }

        public void ForceFacing(Vector2 direction)
        {
            if (this.FacingHandler == null) return;

            this.FacingHandler.ForceFacing(direction);
        }

        //  4. Locomotion-related
        public void SetLocomotionMode(CharacterLocomotionMode locomotionMode)
        {
            if (this.LocomotionAnimationHandler == null) return;

            this.LocomotionAnimationHandler.SetManualLocomotionMode(locomotionMode);
        }

        public void RefreshLocomotionMode()
        {
            if (this.LocomotionAnimationHandler == null) return;

            this.LocomotionAnimationHandler.RefreshResolvedLocomotionMode();
        }

        //  5. Combat-related
        public void TriggerAttack(RuntimeSkill runtimeSkill, WeaponProfile weapon, AttackSide attackSideType)
        {
            if (runtimeSkill == null) return;
            if (this.Animator == null) return;

            this.ApplyCombatAttackAnimationOverride(runtimeSkill, weapon, attackSideType);
            this.Animator.SetTrigger(CharacterVisualController.AttackID);
        }

        public void TriggerHit()
        {
            if (this.Animator == null) return;

            this.Animator.SetTrigger(CharacterVisualController.HitID);
        }

        public void SetDead(bool isDead)
        {
            if (this.Animator == null) return;

            this.Animator.SetBool(CharacterVisualController.DeadID, isDead);
        }

        //  6. Animation Controller-related
        public void RebuildBaseAnimationOverride()
        {
            if (this.Animator == null || this.BaseAnimationController == null) return;

            this.EnsureRuntimeOverrideController();

            if (this.LocomotionAnimationHandler != null)
            {
                this.LocomotionAnimationHandler.ApplyOverridesTo(this.RuntimeOverrideController);
            }

            if (this.CombatAnimationHandler != null)
            {
                this.CombatAnimationHandler.ApplyHitAndDieOverridesTo(this.RuntimeOverrideController);
            }

            this.Animator.runtimeAnimatorController = this.RuntimeOverrideController;
        }

        public void ApplyCombatAttackAnimationOverride(RuntimeSkill runtimeSkill, WeaponProfile weapon, AttackSide attackSideType)
        {
            if (this.Animator == null || this.BaseAnimationController == null) return;

            this.EnsureRuntimeOverrideController();

            if (this.LocomotionAnimationHandler != null)
            {
                this.LocomotionAnimationHandler.ApplyOverridesTo(this.RuntimeOverrideController);
            }

            if (this.CombatAnimationHandler != null)
            {
                this.CombatAnimationHandler.ApplyHitAndDieOverridesTo(this.RuntimeOverrideController);
                this.CombatAnimationHandler.ApplyAttackOverridesTo(this.RuntimeOverrideController, runtimeSkill, weapon, attackSideType);
            }

            this.Animator.runtimeAnimatorController = this.RuntimeOverrideController;
        }

        // --- Helpers ---
        private void InitializeHandlers()
        {
            if (this.FacingHandler != null)
            {
                this.FacingHandler.Initialize(this);
            }

            if (this.LocomotionAnimationHandler != null)
            {
                this.LocomotionAnimationHandler.Initialize(this);
            }

            if (this.CombatAnimationHandler != null)
            {
                this.CombatAnimationHandler.Initialize(this);
            }
        }

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.SpriteRenderer == null) _spriteRenderer = this.GetComponent<SpriteRenderer>();
            if (this.Animator == null) _animator = this.GetComponent<Animator>();
            if (this.CharacterFlipGroup == null) _characterFlipGroup = this.transform.parent;

            if (this.BaseAnimationController == null && this.Animator != null) _baseAnimationController = this.Animator.runtimeAnimatorController;
        }

        private void EnsureRuntimeOverrideController()
        {
            if (this.Animator == null || this.BaseAnimationController == null) return;

            if (this.RuntimeOverrideController != null) return;

            this.RuntimeOverrideController = new AnimatorOverrideController(this.BaseAnimationController);
            this.Animator.runtimeAnimatorController = this.RuntimeOverrideController;
        }
    }
}
