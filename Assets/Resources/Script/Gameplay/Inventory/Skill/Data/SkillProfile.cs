using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class SkillProfile : ScriptableObject, IIdentityProvider, IIconProvider
    {
        // --- Data ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _id;
        [SerializeField] private string _key;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(2, 5)] private string _description;
        public int Id
        {
            get => _id;
            private set => _id = value;
        }
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        //  2. Icon
        [Header("UI (Icon)")]
        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;

        //  3. Style
        public abstract SkillStyle Style 
        {
            get; 
        }

        //  4. Provision (authoring hint; runtime can override)
        [Header("Provision (Authoring)")]
        [SerializeField] private SkillProvisionType _defaultProvisionType = SkillProvisionType.CharacterLearned;
        public SkillProvisionType DefaultProvisionType => _defaultProvisionType;

        //  5. Weapon Compatibility
        [Header("Weapon Compatibility")]
        [SerializeField] private List<WeaponType> _attachableWeaponTypes;
        public IReadOnlyList<WeaponType> AttachableWeaponTypes => _attachableWeaponTypes;

        //  6. Capability Requirements
        [Header("Capability Requirements")]
        [SerializeField] private List<CharacterCapabilityRequirementEntry> _requirements = new();
        public IReadOnlyList<CharacterCapabilityRequirementEntry> Requirements => _requirements;

        //  7. Attack Properties
        [Header("Attack Properties")]
        [SerializeField, Min(0)] private int _damageAmount = 10;
        [SerializeField, Min(0f)] private float _knockbackForce = 5f;
        public int DamageAmount => _damageAmount;
        public float KnockbackForce => _knockbackForce;

        //      7.1. Harvest (Damage) Properties
        [Header("Harvest Properties")]
        [SerializeField]
        private HarvestDamageEntry _harvestDamage = new();
        public HarvestDamageEntry HarvestDamage => _harvestDamage;

        //  8. Resource Cost
        [Header("Resource Cost")]
        [SerializeField] private ResourceCostEntry _resourceCost = new()
        {
            HPFlatCost = 0,
            FPFlatCost = 0,
            StaminaFlatCost = 0,
            HPPercentOfMaxCost = 0f,
            FPPercentOfMaxCost = 0f,
            StaminaPercentOfMaxCost = 0f,
            CostTriggerType = ResourceCostTriggerType.OnAttackStart,
            IsSilenceBlocked = true,
            AllowSelfKillWithHPCost = false
        };
        public ResourceCostEntry ResourceCost => _resourceCost;

        //  9. Status Effects
        [Header("Status Effects")]
        [SerializeField] private List<SkillStatusEffectApplicationEntry> _statusEffectApplications = new();

        public IReadOnlyList<SkillStatusEffectApplicationEntry> StatusEffectApplications => _statusEffectApplications;

        // --- Public APIs ---
        public bool IsCompatibleWithWeaponType(WeaponType weaponType)
        {
            if (this.AttachableWeaponTypes == null || this.AttachableWeaponTypes.Count == 0) return true;
            for (int i = 0; i < this.AttachableWeaponTypes.Count; i++)
            {
                if (this.AttachableWeaponTypes[i] == weaponType) return true;
            }
            return false;
        }

        public bool CanBeUsedBy(CharacterProgressionController progressionController)
        {
            return CharacterCapabilityRequirementUtility.AreRequirementsSatisfied(progressionController, this.Requirements);
        }
    }
}
