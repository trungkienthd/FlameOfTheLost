using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(DamageDealingHandler))]
    public abstract class HitboxController : MonoBehaviour
    {
        // --- Damage Configurator (same GameObject)
        [Header("Damage Configurator (Cached)")]
        [SerializeField] private MonoBehaviour _damageConfiguratorSource;
        protected MonoBehaviour DamageConfiguratorSource => _damageConfiguratorSource;

        private IDamageConfigurator _damageConfigurator;
        protected IDamageConfigurator DamageConfigurator
        {
            get => _damageConfigurator;
            private set => _damageConfigurator = value;
        }

        // --- Context
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

        // --- State
        [Header("State (Runtime)")]
        [SerializeField] private bool _isActive = true;
        [SerializeField] private bool _hasConfirmedHit = false;             // only confirm once per enable window
        [SerializeField] private bool _hasDisabledWithoutHit = false;        // expired without ever confirming a hit
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

        // --- Hit registry (per enable window)
        private readonly HashSet<int> _hitTargetIds = new();
        protected HashSet<int> HitTargetIDs => _hitTargetIds;

        // Boolean: whether to allow multiple targets to be hit during the same enable window.
        //      If false: first valid target consumes this hitbox (your current behavior).
        //      If true: allow multiple targets, but still only once per target per enable window.

        // --- Unity Callbacks ---
        protected virtual void Awake()
        {
            var collider = this.GetComponent<Collider2D>();
            if (collider) collider.isTrigger = true;

            // Cache configurator from same GO
            if (this.DamageConfiguratorSource != null) this.DamageConfigurator = this.DamageConfiguratorSource as IDamageConfigurator;
            if (this.DamageConfigurator == null) this.DamageConfigurator = this.GetComponent<IDamageConfigurator>(); // DamageDealingHandler
            if (this.DamageConfigurator == null) Debug.LogError($"[{this.gameObject.name}.HitboxController] Missing IDamageConfigurator on the same GameObject.", this);
        }

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

        //  2. Notification
        //      2.1. Notify the Hit, called by DamageReceivingHandler (on target) when a valid hit actually happened
        public virtual void NotifyHitConfirmed(CharacterRootController target)
        {
            if (!this.IsActive) return;

            this.HasConfirmedHit = true;
            this.ForwardHitConfirmedToCombat(target);
        }

        //      2.2. Notify disabled before hit
        public void NotifyHitboxDisabledWithoutHit()
        {
            if (!this.IsActive) return;
            if (this.AttackerCombatController == null) return;    // prevents startup spam
            if (this.ActiveRuntimeSkill == null) return;                // prevents startup spam
            if (this.HasConfirmedHit) return;                           // not a "miss"
            if (this.HasDisabledWithoutHit) return;                     // only once

            this.HasDisabledWithoutHit = true;
            this.ForwardHitboxDisabledWithoutHitToCombat();
        }

        //  3. Hitbox
        //      3.3. Enable the Hitbox
        public virtual void EnableHitbox(HitboxDamagePayload payload, AttackSide attackSideType)
        {
            this.IsActive = true;
            this.HasConfirmedHit = false; 
            this.HasDisabledWithoutHit = false;

            this.HitTargetIDs.Clear();

            if (this.DamageConfigurator != null)
            {
                this.DamageConfigurator.SetDamageAmount(payload.DamageAmount);
                this.DamageConfigurator.SetKnockbackForce(payload.KnockbackForce);
                if (payload.AttackOrigin != null) this.DamageConfigurator.SetOrigin(payload.AttackOrigin);
                this.DamageConfigurator.SetSourceCharacterFaction(payload.SourceFaction);
            }
        }

        //      3.4. Disable the Hitbox
        public virtual void DisableHitbox()
        {
            if (!this.IsActive) return;

            // IMPORTANT: must notify BEFORE IsActive becomes false
            this.NotifyHitboxDisabledWithoutHit();
            this.AttackerCombatController?.CombatStatusEffectHandler.ClearTrackingForHitbox(this.gameObject);

            this.IsActive = false;

            this.ClearHitContext();
        }

        //  4. Register Hits (Call before applying damage). Returns:
        //      - True if this hit should be processed (damage + notify).
        //      - False = already hit this target in this enable window.
        public bool TryRegisterHitTarget(CharacterRootController target)
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

        // --- Helpers ---
        protected void ForwardHitConfirmedToCombat(CharacterRootController target)
        {
            if (this.AttackerCombatController == null) return;
            if (this.ActiveRuntimeSkill == null) return;

            this.AttackerCombatController.OnHitConfirmedFromHitbox
            (
                runtimeSkill: this.ActiveRuntimeSkill,
                target: target,
                hitboxGameObject: this.gameObject
            );
        }
        protected void ForwardHitboxDisabledWithoutHitToCombat()
        {
            if (this.AttackerCombatController == null) return;
            if (this.ActiveRuntimeSkill == null) return;

            this.AttackerCombatController.OnHitboxDisabledWithoutHit(runtimeSkill: this.ActiveRuntimeSkill);
        }
    }
}
