using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace FlameOfTheLost
{
    public abstract class SkillProfile : ScriptableObject, IIdentityProvider, IIconProvider
    {
        // --- Identity
        [Header("Identity")]
        [SerializeField] private int _id;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(2, 5)] private string _description;

        public int ID
        {
            get => _id;
            private set => _id = value;
        }
        public string DisplayName => _displayName;
        public string Description => _description;

        // --- Icon
        [Header("UI (Icon)")]
        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;

        // --- Style
        public abstract SkillStyleType Style 
        {
            get; 
        }

        // --- Provision (authoring hint; runtime can override)
        [Header("Provision (Authoring)")]
        [SerializeField] private SkillProvisionType _defaultProvisionType = SkillProvisionType.CharacterLearned;
        public SkillProvisionType DefaultProvisionType => _defaultProvisionType;

        // --- Weapon Compatibility
        [Header("Weapon Compatibility")]
        [SerializeField] private List<WeaponType> _attachableWeaponTypes;
        public IReadOnlyList<WeaponType> AttachableWeaponTypes => _attachableWeaponTypes;

        // --- Compatibility helper (policy: empty list => unrestricted)
        public bool IsCompatibleWithWeaponType(WeaponType weaponType)
        {
            if (this.AttachableWeaponTypes == null || this.AttachableWeaponTypes.Count == 0) return true;
            for (int i = 0; i < this.AttachableWeaponTypes.Count; i++)
            {
                if (this.AttachableWeaponTypes[i] == weaponType) return true;
            }
            return false;
        }

        // --- Attack Properties
        [Header("Attack Properties")]
        [SerializeField, Min(0)] private int _damageAmount = 10;
        [SerializeField, Min(0f)] private float _knockbackForce = 5f;
        public int DamageAmount => _damageAmount;
        public float KnockbackForce => _knockbackForce;

        // --- Resource Cost
        [Header("Resource Cost")]
        [SerializeField] private ResourceCost _resourceCost = new()
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
        public ResourceCost ResourceCost => _resourceCost;

        // --- Status Effects
        [Header("Status Effects")]
        [SerializeField] private List<SkillStatusEffectApplication> _statusEffectApplications = new();

        public IReadOnlyList<SkillStatusEffectApplication> StatusEffectApplications => _statusEffectApplications;

#if UNITY_EDITOR
        //private void OnValidate()
        //{
        //    // Uniqueness check across project (Editor only)
        //    var dup = SkillProfile.FindDuplicateID(this, this.ID);
        //    if (dup != null && dup != this)
        //    {
        //        Debug.LogError($"[SkillProfile] Duplicate ID={this.ID} detected between '{this.name}' and '{dup.name}'. " +
        //                       $"Please assign a different ID to one of them.", this);
        //    }
        //}

        [MenuItem("Tools/Combat/Skills/AssignNextFreeID")]
        private static void AssignNextFreeIDSelected()
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is SkillProfile skill)
                {
                    var used = SkillProfile.CollectUsedIDs();
                    int next = 0;
                    while (used.Contains(next)) next++;
                    Undo.RecordObject(skill, "Assign Next Free Skill ID");
                    skill.ID = next;
                    EditorUtility.SetDirty(skill);
                    Debug.Log($"Assigned ID={next} to Skill '{skill.name}'");
                }
            }
        }

        private static SkillProfile FindDuplicateID(SkillProfile skill, int id)
        {
            var guids = AssetDatabase.FindAssets("t:SkillProfile");
            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                var asset = AssetDatabase.LoadAssetAtPath<SkillProfile>(path);
                if (!asset || asset == skill) continue;
                if (asset.ID == id) return asset;
            }
            return null;
        }

        private static HashSet<int> CollectUsedIDs()
        {
            var set = new HashSet<int>();
            var guids = AssetDatabase.FindAssets("t:SkillProfile");
            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                var asset = AssetDatabase.LoadAssetAtPath<SkillProfile>(path);
                if (asset) set.Add(asset.ID);
            }
            return set;
        }
#endif
    }
}
