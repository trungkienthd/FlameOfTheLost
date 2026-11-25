using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCInteractionHandler : CharacterBehaviourControllerBase,
        IPlayerInteractable,
        IPlayerInteractionPromptSource,
        IPlayerInteractionPromptRefreshNotifier,
        ICharacterRefreshable
    {
        // --- Data / Configs ---
        //  1. Prompt Settings
        [Header("Prompt Settings")]
        [SerializeField] private int _interactionPromptPriority = 100;
        [SerializeField] private string _interactionVerb = "Talk to";
        public int InteractionPromptPriority => _interactionPromptPriority;
        private string InteractionVerb => _interactionVerb;

        // --- References ---
        //  1. NPC Root
        private NPCRootController NPCRootController => this.CharacterRootController as NPCRootController;
        private NPCBrainController NPCBrainController => this.NPCRootController?.NPCBrainController;
        private NPCDialogueHandler DialogueHandler => this.NPCRootController?.DialogueHandler;

        //  2. Interaction Panel
        [Header("References - External")]
        [SerializeField] private NPCDialoguePanelController _dialoguePanelController;
        private NPCDialoguePanelController DialoguePanelController => _dialoguePanelController;

        // --- Runtime ---
        [Header("Runtime")]
        [SerializeField] private bool _isInteractionEnabled = true;
        public bool IsInteractionEnabled
        {
            get => _isInteractionEnabled;
            private set => _isInteractionEnabled = value;
        }

        // --- Events ---
        public event Action PromptRefreshRequested;

        // --- Public APIs ---
        //  1. IPlayerInteractable implementation
        public bool CanInteract(GameObject interactor)
        {
            if (!this.isActiveAndEnabled) return false;
            if (!this.IsInteractionEnabled) return false;
            if (interactor == null) return false;
            if (this.NPCRootController == null) return false;
            if (this.NPCBrainController == null) return false;
            if (this.DialogueHandler == null) return false;
            if (this.DialoguePanelController == null) return false;

            PlayerRootController player = interactor.GetComponent<PlayerRootController>();

            if (player == null)
            {
                player = interactor.GetComponentInParent<PlayerRootController>();
            }

            if (player == null) return false;

            return this.DialogueHandler.CanBeginConversation(player);
        }

        public bool TryInteract(GameObject interactor)
        {
            if (!this.CanInteract(interactor)) return false;

            PlayerRootController player = interactor.GetComponent<PlayerRootController>();
            if (player == null)
            {
                player = interactor.GetComponentInParent<PlayerRootController>();
            }

            if (player == null) return false;

            if (!this.DialogueHandler.TryBeginConversation(player))
            {
                return false;
            }

            this.DialoguePanelController.Open(this.NPCRootController, player);

            return true;
        }

        //  2. IPlayerInteractionPromptSource implementation
        public bool CanShowInteractionPrompt(GameObject interactor)
        {
            return this.CanInteract(interactor);
        }

        public string GetInteractionPromptText(GameObject interactor)
        {
            string displayName = this.NPCRootController != null
                ? this.NPCRootController.DisplayName
                : this.gameObject.name;

            string verb = string.IsNullOrWhiteSpace(this.InteractionVerb)
                ? "Talk to"
                : this.InteractionVerb;

            return $"{verb} {displayName}";
        }

        //  3. Interaction Availability
        public void SetInteractionEnabled(bool isEnabled)
        {
            if (this.IsInteractionEnabled == isEnabled) return;

            this.IsInteractionEnabled = isEnabled;
            this.RequestPromptRefresh();
        }

        public void RequestPromptRefresh()
        {
            this.PromptRefreshRequested?.Invoke();
        }

        //  4. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 100;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            // Preserve externally configured availability.
            // Persistent quest conditions will govern this later.
            this.RequestPromptRefresh();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.DialoguePanelController != null) return;

            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null) return;

            _dialoguePanelController = canvas.GetComponentInChildren<NPCDialoguePanelController>(true);
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.DialoguePanelController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCInteractionHandler)}] {nameof(NPCDialoguePanelController)} reference is missing.", this);
            }
        }
    }
}
