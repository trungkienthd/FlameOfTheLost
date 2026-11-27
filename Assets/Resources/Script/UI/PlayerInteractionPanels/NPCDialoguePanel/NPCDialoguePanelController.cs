using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(NPCDialoguePanelPlaybackHandler))]
    [RequireComponent(typeof(NPCDialoguePanelLayoutHandler))]
    public class NPCDialoguePanelController : PlayerInteractionBasePanelController
    {
        // --- References ---
        //  1. Sub-handlers
        [Header("References - Sub-handlers")]
        [SerializeField] private NPCDialoguePanelPlaybackHandler _playbackHandler;
        [SerializeField] private NPCDialoguePanelLayoutHandler _layoutHandler;
        private NPCDialoguePanelPlaybackHandler PlaybackHandler => _playbackHandler;
        private NPCDialoguePanelLayoutHandler LayoutHandler => _layoutHandler;

        //  2. Dialogue UI
        [Header("References - Dialogue UI")]
        [SerializeField] private TMP_Text _speakerNameText;
        [SerializeField] private TMP_Text _dialogueText;
        [SerializeField] private Button _choiceButtonPrefab;
        [SerializeField] private Button _closeButton;
        private TMP_Text SpeakerNameText => _speakerNameText;
        private TMP_Text DialogueText => _dialogueText;
        private Button CloseButton => _closeButton;
        private Button ChoiceButtonPrefab => _choiceButtonPrefab;
        private Transform ChoicesRoot => this.LayoutHandler?.ChoiceList;

        // --- Runtime ---
        //  1. Current Interaction
        public NPCRootController CurrentNPC { get; private set; }
        public PlayerRootController CurrentPlayer { get; private set; }

        //  2. Dialogue
        private NPCDialogueHandler _currentDialogueHandler;
        private NPCDialogueHandler CurrentDialogueHandler => _currentDialogueHandler;

        //  3. Choice Buttons
        private readonly List<Button> _spawnedChoiceButtons = new();
        private IReadOnlyList<Button> SpawnedChoiceButtons => _spawnedChoiceButtons;

        //  4. Presentation State
        [Header("Runtime - Presentation")]
        [SerializeField] private bool _areChoicesPresented;
        [SerializeField] private bool _isClosing;
        [SerializeField] private bool _isProcessingChoice;
        public bool AreChoicesPresented => _areChoicesPresented;
        public bool IsClosing => _isClosing;
        public bool IsProcessingChoice => _isProcessingChoice;
        public bool IsPlayingDialogue => this.PlaybackHandler != null && this.PlaybackHandler.IsPlaying;

        private int _presentationVersion;
        public int PresentationVersion => _presentationVersion;

        // --- Public APIs ---
        //  1. Opening
        public void Open(NPCRootController npc, PlayerRootController player)
        {
            if (npc == null) return;
            if (player == null) return;
            if (this.IsClosing) return;

            NPCDialogueHandler dialogueHandler = npc?.DialogueHandler;

            if (dialogueHandler == null) return;
            if (!dialogueHandler.IsConversationActive) return;
            if (dialogueHandler.CurrentPlayer != player) return;

            if (this.IsOpen && this.CurrentDialogueHandler != null)
            {
                if (this.CurrentDialogueHandler == dialogueHandler && this.CurrentPlayer == player) return;

                this.Close();
            }

            this.ResolveSubHandlerReferences();

            if (!this.HasRequiredReferences())
            {
                Debug.LogError($"[{this.GetType().Name}] Cannot open Dialogue Panel. One or more required references are missing.", this);

                dialogueHandler.EndConversation();

                return;
            }

            this.CurrentNPC = npc;
            this.CurrentPlayer = player;
            _currentDialogueHandler = dialogueHandler;

            this.BindConversationEvents();
            this.BindPlaybackEvents();

            base.Open();
        }

        //  2. Closing
        public override void Close()
        {
            if (!this.IsOpen) return;
            if (this.IsClosing) return;

            _isClosing = true;

            try
            {
                this.ClearConversationState();

                base.Close();
            }
            finally
            {
                _isClosing = false;
            }
        }

        // --- Helpers ---
        //  1. Panel Hooks
        protected override void OnOpened()
        {
            base.OnOpened();

            this.RefreshDialogueUI();
        }
        protected override void OnClosing()
        {
            this.ResetDialoguePresentation();

            if (this.SpeakerNameText != null)
            {
                this.SpeakerNameText.text = string.Empty;
            }

            base.OnClosing();
        }

        //  2. Dialogue Events
        private void HandleDialogueNodeChanged(NPCDialogueNodeData node)
        {
            if (!this.IsOpen) return;
            if (this.IsClosing) return;

            this.RefreshDialogueUI();
        }
        private void HandleConversationEnded(PlayerRootController player)
        {
            this.Close();
        }

        //  3. Playback Events
        private void HandleDialogueSegmentChanged(string segment)
        {
            if (!this.IsOpen) return;
            if (this.IsClosing) return;

            if (this.DialogueText != null)
            {
                this.DialogueText.text = segment ?? string.Empty;
            }

            this.LayoutHandler?.RefreshLayout();
        }
        private void HandleDialoguePlaybackCompleted()
        {
            if (!this.IsOpen) return;
            if (this.IsClosing) return;

            NPCDialogueHandler dialogueHandler = this.CurrentDialogueHandler;

            if (dialogueHandler == null) return;
            if (!dialogueHandler.IsConversationActive) return;
            if (dialogueHandler.CurrentNode == null) return;

            this.PresentAvailableChoices();
        }

        //  4. Dialogue Presentation
        private void RefreshDialogueUI()
        {
            this.ResetDialoguePresentation();

            NPCDialogueHandler dialogueHandler = this.CurrentDialogueHandler;

            if (dialogueHandler == null) return;
            if (!dialogueHandler.IsConversationActive) return;

            NPCDialogueNodeData node = dialogueHandler.CurrentNode;
            if (node == null) return;

            if (this.SpeakerNameText != null)
            {
                this.SpeakerNameText.text = string.IsNullOrWhiteSpace(node.SpeakerName)
                    ? this.CurrentNPC.DisplayName
                    : node.SpeakerName;
            }

            this.PlaybackHandler.StartPlayback(node);
        }

        private void ResetDialoguePresentation()
        {
            _presentationVersion++;

            this.PlaybackHandler?.StopPlayback();

            _areChoicesPresented = false;

            this.ClearChoiceButtons();

            this.LayoutHandler?.SetChoicesVisible(false);

            if (this.DialogueText != null)
            {
                this.DialogueText.text = string.Empty;
            }
        }

        //  5. Choice Presentation
        private void PresentAvailableChoices()
        {
            _presentationVersion++;
            _areChoicesPresented = false;

            this.ClearChoiceButtons();

            NPCDialogueHandler dialogueHandler = this.CurrentDialogueHandler;

            if (dialogueHandler == null) return;
            if (!dialogueHandler.IsConversationActive) return;
            if (dialogueHandler.CurrentNode == null) return;

            List<NPCDialogueChoiceEntry> choices = dialogueHandler.GetAvailableChoices();
            if (choices.Count == 0)
            {
                this.CreateChoiceButton("Leave", null);
            }
            else
            {
                foreach (NPCDialogueChoiceEntry choice in choices)
                {
                    this.CreateChoiceButton(choice.ChoiceText, choice);
                }
            }

            _areChoicesPresented = this.SpawnedChoiceButtons.Count > 0;

            this.LayoutHandler?.SetChoicesVisible(this.AreChoicesPresented);

            this.LayoutHandler?.RefreshLayout();
        }
        private void CreateChoiceButton(string text, NPCDialogueChoiceEntry choice)
        {
            if (this.ChoiceButtonPrefab == null) return;
            if (this.ChoicesRoot == null) return;

            Button button = UnityEngine.Object.Instantiate(this.ChoiceButtonPrefab, this.ChoicesRoot);

            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                label.text = text;
            }

            int presentationVersion = _presentationVersion;

            button.onClick.AddListener(() => this.HandleChoiceSelected(choice, presentationVersion));

            _spawnedChoiceButtons.Add(button);

            button.gameObject.SetActive(true);
        }

        private void HandleChoiceSelected(NPCDialogueChoiceEntry choice, int presentationVersion)
        {
            if (!this.IsOpen) return;
            if (!this.AreChoicesPresented) return;
            if (this.IsClosing || this.IsProcessingChoice) return;
            if (presentationVersion != this.PresentationVersion) return;

            // Fallback Leave button.
            if (choice == null)
            {
                this.Close();

                return;
            }

            NPCDialogueHandler dialogueHandler = this.CurrentDialogueHandler;
            if (dialogueHandler == null) return;
            if (!dialogueHandler.IsConversationActive) return;

            List<NPCDialogueChoiceEntry> choices = dialogueHandler.GetAvailableChoices();
            int choiceIndex = choices.IndexOf(choice);
            if (choiceIndex < 0)
            {
                this.PresentAvailableChoices();

                return;
            }

            _isProcessingChoice = true;

            try
            {
                bool didSelect = dialogueHandler.TrySelectChoice(choiceIndex);

                if (!didSelect)
                {
                    Debug.LogWarning($"[{this.GetType().Name}] Failed to execute dialogue choice.", this);

                    if (this.IsOpen && this.CurrentDialogueHandler == dialogueHandler && dialogueHandler.IsConversationActive && this.AreChoicesPresented && presentationVersion == _presentationVersion)
                    {
                        this.PresentAvailableChoices();
                    }
                }
            }
            finally
            {
                _isProcessingChoice = false;
            }
        }

        private void ClearChoiceButtons()
        {
            foreach (Button button in this.SpawnedChoiceButtons)
            {
                if (button == null) continue;

                button.interactable = false;

                button.gameObject.SetActive(false);

                UnityEngine.Object.Destroy(button.gameObject);
            }

            _spawnedChoiceButtons.Clear();
        }

        //  6. Event Binding
        //  6.1. Close Button
        private void BindCloseButtonEvent()
        {
            if (this.CloseButton == null) return;

            // Prevent duplicate runtime listeners.
            this.CloseButton.onClick.RemoveListener(this.Close);

            // Preserve the Inspector-configured listener if present.
            if (this.HasPersistentCloseButtonListener()) return;

            this.CloseButton.onClick.AddListener(this.Close);
        }
        private void UnbindCloseButtonEvent()
        {
            if (this.CloseButton == null) return;

            this.CloseButton.onClick.RemoveListener(this.Close);
        }
        private bool HasPersistentCloseButtonListener()
        {
            if (this.CloseButton == null) return false;

            int listenerCount = this.CloseButton.onClick.GetPersistentEventCount();

            for (int i = 0; i < listenerCount; i++)
            {
                if (this.CloseButton.onClick.GetPersistentTarget(i) != this) continue;
                if (this.CloseButton.onClick.GetPersistentMethodName(i) != nameof(this.Close)) continue;
                if (this.CloseButton.onClick.GetPersistentListenerState(i) == UnityEngine.Events.UnityEventCallState.Off) continue;

                return true;
            }

            return false;
        }

        //      6.2. Conversation Events
        private void BindConversationEvents()
        {
            NPCDialogueHandler dialogueHandler = this.CurrentDialogueHandler;

            if (dialogueHandler == null) return;

            dialogueHandler.DialogueNodeChanged -= this.HandleDialogueNodeChanged;
            dialogueHandler.DialogueNodeChanged += this.HandleDialogueNodeChanged;

            dialogueHandler.ConversationEnded -= this.HandleConversationEnded;
            dialogueHandler.ConversationEnded += this.HandleConversationEnded;
        }
        private void UnbindConversationEvents()
        {
            NPCDialogueHandler dialogueHandler = this.CurrentDialogueHandler;

            if (dialogueHandler == null) return;

            dialogueHandler.DialogueNodeChanged -= this.HandleDialogueNodeChanged;

            dialogueHandler.ConversationEnded -= this.HandleConversationEnded;
        }

        //      6.3. Playback Events
        private void BindPlaybackEvents()
        {
            if (this.PlaybackHandler == null) return;

            this.PlaybackHandler.DialogueSegmentChanged -= this.HandleDialogueSegmentChanged;
            this.PlaybackHandler.DialogueSegmentChanged += this.HandleDialogueSegmentChanged;

            this.PlaybackHandler.DialoguePlaybackCompleted -= this.HandleDialoguePlaybackCompleted;
            this.PlaybackHandler.DialoguePlaybackCompleted += this.HandleDialoguePlaybackCompleted;
        }
        private void UnbindPlaybackEvents()
        {
            if (this.PlaybackHandler == null) return;

            this.PlaybackHandler.DialogueSegmentChanged -= this.HandleDialogueSegmentChanged;

            this.PlaybackHandler.DialoguePlaybackCompleted -= this.HandleDialoguePlaybackCompleted;
        }

        //  7. Conversation Cleanup
        private void ClearConversationState()
        {
            this.PlaybackHandler?.StopPlayback();

            _areChoicesPresented = false;

            NPCDialogueHandler dialogueHandler = this.CurrentDialogueHandler;

            this.UnbindConversationEvents();
            this.UnbindPlaybackEvents();

            this.CurrentNPC = null;
            this.CurrentPlayer = null;
            _currentDialogueHandler = null;

            dialogueHandler?.EndConversation();
        }

        //  8. Reference Resolution / Validation
        private void ResolveSubHandlerReferences()
        {
            if (this.PlaybackHandler == null) _playbackHandler = this.GetComponent<NPCDialoguePanelPlaybackHandler>();
            if (this.LayoutHandler == null) _layoutHandler = this.GetComponent<NPCDialoguePanelLayoutHandler>();
        }
        private bool HasRequiredReferences()
        {
            return
                this.PlaybackHandler != null &&
                this.LayoutHandler != null &&
                this.LayoutHandler.HasRequiredReferences &&
                this.SpeakerNameText != null &&
                this.DialogueText != null &&
                this.ChoicesRoot != null &&
                this.CloseButton != null &&
                this.ChoiceButtonPrefab != null &&
                this.ChoiceButtonPrefab.GetComponentInChildren<TMP_Text>(true) != null;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            this.ResolveSubHandlerReferences();

            if (this.CloseButton == null)
            {
                Transform closeButtonTransform = this.transform.Find("DialogueWindow/CloseButton");

                if (closeButtonTransform != null) _closeButton = closeButtonTransform.GetComponent<Button>();
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (!this.HasRequiredReferences())
            {
                Debug.LogError($"[{this.GetType().Name}] One or more required Dialogue Panel references are missing.", this);
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.PlaybackHandler?.Bind(this);
            this.LayoutHandler?.Bind(this);

            this.BindCloseButtonEvent();
        }
        protected override void UnbindInternalReferences()
        {
            this.UnbindCloseButtonEvent();

            this.UnbindConversationEvents();
            this.UnbindPlaybackEvents();

            this.PlaybackHandler?.Unbind(this);
            this.LayoutHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
        protected override void CleanupStates()
        {
            this.ClearConversationState();

            this.ResetDialoguePresentation();

            base.CleanupStates();
        }
    }
}