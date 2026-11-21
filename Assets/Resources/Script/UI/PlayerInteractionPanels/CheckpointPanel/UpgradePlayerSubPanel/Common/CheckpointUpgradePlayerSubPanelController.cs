using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CheckpointUpgradePlayerSubPanelController : CheckpointPanelSubBehaviourBase
    {
        // --- References ---
        [Header("References - Sub Behaviours")]
        [SerializeField] private CheckpointUpgradeBoardController _upgradeBoardController;
        [SerializeField] private CheckpointUpgradeDerivedStatsPreviewController _derivedStatsPreview;
        public CheckpointUpgradeBoardController UpgradeBoardController => _upgradeBoardController;
        public CheckpointUpgradeDerivedStatsPreviewController DerivedStatsPreview => _derivedStatsPreview;

        private PlayerRootController PlayerRootController => this.BrainBehaviour?.PlayerRootController;
        public CharacterProgressionController ProgressionController => this.PlayerRootController?.ProgressionController;
        public CharacterProgressionUpgradeHandler ProgressionUpgradeHandler => this.PlayerRootController?.ProgressionUpgradeHandler;
        public CharacterRewardWalletController RewardWalletController => this.PlayerRootController?.RewardWalletController;
        public CharacterProgressionConfig ProgressionConfig  => this.ProgressionController?.ProgressionConfig;

        // --- Runtime ---
        //  1. Selected Attribute
        [Header("Runtime - Selection")]
        [SerializeField] private CharacterProgressionAttributeType _selectedAttributeType = CharacterProgressionAttributeType.MIGHT;
        public CharacterProgressionAttributeType SelectedAttributeType => _selectedAttributeType;

        //  2. Staged Upgrades
        private readonly Dictionary<CharacterProgressionAttributeType, int> _stagedUpgradeCounts = new();
        public IReadOnlyDictionary<CharacterProgressionAttributeType, int> StagedUpgradeCounts => _stagedUpgradeCounts;

        [SerializeField] private int _stagedTotalCost;
        public int StagedTotalCost => _stagedTotalCost;
        public bool HasStagedUpgrades => this.StagedTotalCost > 0;

        // --- Public APIs ---
        //  1. Visibility
        public void Show()
        {
            this.gameObject.SetActive(true);

            this.ResetStagedUpgrades();

            _selectedAttributeType = CharacterProgressionAttributeType.MIGHT;

            this.RefreshAll();
        }
        public void Hide()
        {
            this.ResetStagedUpgrades();

            this.gameObject.SetActive(false);
        }

        //  2. Attribute Selection
        public void SelectAttribute(CharacterProgressionAttributeType attributeType)
        {
            _selectedAttributeType = attributeType;

            this.RefreshAll();
        }

        //  3. Attribute Values
        public int GetCurrentAttributeValue(CharacterProgressionAttributeType attributeType)
        {
            if (this.ProgressionController == null) return 0;

            return this.ProgressionController.GetAttributeValue(attributeType);
        }
        public int GetStagedUpgradeCount(CharacterProgressionAttributeType attributeType)
        {
            return _stagedUpgradeCounts.TryGetValue(attributeType, out int count)
                ? count
                : 0;
        }
        public int GetProjectedAttributeValue(CharacterProgressionAttributeType attributeType)
        {
            return this.GetCurrentAttributeValue(attributeType) + this.GetStagedUpgradeCount(attributeType);
        }

        //  4. Lost Embers
        public int GetCurrentLostEmbers()
        {
            return this.RewardWalletController != null
                ? this.RewardWalletController.CurrentAmount
                : 0;
        }
        public int GetProjectedLostEmbers()
        {
            return Mathf.Max(0, this.GetCurrentLostEmbers() - this.StagedTotalCost);
        }

        //  5. Upgrade Cost
        public int GetNextUpgradeCost(CharacterProgressionAttributeType attributeType)
        {
            if (this.ProgressionUpgradeHandler == null) return -1;

            int projectedValue = this.GetProjectedAttributeValue(attributeType);

            return this.ProgressionUpgradeHandler.CalculateUpgradeCost(attributeType, projectedValue);
        }
        public bool CanStageUpgrade(CharacterProgressionAttributeType attributeType)
        {
            if (this.ProgressionUpgradeHandler == null) return false;
            if (this.RewardWalletController == null) return false;
            if (this.ProgressionUpgradeHandler.IsDeathDisabled) return false;

            int nextCost = this.GetNextUpgradeCost(attributeType);
            if (nextCost < 0) return false;

            int combinedCost = this.StagedTotalCost + nextCost;

            return this.RewardWalletController.CanSpend(combinedCost);
        }

        //  6. Stage Upgrade
        public bool StageUpgrade(CharacterProgressionAttributeType attributeType)
        {
            if (!this.CanStageUpgrade(attributeType)) return false;

            int nextCost = this.GetNextUpgradeCost(attributeType);

            if (_stagedUpgradeCounts.TryGetValue(attributeType, out int currentCount))
            {
                _stagedUpgradeCounts[attributeType] = currentCount + 1;
            }
            else
            {
                _stagedUpgradeCounts.Add(attributeType, 1);
            }

            _stagedTotalCost += nextCost;

            this.RefreshAll();

            return true;
        }

        //  7. Dismiss / Confirm
        public void DismissStagedUpgrades()
        {
            this.ResetStagedUpgrades();

            // SelectedAttributeType intentionally remains unchanged.

            this.RefreshAll();
        }
        public bool ConfirmStagedUpgrades()
        {
            if (!this.HasStagedUpgrades) return false;
            if (this.ProgressionUpgradeHandler == null) return false;

            bool success = this.ProgressionUpgradeHandler.TryCommitUpgrades(this.StagedUpgradeCounts);

            if (!success)
            {
                this.RefreshAll();
                return false;
            }

            this.ResetStagedUpgrades();
            this.RefreshAll();

            return true;
        }

        //  8. Display Data
        public CharacterProgressionAttributeDisplayEntry GetAttributeDisplayEntry(CharacterProgressionAttributeType attributeType)
        {
            return this.ProgressionConfig?.GetAttributeDisplayEntry(attributeType);
        }

        //  9. Derived Stats
        public CharacterProgressionDerivedStatEntry GetDerivedStatEntry(CharacterProgressionDerivedStatType derivedStatType)
        {
            if (this.ProgressionConfig == null) return null;

            foreach (CharacterProgressionDerivedStatEntry entry in this.ProgressionConfig.DerivedStatEntries)
            {
                if (entry == null) continue;
                if (entry.DerivedStatType != derivedStatType) continue;

                return entry;
            }

            return null;
        }
        public int GetCurrentDerivedStatValue(CharacterProgressionDerivedStatType derivedStatType)
        {
            if (this.ProgressionController == null) return 0;

            return this.ProgressionController.GetDerivedStatValue(derivedStatType);
        }
        public int GetProjectedDerivedStatValue(CharacterProgressionDerivedStatType derivedStatType)
        {
            CharacterProgressionDerivedStatEntry entry = this.GetDerivedStatEntry(derivedStatType);

            if (entry == null) return 0;

            int value = entry.BaseValue;

            foreach (CharacterProgressionDerivedStatContributionEntry contribution in entry.Contributions)
            {
                if (contribution == null) continue;

                int projectedAttributeValue = this.GetProjectedAttributeValue(contribution.AttributeType);

                value += contribution.CalculateValue(projectedAttributeValue);
            }

            if (entry.HasMaximumValue)
            {
                value = Mathf.Min(value, entry.MaximumValue);
            }

            return Mathf.Max(0, value);
        }
        public int GetCurrentDerivedStatAttributeBonus(CharacterProgressionDerivedStatType derivedStatType)
        {
            CharacterProgressionDerivedStatEntry entry = this.GetDerivedStatEntry(derivedStatType);

            if (entry == null) return 0;

            return Mathf.Max(0, this.GetCurrentDerivedStatValue(derivedStatType) - entry.BaseValue);
        }
        public int GetProjectedDerivedStatAttributeBonus(CharacterProgressionDerivedStatType derivedStatType)
        {
            CharacterProgressionDerivedStatEntry entry = this.GetDerivedStatEntry(derivedStatType);

            if (entry == null) return 0;

            return Mathf.Max(0, this.GetProjectedDerivedStatValue(derivedStatType) - entry.BaseValue);
        }

        //  10. Refresh
        public void RefreshAll()
        {
            this.UpgradeBoardController?.RefreshDisplay();
            this.DerivedStatsPreview?.RefreshDisplay();
        }

        // --- Helpers ---
        private void ResetStagedUpgrades()
        {
            _stagedUpgradeCounts.Clear();
            _stagedTotalCost = 0;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.UpgradeBoardController == null) _upgradeBoardController    = this.GetComponentInChildren<CheckpointUpgradeBoardController>(true);
            if (this.DerivedStatsPreview    == null) _derivedStatsPreview       = this.GetComponentInChildren<CheckpointUpgradeDerivedStatsPreviewController>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.UpgradeBoardController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(CheckpointUpgradeBoardController)} reference is missing.", this);
            }
            if (this.DerivedStatsPreview == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(CheckpointUpgradeDerivedStatsPreviewController)} reference is missing.", this);
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.UpgradeBoardController?.Bind(this);
            this.DerivedStatsPreview?.Bind(this);
        }
        protected override void UnbindInternalReferences()
        {
            this.UpgradeBoardController?.Unbind(this);
            this.DerivedStatsPreview?.Unbind(this);

            base.UnbindInternalReferences();
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _selectedAttributeType = CharacterProgressionAttributeType.MIGHT;

            this.ResetStagedUpgrades();
        }
    }
}