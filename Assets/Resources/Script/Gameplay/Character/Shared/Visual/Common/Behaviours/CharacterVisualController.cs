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
    [RequireComponent(typeof(CharacterVisualDodgeAnimationHandler))]
    public class CharacterVisualController : CharacterConfiguredBehaviourBase<CharacterVisualConfig>,
        IFacingProvider,
        ICharacterRefreshable
    {
        // --- References ---
        //  1. Core
        [Header("References (Core)")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _characterFlipGroup;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public Animator Animator => _animator;
        public Transform CharacterFlipGroup => _characterFlipGroup;

        //  2. Sub-behaviours
        [Header("References (Sub-Behaviours)")]
        [SerializeField] private CharacterVisualFacingHandler _facingHandler;
        [SerializeField] private CharacterVisualLocomotionAnimationHandler _locomotionAnimationHandler;
        [SerializeField] private CharacterVisualAttackAnimationHandler _attackAnimationHandler;
        [SerializeField] private CharacterVisualHitDeathAnimationHandler _hitDeathAnimationHandler;
        [SerializeField] private CharacterVisualDodgeAnimationHandler _dodgeAnimationHandler;
        private CharacterVisualFacingHandler FacingHandler => _facingHandler;
        private CharacterVisualLocomotionAnimationHandler LocomotionAnimationHandler => _locomotionAnimationHandler;
        private CharacterVisualAttackAnimationHandler AttackAnimationHandler => _attackAnimationHandler;
        private CharacterVisualHitDeathAnimationHandler HitDeathAnimationHandler => _hitDeathAnimationHandler;
        private CharacterVisualDodgeAnimationHandler DodgeAnimationHandler => _dodgeAnimationHandler;

        // --- Data & Configs ---
        public CharacterVisualConfig VisualConfig => this.Config;

        private Color NormalColor => this.VisualConfig != null
            ? this.VisualConfig.NormalColor
            : Color.white;

        public CharacterAttackAnimationProfile AttackAnimationProfile => this.VisualConfig != null ? this.VisualConfig.AttackAnimationProfile : null;

        // --- Runtime ---
        //  1. Animation Controller
        private RuntimeAnimatorController _baseAnimationController;
        private RuntimeAnimatorController BaseAnimationController => _baseAnimationController;

        private AnimatorOverrideController _runtimeOverrideController;
        public AnimatorOverrideController RuntimeOverrideController
        { 
            get => _runtimeOverrideController;
            private set => _runtimeOverrideController = value;
        }

        // --- Statics ---
        //  1. Animator Parameter IDs
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
                this.FacingHandler.SetFacingDuringRefresh(context.RefreshTarget.Facing);
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

        public void LockFacing()
        {
            if (this.FacingHandler == null) return;

            this.FacingHandler.LockFacing();
        }
        public void UnlockFacing()
        {
            if (this.FacingHandler == null) return;

            this.FacingHandler.UnlockFacing();
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

            this.ApplyAttackAnimationOverride(runtimeSkill, weapon, attackSideType);
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

            if (this.LocomotionAnimationHandler != null) this.LocomotionAnimationHandler.ApplyOverridesTo(this.RuntimeOverrideController);
            if (this.HitDeathAnimationHandler   != null) this.HitDeathAnimationHandler.ApplyOverridesTo(this.RuntimeOverrideController);
            if (this.DodgeAnimationHandler      != null) this.DodgeAnimationHandler.ApplyOverridesTo(this.RuntimeOverrideController);

            this.Animator.runtimeAnimatorController = this.RuntimeOverrideController;
        }
        public void ApplyAttackAnimationOverride(RuntimeSkill runtimeSkill, WeaponProfile weapon, AttackSide attackSideType)
        {
            if (this.Animator == null || this.BaseAnimationController == null) return;

            this.EnsureRuntimeOverrideController();

            if (this.LocomotionAnimationHandler != null) this.LocomotionAnimationHandler.ApplyOverridesTo(this.RuntimeOverrideController);
            if (this.HitDeathAnimationHandler   != null) this.HitDeathAnimationHandler.ApplyOverridesTo(this.RuntimeOverrideController);
            if (this.DodgeAnimationHandler      != null) this.DodgeAnimationHandler.ApplyOverridesTo(this.RuntimeOverrideController);
            if (this.AttackAnimationHandler     != null) this.AttackAnimationHandler.ApplyAttackOverridesTo(this.RuntimeOverrideController, runtimeSkill, weapon, attackSideType);

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

        private void EnsureRuntimeOverrideController()
        {
            if (this.Animator == null || this.BaseAnimationController == null) return;

            if (this.RuntimeOverrideController != null) return;

            this.RuntimeOverrideController = new AnimatorOverrideController(this.BaseAnimationController);
            this.Animator.runtimeAnimatorController = this.RuntimeOverrideController;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.SpriteRenderer == null) _spriteRenderer    = this.GetComponent<SpriteRenderer>();
            if (this.Animator       == null) _animator          = this.GetComponent<Animator>();

            if (this.CharacterFlipGroup == null) _characterFlipGroup = this.transform.parent;

            if (this.FacingHandler              == null) _facingHandler                 = this.GetComponent<CharacterVisualFacingHandler>();
            if (this.LocomotionAnimationHandler == null) _locomotionAnimationHandler    = this.GetComponent<CharacterVisualLocomotionAnimationHandler>();
            if (this.AttackAnimationHandler     == null) _attackAnimationHandler        = this.GetComponent<CharacterVisualAttackAnimationHandler>();
            if (this.HitDeathAnimationHandler   == null) _hitDeathAnimationHandler      = this.GetComponent<CharacterVisualHitDeathAnimationHandler>();
            if (this.DodgeAnimationHandler      == null) _dodgeAnimationHandler         = this.GetComponent<CharacterVisualDodgeAnimationHandler>();

            if (this.BaseAnimationController == null && this.Animator != null)
            {
                _baseAnimationController = this.Animator.runtimeAnimatorController;
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.SpriteRenderer == null) Debug.LogError($"[{this.gameObject.name}.{nameof(CharacterVisualController)}] {nameof(SpriteRenderer)} is missing.", this);
            if (this.Animator       == null) Debug.LogError($"[{this.gameObject.name}.{nameof(CharacterVisualController)}] {nameof(Animator)} is missing.", this);
            
            if (this.CharacterFlipGroup == null) Debug.LogError($"[{this.gameObject.name}.{nameof(CharacterVisualController)}] {nameof(CharacterFlipGroup)} is missing.", this);
            
            if (this.FacingHandler              == null) Debug.LogError($"[{this.gameObject.name}.{nameof(CharacterVisualController)}] {nameof(CharacterVisualFacingHandler)} is missing.", this);
            if (this.LocomotionAnimationHandler == null) Debug.LogError($"[{this.gameObject.name}.{nameof(CharacterVisualController)}] {nameof(CharacterVisualLocomotionAnimationHandler)} is missing.", this);
            if (this.AttackAnimationHandler     == null) Debug.LogError($"[{this.gameObject.name}.{nameof(CharacterVisualController)}] {nameof(CharacterVisualAttackAnimationHandler)} is missing.", this);
            if (this.HitDeathAnimationHandler   == null) Debug.LogError($"[{this.gameObject.name}.{nameof(CharacterVisualController)}] {nameof(CharacterVisualHitDeathAnimationHandler)} is missing.", this);
            if (this.DodgeAnimationHandler      == null) Debug.LogError($"[{this.gameObject.name}.{nameof(CharacterVisualController)}] {nameof(CharacterVisualDodgeAnimationHandler)} is missing.", this);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.FacingHandler?.Bind(this);
            this.LocomotionAnimationHandler?.Bind(this);
            this.AttackAnimationHandler?.Bind(this);
            this.HitDeathAnimationHandler?.Bind(this);
            this.DodgeAnimationHandler?.Bind(this);
        }
        protected override void UnbindInternalReferences()
        {
            this.FacingHandler?.Unbind(this);
            this.LocomotionAnimationHandler?.Unbind(this);
            this.AttackAnimationHandler?.Unbind(this);
            this.HitDeathAnimationHandler?.Unbind(this);
            this.DodgeAnimationHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
        protected override void InitializeGameplay()
        {
            base.InitializeGameplay();

            this.RebuildBaseAnimationOverride();
        }
    }
}
