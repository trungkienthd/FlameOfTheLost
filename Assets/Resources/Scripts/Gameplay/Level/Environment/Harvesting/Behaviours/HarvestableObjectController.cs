using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(HarvestableBrokenStateHandler))]
    [RequireComponent(typeof(HarvestableDropHandler))]
    public class HarvestableObjectController : FOTLMonoBehaviour
    {
        // --- Data ---
        [Header("Data")]
        [SerializeField]
        private HarvestableObjectProfile _profile;
        public HarvestableObjectProfile Profile => _profile;

        // --- References ---
        [Header("References")]
        [SerializeField]
        private HarvestableBrokenStateHandler _brokenStateHandler;
        [SerializeField] private HarvestableDropHandler _dropHandler;
        private HarvestableBrokenStateHandler BrokenStateHandler => _brokenStateHandler;
        private HarvestableDropHandler DropHandler => _dropHandler;

        // --- Configs ---
        //  1. Durability
        public int MaximumDurability => this.Profile.MaximumDurability;

        //  2. Accepted Impacts
        public HarvestImpactType AcceptedImpactTypes => this.Profile.AcceptedImpactTypes;

        // --- Runtime States ---
        [Header("Runtime State")]
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

        // --- Events ---
        public event Action<int, int> DurabilityChanged;
        public event Action<HarvestDamageAppliedContext> HarvestDamageApplied;
        public event Action<HarvestDamageAppliedContext> Broken;

        // --- Unity Callbacks ---
#if UNITY_EDITOR
        private void OnValidate()
        {
            _currentDurability = Mathf.Clamp(this.CurrentDurability, 0, this.MaximumDurability);
        }
#endif

        // --- Public APIs ---
        public bool AcceptsImpactType(HarvestImpactType impactType)
        {
            if (impactType == HarvestImpactType.None) return false;

            return (this.AcceptedImpactTypes & impactType) != 0;
        }

        public bool CanReceiveHarvestDamage(HarvestImpactType impactType, int damageAmount)
        {
            if (this.IsBroken) return false;
            if (damageAmount <= 0) return false;

            if (!this.AcceptsImpactType(impactType)) return false;

            return true;
        }

        public bool TryApplyHarvestDamage(HarvestDamageAppliedContext context)
        {
            if (!this.CanReceiveHarvestDamage(context.ImpactType, context.DamageAmount)) return false;

            int previousDurability = this.CurrentDurability;

            this.CurrentDurability -= context.DamageAmount;

            int appliedDamage = previousDurability - this.CurrentDurability;

            if (appliedDamage <= 0) return false;

            HarvestDamageAppliedContext appliedContext = new HarvestDamageAppliedContext
            (
                sourceCharacter: context.SourceCharacter,
                sourceObject: context.SourceObject,
                impactType: context.ImpactType,
                damageAmount: appliedDamage
            );

            this.DurabilityChanged?.Invoke(this.CurrentDurability, this.MaximumDurability);
            this.HarvestDamageApplied?.Invoke(appliedContext);

            if (this.CurrentDurability <= 0)
            {
                this.Break(appliedContext);
            }

            return true;
        }

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

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.BrokenStateHandler == null) _brokenStateHandler    = this.GetComponent<HarvestableBrokenStateHandler>();
            if (this.DropHandler        == null) _dropHandler           = this.GetComponent<HarvestableDropHandler>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Profile == null)
            {
                Debug.LogError
                (
                    $"[{this.gameObject.name}." +
                    $"{nameof(HarvestableObjectController)}] " +
                    $"{nameof(HarvestableObjectProfile)} reference " +
                    $"is not assigned.",
                    this
                );
            }

            if (this.BrokenStateHandler == null)
            {
                Debug.LogError
                (
                    $"[{this.gameObject.name}." +
                    $"{nameof(HarvestableObjectController)}] " +
                    $"{nameof(HarvestableBrokenStateHandler)} " +
                    $"reference is not assigned.",
                    this
                );
            }

            if (this.DropHandler == null)
            {
                Debug.LogError
                (
                    $"[{this.gameObject.name}." +
                    $"{nameof(HarvestableObjectController)}] " +
                    $"{nameof(HarvestableDropHandler)} " +
                    $"reference is not assigned.",
                    this
                );
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.BrokenStateHandler?.Bind(this);
            this.DropHandler?.Bind(this);
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

            this.DurabilityChanged?.Invoke(this.CurrentDurability, this.MaximumDurability);
        }
        protected override void UnbindInternalReferences()
        {
            this.BrokenStateHandler?.Unbind(this);
            this.DropHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
