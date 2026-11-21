using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class MainMenuNewJourneyConfirmationController : MainMenuSubBehaviourBase
    {
        // --- References ---
        [Header("References - Buttons")]
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;
        private Button ConfirmButton => _confirmButton;
        private Button CancelButton => _cancelButton;

        // --- Public APIs ---
        public void Show()
        {
            this.gameObject.SetActive(true);

            this.SetInteractionEnabled(true);

            if (this.EnsureBound())
            {
                this.MainMenuController.SelectButton(this.CancelButton);
            }
        }
        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public void SetInteractionEnabled(bool interactable)
        {
            if (this.ConfirmButton  != null) this.ConfirmButton.interactable    = interactable;
            if (this.CancelButton   != null) this.CancelButton.interactable     = interactable;
        }

        // --- Helpers ---
        //  1. Button Events
        private void HandleConfirmClicked()
        {
            if (!this.EnsureBound()) return;

            this.MainMenuController.ConfirmNewJourney();
        }
        private void HandleCancelClicked()
        {
            if (!this.EnsureBound()) return;

            this.MainMenuController.ShowMainPanel();
        }

        // --- FOTLMonoBehaviour Overrides ---

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            Transform container = this.transform.Find("Dialog/ButtonContainer");
            if (container == null) return;

            if (this.ConfirmButton == null)
            {
                Transform target = container.Find("Button_Confirm");
                if (target != null) _confirmButton = target.GetComponent<Button>();
            }
            if (this.CancelButton == null)
            {
                Transform target = container.Find("Button_Cancel");
                if (target != null) _cancelButton = target.GetComponent<Button>();
            }
        }

        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.ConfirmButton?.onClick.AddListener(this.HandleConfirmClicked);
            this.CancelButton?.onClick.AddListener(this.HandleCancelClicked);
        }

        protected override void UnbindInternalReferences()
        {
            this.ConfirmButton?.onClick.RemoveListener(this.HandleConfirmClicked);
            this.CancelButton?.onClick.RemoveListener(this.HandleCancelClicked);

            base.UnbindInternalReferences();
        }
    }
}