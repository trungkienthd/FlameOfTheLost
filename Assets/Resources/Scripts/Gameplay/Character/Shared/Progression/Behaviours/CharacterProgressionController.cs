using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterProgressionController : CharacterBehaviourControllerBase, ICharacterPlacementRefreshable
    {
        // --- States ---
        [Header("Progression Initial Configurations")]
        [SerializeField] private CharacterProgressionConfig _progressionConfig;
        public CharacterProgressionConfig ProgressionConfig => _progressionConfig;

        [Header("Progression Attributes (Runtime)")]
        [SerializeField] private List<CharacterProgressionAttributeRuntimeEntry> _runtimeAttributeEntries = new();
        public IReadOnlyList<CharacterProgressionAttributeRuntimeEntry> RuntimeAttributeEntries => _runtimeAttributeEntries;
        private IReadOnlyList<CharacterProgressionDerivedStatEntry> DerivedStatEntries
        {
            get
            {
                if (this.ProgressionConfig == null)
                {
                    return Array.Empty<CharacterProgressionDerivedStatEntry>();
                }

                return this.ProgressionConfig.DerivedStatEntries;
            }
        }

        [Header("Progression Derived Stats (Runtime)")]
        [SerializeField] private DerivedStatValueMap _derivedStatValueMap = new();
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
            this.InitializeProgression();
        }
#if UNITY_EDITOR
        //private void OnValidate()
        //{
        //    this.RemoveDuplicateAttributesInEditor();
        //}
