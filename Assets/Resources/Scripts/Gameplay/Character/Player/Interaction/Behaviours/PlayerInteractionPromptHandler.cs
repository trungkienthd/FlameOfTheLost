using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    public class PlayerInteractionPromptHandler : FOTLMonoBehaviour
    {
        // --- References ---
        private PlayerInteractionHandler _interactionHandler;
        private PlayerInteractionHandler InteractionHandler => _interactionHandler;
        private PlayerInteractionTargetHandler TargetHandler => this.InteractionHandler.TargetHandler;

        [SerializeField] private PlayerInteractionPromptPanelController _promptPanelController;
        private PlayerInteractionPromptPanelController PromptPanelController => _promptPanelController;

        // --- States ---
        [SerializeField] private InputActionReference _interactActionReference;
        private InputActionReference InteractActionReference => _interactActionReference;
        private InputAction InteractAction => this.InteractActionReference != null ? this.InteractActionReference.action : null;

        [SerializeField] private string _fallbackInteractInputDisplayName = "E";
        [SerializeField] private string _fallbackInteractPromptText = "Interact";
        private string FallbackInteractInputDisplayName => _fallbackInteractInputDisplayName;
        private string FallbackInteractPromptText => _fallbackInteractPromptText;

        // --- Public APIs ---
        //  0. Initialization
        public void Initialize(PlayerInteractionHandler interactionHandler)
        {
            _interactionHandler = interactionHandler;
        }

        //  1. Prompt Display
        public void RefreshPrompt()
        {
            if (this.InteractionHandler == null || this.TargetHandler == null)
            {
                this.HidePrompt();
                return;
            }

            GameObject interactor = this.InteractionHandler.GetInteractor();

            IPlayerInteractable nearestInteractable = this.TargetHandler.GetNearestInteractable(interactor);
            if (nearestInteractable != null)
            {
                this.ShowInteractablePrompt(interactor, nearestInteractable);
                return;
            }

            IPlayerInteractionPromptSource bestPromptSource = this.TargetHandler.GetBestPromptSource(interactor);
            if (bestPromptSource != null)
            {
                this.ShowStandalonePromptSource(interactor, bestPromptSource);
                return;
            }

            this.HidePrompt();
        }
        public void ShowPrompt(string text)
        {
            if (this.PromptPanelController == null) return;

            if (string.IsNullOrWhiteSpace(text))
            {
                this.HidePrompt();
                return;
            }

            this.PromptPanelController.Show(text);
        }
        public void HidePrompt() 
        {
            this.PromptPanelController?.Hide(); 
        }

        // --- Helpers ---
        //  1. Prompt Construction
        private void ShowInteractablePrompt(GameObject interactor, IPlayerInteractable interactable)
        {
            string promptText = this.ResolveFallbackPromptText();

            if (interactable is IPlayerInteractionPromptSource promptSource)
            {
                string providedText = promptSource.GetInteractionPromptText(interactor);

                if (!string.IsNullOrWhiteSpace(providedText))
                {
                    promptText = providedText;
                }
            }

            string inputDisplayName = this.GetInteractInputDisplayName();

            this.ShowPrompt($"[{inputDisplayName}] {promptText}");
        }

        private string GetInteractInputDisplayName()
        {
            InputAction interactAction = this.InteractAction;

            if (interactAction == null)
            {
                return this.ResolveFallbackInputDisplayName();
            }

            for (int i = 0; i < interactAction.bindings.Count; i++)
            {
                InputBinding binding = interactAction.bindings[i];

                if (binding.isComposite || binding.isPartOfComposite) continue;
                if (string.IsNullOrWhiteSpace(binding.effectivePath)) continue;
                if (!binding.effectivePath.Contains("<Keyboard>")) continue;

                string displayName = interactAction.GetBindingDisplayString(i, InputBinding.DisplayStringOptions.DontIncludeInteractions);
                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    return displayName;
                }
            }

            return this.ResolveFallbackInputDisplayName();
        }

        private void ShowStandalonePromptSource(GameObject interactor, IPlayerInteractionPromptSource promptSource)
        {
            string promptText = promptSource.GetInteractionPromptText(interactor);

            if (string.IsNullOrWhiteSpace(promptText))
            {
                this.HidePrompt();
                return;
            }

            this.ShowPrompt(promptText);
        }

        private string ResolveFallbackInputDisplayName()
        {
            return string.IsNullOrWhiteSpace(this.FallbackInteractInputDisplayName) ? "I" : this.FallbackInteractInputDisplayName;
        }

        private string ResolveFallbackPromptText()
        {
            return string.IsNullOrWhiteSpace(this.FallbackInteractPromptText) ? "Interact" : this.FallbackInteractPromptText;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            GameObject canvas = GameObject.Find("Canvas");
            if (this.PromptPanelController == null && canvas != null) _promptPanelController = canvas.GetComponentInChildren<PlayerInteractionPromptPanelController>(true);

            if (this.InteractActionReference == null)
            {
                PlayerInput playerInput = this.InteractionHandler?.GetPlayer()?.PlayerInput;
                InputAction interactAction = playerInput?.actions.FindAction("Interact", false);

                if (interactAction != null) _interactActionReference = InputActionReference.Create(interactAction);
            }
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            string ownerName = this.InteractionHandler?.GetPlayer()?.gameObject.name ?? this.gameObject.name;

            if (this.PromptPanelController == null) Debug.LogError($"[{ownerName}.{nameof(PlayerInteractionPromptHandler)}] " + $"{nameof(PlayerInteractionPromptPanelController)} reference is not assigned.", this);

            if (this.InteractActionReference == null || this.InteractActionReference.action == null) Debug.LogError($"[{ownerName}.{nameof(PlayerInteractionPromptHandler)}] " + $"Interact Action Reference is not assigned or has no action.", this);
        }
    }
}
