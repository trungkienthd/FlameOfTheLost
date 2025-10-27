using System.Collections;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CombatLoadoutController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private CharacterStyleLoadout _styleLoadout;   // Provides melee/ranged weapon lists (SO)
        public CharacterStyleLoadout StyleLoadout => _styleLoadout;

        [Header("Controllers")]
        [SerializeField] private MeleeCombatController _meleeCombatController;
        [SerializeField] private RangedCombatController _rangedCombatController;
        public MeleeCombatController MeleeCombatController
        {
            get => _meleeCombatController;
            private set => _meleeCombatController = value;
        }
        public RangedCombatController RangedCombatController
        {
            get => _rangedCombatController;
            private set => _rangedCombatController = value;
        }
        private CombatController CurrentCombatController
        {
            get
            {
                switch (this.CurrentAttackStyle)
                {
                    case AttackStyle.Melee:
                        return this.MeleeCombatController;
                    case AttackStyle.Ranged:
                        return this.RangedCombatController;
                    default:
                        Debug.LogWarning($"[CombatLoadoutController] Unsupported AttackStyle '{this.CurrentAttackStyle}' detected. Returning null.");
                        return null;
                }
            }
        }

        [Header("Style & Weapon")]
        [SerializeField] private AttackStyle _currentAttackStyle = AttackStyle.Melee;
        [SerializeField] private MeleeWeaponProfile _currentMeleeWeapon;
        [SerializeField] private RangedWeaponProfile _currentRangedWeapon;
        // --- Public accessors ---
        public AttackStyle CurrentAttackStyle
        {
            get => _currentAttackStyle;
            set
            {
                if (_currentAttackStyle == value) return;
                _currentAttackStyle = value;
                this.ValidateSlotsForCurrentStyle();
            }
        }
        public MeleeWeaponProfile CurrentMeleeWeapon
        {
            get => _currentMeleeWeapon;
            private set => _currentMeleeWeapon = value;
        }
        public RangedWeaponProfile CurrentRangedWeapon
        {
            get => _currentRangedWeapon;
            private set => _currentRangedWeapon = value;
        }

        [Header("Special Skill Slots (2)")]
        // We keep references per style so we can swap styles without losing chosen skills
        [SerializeField] private MeleeSkillProfile _meleeLeftSpecialSkillSlot;
        [SerializeField] private MeleeSkillProfile _meleeRightSpecialSkillSlot;
        [SerializeField] private RangedSkillProfile _rangedLeftSpecialSkillSlot;
        [SerializeField] private RangedSkillProfile _rangedRightSpecialSkillSlot;
        public SkillProfile CurrentLeftSpecialSkillSlot
        {
            get
            {
                switch (this.CurrentAttackStyle)
                {
                    case AttackStyle.Melee:
                        return _meleeLeftSpecialSkillSlot;
                    case AttackStyle.Ranged:
                        return _rangedLeftSpecialSkillSlot;
                    default:
                        Debug.LogWarning($"[CombatLoadoutController] Unsupported AttackStyle '{this.CurrentAttackStyle}' detected. Returning null.");
                        return null;
                }
            }
            set
            {
                switch (this.CurrentAttackStyle)
                {
                    case AttackStyle.Melee:
                        _meleeLeftSpecialSkillSlot = value as MeleeSkillProfile;
                        break;
                    case AttackStyle.Ranged:
                        _rangedLeftSpecialSkillSlot = value as RangedSkillProfile;
                        break;
                    default:
                        Debug.LogWarning($"[CombatLoadoutController] Unsupported AttackStyle '{this.CurrentAttackStyle}' detected. Returning null.");
                        break;
                }
            }
        }
        public SkillProfile CurrentRightSpecialSkillSlot
        {
            get
            {
                switch (this.CurrentAttackStyle)
                {
                    case AttackStyle.Melee:
                        return _meleeRightSpecialSkillSlot;
                    case AttackStyle.Ranged:
                        return _rangedRightSpecialSkillSlot;
                    default:
                        Debug.LogWarning($"[CombatLoadoutController] Unsupported AttackStyle '{this.CurrentAttackStyle}' detected. Returning null.");
                        return null;
                }
            }
            set
            {
                switch (this.CurrentAttackStyle)
                {
                    case AttackStyle.Melee:
                        _meleeRightSpecialSkillSlot = value as MeleeSkillProfile;
                        break;
                    case AttackStyle.Ranged:
                        _rangedRightSpecialSkillSlot = value as RangedSkillProfile;
                        break;
                    default:
                        Debug.LogWarning($"[CombatLoadoutController] Unsupported AttackStyle '{this.CurrentAttackStyle}' detected. Returning null.");
                        break;
                }
            }
        }

        private void Awake()
        {
            if (!this.MeleeCombatController) this.MeleeCombatController = this.GetComponent<MeleeCombatController>();
            if (!this.RangedCombatController) this.RangedCombatController = this.GetComponent<RangedCombatController>();

            this.ValidateSlotsForCurrentStyle();
        }

        // ==================== STYLE / WEAPON API ====================
        public void SetAttackStyle(AttackStyle style) => this.CurrentAttackStyle = style;
        private static AttackStyle NextStyle(AttackStyle style) => (style == AttackStyle.Melee) ? AttackStyle.Ranged : AttackStyle.Melee;
        private static AttackStyle PreviousStyle(AttackStyle style) => CombatLoadoutController.NextStyle(style);

        private IList GetWeaponList()
        {
            return this.GetWeaponList(this.CurrentAttackStyle);
        }
        private IList GetWeaponList(AttackStyle style)
        {
            // Get the matching weapon list (call the generic API only once)
            IList weaponList = null;
            switch (style)
            {
                case AttackStyle.Melee:
                    if (this.StyleLoadout?.TryGetWeaponList<MeleeSkillProfile>(out var melee) == true)
                        weaponList = melee;
                    break;

                case AttackStyle.Ranged:
                    if (this.StyleLoadout?.TryGetWeaponList<RangedSkillProfile>(out var ranged) == true)
                        weaponList = ranged;
                    break;
            }

            if (weaponList == null || weaponList.Count <= 0) return null;

            return weaponList;
        }

        // Helper methods
        private void EnsureCurrentWeaponValid()
        {
            // Get the matching weapon list (call the generic API only once)
            var weaponList = this.GetWeaponList();

            // No list or empty: clear current weapon and return
            if (weaponList == null || weaponList.Count == 0)
            {
                this.SetCurrentWeaponFromObject(null);
                return;
            }

            var currentWeapon = this.GetCurrentWeaponObject();
            int idx = (currentWeapon != null) ? weaponList.IndexOf(currentWeapon) : -1;

            UnityEngine.Object next =
                idx >= 0
                ? (UnityEngine.Object)weaponList[idx]
                : (weaponList.Count > 0 ? (UnityEngine.Object)weaponList[0] : null);

            // If missing, pick the first; if list empty, set null
            this.SetCurrentWeaponFromObject(next);
        }

        private UnityEngine.Object GetCurrentWeaponObject()
        {
            // ternary keeps it concise; reference equality works on ScriptableObjects
            return this.CurrentAttackStyle == AttackStyle.Melee ? this.CurrentMeleeWeapon : this.CurrentRangedWeapon;
        }

        private void SetCurrentWeaponFromObject(UnityEngine.Object weaponObj)
        {
            switch (this.CurrentAttackStyle)
            {
                case AttackStyle.Melee:
                    this.CurrentMeleeWeapon = weaponObj as MeleeWeaponProfile;
                    break;
                case AttackStyle.Ranged:
                    this.CurrentRangedWeapon = weaponObj as RangedWeaponProfile;
                    break;
            }
        }

        public bool CycleWeapon(int direction)
        {
            if (!this.StyleLoadout) return false;

            int step = (int)Mathf.Sign(direction);
            if (step == 0) return false;

            var currStyle = this.CurrentAttackStyle;
            var otherStyle = (step > 0) ? CombatLoadoutController.NextStyle(currStyle)
                                        : CombatLoadoutController.PreviousStyle(currStyle);

            var currentWeaponList = this.GetWeaponList(currStyle);
            var otherWeaponList = this.GetWeaponList(otherStyle);

            // If current style has no weapons, hop to the other style (if available)
            if (currentWeaponList == null || currentWeaponList.Count == 0)
            {
                if (otherWeaponList == null || otherWeaponList.Count == 0) return false;

                // Change style first, then set weapon in that style
                this.SetAttackStyle(otherStyle);
                var pickAfterSwitch = (UnityEngine.Object)(step > 0 ? otherWeaponList[0] : otherWeaponList[otherWeaponList.Count - 1]);
                this.SetCurrentWeaponFromObject(pickAfterSwitch);
                // SetAttackStyle already validated slots
                return true;
            }

            var currentWeapon = this.GetCurrentWeaponObject();
            int idx = (currentWeapon != null) ? currentWeaponList.IndexOf(currentWeapon) : -1;

            // If current not found, snap to first in the active style
            if (idx < 0)
            {
                this.SetCurrentWeaponFromObject((UnityEngine.Object)currentWeaponList[0]);
                this.ValidateSlotsForCurrentStyle();
                return true;
            }

            bool atStart = (idx == 0);
            bool atEnd = (idx == currentWeaponList.Count - 1);

            // At an end of the current style's Weapon List, try switching styles first (per requirement)
            if ((step < 0 && atStart) || (step > 0 && atEnd))
            {
                if (otherWeaponList != null && otherWeaponList.Count > 0)
                {
                    this.SetAttackStyle(otherStyle); // switch style before setting weapon
                    var pickWeapon = (UnityEngine.Object)(step > 0 ? otherWeaponList[0] : otherWeaponList[otherWeaponList.Count - 1]);
                    this.SetCurrentWeaponFromObject(pickWeapon);
                    // style setter already validated
                    return true;
                }

                // Other style empty — clamp to current end
                var clamp = (UnityEngine.Object)(step > 0 ? currentWeaponList[currentWeaponList.Count - 1] : currentWeaponList[0]);
                this.SetCurrentWeaponFromObject(clamp);
                this.ValidateSlotsForCurrentStyle();
                return true;
            }

            // Normal in-list move
            var next = (UnityEngine.Object)currentWeaponList[idx + step];
            this.SetCurrentWeaponFromObject(next);
            this.ValidateSlotsForCurrentStyle();
            return true;
        }

        // ==================== SKILL SLOT API ====================

        public void CycleSpecialSkillSlot(AttackSlot slot, int direction)
        {
            switch (this.CurrentAttackStyle)
            {
                case AttackStyle.Melee:
                    this.CycleSpecialSkillSlot(slot, direction, this.CurrentMeleeWeapon);
                    break;
                case AttackStyle.Ranged:
                    this.CycleSpecialSkillSlot(slot, direction, this.CurrentRangedWeapon);
                    break;
            }
        }

        // ==================== ATTACK DISPATCH ====================
        // Generic local function keeps compile-time types intact.
        private void DoNormalAttack<TSkill>(AttackSlot slot, WeaponProfile<TSkill> weapon) where TSkill : SkillProfile
        {
            var skill = weapon.GetNormalSkill(slot); // returns TSkill, upcasts to SkillProfile implicitly
            if (!skill) return;
            this.CurrentCombatController?.DoAttack(slot, skill.Id);
        }
        public void TryDoNormalAttack(AttackSlot slot)
        {
            switch (this.CurrentAttackStyle)
            {
                case AttackStyle.Melee:
                    if (!this.CurrentMeleeWeapon) return;
                    this.DoNormalAttack(slot, this.CurrentMeleeWeapon);
                    break;

                case AttackStyle.Ranged:
                    if (!this.CurrentRangedWeapon) return;
                    this.DoNormalAttack(slot, this.CurrentRangedWeapon);
                    break;

                default:
                    Debug.LogWarning($"[CombatLoadoutController] Unsupported AttackStyle {this.CurrentAttackStyle}.");
                    return;
            }
        }


        private void DoSpecialAttack<TSkill>(AttackSlot slot, WeaponProfile<TSkill> weapon) where TSkill : SkillProfile
        {
            SkillProfile chosenSpecial = (slot == AttackSlot.Left) ? this.CurrentLeftSpecialSkillSlot : this.CurrentRightSpecialSkillSlot;

            // If the weapon has specials for this slot and a chosen special exists (and belongs to the list),
            // use it; otherwise fall back to the primary skill for that slot.
            var specialSkillList = weapon.GetSpecialSkillList(slot);
            var chosenTyped = chosenSpecial as TSkill;
            var useSpecial = specialSkillList != null && specialSkillList.Count > 0 && chosenSpecial && specialSkillList.Contains(chosenTyped);

            var skillToUse = useSpecial ? chosenSpecial : weapon.GetNormalSkill(slot);
            if (!skillToUse) return;

            this.CurrentCombatController?.DoAttack(slot, skillToUse.Id);
        }
        public void TryDoSpecialAttack(AttackSlot slot)
        {
            switch (this.CurrentAttackStyle)
            {
                case AttackStyle.Melee:
                    if (!this.CurrentMeleeWeapon) return;
                    this.DoSpecialAttack(slot, this.CurrentMeleeWeapon);
                    break;

                case AttackStyle.Ranged:
                    if (!this.CurrentRangedWeapon) return;
                    this.DoSpecialAttack(slot, this.CurrentRangedWeapon);
                    break;

                default:
                    Debug.LogWarning($"[CombatLoadoutController] Unsupported AttackStyle '{this.CurrentAttackStyle}'");
                    return;
            }
        }

        // ==================== UI HELPERS (Icons/Names) ====================

        public Sprite GetCurrentWeaponIcon()
        {
            IIconProvider iconSrc = this.CurrentAttackStyle == AttackStyle.Melee
                ? (IIconProvider)this.CurrentMeleeWeapon
                : (IIconProvider)this.CurrentRangedWeapon;

            return iconSrc?.Icon;
        }

        public Sprite GetSpecialSkillIcon(AttackSlot slot)
        {
            IIconProvider iconSrc = null;
            iconSrc = (slot == AttackSlot.Left) ? this.CurrentLeftSpecialSkillSlot : this.CurrentRightSpecialSkillSlot;

            return iconSrc?.Icon;
        }

        public float GetCurrentAttackCooldownNormalized()
        {
            var c = this.CurrentCombatController;
            if (c == null) return 0f;

            return c.CooldownNormalized;
        }

        // ==================== Internals ====================
        private void EnsureSkillSlotsValid<TSkill>(WeaponProfile<TSkill> weapon) where TSkill : SkillProfile
        {
            // LEFT
            var leftSkillList = weapon ? weapon.GetSpecialSkillList(AttackSlot.Left) : null;                        // Left slot pool
            var leftCurrent = this.CurrentLeftSpecialSkillSlot as TSkill;                                         // Left Current cast to TSkill

            if (leftSkillList == null || leftSkillList.Count == 0) this.CurrentLeftSpecialSkillSlot = null;
            else if (!leftCurrent || !leftSkillList.Contains(leftCurrent)) this.CurrentLeftSpecialSkillSlot = leftSkillList[0];

            // RIGHT
            var rightSkillList = weapon ? weapon.GetSpecialSkillList(AttackSlot.Right) : null;                      // Right slot pool
            var rightCurrent = this.CurrentRightSpecialSkillSlot as TSkill;                                         // Right Current cast to TSkill

            if (rightSkillList == null || rightSkillList.Count == 0) this.CurrentRightSpecialSkillSlot = null;
            else if (!rightCurrent || !rightSkillList.Contains(rightCurrent)) this.CurrentRightSpecialSkillSlot = rightSkillList[0];
        }
        private void ValidateSlotsForCurrentStyle()
        {
            // Make sure the current weapon reference still exists in the active list
            this.EnsureCurrentWeaponValid();

            switch (this.CurrentAttackStyle)
            {
                case AttackStyle.Melee:
                    this.EnsureSkillSlotsValid(this.CurrentMeleeWeapon);
                    break;

                case AttackStyle.Ranged:
                    this.EnsureSkillSlotsValid(this.CurrentRangedWeapon);
                    break;

                default:
                    Debug.LogWarning($"[CombatLoadoutController] Unsupported AttackStyle '{this.CurrentAttackStyle}'");
                    return;
            }
        }

        private void CycleSpecialSkillSlot<TSkill>(AttackSlot slot, int direction, WeaponProfile<TSkill> weapon) where TSkill : SkillProfile
        {
            var skillList = (weapon ? weapon.GetSpecialSkillList(slot) : null);
            if (skillList == null || skillList.Count == 0) return;

            var current = (slot == AttackSlot.Left) ? this.CurrentLeftSpecialSkillSlot : this.CurrentRightSpecialSkillSlot;
            int idx = Mathf.Max(0, skillList.IndexOf(current as TSkill));
            int next = CombatStatics.WrapIndex(idx + (int)Mathf.Sign(direction), skillList.Count);

            if (skillList.Count > 1)
            {
                var other = (slot == AttackSlot.Left) ? this.CurrentRightSpecialSkillSlot : this.CurrentLeftSpecialSkillSlot;
                if (skillList[next] == other)
                    next = CombatStatics.WrapIndex(next + (int)Mathf.Sign(direction), skillList.Count);
            }

            if (slot == AttackSlot.Left) this.CurrentLeftSpecialSkillSlot = skillList[next];
            else this.CurrentRightSpecialSkillSlot = skillList[next];
        }
    }
}
