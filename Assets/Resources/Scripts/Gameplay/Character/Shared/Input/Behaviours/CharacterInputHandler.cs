using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterInputHandler : CharacterBehaviourControllerBase, 
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- States ---
        //  1. References
        protected MovementHandler MovementHandler => this.CharacterRootController.MovementHandler;
        protected CharacterCombatController CombatController => this.CharacterRootController.CombatController;
        protected CharacterCombatLoadoutController CombatLoadoutController => this.CharacterRootController.CombatLoadoutController;

        //  2. Movement Input
        public Vector2 MoveInput { get; private set; }

        //  3. Death Disable state (Runtime)
        [Header("Death Disable (Runtime)")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled
        {
            get => _isDeathDisabled;
            private set => _isDeathDisabled = value;
        }

        // --- Public APIs ---
        //  1. ICharacterRefreshable implementation
        public virtual int RefreshmentConfigurePriority => 100;
        public virtual void RefreshCharacter(CharacterRefreshContext context)
        {
            this.IsDeathDisabled = false;
            this.ClearMoveInput();
        }

        //  2. ICharacterDeathDisableable implementation
        public virtual int DeathDisablePriority => 10;
        public virtual void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;
            this.ClearMoveInput();
        }

        //  3. Movement Input APIs
        public void SetMoveInput(Vector2 moveInput)
        {
            if (this.IsDeathDisabled)
            {
                moveInput = Vector2.zero;
            }

            this.MoveInput = moveInput;

            if (this.MovementHandler == null) return;

            this.MovementHandler.RawMovementInput = moveInput;
        }

        public void ClearMoveInput()
        {
            this.SetMoveInput(Vector2.zero);
        }

        //  4. Combat Input APIs
        //      4.1. Attack Input
        public void RequestNormalAttack(AttackSide attackSide)
        {
            if (this.IsDeathDisabled) return;
            this.CombatController?.TryDoNormalAttack(attackSide);
        }

        public void RequestSpecialAttack(AttackSide attackSide)
        {
            if (this.IsDeathDisabled) return;
            this.CombatController?.TryDoSpecialAttack(attackSide);
        }

        //      4.2. Combat Loadout Input
        public void RequestCycleWeapon(int direction)
        {
            if (this.IsDeathDisabled) return;
            this.CombatLoadoutController?.TryCycleWeaponEntry(direction);
        }

        public void RequestQuickEquipWeapon()
        {
            if (this.IsDeathDisabled) return;
            this.CombatLoadoutController?.TryQuickEquipWeapon();
        }

        public void RequestCycleConsumableItem(int direction)
        {
            if (this.IsDeathDisabled) return;
            this.CombatLoadoutController?.TryCycleConsumableItemEntry(direction);
        }

        public void RequestConsumeConsumableItem()
        {
            if (this.IsDeathDisabled) return;
            this.CombatLoadoutController?.TryConsumeConsumableItem();
        }

        public void RequestSetActiveWeaponSpecialSkill(SkillProfile skill)
        {
            if (this.IsDeathDisabled) return;
            if (skill == null) return;

            this.CombatLoadoutController?.TrySetActiveEquippedWeaponActiveSpecialSkill(skill);
        }
    }
}