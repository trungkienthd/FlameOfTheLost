using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class WeaponProfile<TSkill> : ScriptableObject, IIconProvider where TSkill : SkillProfile
    {
        [Header("Identity")]
        [Tooltip("Unique, stable identifier.")]
        [SerializeField] private int _id;
        [Tooltip("Shown in UI.")]
        [SerializeField] private string _displayName;
        [TextArea(2, 5)]
        [SerializeField] private string _description;
        [Tooltip("Main icon for inventory/slots.")]
        [SerializeField] private Sprite _icon;
        public int ID => _id;
        public string DisplayName => _displayName;
        public string Description => _description;
        public Sprite Icon => _icon;

        [Header("Skills")]
        [SerializeField] private TSkill _leftNormalSkill;
        [SerializeField] private TSkill _rightNormalSkill;
        [SerializeField] private List<TSkill> _leftSpecialSkills;
        [SerializeField] private List<TSkill> _rightSpecialSkills;
        protected TSkill LeftNormalSkill => _leftNormalSkill;
        protected TSkill RightNormalSkill => _rightNormalSkill;
        protected List<TSkill> LeftSpecialSkills => _leftSpecialSkills;
        protected List<TSkill> RightSpecialSkills => _rightSpecialSkills;

        public TSkill GetNormalSkill(AttackSlotType slot) => slot == AttackSlotType.Left ? this.LeftNormalSkill : this.RightNormalSkill;
        public List<TSkill> GetSpecialSkillList(AttackSlotType slot)
        {
            var list = (slot == AttackSlotType.Left) ? this.LeftSpecialSkills : this.RightSpecialSkills;
            if (list != null && list.Count > 0) return list;       // new per-slot

            return null;
        }

        public int SpecialSkillCount(AttackSlotType slot)
        {
            var skillList = this.GetSpecialSkillList(slot);
            return skillList?.Count ?? 0;
        }

        public bool TryGetSkillById(AttackSlotType slot, int id, out TSkill result)
        {
            var p = this.GetNormalSkill(slot);
            if (p && p.ID == id) 
            {
                result = p; 
                return true; 
            }

            var list = this.GetSpecialSkillList(slot);
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var s = list[i];
                    if (s && s.ID == id)
                    {
                        result = s; 
                        return true; 
                    }
                }
            }

            result = null;
            return false;
        }

        public TSkill GetFirstSpecialOrPrimarySkill(AttackSlotType slot)
        {
            var list = this.GetSpecialSkillList(slot);
            if (list != null && list.Count > 0) return list[0];
            return this.GetNormalSkill(slot);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            //if (this.NormalLeftSkill == null)
            //    Debug.LogError($"[{this.name}] Normal (Primary) LEFT skill is required.", this);
            //if (this.NormalRightSkill == null)
            //    Debug.LogError($"[{this.name}] Normal (Primary) RIGHT skill is required.", this);

            // Optional: prune nulls from lists
            //this.LeftSpecialSkills?.RemoveAll(s => s == null);
            //this.RightSpecialSkills?.RemoveAll(s => s == null);
        }
#endif
    }
}
