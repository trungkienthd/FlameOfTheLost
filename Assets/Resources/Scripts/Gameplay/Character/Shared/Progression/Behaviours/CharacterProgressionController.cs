using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterProgressionController : CharacterBehaviourControllerBase
    {
        // --- States ---
        [Header("Progression Attributes")]
        [SerializeField] private List<CharacterProgressionAttributeEntry> _attributeEntries = new();
        public IReadOnlyList<CharacterProgressionAttributeEntry> AttributeEntries => _attributeEntries;

        [Header("Derived Stats")]
        [SerializeField] private List<CharacterProgressionDerivedStatEntry> _derivedStatEntries = new();
        public IReadOnlyList<CharacterProgressionDerivedStatEntry> DerivedStatEntries => _derivedStatEntries;

        private readonly Dictionary<CharacterProgressionAttributeType, CharacterProgressionAttributeEntry> _attributeMap = new();
        private readonly Dictionary<CharacterProgressionDerivedStatType, int> _derivedStatValueMap = new();
        private IReadOnlyDictionary<CharacterProgressionAttributeType, CharacterProgressionAttributeEntry> AttributeMap => _attributeMap;
        private IReadOnlyDictionary<CharacterProgressionDerivedStatType, int> DerivedStatValueMap => _derivedStatValueMap;

        private HPController HPController => this.CharacterRootController.HPController;
        private FPController FPController => this.CharacterRootController.FPController;
        private StaminaController StaminaController => this.CharacterRootController.StaminaController;
        private CharacterCombatLoadoutController CombatLoadoutController => this.CharacterRootController.CombatLoadoutController;

        // --- Events ---
        public event Action ProgressionChanged;

        // --- Unity Callbacks ---
        private void Start()
        {
            this.RebuildMaps();
            this.RecalculateAndApplyDerivedStats();
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
            if (this.AttributeMap.TryGetValue(attributeType, out CharacterProgressionAttributeEntry entry))
            {
                return entry.CurrentValue;
            }

            return 0;
        }
        public int GetDerivedStatValue(CharacterProgressionDerivedStatType derivedStatType)
        {
            if (this.DerivedStatValueMap.TryGetValue(derivedStatType, out int value))
            {
                return value;
            }

            return 0;
        }

        public bool CanSatisfyRequirements(IReadOnlyList<CharacterCapabilityRequirement> requirements)
        {
            return CharacterCapabilityRequirementUtility.AreRequirementsSatisfied(this, requirements);
        }

        public void AddAttributeBonus(CharacterProgressionAttributeType attributeType, int amount)
        {
            if (amount <= 0) return;

            CharacterProgressionAttributeEntry entry = this.GetOrCreateAttributeEntry(attributeType);
            entry.AddBonus(amount);

            this.RebuildMaps();
            this.RecalculateAndApplyDerivedStats();

            this.ProgressionChanged?.Invoke();
        }

        public void SetAttributeBaseValue(CharacterProgressionAttributeType attributeType, int value)
        {
            CharacterProgressionAttributeEntry entry = this.GetOrCreateAttributeEntry(attributeType);
            entry.SetBaseValue(value);

            this.RebuildMaps();
            this.RecalculateAndApplyDerivedStats();

            this.ProgressionChanged?.Invoke();
        }

        public void SetAttributeBonusValue(CharacterProgressionAttributeType attributeType, int value)
        {
            CharacterProgressionAttributeEntry entry = this.GetOrCreateAttributeEntry(attributeType);
            entry.SetBonusValue(value);

            this.RebuildMaps();
            this.RecalculateAndApplyDerivedStats();

            this.ProgressionChanged?.Invoke();
        }

        // --- Helpers ---
        private void RebuildMaps()
        {
            _attributeMap.Clear();

            foreach (CharacterProgressionAttributeEntry entry in this.AttributeEntries)
            {
                if (entry == null) continue;

                _attributeMap[entry.AttributeType] = entry;
            }
        }

        private CharacterProgressionAttributeEntry GetOrCreateAttributeEntry(CharacterProgressionAttributeType attributeType)
        {
            if (this.AttributeMap.TryGetValue(attributeType, out CharacterProgressionAttributeEntry existingEntry))
            {
                return existingEntry;
            }

            CharacterProgressionAttributeEntry newEntry = new CharacterProgressionAttributeEntry(attributeType, 0);
            _attributeEntries.Add(newEntry);
            _attributeMap[attributeType] = newEntry;

            return newEntry;
        }

        public void RecalculateAndApplyDerivedStats()
        {
            this.RecalculateDerivedStats();
            this.ApplyDerivedStatsToCharacterSystems();
        }

        private void RecalculateDerivedStats()
        {
            _derivedStatValueMap.Clear();

            foreach (CharacterProgressionDerivedStatEntry entry in this.DerivedStatEntries)
            {
                if (entry == null) continue;

                int value = entry.CalculateValue(this);
                _derivedStatValueMap[entry.DerivedStatType] = value;
            }
        }

        private void ApplyDerivedStatsToCharacterSystems()
        {
            this.ApplyResourceDerivedStats();
            this.ApplyLoadoutDerivedStats();
        }

        private void ApplyResourceDerivedStats()
        {
            int maxHP       = this.GetDerivedStatValue(CharacterProgressionDerivedStatType.MAX_HP);
            int maxFP       = this.GetDerivedStatValue(CharacterProgressionDerivedStatType.MAX_FP);
            int maxStamina  = this.GetDerivedStatValue(CharacterProgressionDerivedStatType.MAX_STAMINA);

            if (this.HPController       != null && maxHP        > 0) this.HPController.SetMaxValue(maxHP, true);
            if (this.FPController       != null && maxFP        > 0) this.FPController.SetMaxValue(maxFP, true);
            if (this.StaminaController  != null && maxStamina   > 0) this.StaminaController.SetMaxValue(maxStamina, true);
        }

        private void ApplyLoadoutDerivedStats()
        {
            if (this.CombatLoadoutController == null) return;

            int maxEquippedWeapons = GetDerivedStatValue(CharacterProgressionDerivedStatType.MAX_EQUIPPED_WEAPONS);
            int maxEquippedConsumableItems = GetDerivedStatValue(CharacterProgressionDerivedStatType.MAX_EQUIPPED_CONSUMABLE_ITEMS);

            if (maxEquippedWeapons > 0)
            {
                this.CombatLoadoutController.SetMaxEquippedWeaponsFromProgression(maxEquippedWeapons);
            }

            if (maxEquippedConsumableItems > 0)
            {
                this.CombatLoadoutController.SetMaxEquippedConsumableItemsFromProgression(maxEquippedConsumableItems);
            }
        }

#if UNITY_EDITOR
        private void RemoveDuplicateAttributesInEditor()
        {
            if (_attributeEntries == null) return;

            HashSet<CharacterProgressionAttributeType> seen = new();

            for (int i = this.AttributeEntries.Count - 1; i >= 0; i--)
            {
                CharacterProgressionAttributeEntry entry = this.AttributeEntries[i];
                if (entry == null) continue;

                if (seen.Contains(entry.AttributeType))
                {
                    _attributeEntries.RemoveAt(i);
                    continue;
                }

                seen.Add(entry.AttributeType);
            }
        }
#endif
    }
}
