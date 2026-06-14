using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterVisualFacingHandler))]
    [RequireComponent(typeof(CharacterVisualLocomotionAnimationHandler))]
    [RequireComponent(typeof(CharacterVisualCombatAnimationHandler))]
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

        [Header("Visual Color")]
        [SerializeField] private Color _normalColor = Color.white;
        public Color NormalColor => _normalColor;

        private CharacterVisualFacingHandler FacingHandler => this.CharacterRootController.VisualFacingHandler;
        private CharacterVisualLocomotionAnimationHandler LocomotionAnimationHandler => this.CharacterRootController.VisualLocomotionAnimationHandler;
        private CharacterVisualCombatAnimationHandler CombatAnimationHandler => this.CharacterRootController.VisualCombatAnimationHandler;

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
        public static readonly int DieID = Animator.StringToHash("Die");
        public static readonly int AttackID = Animator.StringToHash("Attack");
        public static readonly int HitID = Animator.StringToHash("Hit");

        // --- Events ---
        public event Action OnDieAnimationFinished;

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
        public void RefreshCharacter(CharacterPlacementContext context)
        {
            if (this.Animator == null) return;

            this.Animator.ResetTrigger(CharacterVisualController.AttackID);
            this.Animator.ResetTrigger(CharacterVisualController.HitID);
            this.Animator.ResetTrigger(CharacterVisualController.DieID);

            this.SetDead(false);

            this.ResetVisualColor();

            if (this.FacingHandler != null)
            {
                this.FacingHandler.SetRespawnFacing(context.PlacementFacing);
                this.FacingHandler.SetSpeedImmediate(0f);
            }

            this.RebuildBaseAnimationOverride();

            // Double-check
            this.ForceAnimatorRefreshAfterPlacement();
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
        public void TriggerDie()
        {
            if (this.Animator == null) return;

            this.Animator.ResetTrigger(CharacterVisualController.AttackID);
            this.Animator.ResetTrigger(CharacterVisualController.HitID);
            this.Animator.ResetTrigger(CharacterVisualController.DieID);

            this.Animator.SetFloat(CharacterVisualController.SpeedID, 0f);

            this.Animator.SetBool(CharacterVisualController.DeadID, true);

            this.Animator.SetTrigger(CharacterVisualController.DieID);
        }
        public void SetDead(bool isDead)
        {
            if (this.Animator == null) return;

            this.Animator.SetBool(CharacterVisualController.DeadID, isDead);
        }
        public void NotifyDieAnimationFinished()
        {
            this.OnDieAnimationFinished?.Invoke();
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

        //  7. Manage color
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

            if (this.CombatAnimationHandler != null)
            {
                this.CombatAnimationHandler.Initialize(this);
            }
        }

        private void ForceAnimatorRefreshAfterPlacement()
        {
            if (this.Animator == null) return;

            this.Animator.ResetTrigger(CharacterVisualController.AttackID);
            this.Animator.ResetTrigger(CharacterVisualController.HitID);
            this.Animator.ResetTrigger(CharacterVisualController.DieID);

            this.Animator.SetBool(CharacterVisualController.DeadID, false);
            this.Animator.SetFloat(CharacterVisualController.SpeedID, 0f);

            this.Animator.Update(0f);
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
