using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class HarvestableDamageReceivingHandler : FOTLMonoBehaviour
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private HarvestableObjectController _harvestableObjectController;
        private HarvestableObjectController HarvestableObjectController => _harvestableObjectController;

        // --- Runtime States ---
        //  1. Last Damage Source (for Duplicate Hit Protection)
        private int _lastHitFrame = -1;
        private IDamager _lastDamager;
        private int LastHitFrame
        {
            get => _lastHitFrame;
            set => _lastHitFrame = Mathf.Max(-1, value);
        }
        private IDamager LastDamager
        {
            get => _lastDamager;
            set => _lastDamager = value;
        }

        // --- Unity Callbacks ---
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == null) return;

            if (this.HarvestableObjectController == null) return;
            if (this.HarvestableObjectController.IsBroken)
            {
                this.LogDebug($"Harvest hit rejected: harvestable object " + $"'{this.HarvestableObjectController.gameObject.name}' " + $"is already broken.");
                return;
            }
            if (!other.TryGetComponent<IDamager>(out IDamager damager)) return;

            this.LogDebug
            (
                $"{nameof(IDamager)} found on " +
                $"'{other.gameObject.name}'. " +
                $"EffectiveDamageAmount={damager.EffectiveDamageAmount}, " +
                $"SourceCharacter='{this.GetObjectName(damager.SourceCharacter)}'."
            );

            IHarvestDamageSource harvestDamageSource = other.GetComponent<IHarvestDamageSource>();
            if (harvestDamageSource == null)
            {
                this.LogDebug($"Harvest hit rejected: collider GameObject " + $"'{other.gameObject.name}' does not contain an " + $"{nameof(IHarvestDamageSource)} component. " + $"{nameof(HarvestDamageSourceProvider)} should normally " + $"be attached to the same GameObject as the hitbox collider.");
                return;
            }

            this.LogDebug
            (
                $"{nameof(IHarvestDamageSource)} found. " +
                $"ImpactType={harvestDamageSource.ImpactType}, " +
                $"TriggerSelfOnHitStatusEffects=" +
                $"{harvestDamageSource.TriggerSelfOnHitStatusEffects}."
            );

            if (Time.frameCount == this.LastHitFrame && System.Object.ReferenceEquals(damager, this.LastDamager))
            {
                this.LogDebug($"Harvest hit rejected as a duplicate hit. " + $"Frame={Time.frameCount}, " + $"Damager='{other.gameObject.name}'.");
                return;
            }

            HarvestImpactType impactType = harvestDamageSource.ImpactType;
            if (!this.HarvestableObjectController.AcceptsImpactType(impactType))
            {
                this.LogDebug($"Harvest hit rejected: impact type '{impactType}' " + $"is not accepted. Accepted impact types are " + $"'{this.HarvestableObjectController.AcceptedImpactTypes}'.");
                return;
            }

            int harvestDamageAmount = harvestDamageSource.ComputeHarvestDamage(damager.EffectiveDamageAmount);
            if (harvestDamageAmount <= 0)
            {
                this.LogDebug($"Harvest hit rejected: computed harvest damage " + $"is {harvestDamageAmount}.");
                return;
            }

            HitboxController hitboxController = null;
            if (other.TryGetComponent<HitboxController>(out HitboxController foundHitboxController))
            {
                hitboxController = foundHitboxController;

                this.LogDebug($"{nameof(HitboxController)} found on " + $"'{other.gameObject.name}'. Attempting to register " + $"the harvestable object as a hit target.");

                bool targetRegistered = hitboxController.TryRegisterHitTarget(this.HarvestableObjectController);

                if (!targetRegistered)
                {
                    this.LogDebug
                    (
                        $"Harvest hit rejected: " +
                        $"{nameof(HitboxController)} refused to register " +
                        $"'{this.HarvestableObjectController.gameObject.name}' " +
                        $"as a new hit target. It may already have been hit " +
                        $"during this hitbox activation."
                    );
                    return;
                }
            }

            HarvestDamageAppliedContext context = new HarvestDamageAppliedContext
            (
                sourceCharacter:    damager.SourceCharacter,
                sourceObject:       other.gameObject,
                impactType:         impactType,
                damageAmount:       harvestDamageAmount
            );
                       
            this.LogDebug
            (
                $"Attempting to apply {harvestDamageAmount} " +
                $"'{impactType}' harvest damage. " +
                $"CurrentDurability=" +
                $"{this.HarvestableObjectController.CurrentDurability}, " +
                $"MaximumDurability=" +
                $"{this.HarvestableObjectController.MaximumDurability}."
            );

            bool applied = this.HarvestableObjectController.TryApplyHarvestDamage(context);

            if (!applied)
            {
                this.LogDebug
                (
                    $"Harvest damage application failed. " +
                    $"IsBroken={this.HarvestableObjectController.IsBroken}, " +
                    $"CurrentDurability=" +
                    $"{this.HarvestableObjectController.CurrentDurability}, " +
                    $"ImpactType={impactType}, " +
                    $"DamageAmount={harvestDamageAmount}."
                );

                return;
            }

            this.LastHitFrame = Time.frameCount;
            this.LastDamager = damager;

            // A Harvestable Object is a valid hit target, but not a
            // Character status-effect target.
            hitboxController?.NotifyHitConfirmed
            (
                target:             this.HarvestableObjectController,
                characterTarget:    null
            );

            if (damager is IHitPostProcessor postProcessor)
            {
                postProcessor.OnSuccessfulHit();
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.HarvestableObjectController == null) _harvestableObjectController = this.GetComponentInParent<HarvestableObjectController>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.HarvestableObjectController == null) Debug.LogError($"[{this.gameObject.name}." + $"{nameof(HarvestableDamageReceivingHandler)}] " + $"{nameof(HarvestableObjectController)} " + $"reference is not assigned.", this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.LastHitFrame = -1;
            this.LastDamager = null;
        }
    }
}