using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class RangedCombatController : CombatController
    {
        [Header("Refs")]
        [SerializeField] private Transform _rangedAnchor;
        public Transform RangedAnchor
        {
            get => _rangedAnchor;
            private set => _rangedAnchor = value;
        }

        protected override void Awake()
        {
            base.Awake();

            if (!this.RangedAnchor) this.RangedAnchor = this.transform.Find("RangedAnchor");
        }

        private void Update()
        {
            if (this.HasActiveCast && this.Channeling && this.CharacterResourcesController != null)
            {
                if (!this.TryTickPerSecondDrain(this.ActiveCost, ref this.LastPerSecondTickTimeRef))
                {
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
            var weapon = this.CombatLoadoutController ? this.CombatLoadoutController.CurrentRangedWeapon : null;
            if (weapon == null) return;
            if (!weapon.TryGetSkillById(slot, skillId, out var skill)) return;  // validate the Id actually exists on the current weapon for this slot

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
                this.RefundReservation(this.ActiveCost, 1f);
                this.ClearActiveCast();
                return;
            }

            // 3) Proceed as before (aim + animation)
            this.TryApplyAimFacingOverride();                       // Aim/facing first
            this.AnimationHandler?.TriggerRangedAttack(skillId);     //  Drive Animator by *Id* (add an int parameter or your existing method can treat the int as Id)
        }

        // Called by Animation Event at the release frame
        public void OnRangedFire()
        {
            var weapon = this.CombatLoadoutController ? this.CombatLoadoutController.CurrentRangedWeapon : null;

            if (weapon == null || !weapon.TryGetSkillById(this.LastSlot, this.LastSkillId, out var skillProfile) || skillProfile == null) return;
            if (skillProfile.ArrowPrefab == null || this.RangedAnchor == null) return;

            this.LockFacingAndSnapshotDirection();

            var projectile = Object.Instantiate(skillProfile.ArrowPrefab, this.RangedAnchor.position, Quaternion.identity);
            // map owner layer -> projectile layer and apply to all children
            ProjectileLayerUtility.ApplyOwnerMeta(projectile, this.gameObject);

            // Retrieve the necessary refs
            var rb = projectile.GetComponent<Rigidbody2D>();
            var allBehaviours = projectile.GetComponentsInChildren<MonoBehaviour>(true);
            IDamageConfigurator damageConfigurator = allBehaviours.FirstOrDefault(b => b is IDamageConfigurator) as IDamageConfigurator;

            // Set Damage amount & Knockback force
            if (damageConfigurator != null)
            {
                damageConfigurator.SetDamageAmount(skillProfile.DamageAmount);
                damageConfigurator.SetKnockbackForce(skillProfile.KnockbackForce);
                if (this.RangedAnchor) damageConfigurator.SetOrigin(this.RangedAnchor);
            }

            // Aim direction captured in DoAttack (fallbacks handled in helper)
            Vector2 direction = this.GetCurrentAttackDirection();
            if (direction.sqrMagnitude < 0.0001f) direction = Vector2.right;
            direction.Normalize();

            // Rotate so 0 (deg) = +X, counter-clockwise positive
            float angleDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply rotation (prefer RB2D API when you have a Rigidbody2D)
            if (rb)
            {
                rb.SetRotation(angleDeg); // sets Z rotation in degrees for 2D bodies
            }
            else
            {
                projectile.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
            }

            // Debug apples-to-apples:
            //float eulerZ = go.transform.eulerAngles.z; // 0..360 range
            //float rbZ = rb ? rb.rotation : float.NaN;  // RB2D rotation in degrees
            //Debug.Log($"AngleDeg={angleDeg:F2} | EulerZ={eulerZ:F2} | RB2D.Z={rbZ:F2}");

            // Velocity along the same direction
            if (rb) rb.linearVelocity = direction * skillProfile.ArrowSpeed;

            // Hook Status effect(s) to instantiated projectile
            var statusEffectHandler = projectile.GetComponent<ProjectileStatusEffectsHandler>();
            if (!statusEffectHandler) statusEffectHandler = projectile.AddComponent<ProjectileStatusEffectsHandler>();
            statusEffectHandler.Initialize(source: this.gameObject, statusEffectApplications: skillProfile.StatusEffectApplications);
                
           if (skillProfile.ArrowLifetime > 0f) Object.Destroy(projectile, skillProfile.ArrowLifetime); // Destroy the arrow after its lifetime
        }

        public void OnRangedEnd()
        {
            this.UnlockFacingAfterActiveWindow();
            this.ClearActiveCast();
            this.MarkAttackEnded();
        }
    }
}
