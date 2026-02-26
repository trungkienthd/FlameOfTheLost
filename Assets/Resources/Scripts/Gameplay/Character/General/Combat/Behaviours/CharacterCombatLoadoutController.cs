using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterCombatLoadoutController : MonoBehaviour, IStatusEffectFlagConsumer
    {
        // --- References
        [Header("References")]
        [SerializeField] private CharacterCombatAttackController _combatAttackController;
        private CharacterCombatAttackController CombatAttackController
        {
            get => _combatAttackController;
            set => _combatAttackController = value;
        }

        // --- Character Tag
        [Header("Character Tag")]
        [SerializeField] private CharacterTagType _characterTag = CharacterTagType.Default;
        public CharacterTagType CharacterTag => _characterTag;

        // --- Capacity
        [Header("Equipped Capacity")]
        [SerializeField, Min(1)] private int _maxEquippedWeapons = 2;
        public int MaxEquippedWeapons
        {
            get => _maxEquippedWeapons;
            private set => _maxEquippedWeapons = Mathf.Max(1, value);
        }

        // --- Equipped weapons (subset of inventory definitions)
        [Header("Equipped Weapons (Definitions)")]
        [SerializeField] private List<WeaponProfile> _equippedWeapons = new();
        public List<WeaponProfile> EquippedWeapons
        {
            get => _equippedWeapons;
            private set => _equippedWeapons = value;
        }

        // --- Equipped Weapon
        [Header("Equipped Weapon")]
        [SerializeField] private WeaponProfile _activeEquippedWeapon;   // Currently equipped weapon
        public WeaponProfile ActiveEquippedWeapon
        {
            get => _activeEquippedWeapon;
            private set => _activeEquippedWeapon = value;
        }
        public RuntimeWeapon ActiveEquippedRuntimeWeapon => this.GetOrCreateRuntimeInstance(this.ActiveEquippedWeapon);

        // --- Per-weapon runtime config (per character)
        // Key: WeaponProfile.ID
        [Header("Runtime Weapon Config (Per Character)")]
        [SerializeField] private readonly RuntimeWeaponDictionary _runtimeWeaponMap = new();
        private RuntimeWeaponDictionary RuntimeWeaponMap => _runtimeWeaponMap;

        // --- Status Effects states
        [Header("Status Effects Flags (Runtime)")]
        [SerializeField] private bool _cannotSwapWeapon;
        [SerializeField] private bool _ignoreSwapLock;
        private bool CannotSwapWeapon
        {
            get => _cannotSwapWeapon;
            set => _cannotSwapWeapon = value;
        }
        private bool IgnoreSwapLock
        {
            get => _ignoreSwapLock;
            set => _ignoreSwapLock = value;
        }
        public bool IsWeaponSwapLocked => this.CannotSwapWeapon && !this.IgnoreSwapLock;

        // --- Events
        public event Action<WeaponProfile> OnActiveEquippedWeaponChange;
        public event Action<AttackSideType, SkillProfile> OnActiveSpecialSkillChange;

        // --- Unity Methods
        private void Awake()
        {
            if (this.CombatAttackController == null) this.CombatAttackController = this.GetComponent<CharacterCombatAttackController>();

            this.ClampEquippedWeaponsToCapacity();
            this.EnsureActiveWeaponIsValid();
        }

        private void OnValidate()
        {
            if (this.MaxEquippedWeapons < 1) this.MaxEquippedWeapons = 1;
        }

        // --- Public APIs
        //  1. Switch Lock
        public bool AttackIsActive => (this.CombatAttackController != null && this.CombatAttackController.IsInAttackWindow);

        //  2. Cycle Weapon: Step:
        //      PositiveValue = Next,
        //      NegativeValue = Previous
        public bool CycleWeapon(int step)
        {
            if (this.IsWeaponSwapLocked)
            {
                Debug.Log($"[{this.gameObject.name}.CharacterCombatLoadoutController] CycleWeapon blocked: CannotSwapWeapon flag is active.");
                return false;
            }
            if (this.AttackIsActive)
            {
                Debug.Log($"[{this.gameObject.name}.CharacterCombatLoadoutController] CycleWeapon blocked: switching is locked.");
                return false;
            }

            step = (int)Mathf.Sign(step);
            if (step == 0) return false;

            if (this.EquippedWeapons == null || this.EquippedWeapons.Count == 0) return false;

            int count = this.EquippedWeapons.Count;

            // If nothing equipped yet: pick first or last depending on direction
            if (this.ActiveEquippedWeapon == null)
            {
                this.SetActiveWeapon(step > 0 ? this.EquippedWeapons[0] : this.EquippedWeapons[count - 1]);
                return true;
            }

            int currentIndex = this.EquippedWeapons.IndexOf(this.ActiveEquippedWeapon);
            if (currentIndex < 0)
            {
                // Equipped weapon not found in loadout: fallback safely
                this.SetActiveWeapon(this.EquippedWeapons[0]);
                return true;
            }

            int nextIndex = (currentIndex + step + count) % count;
            var nextWeapon = this.EquippedWeapons[nextIndex];

            // No change handling
            if (System.Object.ReferenceEquals(nextWeapon, this.ActiveEquippedWeapon)) return false;

            // Equip next weapon
            this.SetActiveWeapon(nextWeapon);
            return true;
        }

        //  3. Equip Specific Weapon (non-active)
        //      3.1. Equip Weapon
        public bool TryEquipWeapon(WeaponProfile weapon)
        {
            if (weapon == null) return false;
            if (this.AttackIsActive) return false;

            if (this.EquippedWeapons.Contains(weapon)) return false;
            if (this.EquippedWeapons.Count >= this.MaxEquippedWeapons) return false;

            this.EquippedWeapons.Add(weapon);

            // Ensure runtime exists for this weapon
            this.GetOrCreateRuntimeInstance(weapon);

            // If no active weapon, set this as active
            if (this.ActiveEquippedWeapon == null) this.SetActiveWeapon(weapon);

            return true;
        }

        //      3.2. Unequip Weapon
        public bool TryUnequipWeapon(WeaponProfile weapon)
        {
            if (weapon == null) return false;
            if (this.AttackIsActive) return false;

            if (!this.EquippedWeapons.Remove(weapon)) return false;

            // If removing active weapon, choose a new active
            if (UnityEngine.Object.ReferenceEquals(this.ActiveEquippedWeapon, weapon))
            {
                var newActive = this.EquippedWeapons.Count > 0 ? this.EquippedWeapons[0] : null;
                this.SetActiveWeapon(newActive);
            }

            return true;
        }


        //  4. UI APIs
        //      4.1. Get Equipped Weapon (Active) Icon
        public Sprite GetActiveEquippedWeaponIcon()
        {
            if (this.ActiveEquippedWeapon != null) return this.ActiveEquippedWeapon.Icon;

            return null; // Or return a default "unarmed" icon
        }

        //      4.2. Get Current Special Skill Icon
        public Sprite GetActiveSpecialSkillIcon(AttackSideType attackSide)
        {
            var weapon = this.ActiveEquippedWeapon;
            if (weapon == null) return null;

            var runtimeWeapon = this.GetOrCreateRuntimeInstance(weapon);
            SkillProfile resolvedSkill = weapon.ResolveSpecialSkill(attackSide, runtimeWeapon?.ActiveLeftSpecialSkill, runtimeWeapon?.ActiveRightSpecialSkill);

            return resolvedSkill != null ? resolvedSkill.Icon : null;
        }

        //  5. Status Effect Flag consumption
        public void SetStatusEffectFlag(StatusEffectFlagType flagType, bool value)
        {
            switch (flagType)
            {
                case StatusEffectFlagType.CannotSwapWeapon:
                    this.CannotSwapWeapon = value;
                    return;

                case StatusEffectFlagType.IgnoreSwapLock:
                    this.IgnoreSwapLock = value;
                    return;

                default:
                    return;
            }
        }

        // --- Helpers
        //  1. Equip Weapon
        private void SetActiveWeapon(WeaponProfile newWeapon)
        {
            if (UnityEngine.Object.ReferenceEquals(this.ActiveEquippedWeapon, newWeapon)) return;

            this.ActiveEquippedWeapon = newWeapon;
            this.OnActiveEquippedWeaponChange?.Invoke(this.ActiveEquippedWeapon);

            // Notify current "active special" (resolved) for UI refresh
            if (newWeapon != null)
            {
                var runtimeWeapon = this.GetOrCreateRuntimeInstance(newWeapon);
                this.OnActiveSpecialSkillChange?.Invoke(AttackSideType.Left, newWeapon.ResolveSpecialSkill(AttackSideType.Left, runtimeWeapon.ActiveLeftSpecialSkill, runtimeWeapon.ActiveRightSpecialSkill));
                this.OnActiveSpecialSkillChange?.Invoke(AttackSideType.Right, newWeapon.ResolveSpecialSkill(AttackSideType.Right, runtimeWeapon.ActiveLeftSpecialSkill, runtimeWeapon.ActiveRightSpecialSkill));
            }
            else
            {
                this.OnActiveSpecialSkillChange?.Invoke(AttackSideType.Left, null);
                this.OnActiveSpecialSkillChange?.Invoke(AttackSideType.Right, null);
            }
        }

        private RuntimeWeapon GetOrCreateRuntimeInstance(WeaponProfile weapon)
        {
            if (weapon == null) return null;

            if (this.RuntimeWeaponMap.TryGetValue(weapon.ID, out var runtimeWeapon) && runtimeWeapon != null) return runtimeWeapon;

            runtimeWeapon = new RuntimeWeapon(weapon, this.gameObject);
            this.RuntimeWeaponMap[weapon.ID] = runtimeWeapon;

            return runtimeWeapon;
        }

        private void ClampEquippedWeaponsToCapacity()
        {
            if (this.EquippedWeapons == null) this.EquippedWeapons = new List<WeaponProfile>();
            if (this.EquippedWeapons.Count <= this.MaxEquippedWeapons) return;

            // Simple policy: trim from end
            this.EquippedWeapons.RemoveRange(this.MaxEquippedWeapons, this.EquippedWeapons.Count - this.MaxEquippedWeapons);
        }

        private void EnsureActiveWeaponIsValid()
        {
            if (this.EquippedWeapons == null || this.EquippedWeapons.Count == 0)
            {
                this.SetActiveWeapon(null);
                return;
            }

            if (this.ActiveEquippedWeapon == null || !this.EquippedWeapons.Contains(this.ActiveEquippedWeapon)) this.SetActiveWeapon(this.EquippedWeapons[0]);
        }
    }
}
