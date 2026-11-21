using System.Collections;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(CharacterCombatResourcesHandler))]
    [RequireComponent(typeof(CharacterCombatStatusEffectApplicationHandler))]
    public class CharacterCombatController : CharacterConfiguredBehaviourBase<CharacterCombatConfig>, 
        IStatusEffectFlagConsumer,
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- Data / Configs ---
        //  1. Aiming
        public bool AimingOverridesFacing => this.Config != null ? this.Config.AimingOverridesFacing : true;
        public bool LockFacingDuringActiveFrames => this.Config != null ? this.Config.LockFacingDuringActiveFrames : true;
        public FacingSnapType FacingSnapType => this.Config != null ? this.Config.FacingSnapType : FacingSnapType.FourWay;
        public bool EightWaySnap => this.Config != null ? this.Config.EightWaySnap : false;
        public float DeadzoneRadius => this.Config != null ? this.Config.DeadzoneRadius : 0.1f;
        public float ComboTurnAllowanceDegree => this.Config != null ? this.Config.ComboTurnAllowanceDegree : 50f;
        public float MaxAimVectorLength => this.Config != null ? this.Config.MaxAimVectorLength : 0;

        // --- References ---
        //  1. Sub-behaviours
        private CharacterCombatLoadoutController CombatLoadoutController => this.CharacterRootController.CombatLoadoutController;
        private CharacterCombatResourcesHandler CombatResourcesHandler => this.CharacterRootController.CombatResourcesHandler;
        private CharacterCombatAimingHandler CombatAimingHandler => this.CharacterRootController.CombatAimingHandler;
        public CharacterCombatStatusEffectApplicationHandler CombatStatusEffectHandler => this.CharacterRootController.CombatStatusEffectApplicationHandler;
        private CharacterVisualController VisualController => this.CharacterRootController.VisualController;

        //  2. Combat
        [Header("References - Core")]
        [SerializeField] private MeleeHitboxController _meleeHitboxController;
        [SerializeField] private Transform _rangedAnchorDown;
        [SerializeField] private Transform _rangedAnchorSide;
        [SerializeField] private Transform _rangedAnchorUp;
        public MeleeHitboxController MeleeHitboxController => _meleeHitboxController;
        private Transform RangedAnchorDown => _rangedAnchorDown;
        private Transform RangedAnchorSide => _rangedAnchorSide;
        private Transform RangedAnchorUp => _rangedAnchorUp;

        // --- Runtime ---
        //  1. Attack State (Runtime)
        [Header("Runtime - Runtime State")]
        [SerializeField] private bool _isInAttackWindow = false;
        public bool IsInAttackWindow
        {
            get => _isInAttackWindow;
            private set => _isInAttackWindow = value;
        }

        //  2. Status Effects Flags states
        [Header("Runtime - Status Effects Flags")]
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

        //  3. Last caching states
        private AttackSide _lastAttackSide;
        public AttackSide LastAttackSide
        {
            get => _lastAttackSide;
            private set => _lastAttackSide = value;
        }

        private RuntimeSkill _activeRuntimeSkill;
        private RuntimeSkill ActiveRuntimeSkill
        {
            get => _activeRuntimeSkill;
            set => _activeRuntimeSkill = value;
        }

        private ProjectileProfile _activeRuntimeProjectile;
        private ProjectileProfile ActiveRuntimeProjectile
        {
            get => _activeRuntimeProjectile;
            set => _activeRuntimeProjectile = value;
        }

        private ProjectileCostRecord _activeRuntimeProjectileCostRecord;
        private ProjectileCostRecord ActiveRuntimeProjectileCostRecord
        {
            get => _activeRuntimeProjectileCostRecord;
            set => _activeRuntimeProjectileCostRecord = value;
        }

        //  4. Attack Coroutine
        private Coroutine _attackRoutine;
        private Coroutine AttackRoutine
        {
            get => _attackRoutine;
            set => _attackRoutine = value;
        }

        //  5. Death Disable State (Runtime)
        [Header("Runtime - Death Disable")]
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
                Debug.Log($"[{this.CharacterName}.{nameof(CharacterCombatController)}] {nameof(this.TryDoNormalAttack)}: Attack is locked.");
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
                Debug.Log($"[{this.CharacterName}.{nameof(CharacterCombatController)}] {nameof(this.TryDoSpecialAttack)}: Special attack is locked.");
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

            // Validate supported skill style early
            switch (this.ActiveRuntimeSkill.Style)
            {
                case SkillStyle.Melee:
                    // Hitbox is enabled later by CoHandleAttack(), after profile delay.
                    break;
                case SkillStyle.Ranged:
                    // Ranged projectile will be spawned at OnAttackWindowEnd()
                    break;
                default:
                    Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatController)}] {nameof(this.OnAttackWindowStart)}: Unsupported skill style {this.ActiveRuntimeSkill.Style}.");
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
                    Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatController)}] {nameof(this.OnAttackWindowEnd)}: Unsupported skill style {activeSkillStyle}.");
                    break;
            }

            // Resolve reserved resource costs 
            this.CombatResourcesHandler?.ResolveOnAttackWindowEnd(skillStyle: activeSkillStyle);

            // Self-applied status effects at attack end
            this.CombatStatusEffectHandler.ApplySkillStatusEffectsToSelf(runtimeSkill, SkillStatusEffectTriggerType.OnAttackEnd);

            // Resolve aiming state at attack window end
            this.CombatAimingHandler?.ResolveOnAttackWindowEnd();

            this.ClearActiveAttackRuntimeState();
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

            SkillStyle interruptedSkillStyle = this.ActiveRuntimeSkill.Style;

            // Resolve reserved costs (interruption/cancel path)
            if (wasInAttackWindow)
            {
                this.CombatResourcesHandler?.ResolveOnAttackWindowInterrupt(skillStyle: interruptedSkillStyle);
            }

            // Resolve projectile refund (if ranged skill)
            ProjectileCostRecord interruptedProjectileCostRecord = this.ActiveRuntimeProjectileCostRecord;
            if (wasInAttackWindow && interruptedSkillStyle == SkillStyle.Ranged)
            {
                if (this.ActiveRuntimeSkill.Profile is RangedSkillProfile rangedSkill && rangedSkill.CostProjectile)
                {
                    this.CombatLoadoutController?.RefundProjectileCost(interruptedProjectileCostRecord);
                }
            }

            this.CancelAttackWindowRuntimeState(resolveAiming: true);
        }

        //  3. Hitbox Events
        public void OnHitConfirmedFromHitbox(RuntimeSkill runtimeSkill, UnityEngine.Object hitTarget, CharacterRootController characterTarget, GameObject hitboxGameObject)
        {
            if (runtimeSkill == null) return;
            if (hitTarget == null) return;
            if (hitboxGameObject == null) return;

            // a) Resource resolution
            this.CombatResourcesHandler?.ResolveOnHitConfirmed(skillStyle: runtimeSkill.Style);

            int hitTargetID = hitTarget.GetInstanceID();

            bool shouldApplySelfOnHitEffects = characterTarget != null;

            if (characterTarget == null)
            {
                HarvestDamageEntry harvestDamage = runtimeSkill.Profile.HarvestDamage;
                RuntimeHarvestDamage runtimeHarvestDamage = new RuntimeHarvestDamage(harvestDamage);

                shouldApplySelfOnHitEffects = runtimeHarvestDamage.TriggerSelfOnHitStatusEffects;
            }

            if (shouldApplySelfOnHitEffects)
            {
                this.CombatStatusEffectHandler?.ApplySkillStatusEffectsToSelfOnHitConfirmed
                (
                    runtimeSkill: runtimeSkill,
                    hitboxGameObject: hitboxGameObject,
                    hitTargetID: hitTargetID
                );
            }

            if (characterTarget == null) return;

            // b) Apply skill status effects on hit (to Target), respecting limits
            this.CombatStatusEffectHandler.ApplySkillStatusEffectsToTargetOnHitConfirmed
            (
                runtimeSkill: runtimeSkill,
                characterTarget: characterTarget,
                hitboxGameObject: hitboxGameObject
            );
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

            this.CancelAttackWindowRuntimeState(resolveAiming: false);

            this.CombatLoadoutController?.RefreshLoadoutAfterCharacterRefreshment();
            this.CombatAimingHandler?.ClearAimingStateAfterCharacterRefresh();
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

            this.CancelAttackWindowRuntimeState(resolveAiming: true);

            this.CombatStatusEffectHandler?.ClearRuntimeTrackingAfterCharacterRefreshment();

            // Prefer a death-specific method later, but this is okay for now.
            this.CombatResourcesHandler?.ClearCombatReservationsAfterCharacterRefreshment();
        }

        // --- Helpers ---
        private bool DoAttack(SkillProfile skill, AttackSide attackSideType)
        {
            this.ClearActiveAttackRuntimeState();

            WeaponProfile weapon = this.CombatLoadoutController.ActiveEquippedRuntimeWeapon?.Profile;

            CharacterType characterType = this.CharacterRootController.CharacterType;

            // Build runtime skill
            RuntimeSkill runtimeSkill = new RuntimeSkill
            (
                profile: skill, 
                weapon: weapon,
                provisionType: skill.DefaultProvisionType, 
                owner: this.gameObject, 
                characterType: characterType,
                animationProfile: this.CharacterRootController.VisualController.AttackAnimationProfile.GetOrNull(skill, weapon)
            );

            if (runtimeSkill.AttackAnimationClipHitboxSetProfile == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterCombatController)}] Missing {nameof(CharacterAttackAnimationClipHitboxSetProfile)} for SkillID={runtimeSkill.SkillID} ('{skill.name}'), Tag={characterType}.", this);
                return false;
            }

            ProjectileProfile resolvedProjectile = null;
            ProjectileCostRecord projectileCostRecord = null;

            // Projectile gate.
            // For consuming ranged skills, this validates the active projectile slot.
            // For free ranged skills, this resolves the skill's default projectile item.
            if (skill is RangedSkillProfile rangedSkill)
            {
                if (!this.CombatLoadoutController.TryResolveProjectileForRangedSkill(rangedSkill, out resolvedProjectile))
                {
                    return false;
                }
            }

            // Projectile cost.
            // Deduct before resource payment so projectile-consume failure cannot leave resource costs spent/reserved. If resource payment fails, refund the projectile.
            if (skill is RangedSkillProfile costRangedSkill && costRangedSkill.CostProjectile)
            {
                if (!this.CombatLoadoutController.TryConsumeProjectileForRangedSkill(costRangedSkill, out projectileCostRecord))
                {
                    return false;
                }

                resolvedProjectile = projectileCostRecord.Projectile;
            }

            // Resource gate.
            if (this.CombatResourcesHandler != null)
            {
                if (!this.CombatResourcesHandler.TryPaySkillCost(runtimeSkill, reservationID: out int reservationID))
                {
                    if (projectileCostRecord != null)
                    {
                        this.CombatLoadoutController?.RefundProjectileCost(projectileCostRecord);
                    }

                    return false;
                }
            }

            // Cache (e.g., for animation events)
            this.ActiveRuntimeSkill = runtimeSkill;
            this.ActiveRuntimeProjectile = resolvedProjectile;
            this.ActiveRuntimeProjectileCostRecord = projectileCostRecord;

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
            float hitboxDelay = runtimeSkill.AttackAnimationClipHitboxSetProfile.HitboxActivationDelay;
            
            attackLength = Mathf.Max(0f, attackLength);
            hitboxDelay = Mathf.Max(0f, hitboxDelay);

            // If the delay is longer than the attack window, the melee hitbox should not activate.
            float clampedHitboxDelay = Mathf.Min(hitboxDelay, attackLength);
            if (clampedHitboxDelay > 0f)
            {
                yield return new WaitForSeconds(clampedHitboxDelay);
            }

            if (this.ActiveRuntimeSkill != runtimeSkill)
            {
                this.CancelAttackWindowRuntimeState();
                this.AttackRoutine = null;
                yield break;
            }

            bool shouldEnableMeleeHitbox = (runtimeSkill.Style == SkillStyle.Melee) && (hitboxDelay < attackLength);
            if (shouldEnableMeleeHitbox)
            {
                this.EnableMeleeHitboxForActiveSkill();
            }

            float remainingAttackLength = attackLength - clampedHitboxDelay;
            if (remainingAttackLength > 0f)
            {
                yield return new WaitForSeconds(remainingAttackLength);
            }

            if (this.ActiveRuntimeSkill != runtimeSkill)
            {
                this.CancelAttackWindowRuntimeState();
                this.AttackRoutine = null;
                yield break;
            }

            this.OnAttackWindowEnd();
            this.AttackRoutine = null;
        }

        private void ClearActiveAttackRuntimeState()
        {
            this.ActiveRuntimeSkill = null;
            this.ActiveRuntimeProjectile = null;
            this.ActiveRuntimeProjectileCostRecord = null;
        }

        private void CancelAttackWindowRuntimeState(bool resolveAiming = true)
        {
            this.IsInAttackWindow = false;

            this.MeleeHitboxController?.DisableHitbox();

            if (resolveAiming)
            {
                this.CombatAimingHandler?.ResolveOnAttackWindowEnd();
            }

            this.ClearActiveAttackRuntimeState();
        }

        private void EnableMeleeHitboxForActiveSkill()
        {
            if (this.ActiveRuntimeSkill == null) return;
            if (this.ActiveRuntimeSkill.Style != SkillStyle.Melee) return;

            var meleeSkill = this.ActiveRuntimeSkill.Profile as MeleeSkillProfile;
            if (meleeSkill == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatController)}] " + $"{nameof(EnableMeleeHitboxForActiveSkill)}: Melee skill profile is null for skill ID {this.ActiveRuntimeSkill.SkillID}.", this);
                return;
            }

            if (this.MeleeHitboxController == null) return;

            this.MeleeHitboxController.BindHitContext(this, this.ActiveRuntimeSkill);

            HarvestDamageEntry harvestDamageEntry = this.ActiveRuntimeSkill.Profile.HarvestDamage;
            RuntimeHarvestDamage runtimeHarvestDamage = new RuntimeHarvestDamage(harvestDamageEntry);

            this.MeleeHitboxController.ConfigureHarvestDamage(runtimeHarvestDamage);

            HitboxActivationContext meleeHitboxDamagePayload = new HitboxActivationContext()
            {
                DamageAmount = meleeSkill.DamageAmount,
                KnockbackForce = meleeSkill.KnockbackForce,
                AttackOrigin = this.transform,
                SourceFaction = this.CharacterRootController.Faction
            };

            this.MeleeHitboxController.EnableHitbox(meleeHitboxDamagePayload, this.LastAttackSide);
        }

        private void FireRangedProjectile(RuntimeSkill runtimeSkill)
        {
            if (runtimeSkill == null) return;
            if (runtimeSkill.Profile is not RangedSkillProfile rangedSkill) return;

            ProjectileProfile projectile = this.ActiveRuntimeProjectile;
            if (projectile == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatController)}] {nameof(this.FireRangedProjectile)} failed: active runtime projectile is null.", this);
                return;
            }
            if (projectile.ProjectilePrefab == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatController)}] {nameof(this.FireRangedProjectile)} failed: projectile '{projectile.name}' has no projectile prefab.", this);
                return;
            }

            Transform rangedAnchor = this.GetCurrentRangedAnchor();
            if (rangedAnchor == null) return;

            // Baseline direction
            Vector2 baselineDirection = this.CombatAimingHandler != null
                ? this.CombatAimingHandler.GetCurrentAttackDirection()
                : (Vector2)rangedAnchor.right;

            if (baselineDirection.sqrMagnitude < 0.0001f) baselineDirection = Vector2.right;
            baselineDirection.Normalize();

            int projectileCount = rangedSkill.ProjectileCount;
            if (projectileCount <= 0)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatController)}] {nameof(this.FireRangedProjectile)} failed: ranged skill '{rangedSkill.name}' has invalid ProjectileCount: {projectileCount}.", this);
                return;
            }

            float angleStep = rangedSkill.ProjectileAngleStep;

            // Symmetric fan around base direction
            float startAngleOffset = -0.5f * angleStep * (projectileCount - 1);

            for (int i = 0; i < projectileCount; i++)
            {
                float angleOffset = startAngleOffset + (i * angleStep);
                Vector2 projectileDirection = baselineDirection.Rotate(angleOffset);

                this.SpawnSingleProjectile(runtimeSkill, rangedSkill, projectile, projectileDirection, rangedAnchor);
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
        private void SpawnSingleProjectile(RuntimeSkill runtimeSkill, RangedSkillProfile rangedSkill, ProjectileProfile projectile, Vector2 direction, Transform rangedAnchor)
        {
            if (runtimeSkill == null || rangedSkill == null || projectile == null) return;
            if (projectile.ProjectilePrefab == null) return;
            if (rangedAnchor == null) return;

            direction.Normalize();

            var projectileObj = UnityEngine.Object.Instantiate
            (
                original: projectile.ProjectilePrefab,
                position: rangedAnchor.position,
                rotation: Quaternion.identity,
                parent: rangedAnchor
            );

            // Rotation assignment
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectileObj.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Bind projectile hitbox to the shooter
            var projectileHitbox = projectileObj.GetComponentInChildren<ProjectileHitboxController>(true);
            if (projectileHitbox != null)
            {
                projectileHitbox.BindHitContext(this, runtimeSkill);

                HarvestDamageEntry harvestDamageEntry = runtimeSkill.Profile.HarvestDamage;
                RuntimeHarvestDamage runtimeHarvestDamage = new RuntimeHarvestDamage(harvestDamageEntry);
                projectileHitbox.ConfigureHarvestDamage(runtimeHarvestDamage);

                HitboxActivationContext projectileHitboxDamagePayload = new HitboxActivationContext()
                {
                    DamageAmount = rangedSkill.DamageAmount,
                    KnockbackForce = rangedSkill.KnockbackForce,
                    AttackOrigin = this.transform,
                    SourceFaction = this.CharacterRootController.Faction
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

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.MeleeHitboxController == null) _meleeHitboxController = this.GetComponentInChildren<MeleeHitboxController>(true);

            if (this.RangedAnchorDown   == null) _rangedAnchorDown  = this.transform.Find("RangedAnchors/RangedAnchor_Down");
            if (this.RangedAnchorSide   == null) _rangedAnchorSide  = this.transform.Find("RangedAnchors/RangedAnchor_Side");
            if (this.RangedAnchorUp     == null) _rangedAnchorUp    = this.transform.Find("RangedAnchors/RangedAnchor_Up");
        }

        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.MeleeHitboxController == null) Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatController)}] MeleeHitboxController reference is missing. Expected to find a child with a MeleeHitboxController component.", this);

            if (this.RangedAnchorDown == null || this.RangedAnchorSide == null || this.RangedAnchorUp == null) Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterCombatController)}] One or more ranged anchors are missing. " + $"Expected: RangedAnchors/RangedAnchor_Down, RangedAnchors/RangedAnchor_Side, RangedAnchors/RangedAnchor_Up.", this);
        }
    }
}
