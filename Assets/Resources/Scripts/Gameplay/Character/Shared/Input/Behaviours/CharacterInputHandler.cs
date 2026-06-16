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
        public virtual void RefreshCharacter(CharacterRefreshmentContext context)
        {
            this.IsDeathDisabled = false;
            this.ClearMoveInput();
        }

        //  2. ICharacterDeathDisableable implementation
        public int DeathDisablePriority => 10;
        public void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;
            this.ClearMoveInput();
        }

        // --- Helpers ---
        protected void SetMoveInput(Vector2 moveInput)
        {
            if (this.IsDeathDisabled)
            {
                moveInput = Vector2.zero;
            }

            this.MoveInput = moveInput;

            if (this.MovementHandler == null) return;

            this.MovementHandler.RawMovementInput = moveInput;
        }

        protected void ClearMoveInput()
        {
            this.SetMoveInput(Vector2.zero);
        }

        protected void RequestNormalAttack(AttackSide attackSide)
        {
            if (this.IsDeathDisabled) return;
            this.CombatController?.TryDoNormalAttack(attackSide);
        }

        protected void RequestSpecialAttack(AttackSide attackSide)
        {
            if (this.IsDeathDisabled) return;
            this.CombatController?.TryDoSpecialAttack(attackSide);
        }

        protected void RequestCycleWeapon(int direction)
        {
            if (this.IsDeathDisabled) return;
            this.CombatLoadoutController?.TryCycleWeaponEntry(direction);
        }

        protected void RequestQuickEquipWeapon()
        {
            if (this.IsDeathDisabled) return;
            this.CombatLoadoutController?.TryQuickEquipWeapon();
        }

        protected void RequestCycleConsumableItem(int direction)
        {
            if (this.IsDeathDisabled) return;
            this.CombatLoadoutController?.TryCycleConsumableItemEntry(direction);
        }

        protected void RequestConsumeConsumableItem()
        {
            if (this.IsDeathDisabled) return;
            this.CombatLoadoutController?.TryConsumeConsumableItem();
        }

        protected void RequestSetActiveWeaponSpecialSkill(SkillProfile skill)
        {
            if (this.IsDeathDisabled) return;
            if (skill == null) return;

            this.CombatLoadoutController?.TrySetActiveEquippedWeaponActiveSpecialSkill(skill);
        }
    }
}