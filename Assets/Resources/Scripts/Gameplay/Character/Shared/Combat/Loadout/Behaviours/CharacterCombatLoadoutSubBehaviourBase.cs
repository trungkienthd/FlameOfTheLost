namespace FlameOfTheLost
{
    public abstract class CharacterCombatLoadoutSubBehaviourBase : FOTLSubBehaviour<CharacterCombatLoadoutController>
    {
        // --- References ---
        protected CharacterCombatLoadoutController CombatLoadoutController => this.BrainBehaviour;

        protected CharacterRootController CharacterRootController => this.CombatLoadoutController != null
            ? this.CombatLoadoutController.GetCharacter()
            : null;

        protected string CharacterName => this.CombatLoadoutController != null
            ? this.CombatLoadoutController.CharacterName
            : this.gameObject.name;

        protected InventoryController InventoryController => this.CombatLoadoutController != null
            ? this.CombatLoadoutController.InventoryController
            : null;

        protected CharacterProgressionController ProgressionController => this.CombatLoadoutController != null
            ? this.CombatLoadoutController.ProgressionController
            : null;
    }
}