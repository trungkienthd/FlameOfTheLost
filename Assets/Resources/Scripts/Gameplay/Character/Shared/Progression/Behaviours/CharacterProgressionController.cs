using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterProgressionController : CharacterConfiguredBehaviourBase<CharacterProgressionConfig>, ICharacterRefreshable, ISaveDataHandler<PlayerProgressionSaveData>
    {
        // --- Data / Configs ---
        public CharacterProgressionConfig ProgressionConfig => this.Config;

        // --- Runtime ---

        //  1. Progression Attributes
        [Header("Runtime - Progression Attributes")]
        [SerializeField] private List<RuntimeCharacterProgressionAttribute> _runtimeAttributeEntries = new();
        public IReadOnlyList<RuntimeCharacterProgressionAttribute> RuntimeAttributeEntries => _runtimeAttributeEntries;

        //  2. Derived Stats
        private IReadOnlyList<CharacterProgressionDerivedStatEntry> DerivedStatEntries
        {
            get
            {
                if (this.ProgressionConfig == null) return Array.Empty<CharacterProgressionDerivedStatEntry>();

                return this.ProgressionConfig.DerivedStatEntries;
            }
        }

        [Header("Runtime - Progression Derived Stats")]
        [SerializeField] private DerivedStatValueMap _derivedStatValueMap = new();
        private IReadOnlyDictionary<CharacterProgressionDerivedStatType, int> DerivedStatValueMap => _derivedStatValueMap;

        // --- References ---
        private HPController HPController => this.CharacterRootController.HPController;
        private FPController FPController => this.CharacterRootController.FPController;
        private StaminaController StaminaController => this.CharacterRootController.StaminaController;
        private CharacterCombatLoadoutController CombatLoadoutController => this.CharacterRootController.CombatLoadoutController;

        // --- Events ---
        public event Action OnProgressionChanged;

        // --- Unity Callbacks ---
        protected override void Start()
        {
            base.Start();

            this.ApplyDerivedStatsToCharacterSystems();
        }

        // --- Public APIs ---
        //  0. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 20;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.RecalculateAndApplyDerivedStats();

            this.OnProgressionChanged?.Invoke();
        }

        //  1. Queries for attributes and derived stats
        public int GetAttributeValue(CharacterProgressionAttributeType attributeType)
        {
            RuntimeCharacterProgressionAttribute entry = this.GetAttributeEntry(attributeType);

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

        //  2. Attribute Modification
        public void AddAttributeBonus(CharacterProgressionAttributeType attributeType, int amount)
        {
            if (amount <= 0) return;

            RuntimeCharacterProgressionAttribute entry = this.GetOrCreateAttributeEntry(attributeType);
            entry.AddBonus(amount);

            this.RecalculateAndApplyDerivedStats();

            this.OnProgressionChanged?.Invoke();
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
            RuntimeCharacterProgressionAttribute entry = this.GetOrCreateAttributeEntry(attributeType);
            entry.SetBonusValue(value);

            this.RecalculateAndApplyDerivedStats();

            this.OnProgressionChanged?.Invoke();
        }

        //  3. ISaveDataHandler implementation
        public PlayerProgressionSaveData CaptureSaveData()
        {
            PlayerProgressionSaveData saveData = new();

            for (int i = 0; i < this.RuntimeAttributeEntries.Count; i++)
            {
                RuntimeCharacterProgressionAttribute runtimeEntry = this.RuntimeAttributeEntries[i];
                if (runtimeEntry == null) continue;

                var saveEntry = new PlayerAttributeSaveEntry
                {
                    AttributeType = runtimeEntry.AttributeType,
                    BonusValue = runtimeEntry.BonusValue
                };
                saveData.Attributes.Add(saveEntry);
            }

            return saveData;
        }
        public void RestoreSaveData(PlayerProgressionSaveData saveData)
        {
            this.ResetRuntimeAttributesToConfig();

            if (saveData?.Attributes != null)
            {
                for (int i = 0; i < saveData.Attributes.Count; i++)
                {
                    PlayerAttributeSaveEntry saveEntry = saveData.Attributes[i];
                    if (saveEntry == null) continue;
                    if (!Enum.IsDefined(typeof(CharacterProgressionAttributeType), saveEntry.AttributeType)) continue;

                    RuntimeCharacterProgressionAttribute runtimeEntry = this.GetOrCreateAttributeEntry(saveEntry.AttributeType);
                    runtimeEntry.SetBonusValue(saveEntry.BonusValue);
                }

            }

            this.RecalculateAndApplyDerivedStats();

            this.OnProgressionChanged?.Invoke();
        }

        // --- Helpers ---
        //  1. Runtime Attribute Initialization
        private void InitializeRuntimeAttributesFromConfig()
        {
            if (this.ProgressionConfig == null) return;
            if (_runtimeAttributeEntries != null && this.RuntimeAttributeEntries.Count > 0) return;

            this.ResetRuntimeAttributesToConfig();
        }
        private void ResetRuntimeAttributesToConfig()
        {
            if (_runtimeAttributeEntries == null)
            {
                _runtimeAttributeEntries = new List<RuntimeCharacterProgressionAttribute>();
            }
            else
            {
                _runtimeAttributeEntries.Clear();
            }

            if (this.ProgressionConfig == null) return;

            IReadOnlyList<CharacterProgressionBaseAttributeEntry> baseAttributeEntries = this.ProgressionConfig.BaseAttributeEntries;
            if (baseAttributeEntries == null) return;

            for (int i = 0; i < baseAttributeEntries.Count; i++)
            {
                CharacterProgressionBaseAttributeEntry configEntry = baseAttributeEntries[i];
                if (configEntry == null) continue;

                RuntimeCharacterProgressionAttribute runtimeEntry = new RuntimeCharacterProgressionAttribute(configEntry);
                _runtimeAttributeEntries.Add(runtimeEntry);
            }
        }

        //  2. Attribute Lookup
        private RuntimeCharacterProgressionAttribute GetAttributeEntry(CharacterProgressionAttributeType attributeType)
        {
            if (this.RuntimeAttributeEntries == null) return null;

            return this.RuntimeAttributeEntries.FirstOrDefault(entry => entry.AttributeType == attributeType);
        }
        private RuntimeCharacterProgressionAttribute GetOrCreateAttributeEntry(CharacterProgressionAttributeType attributeType)
        {
            RuntimeCharacterProgressionAttribute entry = this.GetAttributeEntry(attributeType);
            if (entry != null) return entry;

            RuntimeCharacterProgressionAttribute newEntry = new RuntimeCharacterProgressionAttribute(attributeType, 0);
            _runtimeAttributeEntries.Add(newEntry);

            return newEntry;
        }

        //  3. Derived Stat Recalculation
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

        //  4. Derived Stat Application
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

            int maxEquippedWeapons          = this.GetDerivedStatValue(CharacterProgressionDerivedStatType.MAX_EQUIPPED_WEAPONS);
            int maxEquippedProjectiles      = this.GetDerivedStatValue(CharacterProgressionDerivedStatType.MAX_EQUIPPED_PROJECTILES);
            int maxEquippedConsumableItems  = this.GetDerivedStatValue(CharacterProgressionDerivedStatType.MAX_EQUIPPED_CONSUMABLE_ITEMS);

            if (maxEquippedWeapons          > 0) this.CombatLoadoutController.SetMaxEquippedWeaponsFromProgression(maxEquippedWeapons);
            if (maxEquippedProjectiles      > 0) this.CombatLoadoutController.SetMaxEquippedProjectilesFromProgression(maxEquippedProjectiles);
            if (maxEquippedConsumableItems  > 0) this.CombatLoadoutController.SetMaxEquippedConsumableItemsFromProgression(maxEquippedConsumableItems);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.ProgressionConfig == null) Debug.LogError($"[{this.CharacterName}.{nameof(CharacterProgressionController)}] {nameof(CharacterProgressionConfig)} reference is missing on {this.gameObject.name}");
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.InitializeRuntimeAttributesFromConfig();
            this.RecalculateDerivedStats();
        }
    }
}
