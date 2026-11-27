using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    public class PlayerInteractionPromptHandler : PlayerInteractionSubHandlerBase
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private PlayerInteractionPromptPanelController _promptPanelController;
        private PlayerInteractionPromptPanelController PromptPanelController => _promptPanelController;

        private PlayerInteractionInteractableTargetHandler InteractableTargetHandler => this.InteractionHandler?.InteractableTargetHandler;
        private PlayerInteractionPromptSourceTargetHandler PromptSourceTargetHandler => this.InteractionHandler?.PromptSourceTargetHandler;

        // --- Runtime ---
        [Header("Runtime")]
        [SerializeField] private InputActionReference _interactActionReference;
        private InputActionReference InteractActionReference => _interactActionReference;

        private InputAction _runtimeInteractAction;

        private InputAction InteractAction => _runtimeInteractAction ?? this.InteractActionReference?.action;

        // --- Configs ---
        [Header("Configs")]
        [SerializeField] private string _fallbackInteractInputDisplayName = "E";
        [SerializeField] private string _fallbackInteractPromptText = "Interact";
        private string FallbackInteractInputDisplayName => _fallbackInteractInputDisplayName;
        private string FallbackInteractPromptText => _fallbackInteractPromptText;

        // --- Public APIs ---
        //  1. Prompt Display
        public void RefreshPrompt()
        {
            if (this.InteractionHandler == null)
            {
                this.HidePrompt();
                return;
            }

            GameObject interactor = this.InteractionHandler.GetInteractor();

            // a. Active Hold takes precedence.
            PlayerInteractionHoldHandler holdHandler = this.InteractionHandler.HoldHandler;
            if (holdHandler != null && holdHandler.IsHolding)
            {
                if (holdHandler.ActiveTarget is MonoBehaviour behaviour && behaviour != null && behaviour.isActiveAndEnabled)
                {
                    this.ShowActiveHoldPrompt(interactor, holdHandler.ActiveTarget, holdHandler.NormalizedProgress);
                }
                else
                {
                    this.HidePrompt();
                }

                return;
            }

            // b.Nearest executable interactable.
            IPlayerInteractable nearestInteractable = this.InteractableTargetHandler?.GetNearestInteractable(interactor);
            if (nearestInteractable != null)
            {
                this.ShowInteractablePrompt(interactor, nearestInteractable);
                return;
            }

            // c. Standalone prompt source.
            IPlayerInteractionPromptSource bestPromptSource = this.PromptSourceTargetHandler?.GetBestPromptSource(interactor);
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
        //  1. Prompt Text Resolution
        private string ResolveInteractablePromptText(GameObject interactor, IPlayerInteractable interactable)
        {
            string promptText =this.ResolveFallbackPromptText();

            if (interactable is IPlayerInteractionPromptSource promptSource)
            {
                string providedText = promptSource.GetInteractionPromptText(interactor);
                if (!string.IsNullOrWhiteSpace(providedText))
                {
                    promptText = providedText;
                }
            }

            return promptText;
        }

        //  2. Normal Executable Prompt
        private void ShowInteractablePrompt(GameObject interactor, IPlayerInteractable interactable)
        {
            string promptText = this.ResolveInteractablePromptText(interactor, interactable); 

            string inputDisplayName = this.GetInteractInputDisplayName();

            PlayerInteractionType interactionType = this.InteractionHandler.GetInteractionType(interactable);
            if (interactionType == PlayerInteractionType.Hold)
            {
                this.ShowPrompt($"[Hold {inputDisplayName}] {promptText}");
            }
            else
            {
                this.ShowPrompt($"[{inputDisplayName}] {promptText}");
            }
        }

        //  3. Active Hold Prompt
        private void ShowActiveHoldPrompt(GameObject interactor, IPlayerHoldInteractable interactable, float normalizedProgress)
        {
            if (this.PromptPanelController == null) return;

            string promptText = this.ResolveInteractablePromptText(interactor, interactable);

            string inputDisplayName = this.GetInteractInputDisplayName();

            this.PromptPanelController.ShowHoldProgress
            (
                text: $"[Hold {inputDisplayName}] {promptText}",
                normalizedProgress: normalizedProgress
            );
        }

        //  4. Standalone Prompt
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

        //  5. Input Display Name
        private string GetInteractInputDisplayName()
        {
            InputAction interactAction = this.InteractAction;
            if (interactAction == null)
            {
                return this.ResolveFallbackInputDisplayName();
            }

            PlayerInput playerInput = this.InteractionHandler?.GetPlayer()?.PlayerInput;
            string currentControlScheme = playerInput?.currentControlScheme;

            string bindingGroup = null;

            if (playerInput?.actions != null && !string.IsNullOrWhiteSpace(currentControlScheme))
            {
                foreach (InputControlScheme scheme in playerInput.actions.controlSchemes)
                {
                    if (scheme.name != currentControlScheme) continue;

                    bindingGroup = scheme.bindingGroup;
                    break;
                }
            }

            if (!string.IsNullOrWhiteSpace(bindingGroup))
            {
                string schemeDisplayName = interactAction.GetBindingDisplayString
                (
                    group: bindingGroup,
                    options: InputBinding.DisplayStringOptions.DontIncludeInteractions
                );

                if (!string.IsNullOrWhiteSpace(schemeDisplayName)) return schemeDisplayName;
            }

            // Fallback: Keyboard binding.
            for (int i = 0; i < interactAction.bindings.Count; i++)
            {
                InputBinding binding = interactAction.bindings[i];

                if (binding.isComposite || binding.isPartOfComposite) continue;
                if (string.IsNullOrWhiteSpace(binding.effectivePath)) continue;
                if (!binding.effectivePath.Contains("<Keyboard>")) continue;

                string displayName = interactAction.GetBindingDisplayString(i, InputBinding.DisplayStringOptions.DontIncludeInteractions);
                if (!string.IsNullOrWhiteSpace(displayName)) return displayName;
            }

            return this.ResolveFallbackInputDisplayName();
        }
        private string ResolveFallbackInputDisplayName()
        {
            return string.IsNullOrWhiteSpace(this.FallbackInteractInputDisplayName) ? "E" : this.FallbackInteractInputDisplayName;
        }
        private string ResolveFallbackPromptText()
        {
            return string.IsNullOrWhiteSpace(this.FallbackInteractPromptText) ? "Interact" : this.FallbackInteractPromptText;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.PromptPanelController == null)
            {
                GameObject canvas = GameObject.Find("Canvas");
                if (canvas != null) _promptPanelController = canvas.GetComponentInChildren<PlayerInteractionPromptPanelController>(true);
            }

            PlayerInput playerInput = this.InteractionHandler?.GetPlayer()?.PlayerInput;

            _runtimeInteractAction = playerInput?.actions?.FindAction("Player/Interact", false);
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            string ownerName = this.InteractionHandler?.GetPlayer()?.gameObject.name ?? this.gameObject.name;

            if (this.PromptPanelController == null) Debug.LogError($"[{ownerName}.{nameof(PlayerInteractionPromptHandler)}] {nameof(PlayerInteractionPromptPanelController)} reference is not assigned.", this);

            if (this.InteractAction == null) Debug.LogError($"[{ownerName}.{nameof(PlayerInteractionPromptHandler)}] Interact InputAction is missing.", this);
        }
    }
}
