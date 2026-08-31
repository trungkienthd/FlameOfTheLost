namespace FlameOfTheLost
{
    public abstract class EquippedLoadoutSubPanelSubBehaviourBase : FOTLSubBehaviour<EquippedLoadoutSubPanelController>
    {
        protected EquippedLoadoutSubPanelController EquippedLoadoutSubPanelController => this.BrainBehaviour;

        protected CharacterCombatLoadoutController PlayerCombatLoadoutController => this.EquippedLoadoutSubPanelController != null
            ? this.EquippedLoadoutSubPanelController.PlayerCombatLoadoutController
            : null;

        protected PlayerCarryLoadoutController PlayerCarryLoadoutController => this.EquippedLoadoutSubPanelController != null
            ? this.EquippedLoadoutSubPanelController.PlayerCarryLoadoutController
            : null;
    }
}
