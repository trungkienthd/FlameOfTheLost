using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimationHandler : MonoBehaviour, IFacingProvider
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _characterFlipGroup;
        [SerializeField] private CharacterCombatLoadoutController _combatLoadoutController;
        [SerializeField] private PlayerInteractionLoadoutController _playerInteractionLoadoutController;    // Player only
        private SpriteRenderer SpriteRenderer => _spriteRenderer;
        private Animator Animator => _animator;
        private Transform CharacterFlipGroup => _characterFlipGroup;

        private CharacterCombatLoadoutController CombatLoadoutController => _combatLoadoutController;
        private PlayerInteractionLoadoutController PlayerInteractionLoadoutController => _playerInteractionLoadoutController;

        //  2. Settings
        [Header("Settings")]
        [SerializeField] private float _speedForOne = 10f;      // World units per second that map to Speed=1 in the Locomotion blend trees
        [SerializeField] private float _speedDampTime = 0.05f;  // Damp time for Speed parameter smoothing
        private float SpeedForOne => _speedForOne;
        private float SpeedDampTime => _speedDampTime;

        //  3. Data
        [Header("Animation Data")]
        [SerializeField] private CharacterLocomotionAnimationProfile _locomotionPlaceholderProfile;
        [SerializeField] private CharacterLocomotionAnimationProfile _locomotionRuntimeProfile;
        [SerializeField] private CharacterCombatAnimationProfile _combatAnimationProfile;
        private CharacterLocomotionAnimationProfile GenericLocomotionPlaceholderProfile => _locomotionPlaceholderProfile;
        private CharacterLocomotionAnimationProfile CharacterLocomotionRuntimeProfile => _locomotionRuntimeProfile;
        public CharacterCombatAnimationProfile CharacterCombatAnimationProfile => _combatAnimationProfile;

        //  4. Runtime state
        [Header("Runtime State")]
        [SerializeField] private Vector2 _lastDir = Vector2.down;
        [SerializeField] private CharacterLocomotionModeType _currentLocomotionMode = CharacterLocomotionModeType.Default;
        public Vector2 LastDir
        {
            get => _lastDir;
            private set => _lastDir = value;
        }
        public int FacingSignX { get; private set; } = +1;

        public CharacterLocomotionModeType CurrentLocomotionMode
        {
            get => _currentLocomotionMode;
            private set => _currentLocomotionMode = value;
        }

        public CharacterCarryItemType CurrentCarryItemType
        {
            get
            {
                if (this.PlayerInteractionLoadoutController == null)
                {
                    return CharacterCarryItemType.None;
                }

                return this.PlayerInteractionLoadoutController.GetCharacterCarryItemType();
            }
        }
        public LightSourceType CurrentLightSourceType
        {
            get
            {
                if (this.PlayerInteractionLoadoutController == null)
                {
                    return LightSourceType.None;
                }

                return this.PlayerInteractionLoadoutController.GetCurrentLightSourceType();
            }
        }

        //  5. Animation Controller (important: avoid chaining overrides)
        private RuntimeAnimatorController _baseAnimationController;
        private RuntimeAnimatorController BaseAnimationController
        {
            get => _baseAnimationController;
            set => _baseAnimationController = value;
        }

        //  6. Cache per-skill override to avoid rebuilding every attack
        private readonly Dictionary<CombatAnimationKey, AnimatorOverrideController> _overrideCache = new();

        //  7. Placeholder clips
        //      7.1. Attack animation clips (placeholders from base controller)
        [Header("Attack Placeholder Clips (from base controller)")]
        [SerializeField] private AnimationClip _placeholderAttackDown;
        [SerializeField] private AnimationClip _placeholderAttackSide;
        [SerializeField] private AnimationClip _placeholderAttackUp;
        public AnimationClip PlaceholderAttackDown => _placeholderAttackDown;
        public AnimationClip PlaceholderAttackSide => _placeholderAttackSide;
        public AnimationClip PlaceholderAttackUp => _placeholderAttackUp;

        //      7.2. Hit placeholder clips (from base controller)
        [Header("Hit Placeholder Clips (from base controller)")]
        [SerializeField] private AnimationClip _placeholderHitDown;
        [SerializeField] private AnimationClip _placeholderHitSide;
        [SerializeField] private AnimationClip _placeholderHitUp;
        public AnimationClip PlaceholderHitDown => _placeholderHitDown;
        public AnimationClip PlaceholderHitSide => _placeholderHitSide;
        public AnimationClip PlaceholderHitUp => _placeholderHitUp;

        //      7.3. Die placeholder clips (from base controller)]
        [Header("Die Placeholder Clips (from base controller)")]
        [SerializeField] private AnimationClip _placeholderDieDown;
        [SerializeField] private AnimationClip _placeholderDieSide;
        [SerializeField] private AnimationClip _placeholderDieUp;
        public AnimationClip PlaceholderDieDown => _placeholderDieDown;
        public AnimationClip PlaceholderDieSide => _placeholderDieSide;
        public AnimationClip PlaceholderDieUp => _placeholderDieUp;

        //  8. Animator Parameter IDs
        public static readonly int SpeedID = Animator.StringToHash("Speed");
        public static readonly int DirXID = Animator.StringToHash("DirX");
        public static readonly int DirYID = Animator.StringToHash("DirY");
        public static readonly int FacingID = Animator.StringToHash("Facing");
        public static readonly int DeadID = Animator.StringToHash("Dead");
        public static readonly int AttackID = Animator.StringToHash("Attack");
        public static readonly int HitID = Animator.StringToHash("Hit");

        //  9. Facing states
        public FacingStatusType FacingStatus
        {
            get
            {
                // If you store last move vector or have an int param:
                int facingInt = this.Animator.GetInteger(CharacterAnimationHandler.FacingID); // 0=Down,1=Side,2=Up
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
                float dx = this.Animator.GetFloat(CharacterAnimationHandler.DirXID);
                if (Mathf.Abs(dx) > 0.001f) return dx > 0f;
                return true; // default to right if unknown
            }
        }

        // --- Unity methods ---
        private void Awake()
        {
            if (this.SpriteRenderer == null) _spriteRenderer = this.GetComponent<SpriteRenderer>();
            if (this.Animator == null) _animator = this.GetComponent<Animator>();
            if (this.CharacterFlipGroup == null) _characterFlipGroup = this.transform.parent;
            if (this.CombatLoadoutController == null) _combatLoadoutController = this.GetComponentInParent<CharacterCombatLoadoutController>();

            if (this.PlayerInteractionLoadoutController == null) _playerInteractionLoadoutController = this.GetComponentInParent<PlayerInteractionLoadoutController>();

            if (this.GenericLocomotionPlaceholderProfile == null) _locomotionPlaceholderProfile = Resources.Load<CharacterLocomotionAnimationProfile>("Data/Animations/Locomotion/Generic/LAni_Generic");
            if (this.PlaceholderAttackDown  == null) _placeholderAttackDown = Resources.Load<AnimationClip>("Animations/Characters/Generic/Aclip_Generic_Combat_Down_Placeholder");
            if (this.PlaceholderAttackSide  == null) _placeholderAttackSide = Resources.Load<AnimationClip>("Animations/Characters/Generic/Aclip_Generic_Combat_Side_Placeholder");
            if (this.PlaceholderAttackUp    == null) _placeholderAttackUp   = Resources.Load<AnimationClip>("Animations/Characters/Generic/Aclip_Generic_Combat_Up_Placeholder");
            if (this.PlaceholderHitDown     == null) _placeholderHitDown    = Resources.Load<AnimationClip>("Animations/Characters/Generic/Aclip_Generic_Hit_Down_Placeholder");
            if (this.PlaceholderHitSide     == null) _placeholderHitSide    = Resources.Load<AnimationClip>("Animations/Characters/Generic/Aclip_Generic_Hit_Side_Placeholder");
            if (this.PlaceholderHitUp       == null) _placeholderHitUp      = Resources.Load<AnimationClip>("Animations/Characters/Generic/Aclip_Generic_Hit_Up_Placeholder");
            if (this.PlaceholderDieSide     == null) _placeholderDieSide    = Resources.Load<AnimationClip>("Animations/Characters/Generic/Aclip_Generic_Die_Side_Placeholder");

            if (this.BaseAnimationController == null && this.Animator != null) this.BaseAnimationController = this.Animator.runtimeAnimatorController;
        }
        private void OnEnable()
        {
            this.BindPlayerInteractionEvents();
            this.RefreshLocomotionMode();
            this.ApplyFixedCharacterAnimationOverride();
            this.ApplyLocomotionAnimationOverride();
        }
        private void OnDisable()
        {
            this.UnbindPlayerInteractionEvents();
        }

        // --- Public APIs ---
        //  1. Setting facing direction directly
        public void SetFacing(int facing)
        {
            this.Animator.SetInteger(CharacterAnimationHandler.FacingID, facing);
        }
        public void SetFacing(FacingStatusType facing)
        {
            this.SetFacing((int)facing);
        }

        //  2. Setting locomotion mode (which can switch between different blend trees or states in the animator)
        public void SetLocomotionMode(CharacterLocomotionModeType locomotionMode)
        {
            this.CurrentLocomotionMode = locomotionMode;
        }

        public void RefreshLocomotionMode()
        {
            CharacterLocomotionModeType resolvedMode = CharacterLocomotionModeType.Default;
            if ((this.PlayerInteractionLoadoutController != null) && this.PlayerInteractionLoadoutController.IsCarryingInteractableItem) resolvedMode = CharacterLocomotionModeType.Carry;

            this.SetLocomotionMode(resolvedMode);
        }

        //  3. Feeding movement input
        public void FeedMovement(Vector2 worldVel)
        {
            // Keep the last non-zero direction
            if (worldVel.sqrMagnitude > 0.0001f)
            {
                this.LastDir = worldVel.normalized;
            }

            // Smooth speed in [0..1] range
            float speed01 = (this.SpeedForOne > 0f) ? Mathf.Clamp01(worldVel.magnitude / this.SpeedForOne) : 0f; // Scale: Assuming 10 is the max speed
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

        //  4. Combat animation triggers
        //      4.1. Trigger generic attack
        public void TriggerAttack(RuntimeSkill runtimeSkill, AttackSideType attackSideType)
        {
            if (runtimeSkill == null) return;

            // Apply correct animator override for this skill
            this.ApplyCombatAnimationOverride(runtimeSkill, attackSideType);

            this.Animator.SetTrigger(CharacterAnimationHandler.AttackID);
        }
        //      4.2. Trigger hit reaction
        public void TriggerHit()
        {
            this.Animator.SetTrigger(CharacterAnimationHandler.HitID);
        }
        //      4.3. Set dead/alive state
        public void SetDead(bool isDead)
        {
            this.Animator.SetBool(CharacterAnimationHandler.DeadID, isDead);
        }

        //  5. IFacingProvider implementation
        public void ForceFacing(Vector2 dir)
        {
            if (dir.sqrMagnitude <= 0.0001f) return;
            this.LastDir = dir.normalized;
        }

        // --- Helpers ---
        private void BindPlayerInteractionEvents()
        {
            if (this.PlayerInteractionLoadoutController == null) return;

            this.PlayerInteractionLoadoutController.OnInteractableItemCarryingStateChanged += this.OnInteractableItemCarryingStateChanged;
            this.PlayerInteractionLoadoutController.OnEquippedInteractableItemChanged += this.OnEquippedInteractableItemChanged;
        }

        private void UnbindPlayerInteractionEvents()
        {
            if (this.PlayerInteractionLoadoutController == null) return;

            this.PlayerInteractionLoadoutController.OnInteractableItemCarryingStateChanged -= this.OnInteractableItemCarryingStateChanged;
            this.PlayerInteractionLoadoutController.OnEquippedInteractableItemChanged -= this.OnEquippedInteractableItemChanged;
        }

        private void OnInteractableItemCarryingStateChanged(bool isCarrying)
        {
            this.RefreshLocomotionMode();
            this.ApplyLocomotionAnimationOverride();
        }

        private void OnEquippedInteractableItemChanged(InventoryItemEntry interactableEntry)
        {
            // In case item changes while the carrying state is still true/false,
            // keep animator locomotion state synced.
            this.RefreshLocomotionMode();
            this.ApplyLocomotionAnimationOverride();
        }

        private void ApplyFixedCharacterAnimationOverride()
        {
            if (this.Animator == null || this.BaseAnimationController == null) return;

            var overrideController = new AnimatorOverrideController(this.BaseAnimationController);

            // Fixed per-character clips
            this.ApplyCharacterFixedOverridesTo(overrideController);

            this.Animator.runtimeAnimatorController = overrideController;

            _overrideCache.Clear();
        }

        private void ApplyCharacterFixedOverridesTo(AnimatorOverrideController overrideController)
        {
            if (overrideController == null) return;
            if (this.CharacterCombatAnimationProfile == null) return;

            FacingAnimationClipSet hitSet = this.CharacterCombatAnimationProfile.HitAnimationClipSet;
            FacingAnimationClipSet dieSet = this.CharacterCombatAnimationProfile.DieAnimationClipSet;

            if (!hitSet.IsValid) return;
            if (!dieSet.IsValid) return;

            // Hit
            if (this.PlaceholderHitDown != null && hitSet.Down  != null) overrideController[this.PlaceholderHitDown]    = hitSet.Down;
            if (this.PlaceholderHitSide != null && hitSet.Side  != null) overrideController[this.PlaceholderHitSide]    = hitSet.Side;
            if (this.PlaceholderHitUp   != null && hitSet.Up    != null) overrideController[this.PlaceholderHitUp]      = hitSet.Up;

            // Die
            if (this.PlaceholderDieDown != null && dieSet.Down  != null) overrideController[this.PlaceholderDieDown]    = dieSet.Down;
            if (this.PlaceholderDieSide != null && dieSet.Side  != null) overrideController[this.PlaceholderDieSide]    = dieSet.Side;
            if (this.PlaceholderDieUp   != null && dieSet.Up    != null) overrideController[this.PlaceholderDieUp]      = dieSet.Up;
        }

        private void ApplyLocomotionAnimationOverride()
        {
            if (this.Animator == null || this.BaseAnimationController == null) return;

            var overrideController = new AnimatorOverrideController(this.BaseAnimationController);

            // Always preserve fixed character clips
            this.ApplyCharacterFixedOverridesTo(overrideController);

            // Then apply the current locomotion mapping onto a fresh override
            this.ApplyCurrentLocomotionOverridesTo(overrideController);

            this.Animator.runtimeAnimatorController = overrideController;

            _overrideCache.Clear();
        }

        private void ApplyCurrentLocomotionOverridesTo(AnimatorOverrideController overrideController)
        {
            if (this.Animator == null || this.BaseAnimationController == null) return;
            if (this.GenericLocomotionPlaceholderProfile == null) return;
            if (this.CharacterLocomotionRuntimeProfile == null) return;

            CharacterLocomotionModeType locomotionMode = this.CurrentLocomotionMode;
            CharacterCarryItemType carryItemType = this.CurrentCarryItemType;
            LightSourceType lightSourceType = this.CurrentLightSourceType;

            // 1. Resolve REAL clips from the character runtime profile
            AnimationClip realIdleDown  = this.CharacterLocomotionRuntimeProfile.GetIdleClip(FacingStatusType.Down, locomotionMode, carryItemType, lightSourceType);
            AnimationClip realIdleSide  = this.CharacterLocomotionRuntimeProfile.GetIdleClip(FacingStatusType.Side, locomotionMode, carryItemType, lightSourceType);
            AnimationClip realIdleUp    = this.CharacterLocomotionRuntimeProfile.GetIdleClip(FacingStatusType.Up, locomotionMode, carryItemType, lightSourceType);
            AnimationClip realWalkDown  = this.CharacterLocomotionRuntimeProfile.GetWalkClip(FacingStatusType.Down, locomotionMode, carryItemType, lightSourceType);
            AnimationClip realWalkSide  = this.CharacterLocomotionRuntimeProfile.GetWalkClip(FacingStatusType.Side, locomotionMode, carryItemType, lightSourceType);
            AnimationClip realWalkUp    = this.CharacterLocomotionRuntimeProfile.GetWalkClip(FacingStatusType.Up, locomotionMode, carryItemType, lightSourceType);

            // 2. Read the 6 placeholder clips from LA_Generic (Default only)
            AnimationClip phIdleDown    = this.GenericLocomotionPlaceholderProfile.GetIdleClip(FacingStatusType.Down);
            AnimationClip phIdleSide    = this.GenericLocomotionPlaceholderProfile.GetIdleClip(FacingStatusType.Side);
            AnimationClip phIdleUp      = this.GenericLocomotionPlaceholderProfile.GetIdleClip(FacingStatusType.Up);
            AnimationClip phWalkDown    = this.GenericLocomotionPlaceholderProfile.GetWalkClip(FacingStatusType.Down);
            AnimationClip phWalkSide    = this.GenericLocomotionPlaceholderProfile.GetWalkClip(FacingStatusType.Side);
            AnimationClip phWalkUp      = this.GenericLocomotionPlaceholderProfile.GetWalkClip(FacingStatusType.Up);

            // 4. Override only these 6 placeholders
            if (phIdleDown  != null && realIdleDown != null) overrideController[phIdleDown] = realIdleDown;
            if (phIdleSide  != null && realIdleSide != null) overrideController[phIdleSide] = realIdleSide;
            if (phIdleUp    != null && realIdleUp   != null) overrideController[phIdleUp]   = realIdleUp;

            if (phWalkDown  != null && realWalkDown != null) overrideController[phWalkDown] = realWalkDown;
            if (phWalkSide  != null && realWalkSide != null) overrideController[phWalkSide] = realWalkSide;
            if (phWalkUp    != null && realWalkUp   != null) overrideController[phWalkUp]   = realWalkUp;
        }

        private void ApplyCombatAnimationOverride(RuntimeSkill runtimeSkill, AttackSideType attackSideType)
        {
            if (runtimeSkill == null) return;
            if (this.CharacterCombatAnimationProfile == null) return;

            WeaponProfile currentWeapon = null;
            if (this.CombatLoadoutController != null)
            {
                RuntimeWeapon activeRuntimeWeapon = this.CombatLoadoutController.ActiveEquippedRuntimeWeapon;
                if (activeRuntimeWeapon != null)
                {
                    currentWeapon = activeRuntimeWeapon.Profile;
                }
            }

            if (currentWeapon == null) return;

            if (!this.CharacterCombatAnimationProfile.TryGetElement(runtimeSkill.Profile, currentWeapon, out CharacterCombatAnimationClipHitboxSetProfile element) || element == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.CharacterAnimationHandler] No combat animation entry found for skill '{runtimeSkill.Profile?.name}' and weapon '{currentWeapon.name}'.", this);
                return;
            }

            var key = new CombatAnimationKey(runtimeSkill.SkillID, currentWeapon.ID, attackSideType);
            if (_overrideCache.TryGetValue(key, out AnimatorOverrideController cached))
            {
                this.Animator.runtimeAnimatorController = cached;
                return;
            }

            if (this.BaseAnimationController == null)
            {
                this.BaseAnimationController = this.Animator.runtimeAnimatorController;
            }

            // IMPORTANT:
            // Build from the true base controller only, then re-apply locomotion overrides first,
            // then apply attack overrides on top.
            var overrideAnimatorController = new AnimatorOverrideController(this.BaseAnimationController);

            // Re-apply current overrides so other states (locomotion, hit, die, e.t.c) do not fall back to placeholders
            this.ApplyCharacterFixedOverridesTo(overrideAnimatorController);
            this.ApplyCurrentLocomotionOverridesTo(overrideAnimatorController);

            // Select the correct clip set by side (Left/Right)
            var animationClipSet = element.GetFacingAnimationClipSet(attackSideType);
            if (!animationClipSet.IsValid)
            {
                Debug.LogWarning($"[{this.gameObject.name}.CharacterAnimationHandler] Facing animation clip set is not valid for skill '{runtimeSkill.Profile?.name}' and weapon '{currentWeapon.name}'.", this);
                return;
            }

            var down    = animationClipSet.Down;
            var side    = animationClipSet.Side;
            var up      = animationClipSet.Up;

            // Override placeholders -> skill clips
            if (this.PlaceholderAttackDown  != null && down != null) overrideAnimatorController[this.PlaceholderAttackDown] = down;
            if (this.PlaceholderAttackSide  != null && side != null) overrideAnimatorController[this.PlaceholderAttackSide] = side;
            if (this.PlaceholderAttackUp    != null && up   != null) overrideAnimatorController[this.PlaceholderAttackUp]   = up;

            _overrideCache.Add(key, overrideAnimatorController);
            this.Animator.runtimeAnimatorController = overrideAnimatorController;
        }
    }
}
