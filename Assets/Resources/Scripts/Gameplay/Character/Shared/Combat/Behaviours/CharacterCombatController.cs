using System.Collections;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(CharacterCombatLoadoutController))]
    [RequireComponent(typeof(CharacterCombatResourcesHandler))]
    [RequireComponent(typeof(CharacterCombatStatusEffectApplicationHandler))]
    [RequireComponent(typeof(CharacterCombatAimingHandler))]
    public class CharacterCombatController : CharacterBehaviourBaseController, 
        IStatusEffectFlagConsumer,
        ICharacterRespawnRefreshable
    {
        // --- States ---
        //  1. References
        private CharacterCombatLoadoutController CombatLoadoutController => this.CharacterComponentController.CombatLoadoutController;
        private CharacterCombatResourcesHandler CombatResourcesHandler => this.CharacterComponentController.CombatResourcesHandler;
        private CharacterCombatAimingHandler CombatAimingHandler => this.CharacterComponentController.CombatAimingHandler;
        public CharacterCombatStatusEffectApplicationHandler CombatStatusEffectHandler => this.CharacterComponentController.CombatStatusEffectApplicationHandler;
        private CharacterVisualController VisualController => this.CharacterComponentController.VisualController;

        [Header("References (Combat)")]
        [SerializeField] private MeleeHitboxController _meleeHitboxController;
        [SerializeField] private Transform _rangedAnchor;
        public MeleeHitboxController MeleeHitboxController => _meleeHitboxController;
        private Transform RangedAnchor => _rangedAnchor;

        //  2. Attack State
        [Header("Runtime State (Read Only)")]
        [SerializeField] private bool _isInAttackWindow = false;
        public bool IsInAttackWindow
        {
            get => _isInAttackWindow;
            set => _isInAttackWindow = value;
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
        private AttackSideType _lastAttackSide;
        public AttackSideType LastAttackSide
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

        //  6. ICharacterRespawnRefreshable implementation: Refresh Respawn Priority
        public int RespawnRefreshPriority => 40;

        // --- Public APIs ---
        //      1.1. Try to perform a normal attack with the given side. Returns true if the attack was initiated.
        public bool TryDoNormalAttack(AttackSideType attackSideType = AttackSideType.Left)
        {
            if (this.IsAttackLocked) return false;

            if (this.CombatLoadoutController == null) return false;
            if (this.IsInAttackWindow) return false;

            var runtimeWeapon = this.CombatLoadoutController.ActiveEquippedRuntimeWeapon;
            if (runtimeWeapon == null || runtimeWeapon.Profile == null) return false;

            var skill = runtimeWeapon.NormalSkill;
            if (skill == null) return false;
            
            return this.DoAttack(skill, attackSideType);
        }

        //      1.2. Try to perform a special attack with the given side. Returns true if the attack was initiated.
        public bool TryDoSpecialAttack(AttackSideType attackSideType = AttackSideType.Left)
        {
            if (this.IsSpecialAttackLocked) return false;

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
                case SkillStyleType.Melee:
                    // Get melee skill profile
                    var meleeSkill = this.ActiveRuntimeSkill.Profile as MeleeSkillProfile;
                    if (meleeSkill == null)
                    {
                        Debug.LogError($"[{this.CharacterName}.CharacterCombatAttackController] OnAttackWindowStart: Melee skill profile is null for skill ID {this.ActiveRuntimeSkill.SkillID}.");
                        break;
                    }

                    // Enable hitbox
                    if (this.MeleeHitboxController != null)
                    {
                        this.MeleeHitboxController.BindHitContext(this, this.ActiveRuntimeSkill);   // Bind combat context

                        HitboxDamagePayload meleeHitboxDamagePayload = new HitboxDamagePayload()
                        {
                            DamageAmount = meleeSkill.DamageAmount,
                            KnockbackForce = meleeSkill.KnockbackForce,
                            AttackOrigin = this.transform,
                            SourceFaction = this.CombatResourcesHandler.ResourcesController.HP.CharacterFaction,
                        };
                        this.MeleeHitboxController.EnableHitbox(meleeHitboxDamagePayload, this.LastAttackSide);          // Enable the melee hitbox
                    }
                    break;
                case SkillStyleType.Ranged:
                    // Ranged projectile will be spawned at OnAttackWindowEnd()
                    break;
                default:
                    Debug.LogError($"[{this.CharacterName}.CharacterCombatAttackController] OnAttackWindowStart: Unsupported skill style {this.ActiveRuntimeSkill.Style}.");
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
            SkillStyleType activeSkillStyle = this.ActiveRuntimeSkill.Style;
            switch (activeSkillStyle)
            {
                case SkillStyleType.Melee:
                    this.MeleeHitboxController?.DisableHitbox();     // Disable hitbox
                    break;
                case SkillStyleType.Ranged:
                    this.FireRangedProjectile(runtimeSkill);
                    break;
                default:
                    Debug.LogError($"[{this.CharacterName}.CharacterCombatAttackController] OnAttackWindowEnd: Unsupported skill style {activeSkillStyle}.");
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
        public void OnHitConfirmedFromHitbox(RuntimeSkill runtimeSkill, GameObject targetGameObject, GameObject hitboxGameObject)
        {
            if (runtimeSkill == null) return;

            // a) Resource resolution
            this.CombatResourcesHandler?.ResolveOnHitConfirmed(skillStyle: runtimeSkill.Style);

            // b) Apply skill status effects on hit (self + target), respecting limits
            this.CombatStatusEffectHandler.ApplySkillStatusEffectsOnHitConfirmed(runtimeSkill, targetGameObject, hitboxGameObject);
        }

        public void OnHitboxDisabledWithoutHit(RuntimeSkill runtimeSkill)
        {
            if (runtimeSkill == null) return;

            // Resource resolution: "miss" path for OnHitConfirmed reservations
            this.CombatResourcesHandler?.ResolveOnHitboxDisabledWithoutHit(skillStyle: runtimeSkill.Style);
        }

        //  4. Status Effect Flag consumption
        public void SetStatusEffectFlag(StatusEffectFlagType flagType, bool value)
        {
            switch (flagType)
            {
                case StatusEffectFlagType.CannotAttack:
                    this.CannotAttack = value;
                    return;

                case StatusEffectFlagType.CannotCastSpecialSkill:
                    this.CannotCastSpecialSkill = value;
                    return;

                case StatusEffectFlagType.IgnoreAttackLock:
                    this.IgnoreAttackLock = value;
                    return;

                default:
                    return;
            }
        }

        //  5. ICharacterRespawnRefreshable implementation: Refresh states after respawn
        public void RefreshAfterRespawn(CharacterRespawnRefreshContext context)
        {
            if (this.AttackRoutine != null)
            {
                this.StopCoroutine(this.AttackRoutine);
                this.AttackRoutine = null;
            }

            this.IsInAttackWindow = false;
            this.ActiveRuntimeSkill = null;

            this.MeleeHitboxController?.DisableHitbox();

            this.CombatLoadoutController?.RefreshLoadoutAfterRespawn();
            this.CombatAimingHandler?.ClearAimingStateAfterRespawn();
            this.CombatResourcesHandler?.ClearCombatReservationsAfterRespawn();
            this.CombatStatusEffectHandler?.ClearRuntimeTrackingAfterRespawn();
        }

        // --- Helpers ---
        private bool DoAttack(SkillProfile skill, AttackSideType attackSideType)
        {
            this.ActiveRuntimeSkill = null;

            WeaponProfile weapon = this.CombatLoadoutController.ActiveEquippedRuntimeWeapon?.Profile;

            CharacterTagType characterTag = this.CharacterComponentController.IdentityController.CharacterTag;

            // Build runtime skill
            RuntimeSkill runtimeSkill = new RuntimeSkill
            (
                profile: skill, 
                weapon: weapon,
                provisionType: skill.DefaultProvisionType, 
                owner: this.gameObject, 
                characterTag: characterTag,
                animationProfile: this.VisualController.CharacterCombatAnimationProfile.GetOrNull(skill, weapon)
            );

            if (runtimeSkill.CombatAnimationClipHitboxSetProfile == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.CharacterCombatAttackController] Missing CombatAnimationClipHitboxSetProfile for SkillID={runtimeSkill.SkillID} ('{skill.name}'), Tag={characterTag}.", this);
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
            if (this.VisualController != null) this.VisualController.TriggerAttack(runtimeSkill, this.LastAttackSide);

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
            if (runtimeSkill == null || runtimeSkill.CombatAnimationClipHitboxSetProfile == null)
            {
                this.AttackRoutine = null;
                yield break;
            }

            this.OnAttackWindowStart();

            float attackLength = runtimeSkill.CombatAnimationClipHitboxSetProfile.AttackWindowDuration;
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
            if (!rangedSkill.ArrowPrefab) return;
            if (!this.RangedAnchor) return;

            // Baseline direction
            Vector2 baselineDirection = this.CombatAimingHandler != null
                ? this.CombatAimingHandler.GetCurrentAttackDirection()
                : (Vector2)this.RangedAnchor.right;

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

                this.SpawnSingleProjectile(runtimeSkill, rangedSkill, projectileDirection);
            }
        }

        private void SpawnSingleProjectile(RuntimeSkill runtimeSkill, RangedSkillProfile rangedSkill, Vector2 direction)
        {
            if (runtimeSkill == null || rangedSkill == null) return;
            if (!rangedSkill.ArrowPrefab) return;
            if (!this.RangedAnchor) return;

            direction.Normalize();

            var projectileObj = UnityEngine.Object.Instantiate
            (
                rangedSkill.ArrowPrefab,
                this.RangedAnchor.position,
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

                HitboxDamagePayload projectileHitboxDamagePayload = new HitboxDamagePayload()
                {
                    DamageAmount = rangedSkill.DamageAmount,
                    KnockbackForce = rangedSkill.KnockbackForce,
                    AttackOrigin = this.transform,
                    SourceFaction = this.CombatResourcesHandler.ResourcesController.HP.CharacterFaction,
                };

                projectileHitbox.EnableHitbox(projectileHitboxDamagePayload, this.LastAttackSide);
            }

            // Velocity assignment
            var rigidbody2D = projectileObj.GetComponent<Rigidbody2D>();
            if (rigidbody2D != null)
            {
                rigidbody2D.linearVelocity = direction * rangedSkill.ArrowSpeed;
            }
        }

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.MeleeHitboxController == null) _meleeHitboxController = this.GetComponentInChildren<MeleeHitboxController>(true);
            if (this.RangedAnchor == null) _rangedAnchor = this.transform.Find("RangedAnchor");
            if (this.RangedAnchor == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.CharacterCombatAttackController] RangedAnchor is not found. Default to {this.CharacterName}.transform");
                _rangedAnchor = this.CharacterComponentController.transform;
            }
        }
    }
}
