using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class CombatHUDController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterCombatLoadoutController _combatLoadoutController;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController
        {
            get => _combatLoadoutController;
            set => _combatLoadoutController = value;
        }

        [Header("Weapon")]
        [SerializeField] private Image _weaponIcon;
        private Image WeaponIcon
        {
            get => _weaponIcon;
        }            

        [Header("Special Skills")]
        [SerializeField] private Image _leftSpecialSkillIcon;
        [SerializeField] private Image _rightSpecialSkillIcon;
        private Image LeftSpecialSkillIcon
        {
            get => _leftSpecialSkillIcon;
        }
        private Image RightSpecialSkillIcon
        {
            get => _rightSpecialSkillIcon;
        }

        [Header("Next Special Skill Previews")]
        [SerializeField] private Image _nextLeftSpecialSkillIcon;   // child: CombatHUD/SkillBar/SlotLeft/NextSlotLeft/Icon
        [SerializeField] private Image _nextRightSpecialSkillIcon;  // child: CombatHUD/SkillBar/SlotRight/NextSlotRight/Icon
        [Tooltip("Hide next-skill previews when there is only one skill in the weapon.")]
        [SerializeField] private bool _hideNextWhenSingle = true;
        private Image NextLeftSpecialSkillIcon
        {
            get => _nextLeftSpecialSkillIcon;
        }
        private Image NextRightSpecialSkillIcon
        {
            get => _nextRightSpecialSkillIcon;
        }
        private bool HideNextWhenSingle
        {
            get => _hideNextWhenSingle;
        }

        [Header("Quick Item")]
        [SerializeField] private Image _quickItemIcon;
        [SerializeField] private MonoBehaviour _quickItemProviderBehaviour; // must implement IQuickItemSource
        private IQuickItemProvider _quickItemProvider;
        private Image QuickItemIcon
        {
            get => _quickItemIcon;
        }
        private MonoBehaviour QuickItemProviderBehaviour
        {
            get => _quickItemProviderBehaviour;
        }
        private IQuickItemProvider QuickItemProvider
        {
            get => _quickItemProvider;
            set => _quickItemProvider = value;
        }

        [Header("Refresh")]
        [Tooltip("If you don't have events yet, a very light polling is fine.")]
        [SerializeField] private float _refreshInterval = 0.1f;
        private float RefreshInterval
        {
            get => _refreshInterval;
        }

        private float _time;
        private float TimeData
        {
            get => _time;
            set => _time = value;
        }

        private void Awake()
        {
            if (!this.PlayerCombatLoadoutController) Debug.LogError($"[{this.name}] No Player's CharacterCombatLoadoutController found.", this);    

            // Optional Quick Item source hookup
            if (this.QuickItemProviderBehaviour is IQuickItemProvider src)
            {
                this.QuickItemProvider = src;
                this.QuickItemProvider.OnQuickItemChanged += this.RefreshQuickItem;
            }
        }

        private void OnEnable()
        {
            this.RefreshAll();
        }

        private void OnDisable()
        {
            if (this.QuickItemProvider != null)
            {
                this.QuickItemProvider.OnQuickItemChanged -= this.RefreshQuickItem;
            }
        }

        private void Update()
        {
            this.TimeData += Time.unscaledDeltaTime;
            if (this.TimeData >= this.RefreshInterval)
            {
                this.TimeData = 0f;
                this.RefreshAll();
            }
        }
        private void RefreshAll()
        {
            if (!this.PlayerCombatLoadoutController) return;

            // Weapon
            if (this.WeaponIcon)
            {
                var w = this.PlayerCombatLoadoutController.GetActiveEquippedWeaponIcon();
                this.WeaponIcon.enabled = w != null;
                this.WeaponIcon.sprite = w;
            }

            // Skills
            if (this.LeftSpecialSkillIcon)
            {
                var l = this.PlayerCombatLoadoutController.GetActiveSpecialSkillIcon(AttackSideType.Left);
                this.LeftSpecialSkillIcon.enabled = l != null;
                this.LeftSpecialSkillIcon.sprite = l;
            }
            if (this.RightSpecialSkillIcon)
            {
                var r = this.PlayerCombatLoadoutController.GetActiveSpecialSkillIcon(AttackSideType.Right);
                this.RightSpecialSkillIcon.enabled = r != null;
                this.RightSpecialSkillIcon.sprite = r;
            }

            // Next previews
            this.SetBothNextSpecialSkillPreviews(this.NextLeftSpecialSkillIcon, this.PlayerCombatLoadoutController.ActiveEquippedWeapon);

            // Quick item
            this.RefreshQuickItem();
        }

        /// Look ahead without changing the loadout: what would the next icon be if we cycled forward?
        private Sprite PeekNextSkillIcon(AttackSideType sideType, int direction, WeaponProfile weapon)
        {
            //direction = direction == 0 ? 1 : (direction > 0 ? 1 : -1);

            //if (weapon == null) return null;

            //var skills = weapon.GetSpecialSkills(sideType);
            //if (skills == null || skills.Count == 0) return null;

            //var currentSpecialSkill = (sideType == AttackSideType.Left) 
            //    ? this.PlayerCombatLoadoutController.CurrentLeftSpecialSkill 
            //    : this.PlayerCombatLoadoutController.CurrentRightSpecialSkill;

            //int index = 0;
            //if (currentSpecialSkill != null)
            //{
            //    for (int i = 0; i < skills.Count; i++)
            //    {
            //        if (System.Object.ReferenceEquals(skills[i], currentSpecialSkill))
            //        {
            //            index = i;
            //            break;
            //        }
            //    }
            //}

            //int nextIndex = CombatUtility.WrapIndex(index + direction, skills.Count);
            //return skills[nextIndex] ? skills[nextIndex].Icon : null;
            return null;
        }
        private bool HasOnlyOneSpecialSkillInCurrentWeapon(AttackSideType slot, WeaponProfile weapon)
        {
            //return weapon != null && weapon.SpecialSkillCount(slot) <= 1;
            return false;
        }
        private void SetNextSpecialSkillPreview(Image target, AttackSideType slot, WeaponProfile weapon)
        {
            Sprite next = this.PeekNextSkillIcon(slot, +1, weapon);
            var hasOnlyOne = this.HasOnlyOneSpecialSkillInCurrentWeapon(slot, weapon);

            if (this.HideNextWhenSingle && hasOnlyOne)
            {
                target.enabled = false;
                target.sprite = null;
            }
            else
            {
                target.enabled = next != null;
                target.sprite = next;
            }
        }
        private void SetBothNextSpecialSkillPreviews(Image target, WeaponProfile weapon)
        {
            if (this.NextLeftSpecialSkillIcon)  this.SetNextSpecialSkillPreview(target, AttackSideType.Left,    weapon);
            if (this.NextRightSpecialSkillIcon) this.SetNextSpecialSkillPreview(target, AttackSideType.Right,   weapon);
        }

        private void RefreshQuickItem()
        {
            if (!this.QuickItemIcon) return;

            if (this.QuickItemProvider == null)
            {
                // If no source provided, hide the slot (or keep frame only)
                this.QuickItemIcon.enabled = false;
                this.QuickItemIcon.sprite = null;
                return;
            }

            var s = this.QuickItemProvider.CurrentQuickItemIcon;
            this.QuickItemIcon.enabled = s != null;
            this.QuickItemIcon.sprite = s;
        }
    }
}