#endif

        // --- Public APIs ---
        //  1. Getters for attributes and derived stats
        public int GetAttributeValue(CharacterProgressionAttributeType attributeType)
        {
            CharacterProgressionAttributeRuntimeEntry entry = this.GetAttributeEntry(attributeType);

            return entry != null ? entry.CurrentValue : 0;
        }
        public int GetDerivedStatValue(CharacterProgressionDerivedStatType derivedStatType)
        {
            if (this.DerivedStatValueMap.TryGetValue(derivedStatType, out int value))
            {
                return value;
            }

            return 0;
        }

        //  2. Methods to modify attributes (e.g., adding bonuses from upgrades)
        public void AddAttributeBonus(CharacterProgressionAttributeType attributeType, int amount)
        {
            if (amount <= 0) return;

            CharacterProgressionAttributeRuntimeEntry entry = this.GetOrCreateAttributeEntry(attributeType);
            entry.AddBonus(amount);

            this.RecalculateAndApplyDerivedStats();

            this.ProgressionChanged?.Invoke();
        }

        //public void SetAttributeBaseValue(CharacterProgressionAttributeType attributeType, int value)
        //{
        //    CharacterProgressionAttributeRuntimeEntry entry = this.GetOrCreateAttributeEntry(attributeType);
        //    entry.SetBaseValue(value);

        //    this.RebuildMaps();
        //    this.RecalculateAndApplyDerivedStats();

        //    this.ProgressionChanged?.Invoke();
        //}

        public void SetAttributeBonusValue(CharacterProgressionAttributeType attributeType, int value)
        {
            CharacterProgressionAttributeRuntimeEntry entry = this.GetOrCreateAttributeEntry(attributeType);
            entry.SetBonusValue(value);

            this.RecalculateAndApplyDerivedStats();

            this.ProgressionChanged?.Invoke();
        }

        //  3. ICharacterPlacementRefreshable implementation
        public int PlacementConfigurePriority => 20;
        public void RefreshAfterCharacterPlacement(CharacterPlacementContext context)
        {
            this.InitializeProgression();
        }

        // --- Helpers ---
        private void InitializeProgression()
        {
            this.InitializeRuntimeAttributesFromConfig();
            this.RecalculateAndApplyDerivedStats();
        }

        private void InitializeRuntimeAttributesFromConfig()
        {
            if (this.ProgressionConfig == null) return;

            // Do not overwrite runtime attributes if they already exist.
            // This keeps the system compatible with future save/load.
            if (_runtimeAttributeEntries != null && this.RuntimeAttributeEntries.Count > 0) return;

            _runtimeAttributeEntries = new List<CharacterProgressionAttributeRuntimeEntry>();

            foreach (CharacterProgressionBaseAttributeConfigEntry configEntry in this.ProgressionConfig.BaseAttributeEntries)
            {
                if (configEntry == null) continue;

                CharacterProgressionAttributeRuntimeEntry runtimeEntry = new CharacterProgressionAttributeRuntimeEntry(configEntry);
                _runtimeAttributeEntries.Add(runtimeEntry);
            }
        }

        private CharacterProgressionAttributeRuntimeEntry GetAttributeEntry(CharacterProgressionAttributeType attributeType)
        {
            return this.RuntimeAttributeEntries.FirstOrDefault(entry => entry.AttributeType == attributeType);
        }

        private CharacterProgressionAttributeRuntimeEntry GetOrCreateAttributeEntry(CharacterProgressionAttributeType attributeType)
        {
            CharacterProgressionAttributeRuntimeEntry entry = this.GetAttributeEntry(attributeType);

            if (entry != null) return entry;

            CharacterProgressionAttributeRuntimeEntry newEntry = new CharacterProgressionAttributeRuntimeEntry(attributeType, 0);
            _runtimeAttributeEntries.Add(newEntry);

            return newEntry;
        }

        private void RecalculateAndApplyDerivedStats()
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
            int maxHP = this.GetDerivedStatValue(CharacterProgressionDerivedStatType.MAX_HP);
            int maxFP = this.GetDerivedStatValue(CharacterProgressionDerivedStatType.MAX_FP);
            int maxStamina = this.GetDerivedStatValue(CharacterProgressionDerivedStatType.MAX_STAMINA);

            if (this.HPController != null && maxHP > 0) this.HPController.SetMaxValue(maxHP, true);
            if (this.FPController != null && maxFP > 0) this.FPController.SetMaxValue(maxFP, true);
            if (this.StaminaController != null && maxStamina > 0) this.StaminaController.SetMaxValue(maxStamina, true);
        }

        private void ApplyLoadoutDerivedStats()
        {
            if (this.CombatLoadoutController == null) return;

            int maxEquippedWeapons = this.GetDerivedStatValue(CharacterProgressionDerivedStatType.MAX_EQUIPPED_WEAPONS);
            int maxEquippedConsumableItems = this.GetDerivedStatValue(CharacterProgressionDerivedStatType.MAX_EQUIPPED_CONSUMABLE_ITEMS);

            if (maxEquippedWeapons > 0)
            {
                this.CombatLoadoutController.SetMaxEquippedWeaponsFromProgression(maxEquippedWeapons);
            }

            if (maxEquippedConsumableItems > 0)
            {
                this.CombatLoadoutController.SetMaxEquippedConsumableItemsFromProgression(maxEquippedConsumableItems);
            }
        }

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.ProgressionConfig == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterProgressionUpgradeHandler)}] ProgressionConfig reference is not set on {this.gameObject.name}. Loading the default config from Resources.");
                _progressionConfig = Resources.Load<CharacterProgressionConfig>("Data/Character/Shared/Progression/Prg_Generic");
            }
        }

        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.ProgressionConfig == null) Debug.LogError($"[{this.CharacterName}.{nameof(CharacterProgressionController)}] ProgressionConfig reference is missing on {this.gameObject.name}");
        }

#if UNITY_EDITOR
        private void RemoveDuplicateAttributesInEditor()
        {
            if (_runtimeAttributeEntries == null) return;

            HashSet<CharacterProgressionAttributeType> seen = new();

            for (int i = this.RuntimeAttributeEntries.Count - 1; i >= 0; i--)
            {
                CharacterProgressionAttributeRuntimeEntry entry = this.RuntimeAttributeEntries[i];
                if (entry == null) continue;

                if (seen.Contains(entry.AttributeType))
                {
                    _runtimeAttributeEntries.RemoveAt(i);
                    continue;
                }

                seen.Add(entry.AttributeType);
            }
        }
#endif
    }
}
