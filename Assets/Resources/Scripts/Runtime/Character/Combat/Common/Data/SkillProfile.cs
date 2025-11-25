using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class SkillProfile : ScriptableObject, IIconProvider
    {
        [Header("Identity")]
        [Tooltip("Unique index in Combat, Animation, ...")]
        [SerializeField] private int _id;

        [Tooltip("Name for clarity")]
        [SerializeField] private string _skillName;

        [Tooltip("Shown in UI.")]
        [SerializeField] private string _displayName;

        [TextArea(2, 4)]
        [SerializeField] private string _description;

        [Tooltip("Main icon for inventory/slots.")]
        [SerializeField] private Sprite _icon;

        public int ID
        {
            get => _id;
            private set => _id = value;
        }
        public string SkillName => _skillName;
        public string DisplayName => _displayName;
        public string Description => _description;
        public Sprite Icon => _icon;

        [Header("Attack Properties")]
        [SerializeField, Min(0)] private int _damageAmount = 10;
        [SerializeField, Min(0f)] private float _knockbackForce = 5f;

        [Min(0)] public int DamageAmount => _damageAmount;
        [Min(0f)] public float KnockbackForce => _knockbackForce;

        [Header("Resource Cost")]
        [SerializeField]
        private ResourceCost _resourceCost = new()
        {
            Stamina = 0,
            FP = 0,
            StaminaPercentOfMax = 0f,
            FPPercentOfMax = 0f,
            CostMode = ResourceCostType.OnStart,
            RequiresFP = false,
            PerSecondStamina = 0f,
            PerSecondFP = 0f
        };
        public ResourceCost ResourceCost => _resourceCost;

        [Header("Status Effects")]
        [SerializeField] private List<StatusEffectApplication> _statusEffectApplications;
        public IReadOnlyList<StatusEffectApplication> StatusEffectApplications => _statusEffectApplications;

#if UNITY_EDITOR
        //private void OnValidate()
        //{
        //    // Uniqueness check across project (Editor only)
        //    var dup = SkillProfile.FindDuplicateId(this, this.ID);
        //    if (dup != null && dup != this)
        //    {
        //        Debug.LogError($"[SkillProfile] Duplicate Id={this.ID} detected between '{this.name}' and '{dup.name}'. " +
        //                       $"Please assign a different Id to one of them.", this);
        //    }
        //}

        [MenuItem("Tools/Combat/Skills/AssignNextFreeId")]
        private static void AssignNextFreeIdSelected()
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is SkillProfile sp)
                {
                    var used = SkillProfile.CollectUsedIds();
                    int next = 0;
                    while (used.Contains(next)) next++;
                    Undo.RecordObject(sp, "Assign Next Free Skill Id");
                    sp.ID = next;
                    EditorUtility.SetDirty(sp);
                    Debug.Log($"Assigned Id={next} to Skill '{sp.name}'");
                }
            }
        }

        private static SkillProfile FindDuplicateId(SkillProfile me, int id)
        {
            var guids = AssetDatabase.FindAssets("t:SkillProfile");
            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                var asset = AssetDatabase.LoadAssetAtPath<SkillProfile>(path);
                if (!asset || asset == me) continue;
                if (asset.ID == id) return asset;
            }
            return null;
        }

        private static HashSet<int> CollectUsedIds()
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
