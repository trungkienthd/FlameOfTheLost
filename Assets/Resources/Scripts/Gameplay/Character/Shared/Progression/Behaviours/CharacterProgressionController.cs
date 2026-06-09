using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterProgressionController : CharacterBehaviourControllerBase
    {
        // --- States ---
        [Header("Progression Attributes")]
        [SerializeField] private List<CharacterProgressionAttributeEntry> _attributes = new();
        public IReadOnlyList<CharacterProgressionAttributeEntry> Attributes => _attributes;

        private HPController HPController => this.CharacterRootController.HPController;
        private FPController FPController => this.CharacterRootController.FPController;
        private StaminaController StaminaController => this.CharacterRootController.StaminaController;
        private CharacterCombatLoadoutController CombatLoadoutController => this.CharacterRootController.CombatLoadoutController;
        private CharacterRewardWalletController RewardWalletController => this.CharacterRootController.RewardWalletController;

        // --- Events ---
        public event Action<CharacterProgressionAttributeType, int> OnAttributeValueChanged;
        public event Action<CharacterProgressionAttributeType, int> OnAttributeLevelChanged;

        // --- Unity Callbacks ---
        private void Start()
        {
            this.ApplyProgressionToCharacterSystems(keepResourceRatio: false);
        }
#if UNITY_EDITOR
        //private void OnValidate()
        //{
        //    this.RemoveDuplicateAttributesInEditor();
        //}
#endif

        // --- Public APIs ---
        public int GetAttributeValue(CharacterProgressionAttributeType attributeType)
        {
            if (!this.TryGetAttributeEntry(attributeType, out CharacterProgressionAttributeEntry entry)) return 0;
            return entry.EffectiveValue;
        }
        public int GetAttributeLevel(CharacterProgressionAttributeType attributeType)
        {
            if (!this.TryGetAttributeEntry(attributeType, out CharacterProgressionAttributeEntry entry)) return 0;
            return entry.Level;
        }

        public bool CanUpgradeAttribute(CharacterProgressionAttributeType attributeType)
        {
            if (!this.TryGetAttributeEntry(attributeType, out CharacterProgressionAttributeEntry entry)) return false;
            if (entry.IsMaxLevel) return false;

            if (this.RewardWalletController == null) return false;
            return this.RewardWalletController.CanSpend(entry.NextLevelUpgradeCost);
        }

        public bool TryUpgradeAttribute(CharacterProgressionAttributeType attributeType)
        {
            if (!this.TryGetAttributeEntry(attributeType, out CharacterProgressionAttributeEntry entry)) return false;
            if (entry.IsMaxLevel) return false;
            if (this.RewardWalletController == null) return false;

            int cost = entry.NextLevelUpgradeCost;
            if (!this.RewardWalletController.TrySpend(cost)) return false;

            if (!entry.TryIncreaseLevel()) return false;

            this.ApplyProgressionToCharacterSystems(keepResourceRatio: true);

            this.OnAttributeLevelChanged?.Invoke(attributeType, entry.Level);
            this.OnAttributeValueChanged?.Invoke(attributeType, entry.EffectiveValue);

            return true;
        }

        public bool CanSatisfyRequirement(CharacterCapabilityRequirement requirement)
        {
            if (requirement == null) return true;

            int currentValue = this.GetAttributeValue(requirement.AttributeType);
            return currentValue >= requirement.RequiredValue;
        }

        public bool CanSatisfyRequirements(IReadOnlyList<CharacterCapabilityRequirement> requirements)
        {
            if (requirements == null || requirements.Count == 0) return true;

            for (int i = 0; i < requirements.Count; i++)
            {
                if (!this.CanSatisfyRequirement(requirements[i])) return false;
            }

            return true;
        }

        public void ApplyProgressionToCharacterSystems(bool keepResourceRatio)
        {
            int maxHP = this.GetAttributeValue(CharacterProgressionAttributeType.MAX_HP);
            int maxFP = this.GetAttributeValue(CharacterProgressionAttributeType.MAX_FP);
            int maxStamina = this.GetAttributeValue(CharacterProgressionAttributeType.MAX_STAMINA);

            int maxEquippedWeapons = this.GetAttributeValue(CharacterProgressionAttributeType.MAX_EQUIPPED_WEAPONS);
            int maxEquippedConsumables = this.GetAttributeValue(CharacterProgressionAttributeType.MAX_EQUIPPED_CONSUMABLES);

            if (this.HPController       != null & maxHP     > 0) this.HPController.SetMaxValue(maxHP, keepRatio: keepResourceRatio);
            if (this.FPController       != null & maxFP         > 0) this.FPController.SetMaxValue(maxFP, keepRatio: keepResourceRatio);
            if (this.StaminaController  != null & maxStamina    > 0) this.StaminaController.SetMaxValue(maxStamina, keepRatio: keepResourceRatio);

            if (this.CombatLoadoutController != null)
            {
                this.CombatLoadoutController.SetMaxEquippedWeaponsFromProgression(maxEquippedWeapons);
                this.CombatLoadoutController.SetMaxEquippedConsumablesFromProgression(maxEquippedConsumables);
            }
        }

        // --- Helpers ---
        private bool TryGetAttributeEntry(CharacterProgressionAttributeType attributeType, out CharacterProgressionAttributeEntry entry)
        {
            entry = null;

            if (_attributes == null) return false;

            for (int i = 0; i < this.Attributes.Count; i++)
            {
                CharacterProgressionAttributeEntry currentEntry = this.Attributes[i];
                if (currentEntry == null) continue;
                if (currentEntry.AttributeType != attributeType) continue;

                entry = currentEntry;
                return true;
            }

            return false;
        }

#if UNITY_EDITOR
        private void RemoveDuplicateAttributesInEditor()
        {
            if (_attributes == null) return;

            HashSet<CharacterProgressionAttributeType> seen = new();

            for (int i = this.Attributes.Count - 1; i >= 0; i--)
            {
                CharacterProgressionAttributeEntry entry = this.Attributes[i];
                if (entry == null) continue;

                if (seen.Contains(entry.AttributeType))
                {
                    _attributes.RemoveAt(i);
                    continue;
                }

                seen.Add(entry.AttributeType);
            }
        }
#endif
    }
}
