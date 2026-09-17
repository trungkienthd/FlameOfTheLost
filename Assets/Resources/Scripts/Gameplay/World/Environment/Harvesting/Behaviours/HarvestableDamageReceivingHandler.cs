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

            this.LogDebug($"{nameof(IDamager)} found on '{other.gameObject.name}'. EffectiveDamageAmount={damager.EffectiveDamageAmount}, SourceCharacter='{this.GetObjectName(damager.SourceCharacter)}'.");

            IHarvestDamageSource harvestDamageSource = other.GetComponent<IHarvestDamageSource>();
            if (harvestDamageSource == null)
            {
                this.LogDebug($"Harvest hit rejected: collider GameObject " + $"'{other.gameObject.name}' does not contain an " + $"{nameof(IHarvestDamageSource)} component. " + $"{nameof(HarvestDamageSourceProvider)} should normally " + $"be attached to the same GameObject as the hitbox collider.");
                return;
            }

            this.LogDebug($"{nameof(IHarvestDamageSource)} found. ImpactType={harvestDamageSource.ImpactType}, TriggerSelfOnHitStatusEffects={harvestDamageSource.TriggerSelfOnHitStatusEffects}.");

            if (Time.frameCount == this.LastHitFrame && System.Object.ReferenceEquals(damager, this.LastDamager))
            {
                this.LogDebug($"Harvest hit rejected as a duplicate hit. " + $"Frame={Time.frameCount}, " + $"Damager='{other.gameObject.name}'.");
                return;
            }

            HarvestImpactType impactType = harvestDamageSource.ImpactType;
            if (impactType == HarvestImpactType.None)
            {
                this.LogDebug($"Harvest hit rejected: {nameof(HarvestImpactType)} is {HarvestImpactType.None}.");
                return;
            }

            int baseHarvestDamageAmount = harvestDamageSource.ComputeBaseHarvestDamage(damager.EffectiveDamageAmount);
            if (baseHarvestDamageAmount <= 0)
            {
                this.LogDebug($"Harvest hit rejected: computed base harvest damage is {baseHarvestDamageAmount}.");
                return;
            }

            // Register this harvestable with the hitbox before applying the hit. This prevents repeated hits from the same hitbox activation even when effective durability damage becomes 0.
            HitboxController hitboxController = null;
            if (other.TryGetComponent<HitboxController>(out HitboxController foundHitboxController))
            {
                hitboxController = foundHitboxController;

                this.LogDebug($"{nameof(HitboxController)} found on '{other.gameObject.name}'. Attempting to register the harvestable object as a hit target.");

                bool targetRegistered = hitboxController.TryRegisterHitTarget(this.HarvestableObjectController);
                if (!targetRegistered)
                {
                    this.LogDebug($"Harvest hit rejected: {nameof(HitboxController)} refused to register '{this.HarvestableObjectController.gameObject.name}' as a new hit target. It may already have been hit during this hitbox activation.");
                    return;
                }
            }

            HarvestDamageRequestContext requestContext = new HarvestDamageRequestContext
            (
                sourceCharacter:    damager.SourceCharacter,
                sourceObject:       other.gameObject,
                impactType:         impactType,
                baseDamageAmount:   baseHarvestDamageAmount
            );

            this.LogDebug($"Attempting harvest hit. ImpactType={impactType}, BaseHarvestDamage={baseHarvestDamageAmount}, CurrentDurability={this.HarvestableObjectController.CurrentDurability}, MaximumDurability={this.HarvestableObjectController.MaximumDurability}.");

            bool hitApplied = this.HarvestableObjectController.TryApplyHarvestDamage(requestContext, out HarvestDamageAppliedContext appliedContext);

            if (!hitApplied)
            {
                this.LogDebug($"Harvest hit application failed. IsBroken={this.HarvestableObjectController.IsBroken}, ImpactType={impactType}, BaseDamage={baseHarvestDamageAmount}.");
                return;
            }

            this.LogDebug($"Harvest hit resolved. ImpactType={appliedContext.ImpactType}, BaseDamage={appliedContext.BaseDamageAmount}, ImpactMultiplier={appliedContext.ImpactDamageMultiplier}, AppliedDamage={appliedContext.AppliedDamageAmount}, CurrentDurability={this.HarvestableObjectController.CurrentDurability}.");

            this.LastHitFrame = Time.frameCount;
            this.LastDamager = damager;

            // The Harvestable Object is still a valid hit target even when its impact response reduces durability damage to zero.
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