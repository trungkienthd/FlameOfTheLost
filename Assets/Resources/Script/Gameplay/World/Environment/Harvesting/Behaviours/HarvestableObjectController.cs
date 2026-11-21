using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(HarvestableBrokenStateHandler))]
    [RequireComponent(typeof(HarvestableDropHandler))]
    public class HarvestableObjectController : FOTLWorldObjectProfileBehaviour<HarvestableObjectProfile>
    {
        // --- References ---
        [Header("References")]

        //  1. Sub-behaviours
        [SerializeField] private HarvestableBrokenStateHandler _brokenStateHandler;
        [SerializeField] private HarvestableDropHandler _dropHandler;
        [SerializeField] private HarvestableDamageReceivingHandler _damageReceivingHandler;
        private HarvestableBrokenStateHandler BrokenStateHandler => _brokenStateHandler;
        private HarvestableDropHandler DropHandler => _dropHandler;
        private HarvestableDamageReceivingHandler DamageReceivingHandler => _damageReceivingHandler;

        //  2. Broken State
        [SerializeField] private Transform _brokenColliderPresetTransform;
        private Transform BrokenColliderPresetTransform => _brokenColliderPresetTransform;

        // --- Configs ---
        //  1. Durability
        public int MaximumDurability => this.Profile.MaximumDurability;

        // --- Runtime ---
        [Header("Runtime (State)")]
        [SerializeField] private int _currentDurability;
        [SerializeField] private bool _isBroken;
        public int CurrentDurability
        {
            get => _currentDurability;
            private set => _currentDurability = Mathf.Clamp(value, 0, this.MaximumDurability);
        }
        public bool IsBroken
        {
            get => _isBroken;
            private set => _isBroken = value;
        }

        private Collider2D _activeCollider;
        private Collider2D _damageReceiverCollider;
        private Collider2D _brokenColliderPreset;
        private Collider2D ActiveCollider
        {
            get => _activeCollider;
            set => _activeCollider = value;
        }
        private Collider2D DamageReceiverCollider
        {
            get => _damageReceiverCollider;
            set => _damageReceiverCollider = value;
        }
        private Collider2D BrokenColliderPreset
        {
            get => _brokenColliderPreset;
            set => _brokenColliderPreset = value;
        }

        // --- Events ---
        public event Action<int, int> DurabilityChanged;
        public event Action<HarvestDamageAppliedContext> HarvestHit;            // Raised for every valid harvest hit, including hits that resolve to zero durability damage.
        public event Action<HarvestDamageAppliedContext> HarvestDamageApplied;  // Raised only when durability is actually reduced.
        public event Action<HarvestDamageAppliedContext> Broken;

        // --- Public APIs ---
        //  1. Harvest Damage
        public bool CanReceiveHarvestHit(HarvestDamageRequestContext context)
        {
            if (this.IsBroken) return false;
            if (context.ImpactType == HarvestImpactType.None) return false;
            if (context.BaseDamageAmount <= 0) return false;

            return true;
        }
        public bool TryApplyHarvestDamage(HarvestDamageRequestContext requestContext, out HarvestDamageAppliedContext appliedContext)
        {
            appliedContext = default;

            if (!this.CanReceiveHarvestHit(requestContext)) return false;

            float impactDamageMultiplier = this.Profile.GetImpactDamageMultiplier(requestContext.ImpactType);

            int resolvedDamageAmount = this.ComputeEffectiveHarvestDamage(requestContext.BaseDamageAmount, impactDamageMultiplier);

            int previousDurability = this.CurrentDurability;

            if (resolvedDamageAmount > 0)
            {
                this.CurrentDurability -= resolvedDamageAmount;
            }

            int appliedDamageAmount = previousDurability - this.CurrentDurability;

            appliedContext = new HarvestDamageAppliedContext
            (
                sourceCharacter: requestContext.SourceCharacter,
                sourceObject: requestContext.SourceObject,
                impactType: requestContext.ImpactType,
                baseDamageAmount: requestContext.BaseDamageAmount,
                impactDamageMultiplier: impactDamageMultiplier,
                appliedDamageAmount: appliedDamageAmount
            );

            // A valid hit occurred even when the resulting durability damage is zero.
            this.HarvestHit?.Invoke(appliedContext);

            if (appliedDamageAmount <= 0)
            {
                return true;
            }

            this.DurabilityChanged?.Invoke(this.CurrentDurability, this.MaximumDurability);
            this.HarvestDamageApplied?.Invoke(appliedContext);

            if (this.CurrentDurability <= 0)
            {
                this.Break(appliedContext);
            }

            return true;
        }

        //  2. Impact Resolution
        public int ComputeEffectiveHarvestDamage(int baseDamageAmount, float impactDamageMultiplier)
        {
            baseDamageAmount = Mathf.Max(0, baseDamageAmount);
            impactDamageMultiplier = Mathf.Max(0f, impactDamageMultiplier);

            if (baseDamageAmount <= 0) return 0;
            if (impactDamageMultiplier <= 0f) return 0;

            float rawDamage = baseDamageAmount * impactDamageMultiplier;

            return Mathf.Max(0, Mathf.RoundToInt(rawDamage));
        }

        //  3. State
        public void ResetHarvestable()
        {
            this.IsBroken = false;
            this.CurrentDurability = this.MaximumDurability;

            this.BrokenStateHandler?.RestoreDefaultState();

            this.DurabilityChanged?.Invoke(this.CurrentDurability, this.MaximumDurability);
        }

        // --- Helpers ---
        //  1. Breaking
        private void Break(HarvestDamageAppliedContext context)
        {
            if (this.IsBroken) return;

            this.IsBroken = true;
            this.CurrentDurability = 0;

            this.BrokenStateHandler?.ApplyBrokenState();

            this.Broken?.Invoke(context);
        }

        //  2. Collider Configuration
        private void ConfigureColliders()
        {
            if (this.Profile == null) return;

            HarvestableColliderConfigurationRecord configuration = this.Profile.ColliderConfiguration;
            if (configuration == null) return;

            // a. Active Collider
            this.ActiveCollider = configuration.ActiveCollider?.ConfigureCollider(this.gameObject);

            // b. Damage Receiver Collider
            if (this.DamageReceivingHandler != null)
            {
                this.DamageReceiverCollider = configuration.DamageReceiverCollider?.ConfigureCollider(this.DamageReceivingHandler.gameObject);
            }

            // c. Broken Collider Preset
            if (this.BrokenColliderPresetTransform != null)
            {
                this.BrokenColliderPreset = configuration.BrokenColliderPreset?.ConfigureCollider(this.BrokenColliderPresetTransform.gameObject);
            }

            if (this.BrokenColliderPreset != null)
            {
                this.BrokenColliderPreset.enabled = false;
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.BrokenStateHandler     == null) _brokenStateHandler        = this.GetComponent<HarvestableBrokenStateHandler>();
            if (this.DropHandler            == null) _dropHandler               = this.GetComponent<HarvestableDropHandler>();
            if (this.DamageReceivingHandler == null) _damageReceivingHandler    = this.GetComponentInChildren<HarvestableDamageReceivingHandler>(true);

            if (this.BrokenColliderPresetTransform == null) _brokenColliderPresetTransform = this.transform.Find("BrokenColliderPreset");
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.BrokenStateHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(HarvestableObjectController)}] {nameof(HarvestableBrokenStateHandler)} reference is not assigned.", this);
            }
            if (this.DropHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(HarvestableObjectController)}] {nameof(HarvestableDropHandler)} reference is not assigned.", this);
            }
            if (this.DamageReceivingHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(HarvestableObjectController)}] {nameof(HarvestableDamageReceivingHandler)} reference is not assigned.", this);
            }

            if (this.BrokenColliderPresetTransform == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(HarvestableObjectController)}] Broken collider preset Transform reference is not assigned.", this);
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.BrokenStateHandler?.Bind(this);
            this.DropHandler?.Bind(this);
            this.DamageReceivingHandler?.Bind(this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.IsBroken = false;
            this.CurrentDurability = this.MaximumDurability;
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            this.ConfigureColliders();

            this.BrokenStateHandler?.ConfigureColliders
            (
                this.ActiveCollider,
                this.BrokenColliderPreset
            );

            this.DurabilityChanged?.Invoke(this.CurrentDurability, this.MaximumDurability);
        }
        protected override void UnbindInternalReferences()
        {
            this.DamageReceivingHandler?.Unbind(this);
            this.DropHandler?.Unbind(this);
            this.BrokenStateHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
