using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(DamageDealingHandler))]
    public abstract class HitboxController : FOTLMonoBehaviour
    {
        // --- References ---
        //  1. Damage Configurator
        [Header("Damage Configurator (Cached)")]
        [SerializeField] private MonoBehaviour _damageConfiguratorSource;
        protected MonoBehaviour DamageConfiguratorSource => _damageConfiguratorSource;

        private IDamageConfigurator _damageConfigurator;
        protected IDamageConfigurator DamageConfigurator
        {
            get => _damageConfigurator;
            private set => _damageConfigurator = value;
        }

        //  2. Runtime Context (Bound per attack instance)
        [Header("Runtime Context (Bound per attack instance)")]
        [SerializeField] private CharacterCombatController _attackerCombatController;
        [SerializeField] private RuntimeSkill _activeRuntimeSkill;
        protected CharacterCombatController AttackerCombatController
        {
            get => _attackerCombatController;
            private set => _attackerCombatController = value;
        }
        protected RuntimeSkill ActiveRuntimeSkill
        {
            get => _activeRuntimeSkill;
            private set => _activeRuntimeSkill = value;
        }

        [Header("Harvest Damage Source (Optional)")]
        [SerializeField] private HarvestDamageSourceProvider _harvestDamageSourceProvider;
        protected HarvestDamageSourceProvider HarvestDamageSourceProvider => _harvestDamageSourceProvider;

        // --- Runtime States ---
        [Header("State (Runtime)")]
        [SerializeField] private bool _isActive = false;
        [SerializeField] private bool _hasConfirmedHit = false;             // True after at least one valid target has been hit during the current enable window.
        [SerializeField] private bool _hasDisabledWithoutHit = false;       // expired without ever confirming a hit
        protected bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }
        protected bool HasConfirmedHit
        {
            get => _hasConfirmedHit;
            set => _hasConfirmedHit = value;
        }
        protected bool HasDisabledWithoutHit
        {
            get => _hasDisabledWithoutHit;
            set => _hasDisabledWithoutHit = value;
        }
        protected abstract bool AllowMultipleTargetsPerEnableWindow
        {
            get;
        }

        //  2. Hit registry (per enable window)
        private readonly HashSet<int> _hitTargetIds = new();
        protected HashSet<int> HitTargetIDs => _hitTargetIds;

        // --- Public APIs ---
        //      1.1 Binding Context
        public virtual void BindHitContext(CharacterCombatController attackerCombatController, RuntimeSkill runtimeSkill)
        {
            this.AttackerCombatController = attackerCombatController;
            this.ActiveRuntimeSkill = runtimeSkill;
        }
        //      1.2. Clearing Context
        public virtual void ClearHitContext()
        {
            this.AttackerCombatController = null;
            this.ActiveRuntimeSkill = null;
        }

        //  2. Hit Registration
        public bool TryRegisterHitTarget(UnityEngine.Object target)
        {
            if (!this.IsActive) return false;
            if (target == null) return false;

            int targetID = target.GetInstanceID();

            // Already hit this target during this enable window
            if (this.HitTargetIDs.Contains(targetID)) return false;

            // If we don't allow multi-target, block after first hit is confirmed
            if (!this.AllowMultipleTargetsPerEnableWindow && this.HasConfirmedHit) return false;

            // Register hit
            this.HitTargetIDs.Add(targetID);
            return true;
        }

        //  2. Hit Confirmation: Notification
        //      2.1. Notify the Hit, called by DamageReceivingHandler (on target) when a valid hit actually happened
        public virtual void NotifyHitConfirmed(UnityEngine.Object target, CharacterRootController characterTarget)
        {
            if (!this.IsActive) return;
            if (target == null) return;

            this.HasConfirmedHit = true;

            this.ForwardHitConfirmedToCombat
            (
                hitTarget: target,
                characterTarget: characterTarget
            );
        }

        //      2.2. Notify disabled before hit
        public void NotifyHitboxDisabledWithoutHit()
        {
            if (!this.IsActive) return;
            if (this.AttackerCombatController == null) return;          // prevents startup spam
            if (this.ActiveRuntimeSkill == null) return;                // prevents startup spam
            if (this.HasConfirmedHit) return;                           // not a "miss"
            if (this.HasDisabledWithoutHit) return;                     // only once

            this.HasDisabledWithoutHit = true;
            this.ForwardHitboxDisabledWithoutHitToCombat();
        }

        //  3. Hitbox Activation / Deactivation
        //      3.1. Enable the Hitbox
        public virtual void EnableHitbox(HitboxActivationContext payload, AttackSide attackSideType)
        {
            this.IsActive = true;
            this.HasConfirmedHit = false; 
            this.HasDisabledWithoutHit = false;

            this.HitTargetIDs.Clear();

            if (this.DamageConfigurator == null) return;

            this.DamageConfigurator.SetDamageAmount(payload.DamageAmount);
            this.DamageConfigurator.SetKnockbackForce(payload.KnockbackForce);

            if (payload.AttackOrigin != null)
            {
                this.DamageConfigurator.SetOrigin(payload.AttackOrigin);
            }

            this.DamageConfigurator.SetSourceCharacterFaction(payload.SourceFaction);

            if (this.AttackerCombatController != null)
            {
                CharacterRootController sourceCharacter = this.AttackerCombatController.GetCharacterRootController();
                this.DamageConfigurator.SetSourceCharacterRootController(sourceCharacter);
            }
        }

        //      3.2. Disable the Hitbox
        public virtual void DisableHitbox()
        {
            if (!this.IsActive) return;

            // IMPORTANT: must notify BEFORE IsActive becomes false
            this.NotifyHitboxDisabledWithoutHit();

            this.AttackerCombatController?.CombatStatusEffectHandler.ClearTrackingForHitbox(this.gameObject);

            this.IsActive = false;

            this.ClearHarvestDamage();
            this.ClearHitContext();
        }

        //  4. Damage Harvest
        public void ConfigureHarvestDamage(RuntimeHarvestDamage runtimeHarvestDamage)
        {
            if (this.HarvestDamageSourceProvider == null) return;

            this.HarvestDamageSourceProvider.ConfigureHarvestDamage(runtimeHarvestDamage);
        }

        public void ClearHarvestDamage()
        {
            if (this.HarvestDamageSourceProvider == null) return;

            this.HarvestDamageSourceProvider.ClearHarvestDamage();
        }

        // --- Helpers ---
        protected void ForwardHitConfirmedToCombat(UnityEngine.Object hitTarget, CharacterRootController characterTarget)
        {
            if (this.AttackerCombatController == null) return;
            if (this.ActiveRuntimeSkill == null) return;
            if (hitTarget == null) return;

            this.AttackerCombatController.OnHitConfirmedFromHitbox
            (
                runtimeSkill: this.ActiveRuntimeSkill,
                hitTarget: hitTarget,
                characterTarget: characterTarget,
                hitboxGameObject: this.gameObject
            );
        }
        protected void ForwardHitboxDisabledWithoutHitToCombat()
        {
            if (this.AttackerCombatController == null) return;
            if (this.ActiveRuntimeSkill == null) return;

            this.AttackerCombatController.OnHitboxDisabledWithoutHit
            (
                runtimeSkill: this.ActiveRuntimeSkill
            );
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.DamageConfiguratorSource != null) this.DamageConfigurator = this.DamageConfiguratorSource as IDamageConfigurator;
            if (this.DamageConfigurator == null) this.DamageConfigurator = this.GetComponent<IDamageConfigurator>(); // DamageDealingHandler

            if (this.HarvestDamageSourceProvider == null) _harvestDamageSourceProvider = this.GetComponent<HarvestDamageSourceProvider>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.DamageConfigurator == null)
            {
                Debug.LogError
                (
                    $"[{this.gameObject.name}.{this.GetType().Name}] " +
                    $"Missing {nameof(IDamageConfigurator)} " +
                    $"on the same GameObject.",
                    this
                );
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.IsActive = false;
            this.HasConfirmedHit = false;
            this.HasDisabledWithoutHit = false;

            this.HitTargetIDs.Clear();
            this.ClearHarvestDamage();

            var collider = this.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }
        }
    }
}
