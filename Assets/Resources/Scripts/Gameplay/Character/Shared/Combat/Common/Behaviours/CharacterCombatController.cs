using System.Collections;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(CharacterCombatResourcesHandler))]
    [RequireComponent(typeof(CharacterCombatStatusEffectApplicationHandler))]
    public class CharacterCombatController : CharacterBehaviourControllerBase, 
        IStatusEffectFlagConsumer,
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- States ---
        //  1. References
        private CharacterCombatLoadoutController CombatLoadoutController => this.CharacterRootController.CombatLoadoutController;
        private CharacterCombatResourcesHandler CombatResourcesHandler => this.CharacterRootController.CombatResourcesHandler;
        private CharacterCombatAimingHandler CombatAimingHandler => this.CharacterRootController.CombatAimingHandler;
        public CharacterCombatStatusEffectApplicationHandler CombatStatusEffectHandler => this.CharacterRootController.CombatStatusEffectApplicationHandler;
        private CharacterVisualController VisualController => this.CharacterRootController.VisualController;

        [Header("References (Combat)")]
        [SerializeField] private MeleeHitboxController _meleeHitboxController;
        [SerializeField] private Transform _rangedAnchorDown;
        [SerializeField] private Transform _rangedAnchorSide;
        [SerializeField] private Transform _rangedAnchorUp;
        public MeleeHitboxController MeleeHitboxController => _meleeHitboxController;
        private Transform RangedAnchorDown => _rangedAnchorDown;
        private Transform RangedAnchorSide => _rangedAnchorSide;
        private Transform RangedAnchorUp => _rangedAnchorUp;

        //  2. Attack State (Runtime)
        [Header("Runtime State (Runtime)")]
        [SerializeField] private bool _isInAttackWindow = false;
        public bool IsInAttackWindow
        {
            get => _isInAttackWindow;
            private set => _isInAttackWindow = value;
        }
        
        private RuntimeSkill _activeRuntimeSkill;
        private RuntimeSkill ActiveRuntimeSkill
        {
            get => _activeRuntimeSkill;
            set => _activeRuntimeSkill = value;
        }

        //  3. Status Effects states
        [Header("Status Effects Flags (Runtime)")]
        [SerializeField] private bool _cannotAttack;
        [SerializeField] private bool _cannotCastSpecialSkill;
        [SerializeField] private bool _ignoreAttackLock;
        private bool CannotAttack
        {
            get => _cannotAttack;
            set => _cannotAttack = value;
        }
        private bool CannotCastSpecialSkill
        {
            get => _cannotCastSpecialSkill;
            set => _cannotCastSpecialSkill = value;
        }
        private bool IgnoreAttackLock
        {
            get => _ignoreAttackLock;
            set => _ignoreAttackLock = value;
        }
        public bool IsAttackLocked => this.CannotAttack && !this.IgnoreAttackLock;
        public bool IsSpecialAttackLocked => (this.CannotAttack || this.CannotCastSpecialSkill) && !this.IgnoreAttackLock;

        //  4. Last states
        private AttackSide _lastAttackSide;
        public AttackSide LastAttackSide
        {
            get => _lastAttackSide;
            private set => _lastAttackSide = value;
        }

        //  5. Attack Coroutine
        private Coroutine _attackRoutine;
        private Coroutine AttackRoutine
        {
            get => _attackRoutine;
            set => _attackRoutine = value;
        }

        //  6. Death Disable State (Runtime)
        [Header("Death Disable")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled
        {
            get => _isDeathDisabled;
            private set => _isDeathDisabled = value;
        }

        // --- Public APIs ---
        //      1.1. Try to perform a normal attack with the given side. Returns true if the attack was initiated.
        public bool TryDoNormalAttack(AttackSide attackSideType = AttackSide.Left)
        {
            if (this.IsDeathDisabled) return false;

            if (this.IsAttackLocked)
            {
                Debug.Log($"[{this.CharacterName}.{nameof(CharacterCombatController)}] {nameof(TryDoNormalAttack)}: Attack is locked.");
                return false;
            }

            if (this.CombatLoadoutController == null) return false;
            if (this.IsInAttackWindow) return false;

            var runtimeWeapon = this.CombatLoadoutController.ActiveEquippedRuntimeWeapon;
            if (runtimeWeapon == null || runtimeWeapon.Profile == null) return false;

            var skill = runtimeWeapon.NormalSkill;
            if (skill == null) return false;
            
            return this.DoAttack(skill, attackSideType);
        }

        //      1.2. Try to perform a special attack with the given side. Returns true if the attack was initiated.
        public bool TryDoSpecialAttack(AttackSide attackSideType = AttackSide.Left)
        {
            if (this.IsDeathDisabled) return false;

            if (this.IsSpecialAttackLocked)
            {
                Debug.Log($"[{this.CharacterName}.{nameof(CharacterCombatController)}] {nameof(TryDoSpecialAttack)}: Special attack is locked.");
                return false;
            }

            if (this.CombatLoadoutController == null) return false;
            if (this.IsInAttackWindow) return false;

            var runtimeWeapon = this.CombatLoadoutController.ActiveEquippedRuntimeWeapon;
            if (runtimeWeapon == null || runtimeWeapon.Profile == null) return false;

            var skill = runtimeWeapon.ActiveSpecialSkill;

            if (skill == null) return false;
            return this.DoAttack(skill, attackSideType);
        }

        //  2. Attack window
        //      2.1. Attack window start
        public void OnAttackWindowStart()
        {
            // Resolve cached skill
            if (this.ActiveRuntimeSkill == null)
            {
                this.IsInAttackWindow = false;
                this.CombatAimingHandler?.ResolveOnAttackWindowEnd();
                return;
            }

            // Start window now
            this.IsInAttackWindow = true;

            // Lock facing direction during attack
            this.CombatAimingHandler?.ResolveOnAttackWindowStart();

            // MELEE: enable hitbox for the whole animation window
            switch (this.ActiveRuntimeSkill.Style)
            {
                case SkillStyle.Melee:
                    // Get melee skill profile
                    var meleeSkill = this.ActiveRuntimeSkill.Profile as MeleeSkillProfile;
                    if (meleeSkill == null)
                    {
                        Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatController)}] {nameof(OnAttackWindowStart)}: Melee skill profile is null for skill ID {this.ActiveRuntimeSkill.SkillID}.");
                        break;
                    }

                    // Enable hitbox
                    if (this.MeleeHitboxController != null)
                    {
                        this.MeleeHitboxController.BindHitContext(this, this.ActiveRuntimeSkill);   // Bind combat context

                        HitboxActivationContext meleeHitboxDamagePayload = new HitboxActivationContext()
                        {
                            DamageAmount = meleeSkill.DamageAmount,
                            KnockbackForce = meleeSkill.KnockbackForce,
                            AttackOrigin = this.transform,
                            SourceFaction = this.CharacterRootController.IdentityController.Faction
                        };
                        this.MeleeHitboxController.EnableHitbox(meleeHitboxDamagePayload, this.LastAttackSide);          // Enable the melee hitbox
                    }
                    break;
                case SkillStyle.Ranged:
                    // Ranged projectile will be spawned at OnAttackWindowEnd()
                    break;
                default:
                    Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatController)}] {nameof(OnAttackWindowStart)}: Unsupported skill style {this.ActiveRuntimeSkill.Style}.");
                    break;
            }

            // Self-applied status effects (if needed) at attack start
            this.CombatStatusEffectHandler.ApplySkillStatusEffectsToSelf(this.ActiveRuntimeSkill, SkillStatusEffectTriggerType.OnAttackStart);
        }

        //      2.2. Attack window end
        public void OnAttackWindowEnd()
        {
            // End window
            this.IsInAttackWindow = false;

            // Resolve cached skill
            RuntimeSkill runtimeSkill = this.ActiveRuntimeSkill;
            if (runtimeSkill == null)
            {
                this.CombatAimingHandler?.ResolveOnAttackWindowEnd();
                return;
            }

            // MELEE
            SkillStyle activeSkillStyle = this.ActiveRuntimeSkill.Style;
            switch (activeSkillStyle)
            {
                case SkillStyle.Melee:
                    this.MeleeHitboxController?.DisableHitbox();     // Disable hitbox
                    break;
                case SkillStyle.Ranged:
                    this.FireRangedProjectile(runtimeSkill);
                    break;
                default:
                    Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatController)}] {nameof(OnAttackWindowEnd)}: Unsupported skill style {activeSkillStyle}.");
                    break;
            }

            // Resolve reserved resource costs 
            this.CombatResourcesHandler?.ResolveOnAttackWindowEnd(skillStyle: activeSkillStyle);

            // Self-applied status effects at attack end
            this.CombatStatusEffectHandler.ApplySkillStatusEffectsToSelf(runtimeSkill, SkillStatusEffectTriggerType.OnAttackEnd);

            // Resolve aiming state at attack window end
            this.CombatAimingHandler?.ResolveOnAttackWindowEnd();

            this.ActiveRuntimeSkill = null;
        }

        //      2.3. Attack window interruption
        //          Called when the attacker takes damage during the active attack window.
        //          Ends the attack window immediately (hitbox off) and triggers refund flow.
        public void OnAttackWindowInterrupt()
        {
            if (this.ActiveRuntimeSkill == null) return;

            if (this.AttackRoutine != null)
            {
                this.StopCoroutine(this.AttackRoutine);
                this.AttackRoutine = null;
            }

            bool wasInAttackWindow = this.IsInAttackWindow;

            // End window now
            this.IsInAttackWindow = false;
            this.MeleeHitboxController?.DisableHitbox();    // If melee hitbox is active, disable immediately
            this.CombatAimingHandler?.ResolveOnAttackWindowEnd();

            // Resolve reserved costs (interruption/cancel path)
            if (wasInAttackWindow)
            {
                this.CombatResourcesHandler?.ResolveOnAttackWindowInterrupt(skillStyle: this.ActiveRuntimeSkill.Style);
            }

            this.ActiveRuntimeSkill = null;
        }

        //  3. Hitbox Events
        public void OnHitConfirmedFromHitbox(RuntimeSkill runtimeSkill, CharacterRootController target, GameObject hitboxGameObject)
        {
            if (runtimeSkill == null) return;

            // a) Resource resolution
            this.CombatResourcesHandler?.ResolveOnHitConfirmed(skillStyle: runtimeSkill.Style);

            // b) Apply skill status effects on hit (self + target), respecting limits
            this.CombatStatusEffectHandler.ApplySkillStatusEffectsOnHitConfirmed(runtimeSkill, target, hitboxGameObject);
        }

        public void OnHitboxDisabledWithoutHit(RuntimeSkill runtimeSkill)
        {
            if (runtimeSkill == null) return;

            // Resource resolution: "miss" path for OnHitConfirmed reservations
            this.CombatResourcesHandler?.ResolveOnHitboxDisabledWithoutHit(skillStyle: runtimeSkill.Style);
        }

        //  4. Status Effect Flag consumption
        public void SetStatusEffectFlag(StatusEffectFlagTarget flagType, bool value)
        {
            switch (flagType)
            {
                case StatusEffectFlagTarget.CannotAttack:
                    this.CannotAttack = value;
                    return;

                case StatusEffectFlagTarget.CannotCastSpecialSkill:
                    this.CannotCastSpecialSkill = value;
                    return;

                case StatusEffectFlagTarget.IgnoreAttackLock:
                    this.IgnoreAttackLock = value;
                    return;

                default:
                    return;
            }
        }

        //  5. Get Root (Composition) Controller
        public CharacterRootController GetCharacterRootController()
        {
            return this.CharacterRootController;
        }

        //  6. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 40;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.IsDeathDisabled = false;

            if (this.AttackRoutine != null)
            {
                this.StopCoroutine(this.AttackRoutine);
                this.AttackRoutine = null;
            }

            this.IsInAttackWindow = false;
            this.ActiveRuntimeSkill = null;

            this.MeleeHitboxController?.DisableHitbox();

            this.CombatLoadoutController?.RefreshLoadoutAfterCharacterRefreshment();
            this.CombatAimingHandler?.ClearAimingStateAfterCharacterRefreshment();
            this.CombatResourcesHandler?.ClearCombatReservationsAfterCharacterRefreshment();
            this.CombatStatusEffectHandler?.ClearRuntimeTrackingAfterCharacterRefreshment();
        }

        //  7. ICharacterDeathDisableable implementation
        public int DeathDisablePriority => 30;
        public void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;

            if (this.AttackRoutine != null)
            {
                this.StopCoroutine(this.AttackRoutine);
                this.AttackRoutine = null;
            }

            this.IsInAttackWindow = false;
            this.ActiveRuntimeSkill = null;

            this.MeleeHitboxController?.DisableHitbox();

            this.CombatAimingHandler?.ResolveOnAttackWindowEnd();
            this.CombatStatusEffectHandler?.ClearRuntimeTrackingAfterCharacterRefreshment();

            // Prefer a death-specific method later, but this is okay for now.
            this.CombatResourcesHandler?.ClearCombatReservationsAfterCharacterRefreshment();
        }

        // --- Helpers ---
        private bool DoAttack(SkillProfile skill, AttackSide attackSideType)
        {
            this.ActiveRuntimeSkill = null;

            WeaponProfile weapon = this.CombatLoadoutController.ActiveEquippedRuntimeWeapon?.Profile;

            CharacterType characterType = this.CharacterRootController.IdentityController.CharacterType;

            // Build runtime skill
            RuntimeSkill runtimeSkill = new RuntimeSkill
            (
                profile: skill, 
                weapon: weapon,
                provisionType: skill.DefaultProvisionType, 
                owner: this.gameObject, 
                characterType: characterType,
                animationProfile: this.CharacterRootController.VisualAttackAnimationHandler.ActiveProfile.GetOrNull(skill, weapon)
            );

            if (runtimeSkill.AttackAnimationClipHitboxSetProfile == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterCombatController)}] Missing CombatAnimationClipHitboxSetProfile for SkillID={runtimeSkill.SkillID} ('{skill.name}'), Tag={characterType}.", this);
                return false;
            }

            // Resource gate (pay/reserve). You already have reserve/refund logic in handler.
            if (this.CombatResourcesHandler != null)
            {
                if (!this.CombatResourcesHandler.TryPaySkillCost(runtimeSkill, reservationID: out int reservationID)) return false;
            }

            // Cache for animation events
            this.ActiveRuntimeSkill = runtimeSkill;

            // Aim + face BEFORE animation starts
            this.CombatAimingHandler?.PrepareAimingForAttack();

            // Cache last attack side for animation & hitbox use
            this.LastAttackSide = attackSideType;

            // Trigger attack animation
            if (this.VisualController != null) this.VisualController.TriggerAttack(runtimeSkill, weapon, this.LastAttackSide);

            if (this.AttackRoutine != null)
            {
                this.StopCoroutine(this.AttackRoutine);
                this.AttackRoutine = null;
            }

            this.AttackRoutine = this.StartCoroutine(this.CoHandleAttack(runtimeSkill));

            return true;
        }

        private IEnumerator CoHandleAttack(RuntimeSkill runtimeSkill)
        {
            if (runtimeSkill == null || runtimeSkill.AttackAnimationClipHitboxSetProfile == null)
            {
                this.AttackRoutine = null;
                yield break;
            }

            this.OnAttackWindowStart();

            float attackLength = runtimeSkill.AttackAnimationClipHitboxSetProfile.AttackWindowDuration;
            if (attackLength > 0f) yield return new WaitForSeconds(attackLength);

            if (this.ActiveRuntimeSkill != runtimeSkill)
            {
                this.AttackRoutine = null;
                yield break;
            }

            this.OnAttackWindowEnd();
            this.AttackRoutine = null;
        }

        private void FireRangedProjectile(RuntimeSkill runtimeSkill)
        {
            if (runtimeSkill == null) return;
            if (!(runtimeSkill.Profile is RangedSkillProfile rangedSkill)) return;
            if (!rangedSkill.ProjectilePrefab) return;

            Transform rangedAnchor = this.GetCurrentRangedAnchor();
            if (rangedAnchor == null) return;

            // Baseline direction
            Vector2 baselineDirection = this.CombatAimingHandler != null
                ? this.CombatAimingHandler.GetCurrentAttackDirection()
                : (Vector2)rangedAnchor.right;

            if (baselineDirection.sqrMagnitude < 0.0001f) baselineDirection = Vector2.right;
            baselineDirection.Normalize();

            int projectileCount = rangedSkill.ProjectileCount;
            float angleStep = rangedSkill.ProjectileAngleStep;

            // Symmetric fan around base direction
            float startAngleOffset = -0.5f * angleStep * (projectileCount - 1);

            for (int i = 0; i < projectileCount; i++)
            {
                float angleOffset = startAngleOffset + (i * angleStep);
                Vector2 projectileDirection = baselineDirection.Rotate(angleOffset);

                this.SpawnSingleProjectile(runtimeSkill, rangedSkill, projectileDirection, rangedAnchor);
            }
        }
        private Transform GetCurrentRangedAnchor()
        {
            CharacterFacingStatus facingStatus = this.VisualController != null
            ? this.VisualController.FacingStatus
            : CharacterFacingStatus.Down;

            switch (facingStatus)
            {
                case CharacterFacingStatus.Down:
                    return this.RangedAnchorDown != null
                        ? this.RangedAnchorDown
                        : this.transform;

                case CharacterFacingStatus.Side:
                    return this.RangedAnchorSide != null
                        ? this.RangedAnchorSide
                        : this.transform;

                case CharacterFacingStatus.Up:
                    return this.RangedAnchorUp != null
                        ? this.RangedAnchorUp
                        : this.transform;

                default:
                    return this.transform;
            }
        }
        private void SpawnSingleProjectile(RuntimeSkill runtimeSkill, RangedSkillProfile rangedSkill, Vector2 direction, Transform rangedAnchor)
        {
            if (runtimeSkill == null || rangedSkill == null) return;
            if (!rangedSkill.ProjectilePrefab) return;
            if (rangedAnchor == null) return;

            direction.Normalize();

            var projectileObj = UnityEngine.Object.Instantiate
            (
                rangedSkill.ProjectilePrefab,
                rangedAnchor.position,
                Quaternion.identity
            );

            // Rotation assignment
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectileObj.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Bind projectile hitbox to the shooter
            var projectileHitbox = projectileObj.GetComponentInChildren<ProjectileHitboxController>(true);
            if (projectileHitbox != null)
            {
                projectileHitbox.BindHitContext(this, runtimeSkill);

                HitboxActivationContext projectileHitboxDamagePayload = new HitboxActivationContext()
                {
                    DamageAmount = rangedSkill.DamageAmount,
                    KnockbackForce = rangedSkill.KnockbackForce,
                    AttackOrigin = this.transform,
                    SourceFaction = this.CharacterRootController.IdentityController.Faction
                };

                projectileHitbox.EnableHitbox(projectileHitboxDamagePayload, this.LastAttackSide);
            }

            // Velocity assignment
            var rigidbody2D = projectileObj.GetComponent<Rigidbody2D>();
            if (rigidbody2D != null)
            {
                rigidbody2D.linearVelocity = direction * rangedSkill.ProjectileSpeed;
            }
        }

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.MeleeHitboxController == null) _meleeHitboxController = this.GetComponentInChildren<MeleeHitboxController>(true);

            if (this.RangedAnchorDown   == null) _rangedAnchorDown  = this.transform.Find("RangedAnchors/RangedAnchor_Down");
            if (this.RangedAnchorSide   == null) _rangedAnchorSide  = this.transform.Find("RangedAnchors/RangedAnchor_Side");
            if (this.RangedAnchorUp     == null) _rangedAnchorUp    = this.transform.Find("RangedAnchors/RangedAnchor_Up");
        }

        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.MeleeHitboxController == null) Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatController)}] MeleeHitboxController reference is missing. Expected to find a child with a MeleeHitboxController component.", this);

            if (this.RangedAnchorDown == null || this.RangedAnchorSide == null || this.RangedAnchorUp == null) Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterCombatController)}] One or more ranged anchors are missing. " + $"Expected: RangedAnchors/RangedAnchor_Down, RangedAnchors/RangedAnchor_Side, RangedAnchors/RangedAnchor_Up.", this);
        }
    }
}
