using UnityEngine;

namespace FlameOfTheLost
{
    public class MainMenuController : FOTLMonoBehaviour
    {
        // --- References ---
        [Header("References - Sub Panels")]

        [SerializeField] private MainMenuMainPanelController _mainPanelController;

        [SerializeField] private MainMenuNewJourneyConfirmationController _newJourneyConfirmationController;

        [SerializeField] private MainMenuSettingsPanelController _settingsPanelController;

        [SerializeField] private MainMenuCreditsPanelController _creditsPanelController;

        [SerializeField] private MainMenuTransitionController _transitionController;

        private MainMenuMainPanelController MainPanelController => _mainPanelController;
        private MainMenuNewJourneyConfirmationController NewJourneyConfirmationController => _newJourneyConfirmationController;

        private MainMenuSettingsPanelController SettingsPanelController => _settingsPanelController;

        private MainMenuCreditsPanelController CreditsPanelController => _creditsPanelController;

        private MainMenuTransitionController TransitionController => _transitionController;
    }
}