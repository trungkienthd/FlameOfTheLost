using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class MeleeCombatController : CombatController
    {
        [Header("Refs")]
        [SerializeField] private MeleeHitboxController _meleeHitboxController;
        [Tooltip("Component that accepts runtime payload updates (e.g., DamageDealingHandler).")]
        [SerializeField] private MonoBehaviour _damageConfiguratorSource;
        [Tooltip("Optional override for knockback direction (e.g., weapon bone)")]
        [SerializeField] private Transform _attackOriginOverride;
        public MeleeHitboxController MeleeHitboxController
        {
            get => _meleeHitboxController;
            private set => _meleeHitboxController = value;
        }
        public MonoBehaviour DamageConfiguratorSource
        {
            get => _damageConfiguratorSource;
            private set => _damageConfiguratorSource = value;
        }
        public Transform AttackOriginOverride
        {
            get => _attackOriginOverride;
            private set => _attackOriginOverride = value;
        }

        private IDamageConfigurator _damageConfigurator;
        private IDamageConfigurator DamageConfigurator
        {
            get => _damageConfigurator;
            set => _damageConfigurator = value;
        }

        protected override void Awake()
        {
            base.Awake();

            if (!this.MeleeHitboxController) this.MeleeHitboxController = this.GetComponentInChildren<MeleeHitboxController>();

            // Bind configurator (interface) from the serialized field if provided…
            if (this.DamageConfiguratorSource != null)
            {
                this.DamageConfigurator = this.DamageConfiguratorSource as IDamageConfigurator;
            }

            // …otherwise try to auto-locate any component in children that implements IDamageConfigurator.
            if (this.DamageConfigurator == null)
            {
                var allBehaviours = this.GetComponentsInChildren<MonoBehaviour>(true);
                this.DamageConfigurator = allBehaviours.FirstOrDefault(b => b is IDamageConfigurator) as IDamageConfigurator;
                if (this.DamageConfigurator == null)
                {
                    Debug.LogWarning("[MeleeCombatController] No IDamageConfigurator found. Damage will not update per skill.", this);
                }
            }
        }

        private void Update()
        {
            if (this.HasActiveCast && this.Channeling && this.CharacterResourcesController != null)
            {
                if (!this.TryTickPerSecondDrain(this.ActiveCost, ref this.LastPerSecondTickTimeRef))
                {                        
                    // Ran out mid-channel: stop hitbox if needed and end early (no refund)
                    if (this.MeleeHitboxController) this.MeleeHitboxController.EnableHitbox(false);
                    this.UnlockFacingAfterActiveWindow();
                    this.ClearActiveCast();

                    // Do NOT call MarkAttackEnded() here, because we didn’t reach animation End.
                    // (You can trigger a fizzle animation if you have one.)
                    return;
                }
            }
        }

        public override void DoAttack(AttackSlot slot, int skillId)
        {
            var weapon = this.CombatLoadoutController ? this.CombatLoadoutController.CurrentMeleeWeapon : null;
            if (weapon == null) return;                  // If weapon is not found, skip (non-melee character)
            if (!weapon.TryGetSkillById(slot, skillId, out var skill)) return; // Verify that the current weapon actually has this Id in the given slot

            // 1) Resource gate + reserve (base helper)
            if (!this.TryGateAndReserveSkill(
                    skill,
                    onFail: NotifyGateFail,
                    reservationCost: out this.ActiveCostRef,
                    costMode: out this.ActiveCostModeRef,
                    isChannel: out this.ChannelingRef,
                    channelStartTime: out this.LastPerSecondTickTimeRef))
            {
                return;
            }
            this.HasActiveCast = true;

            // 2) Now ask the controller’s own cooldown gate
            if (!this.TryBeginAttack(slot, skillId))
            {
                // Controller blocked it (time/required end). Refund our reservation
                this.RefundReservation(this.ActiveCost, 1f);
                this.ClearActiveCast();

                return;
            }

            // 3) Proceed as before (aim + animation)
            this.TryApplyAimFacingOverride();                       // Aim/facing first
            this.AnimationHandler?.TriggerMeleeAttack(skillId);     //  Drive Animator by *Id* (add an int parameter or your existing method can treat the int as Id)
        }

        // Called by Animation Events on the melee clips
        public void OnMeleeHitboxWindowStart()
        {
            if (!this.MeleeHitboxController) return;

            var weapon = this.CombatLoadoutController ? this.CombatLoadoutController.CurrentMeleeWeapon : null;
            // Non-melee or invalid skill: do nothing
            if (weapon == null)
            {
                Debug.LogWarning("[Melee] MeleeWeaponProfile is null.");
                return;
            }
            if (!weapon.TryGetSkillById(this.LastSlot, this.LastSkillId, out var skillProfile) || skillProfile == null)
            {
                Debug.LogWarning($"[Melee] No skill for slot={this.LastSlot}, id={this.LastSkillId}.");
                return;
            }

            this.LockFacingAndSnapshotDirection();

            // Push payload into the damager
            if (this.DamageConfigurator != null)
            {
                this.DamageConfigurator.SetDamageAmount(skillProfile.DamageAmount);
                this.DamageConfigurator.SetKnockbackForce(skillProfile.KnockbackForce);
                if (this.AttackOriginOverride) this.DamageConfigurator.SetOrigin(this.AttackOriginOverride);
            }

            // Align collider to current facing before enabling
            this.MeleeHitboxController.SetOverrideProfile(skillProfile.MeleeHitboxProfile);
            this.MeleeHitboxController.SyncPolygonColliderShapeFromFacing();

            // Hand the active skill's status effects to the Hitbox for this window
            this.MeleeHitboxController.BeginStatusEffectWindow(skillProfile.StatusEffectApplications, this.gameObject);

            this.MeleeHitboxController.EnableHitbox(true);
        }
        public void OnMeleeHitboxWindowEnd()
        {
            this.UnlockFacingAfterActiveWindow();

            if (this.MeleeHitboxController) this.MeleeHitboxController.ClearOverrideProfile();
            this.MeleeHitboxController?.EnableHitbox(false);

            // stop effect counting/context
            this.MeleeHitboxController?.EndStatusEffectWindow();

            // Finalize ticket and start cooldown (your base handles the cooldown)
            this.ClearActiveCast();

            this.MarkAttackEnded();
        }
    }
}
