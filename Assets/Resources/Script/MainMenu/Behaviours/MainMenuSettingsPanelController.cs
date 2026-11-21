using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class MainMenuSettingsPanelController : MainMenuSubBehaviourBase
    {
        // --- References ---
        [Header("References - Categories")]
        [SerializeField] private Button _audioButton;
        [SerializeField] private Button _graphicsButton;
        [SerializeField] private Button _gameplayButton;
        [SerializeField] private Button _controlsButton;

        [Header("References - Sections")]
        [SerializeField] private GameObject _audioSection;
        [SerializeField] private GameObject _graphicsSection;
        [SerializeField] private GameObject _gameplaySection;
        [SerializeField] private GameObject _controlsSection;

        // --- Public APIs ---
        public void Show()
        {
            this.gameObject.SetActive(true);

            this.ShowAudioSection();

            if (this.EnsureBound()) this.MainMenuController.SelectButton(_audioButton);
        }
        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        // --- Helpers ---
        //  1. Section Navigation
        private void ShowAudioSection()
        {
            this.ShowSection(_audioSection);
        }
        private void ShowGraphicsSection()
        {
            this.ShowSection(_graphicsSection);
        }
        private void ShowGameplaySection()
        {
            this.ShowSection(_gameplaySection);
        }
        private void ShowControlsSection()
        {
            this.ShowSection(_controlsSection);
        }

        private void ShowSection(GameObject target)
        {
            if (_audioSection    != null) _audioSection.SetActive(_audioSection == target);
            if (_graphicsSection != null) _graphicsSection.SetActive(_graphicsSection == target);
            if (_gameplaySection != null) _gameplaySection.SetActive(_gameplaySection == target);
            if (_controlsSection != null) _controlsSection.SetActive(_controlsSection == target);
        }

        // --- FOTLMonoBehaviour Overrides ---

        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            _audioButton?.onClick.AddListener(this.ShowAudioSection);
            _graphicsButton?.onClick.AddListener(this.ShowGraphicsSection);
            _gameplayButton?.onClick.AddListener(this.ShowGameplaySection);
            _controlsButton?.onClick.AddListener(this.ShowControlsSection);
        }

        protected override void UnbindInternalReferences()
        {
            _audioButton?.onClick.RemoveListener(this.ShowAudioSection);
            _graphicsButton?.onClick.RemoveListener(this.ShowGraphicsSection);
            _gameplayButton?.onClick.RemoveListener(this.ShowGameplaySection);
            _controlsButton?.onClick.RemoveListener(this.ShowControlsSection);

            base.UnbindInternalReferences();
        }
    }
}
