using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterInputHandler : CharacterBehaviourControllerBase, ICharacterRefreshable
    {
        // --- States ---
        //  1. References
        protected MovementHandler MovementHandler => this.CharacterRootController.MovementHandler;
        protected CharacterCombatController CombatController => this.CharacterRootController.CombatController;
        protected CharacterCombatLoadoutController CombatLoadoutController => this.CharacterRootController.CombatLoadoutController;

        //  2. Movement Input
        public Vector2 MoveInput { get; private set; }

        // --- Public APIs ---
        //  1. ICharacterPlacementRefreshable implementation
        public virtual int RefreshmentConfigurePriority => 100;
        public virtual void RefreshCharacter(CharacterRefreshmentContext context)
        {
            this.ClearMoveInput();
        }

        // --- Helpers ---
        protected void SetMoveInput(Vector2 moveInput)
        {
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
            this.CombatController?.TryDoNormalAttack(attackSide);
        }

        protected void RequestSpecialAttack(AttackSide attackSide)
        {
            this.CombatController?.TryDoSpecialAttack(attackSide);
        }

        protected void RequestCycleWeapon(int direction)
        {
            this.CombatLoadoutController?.TryCycleWeaponEntry(direction);
        }

        protected void RequestQuickEquipWeapon()
        {
            this.CombatLoadoutController?.TryQuickEquipWeapon();
        }

        protected void RequestCycleConsumableItem(int direction)
        {
            this.CombatLoadoutController?.TryCycleConsumableItemEntry(direction);
        }

        protected void RequestConsumeConsumableItem()
        {
            this.CombatLoadoutController?.TryConsumeConsumableItem();
        }

        protected void RequestSetActiveWeaponSpecialSkill(SkillProfile skill)
        {
            if (skill == null) return;

            this.CombatLoadoutController?.TrySetActiveEquippedWeaponActiveSpecialSkill(skill);
        }
    }
}