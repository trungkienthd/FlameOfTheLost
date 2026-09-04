using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterProgressionUpgradeHandler : CharacterBehaviourControllerBase,
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- Data / Configs ---
        public CharacterProgressionUpgradeConfig UpgradeConfig => this.ProgressionController?.ProgressionConfig?.UpgradeConfig;

        // --- References ---
        private CharacterProgressionController ProgressionController => this.CharacterRootController.ProgressionController;
        private CharacterRewardWalletController RewardWalletController => this.CharacterRootController.RewardWalletController;

        // --- Runtime
        //  1. Runtime Dictionary
        private readonly Dictionary<CharacterProgressionAttributeType, CharacterProgressionAttributeUpgradeCostEntry> _upgradeCostMap = new();
        private IReadOnlyDictionary<CharacterProgressionAttributeType, CharacterProgressionAttributeUpgradeCostEntry> UpgradeCostMap => _upgradeCostMap;

        //  2. Death Disable State (Runtime)
        [Header("Runtime - Death Disable")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled
        {
            get => _isDeathDisabled;
            private set => _isDeathDisabled = value;
        }

        // --- Public APIs ---
        //  1. Upgrade Handling
        public bool TryUpgrade(CharacterProgressionAttributeType attributeType)
        {
            if (!this.CanUpgrade(attributeType)) return false;

            int cost = this.GetUpgradeCost(attributeType);
            if (!this.RewardWalletController.TrySpend(cost))
            {
                return false;
            }

            this.ProgressionController.AddAttributeBonus(attributeType, 1);

            return true;
        }

        //  2. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 110;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.IsDeathDisabled = false;
        }

        //  3. ICharacterDeathDisableable implementation
        public int DeathDisablePriority => 70;
        public void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;
        }

        // --- Helpers ---
        //  1. Rebuild Cost Map
        private void RebuildCostMap()
        {
            _upgradeCostMap.Clear();

            if (this.UpgradeConfig == null) return;

            foreach (CharacterProgressionAttributeUpgradeCostEntry cost in this.UpgradeConfig.UpgradeCosts)
            {
                if (cost == null) continue;

                _upgradeCostMap[cost.AttributeType] = cost;
            }
        }

        //  2. Upgrade Handling
        private bool CanUpgrade(CharacterProgressionAttributeType attributeType)
        {
            if (this.IsDeathDisabled) return false;

            if (this.ProgressionController == null) return false;
            if (this.RewardWalletController == null) return false;

            int cost = this.GetUpgradeCost(attributeType);
            if (cost < 0) return false;

            return this.RewardWalletController.CanSpend(cost);
        }

        private int GetUpgradeCost(CharacterProgressionAttributeType attributeType)
        {
            if (this.ProgressionController == null) return -1;

            int currentValue = this.ProgressionController.GetAttributeValue(attributeType);

            if (this.UpgradeCostMap.TryGetValue(attributeType, out CharacterProgressionAttributeUpgradeCostEntry cost))
            {
                return cost.CalculateCost(currentValue);
            }

            return -1;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.UpgradeConfig == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterProgressionUpgradeHandler)}] {nameof(this.UpgradeConfig)} could not be resolved from {nameof(CharacterProgressionController)}.{nameof(CharacterProgressionController.ProgressionConfig)}.", this);
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.IsDeathDisabled = false;

            this.RebuildCostMap();
        }
    }
}
