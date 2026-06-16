using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterProgressionUpgradeHandler : CharacterBehaviourControllerBase,
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- States ---
        //  1. Data
        [Header("Progression Upgrade Config")]
        [SerializeField] private CharacterProgressionUpgradeConfig _upgradeConfig;
        public CharacterProgressionUpgradeConfig UpgradeConfig => _upgradeConfig;

        //  2. References
        private CharacterProgressionController ProgressionController => this.CharacterRootController.ProgressionController;
        private CharacterRewardWalletController RewardWalletController => this.CharacterRootController.RewardWalletController;

        //  3. Runtime Dictionary
        private readonly Dictionary<CharacterProgressionAttributeType, CharacterProgressionAttributeUpgradeCost> _upgradeCostMap = new();
        private IReadOnlyDictionary<CharacterProgressionAttributeType, CharacterProgressionAttributeUpgradeCost> UpgradeCostMap => _upgradeCostMap;

        //  4. Death Disable State (Runtime)
        [Header("Death Disable (Runtime)")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled
        {
            get => _isDeathDisabled;
            private set => _isDeathDisabled = value;
        }

        // --- Unity Callbacks ---
        private void Start()
        {
            this.RebuildCostMap();
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
        public void RefreshCharacter(CharacterRefreshmentContext context)
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

            foreach (CharacterProgressionAttributeUpgradeCost cost in this.UpgradeConfig.UpgradeCosts)
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

            if (this.UpgradeCostMap.TryGetValue(attributeType, out CharacterProgressionAttributeUpgradeCost cost))
            {
                return cost.CalculateCost(currentValue);
            }

            return -1;
        }

        //  2. Cache and Validate References
        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.UpgradeConfig == null)
            {
                _upgradeConfig = Resources.Load<CharacterProgressionUpgradeConfig>("Data/Character/Shared/Progression/PrgUpgrade_Generic");
                Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterProgressionUpgradeHandler)}] UpgradeConfig reference is not set on {this.gameObject.name}. Attempting to load default config from Resources. Loaded: {(this.UpgradeConfig != null)}");
            }
        }
        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.UpgradeConfig == null) Debug.LogError($"[{this.CharacterName}.{nameof(CharacterProgressionUpgradeHandler)}] UpgradeConfig reference is missing on {this.gameObject.name}");
        }
    }
}
