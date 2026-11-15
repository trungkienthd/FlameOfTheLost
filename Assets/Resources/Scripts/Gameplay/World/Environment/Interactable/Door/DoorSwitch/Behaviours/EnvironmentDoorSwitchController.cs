using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class EnvironmentDoorSwitchController : FOTLMonoBehaviour,
        IPlayerInteractable,
        IPlayerInteractionPromptSource,
        IPlayerInteractionPromptRefreshNotifier
    {
        // --- Configs ---
        [Header("Interaction")]
        [SerializeField] private bool _interactionEnabled = true;
        public bool InteractionEnabled => _interactionEnabled;

        [Header("Interaction Prompt")]
        [SerializeField] private int _interactionPromptPriority = 0;
        [SerializeField] private string _openPromptText = "Open Door";
        [SerializeField] private string _closePromptText = "Close Door";

        public int InteractionPromptPriority => _interactionPromptPriority;

        // --- References ---
        [Header("References")]
        [SerializeField] private EnvironmentDoorController _targetDoorController;
        private EnvironmentDoorController TargetDoorController => _targetDoorController;

        // --- Events ---
        public event Action PromptRefreshRequested;

        // --- Public APIs ---
        //  1. IPlayerInteractable Implementation
        public bool CanInteract(GameObject interactor)
        {
            if (!this.isActiveAndEnabled) return false;
            if (!this.InteractionEnabled) return false;
            if (interactor == null) return false;
            if (this.TargetDoorController == null) return false;
            if (!this.TargetDoorController.isActiveAndEnabled) return false;

            return true;
        }
        public bool TryInteract(GameObject interactor)
        {
            if (!this.CanInteract(interactor)) return false;

            this.TargetDoorController.Toggle();

            this.LogDebug($"Door switch activated by {this.GetObjectName(interactor)}.");

            return true;
        }

        //  2. IPlayerInteractionPromptSource Implementation
        public bool CanShowInteractionPrompt(GameObject interactor)
        {
            return this.CanInteract(interactor);
        }
        public string GetInteractionPromptText(GameObject interactor)
        {
            if (!this.CanShowInteractionPrompt(interactor))
                return string.Empty;

            return this.TargetDoorController.IsOpen
                ? _closePromptText
                : _openPromptText;
        }

        //  3. Interaction State
        public void SetInteractionEnabled(bool enabled)
        {
            if (_interactionEnabled == enabled) return;

            _interactionEnabled = enabled;

            this.RequestPromptRefresh();
        }

        // --- Helpers ---
        //  1. Event Handling
        private void HandleDoorStateChanged(bool isOpen)
        {
            this.RequestPromptRefresh();
        }

        private void RequestPromptRefresh()
        {
            this.PromptRefreshRequested?.Invoke();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.TargetDoorController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(EnvironmentDoorSwitchController)}] {nameof(EnvironmentDoorController)} reference is missing.", this);
            }
        }
        protected override void BindExternalReferences()
        {
            base.BindExternalReferences();

            if (this.TargetDoorController != null) this.TargetDoorController.OnDoorStateChanged += this.HandleDoorStateChanged;
        }
        protected override void UnbindExternalReferences()
        {
            if (this.TargetDoorController != null) this.TargetDoorController.OnDoorStateChanged -= this.HandleDoorStateChanged;

            base.UnbindExternalReferences();
        }
        protected override void CleanupStates()
        {
            base.CleanupStates();

            this.PromptRefreshRequested = null;
        }
    }
}