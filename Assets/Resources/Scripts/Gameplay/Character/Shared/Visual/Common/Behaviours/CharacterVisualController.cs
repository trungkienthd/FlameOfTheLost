using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterVisualFacingHandler))]
    [RequireComponent(typeof(CharacterVisualLocomotionAnimationHandler))]
    [RequireComponent(typeof(CharacterVisualAttackAnimationHandler))]
    [RequireComponent(typeof(CharacterVisualHitDeathAnimationHandler))]
    public class CharacterVisualController : CharacterBehaviourControllerBase,
        IFacingProvider,
        ICharacterRefreshable
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

        private CharacterVisualFacingHandler FacingHandler => this.CharacterRootController.VisualFacingHandler;
        private CharacterVisualLocomotionAnimationHandler LocomotionAnimationHandler => this.CharacterRootController.VisualLocomotionAnimationHandler;
        private CharacterVisualAttackAnimationHandler AttackAnimationHandler => this.CharacterRootController.VisualAttackAnimationHandler;
        private CharacterVisualHitDeathAnimationHandler HitDeathAnimationHandler => this.CharacterRootController.VisualHitDeathAnimationHandler;
        private CharacterVisualDodgeAnimationHandler DodgeAnimationHandler => this.CharacterRootController.VisualDodgeAnimationHandler;

        //  2. Color configuration
        [Header("Visual Color")]
        [SerializeField] private Color _normalColor = Color.white;
        public Color NormalColor => _normalColor;

        //  3. Animation Controller
        private RuntimeAnimatorController _baseAnimationController;
        private RuntimeAnimatorController BaseAnimationController => _baseAnimationController;

        private AnimatorOverrideController _runtimeOverrideController;
        public AnimatorOverrideController RuntimeOverrideController
        { 
            get => _runtimeOverrideController;
            private set => _runtimeOverrideController = value;
        }

        //  4. Animator Parameter IDs
        public static readonly int SpeedID = Animator.StringToHash("Speed");
        public static readonly int DirXID = Animator.StringToHash("DirX");
        public static readonly int DirYID = Animator.StringToHash("DirY");
        public static readonly int FacingID = Animator.StringToHash("Facing");
        public static readonly int DeadID = Animator.StringToHash("Dead");
        public static readonly int DieID = Animator.StringToHash("Die");
        public static readonly int AttackID = Animator.StringToHash("Attack");
        public static readonly int HitID = Animator.StringToHash("Hit");
        public static readonly int DodgeID = Animator.StringToHash("Dodge");

        // --- Events ---
        public event Action OnDeathAnimationFinished
        {
            add
            {
                if (this.HitDeathAnimationHandler == null) return;
                this.HitDeathAnimationHandler.OnDeathAnimationFinished += value;
            }
            remove
            {
                if (this.HitDeathAnimationHandler == null) return;
                this.HitDeathAnimationHandler.OnDeathAnimationFinished -= value;
            }
        }

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

        //  2. ICharacterPlacementRefreshable implementation
        public int RefreshmentConfigurePriority => 50;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            if (this.Animator == null) return;

            this.HitDeathAnimationHandler?.ResetSequenceState();

            this.Animator.ResetTrigger(CharacterVisualController.AttackID);
            this.Animator.ResetTrigger(CharacterVisualController.HitID);
            this.Animator.ResetTrigger(CharacterVisualController.DieID);
            this.Animator.ResetTrigger(CharacterVisualController.DodgeID);

            this.SetDead(false);

            this.ResetVisualColor();

            if (this.FacingHandler != null)
            {
                this.FacingHandler.SetRespawnFacing(context.RefreshTarget.Facing);
                this.FacingHandler.SetSpeedImmediate(0f);
            }

            this.RebuildBaseAnimationOverride();

            // Double-check
            this.ForceAnimatorRefreshAfterCharacterRefreshment();
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

        //  5. Attack-related
        public void TriggerAttack(RuntimeSkill runtimeSkill, WeaponProfile weapon, AttackSide attackSideType)
        {
            if (runtimeSkill == null) return;
            if (this.Animator == null) return;

            this.ApplyAttackAttackAnimationOverride(runtimeSkill, weapon, attackSideType);
            this.Animator.SetTrigger(CharacterVisualController.AttackID);
        }

        //  6. Hit/Death-related
        public void PlayHitSequence(bool shouldPlayDieAfterHit)
        {
            if (this.HitDeathAnimationHandler == null) return;

            this.HitDeathAnimationHandler.PlayHitSequence(shouldPlayDieAfterHit);
        }
        public void PlaySelfCostSequence(bool shouldPlayDieAfterSelfCost)
        {
            if (this.HitDeathAnimationHandler == null) return;

            this.HitDeathAnimationHandler.PlaySelfCostSequence(shouldPlayDieAfterSelfCost);
        }

        public void TriggerHit()
        {
            if (this.HitDeathAnimationHandler == null) return;

            this.HitDeathAnimationHandler.TriggerHit();
        }
        public void TriggerDie()
        {
            if (this.HitDeathAnimationHandler == null) return;

            this.HitDeathAnimationHandler.TriggerDeath();
        }
        public void SetDead(bool isDead)
        {
            if (this.Animator == null) return;

            this.Animator.SetBool(CharacterVisualController.DeadID, isDead);
        }

        //  7. Dodge-related
        public void TriggerDodge()
        {
            if (this.DodgeAnimationHandler == null) return;

            this.DodgeAnimationHandler.TriggerDodge();
        }
        public void TriggerDodge(Vector2 dodgeDirection)
        {
            if (this.DodgeAnimationHandler == null) return;

            this.DodgeAnimationHandler.TriggerDodge(dodgeDirection);
        }

        //  8. Animation Controller-related
        public void RebuildBaseAnimationOverride()
        {
            if (this.Animator == null || this.BaseAnimationController == null) return;

            this.EnsureRuntimeOverrideController();

            if (this.LocomotionAnimationHandler != null)
            {
                this.LocomotionAnimationHandler.ApplyOverridesTo(this.RuntimeOverrideController);
            }

            if (this.HitDeathAnimationHandler != null)
            {
                this.HitDeathAnimationHandler.ApplyOverridesTo(this.RuntimeOverrideController);
            }

            if (this.DodgeAnimationHandler != null)
            {
                this.DodgeAnimationHandler.ApplyOverridesTo(this.RuntimeOverrideController);
            }

            this.Animator.runtimeAnimatorController = this.RuntimeOverrideController;
        }
        public void ApplyAttackAttackAnimationOverride(RuntimeSkill runtimeSkill, WeaponProfile weapon, AttackSide attackSideType)
        {
            if (this.Animator == null || this.BaseAnimationController == null) return;

            this.EnsureRuntimeOverrideController();

            if (this.LocomotionAnimationHandler != null)
            {
                this.LocomotionAnimationHandler.ApplyOverridesTo(this.RuntimeOverrideController);
            }

            if (this.HitDeathAnimationHandler != null)
            {
                this.HitDeathAnimationHandler.ApplyOverridesTo(this.RuntimeOverrideController);
            }

            if (this.DodgeAnimationHandler != null)
            {
                this.DodgeAnimationHandler.ApplyOverridesTo(this.RuntimeOverrideController);
            }

            if (this.AttackAnimationHandler != null)
            {
                this.AttackAnimationHandler.ApplyAttackOverridesTo(this.RuntimeOverrideController, runtimeSkill, weapon, attackSideType);
            }

            this.Animator.runtimeAnimatorController = this.RuntimeOverrideController;
        }

        //  9. Manage color
        public void SetVisualColor(Color color)
        {
            if (this.SpriteRenderer == null) return;

            this.SpriteRenderer.color = color;
        }
        public void ResetVisualColor()
        {
            this.SetVisualColor(this.NormalColor);
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

            if (this.AttackAnimationHandler != null)
            {
                this.AttackAnimationHandler.Initialize(this);
            }

            if (this.HitDeathAnimationHandler != null)
            {
                this.HitDeathAnimationHandler.Initialize(this);
            }

            if (this.DodgeAnimationHandler != null)
            {
                this.DodgeAnimationHandler.Initialize(this);
            }
        }

        private void ForceAnimatorRefreshAfterCharacterRefreshment()
        {
            if (this.Animator == null) return;

            this.Animator.ResetTrigger(CharacterVisualController.AttackID);
            this.Animator.ResetTrigger(CharacterVisualController.HitID);
            this.Animator.ResetTrigger(CharacterVisualController.DieID);
            this.Animator.ResetTrigger(CharacterVisualController.DodgeID);

            this.Animator.SetBool(CharacterVisualController.DeadID, false);
            this.Animator.SetFloat(CharacterVisualController.SpeedID, 0f);

            this.Animator.Update(0f);
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

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
