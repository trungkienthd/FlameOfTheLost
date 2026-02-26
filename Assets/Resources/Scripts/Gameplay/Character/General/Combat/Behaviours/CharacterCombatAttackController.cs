using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(CharacterCombatLoadoutController))]
    [RequireComponent(typeof(CharacterCombatResourcesHandler))]
    [RequireComponent(typeof(CharacterCombatSkillStatusEffectHandler))]
    [RequireComponent(typeof(CharacterCombatAimingHandler))]
    public class CharacterCombatAttackController : MonoBehaviour, IStatusEffectFlagConsumer
    {
        // --- References
        [Header("Combat References")]
        [SerializeField] private CharacterCombatLoadoutController _combatLoadoutController;
        [SerializeField] private CharacterCombatResourcesHandler _combatResourcesHandler;
        [SerializeField] private CharacterCombatAimingHandler _combatAimingHandler;
        [SerializeField] private CharacterCombatSkillStatusEffectHandler _combatSkillStatusEffectHandler;
        private CharacterCombatLoadoutController CombatLoadoutController
        {
            get => _combatLoadoutController;
            set => _combatLoadoutController = value;
        }
        private CharacterCombatResourcesHandler CombatResourcesHandler
        {
            get => _combatResourcesHandler;
            set => _combatResourcesHandler = value;
        }
        private CharacterCombatAimingHandler CombatAimingHandler
        {
            get => _combatAimingHandler;
            set => _combatAimingHandler = value;
        }
        public CharacterCombatSkillStatusEffectHandler CombatSkillStatusEffectHandler
        {
            get => _combatSkillStatusEffectHandler;
            private set => _combatSkillStatusEffectHandler = value;
        }

        [Header("Animation References")]
        [SerializeField] private CharacterAnimationHandler _animationHandler;
        private CharacterAnimationHandler AnimationHandler
        {
            get => _animationHandler;
            set => _animationHandler = value;
        }

        [Header("Other References")]
        [SerializeField] private MeleeHitboxController _meleeHitboxController;
        [SerializeField] private Transform _rangedAnchor;
        public MeleeHitboxController MeleeHitboxController
        {
            get => _meleeHitboxController;
            private set => _meleeHitboxController = value;
        }
        private Transform RangedAnchor
        {
            get => _rangedAnchor;
            set => _rangedAnchor = value;
        }

        // --- Attack State
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

        // --- Status Effects states
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

        // --- Unity Methods --- 
        private void Awake()
        {
            if (this.CombatLoadoutController == null)           this.CombatLoadoutController        = this.GetComponent<CharacterCombatLoadoutController>();
            if (this.CombatResourcesHandler == null)            this.CombatResourcesHandler         = this.GetComponent<CharacterCombatResourcesHandler>();
            if (this.CombatAimingHandler == null)               this.CombatAimingHandler            = this.GetComponent<CharacterCombatAimingHandler>();
            if (this.CombatSkillStatusEffectHandler == null)    this.CombatSkillStatusEffectHandler = this.GetComponent<CharacterCombatSkillStatusEffectHandler>();

            if (this.AnimationHandler == null) this.AnimationHandler = this.GetComponentInChildren<CharacterAnimationHandler>();

            if (this.MeleeHitboxController == null) this.MeleeHitboxController = this.GetComponentInChildren<MeleeHitboxController>(true);
            if (this.RangedAnchor == null) this.RangedAnchor = this.transform.Find("RangedAnchor");
            if (this.RangedAnchor == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.CharacterCombatAttackController] RangedAnchor is not found. Default to {this.gameObject.name}.transform");
                this.RangedAnchor = this.transform;
            }
        }
        private void Start()
        {
            SkillCharacterAnimationProfileUtility.WarmUp();
        }

        // --- Public APIs ---
        //      1.1. Try to perform a normal attack with the given side. Returns true if the attack was initiated.
        public bool TryDoNormalAttack(AttackSideType attackSide)
        {
            if (this.IsAttackLocked) return false;

            if (this.CombatLoadoutController == null) return false;
            if (this.IsInAttackWindow) return false;

            var weapon = this.CombatLoadoutController.ActiveEquippedWeapon;
            if (weapon == null) return false;

            var skill = this.CombatLoadoutController.ActiveEquippedWeapon.GetNormalSkill(attackSide);
            if (skill == null) return false;
            
            return this.DoAttack(attackSide, skill);
        }

        //      1.2. Try to perform a special attack with the given side. Returns true if the attack was initiated.
        public bool TryDoSpecialAttack(AttackSideType attackSide)
        {
            if (this.IsSpecialAttackLocked) return false;

            if (this.CombatLoadoutController == null) return false;
            if (this.IsInAttackWindow) return false;

            var weapon = this.CombatLoadoutController.ActiveEquippedWeapon;
            if (weapon == null) return false;

            var skill = (attackSide == AttackSideType.Left)
                ? this.CombatLoadoutController.ActiveEquippedRuntimeWeapon.ActiveLeftSpecialSkill
                : this.CombatLoadoutController.ActiveEquippedRuntimeWeapon.ActiveRightSpecialSkill;

            if (skill == null) return false;
            return this.DoAttack(attackSide, skill);
        }

        //  2. Attack window
        //      2.1. Attack window start
        public void OnAttackWindowStart()
        {
            // Resolve cached skill
            if (this.ActiveRuntimeSkill == null)
            {
                this.IsInAttackWindow = false;
                this.CombatAimingHandler?.UnlockFacing();
                return;
            }

            // Start window now
            this.IsInAttackWindow = true;

            // Lock facing direction during attack
            this.CombatAimingHandler?.LockFacing();

            // MELEE: enable hitbox for the whole animation window
            switch (this.ActiveRuntimeSkill.Style)
            {
                case SkillStyleType.Melee:
                    // Get melee skill profile
                    var meleeSkill = this.ActiveRuntimeSkill.Profile as MeleeSkillProfile;
                    if (meleeSkill == null)
                    {
                        Debug.LogError($"[{this.gameObject.name}.CharacterCombatAttackController] OnAttackWindowStart: Melee skill profile is null for skill ID {this.ActiveRuntimeSkill.SkillID}.");
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
                        this.MeleeHitboxController.EnableHitbox(meleeHitboxDamagePayload);          // Enable the melee hitbox
                    }
                    break;
                case SkillStyleType.Ranged:
                    // Ranged projectile will be spawned at OnAttackWindowEnd()
                    break;
                default:
                    Debug.LogError($"[{this.gameObject.name}.CharacterCombatAttackController] OnAttackWindowStart: Unsupported skill style {this.ActiveRuntimeSkill.Style}.");
                    break;
            }

            // Self-applied status effects (if needed) at attack start
            this.CombatSkillStatusEffectHandler.ApplySkillStatusEffectsToSelf(this.ActiveRuntimeSkill, SkillStatusEffectTriggerType.OnAttackStart);
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
                this.CombatAimingHandler?.UnlockFacing();
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
                    Debug.LogError($"[{this.gameObject.name}.CharacterCombatAttackController] OnAttackWindowEnd: Unsupported skill style {activeSkillStyle}.");
                    break;
            }

            // Resolve reserved resource costs 
            this.CombatResourcesHandler?.ResolveOnAttackWindowEnd(skillStyle: activeSkillStyle);

            // Self-applied status effects at attack end
            this.CombatSkillStatusEffectHandler.ApplySkillStatusEffectsToSelf(runtimeSkill, SkillStatusEffectTriggerType.OnAttackEnd);

            // Unlock facing direction at the end of attack
            this.CombatAimingHandler?.UnlockFacing();
        }

        //      2.3. Attack window interruption
        //          Called when the attacker takes damage during the active attack window.
        //          Ends the attack window immediately (hitbox off) and triggers refund flow.
        public void OnAttackWindowInterrupt()
        {
            if (!this.IsInAttackWindow) return;
            if (this.ActiveRuntimeSkill == null) return;

            // End window now
            this.IsInAttackWindow = false;

            // Unlock facing direction at the interruption of attack
            this.CombatAimingHandler?.UnlockFacing();

            // If melee hitbox is active, disable immediately
            this.MeleeHitboxController?.DisableHitbox();

            // Resolve reserved costs (interruption/cancel path)
            this.CombatResourcesHandler?.ResolveOnAttackWindowInterrupt(skillStyle: this.ActiveRuntimeSkill.Style);
        }

        //  3. Hitbox Events
        public void OnHitConfirmedFromHitbox(RuntimeSkill runtimeSkill, GameObject targetGameObject, GameObject hitboxGameObject)
        {
            if (runtimeSkill == null) return;

            // a) Resource resolution
            this.CombatResourcesHandler?.ResolveOnHitConfirmed(skillStyle: runtimeSkill.Style);

            // b) Apply skill status effects on hit (self + target), respecting limits
            this.CombatSkillStatusEffectHandler.ApplySkillStatusEffectsOnHitConfirmed(runtimeSkill, targetGameObject, hitboxGameObject);
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

        // --- Helpers ---
        private bool DoAttack(AttackSideType attackSide, SkillProfile skill)
        {
            this.ActiveRuntimeSkill = null;

            // Build runtime skill and resolve the corresponding SkillCharacterAnimationProfile via the static map.
            RuntimeSkill runtimeSkill = new RuntimeSkill(profile: skill, provisionType: skill.DefaultProvisionType, owner: this.gameObject, characterTag: this.CombatLoadoutController.CharacterTag);

            if (runtimeSkill.AnimationProfile == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.CharacterCombatAttackController] Missing SkillCharacterAnimationProfile for SkillID={runtimeSkill.SkillID} ('{skill.name}'), Tag={this.CombatLoadoutController.CharacterTag}.", this);
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

            // Drive attack animation
            if (this.AnimationHandler != null) this.AnimationHandler.TriggerAttack(runtimeSkill);

            return true;
        }
        
        private void FireRangedProjectile(RuntimeSkill runtimeSkill)
        {
            if (runtimeSkill == null) return;
            if (!(runtimeSkill.Profile is RangedSkillProfile rangedSkill)) return;
            if (!rangedSkill.ArrowPrefab) return;
            if (!this.RangedAnchor) return;

            var projectileObj = UnityEngine.Object.Instantiate(rangedSkill.ArrowPrefab, this.RangedAnchor.position, Quaternion.identity);

            // Bind projectile hitbox to the shooter
            var projectileHitbox = projectileObj.GetComponentInChildren<ProjectileHitboxController>(true);
            if (projectileHitbox != null)
            {
                // Bind combat context
                projectileHitbox.BindHitContext(this, runtimeSkill);

                HitboxDamagePayload projectileHitboxDamagePayload = new HitboxDamagePayload()
                {
                    DamageAmount = rangedSkill.DamageAmount,
                    KnockbackForce = rangedSkill.KnockbackForce,
                    AttackOrigin = this.transform,
                    SourceFaction = this.CombatResourcesHandler.ResourcesController.HP.CharacterFaction,
                };
                projectileHitbox.EnableHitbox(projectileHitboxDamagePayload);    // Enable the projectile hitbox
            }

            // Baseline direction
            Vector2 direction = this.CombatAimingHandler != null
                ? this.CombatAimingHandler.GetCurrentAttackDirection()
                : (Vector2)this.RangedAnchor.right;
            if (direction.sqrMagnitude < 0.0001f) direction = Vector2.right;
            direction.Normalize();

            var rigidbody2D = projectileObj.GetComponent<Rigidbody2D>();
            if (rigidbody2D != null) rigidbody2D.linearVelocity = direction * rangedSkill.ArrowSpeed;
        }
    }
}
