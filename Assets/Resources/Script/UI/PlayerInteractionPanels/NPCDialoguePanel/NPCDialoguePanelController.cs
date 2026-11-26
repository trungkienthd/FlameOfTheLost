using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class NPCDialoguePanelController : PlayerInteractionBasePanelController
    {
        // --- References ---
        [Header("References - Dialogue UI")]
        [SerializeField] private TMP_Text _speakerNameText;
        [SerializeField] private TMP_Text _dialogueText;
        [SerializeField] private Transform _choicesRoot;
        [SerializeField] private Button _choiceButtonPrefab;
        private TMP_Text SpeakerNameText => _speakerNameText;
        private TMP_Text DialogueText => _dialogueText;
        private Transform ChoicesRoot => _choicesRoot;
        private Button ChoiceButtonPrefab => _choiceButtonPrefab;

        // --- Runtime ---
        //  1. Current Interaction
        public NPCRootController CurrentNPC { get; private set; }
        public PlayerRootController CurrentPlayer { get; private set; }

        //  2. Dialogue
        private NPCDialogueHandler CurrentDialogueHandler => this.CurrentNPC?.DialogueHandler;

        private readonly List<Button> _spawnedChoiceButtons = new();
        private IReadOnlyList<Button> SpawnedChoiceButtons => _spawnedChoiceButtons;

        // --- Public APIs ---
        //  1. Opening
        public void Open(NPCRootController npc, PlayerRootController player)
        {
            if (npc == null) return;
            if (player == null) return;

            NPCDialogueHandler dialogueHandler = npc?.DialogueHandler;

            if (dialogueHandler == null) return;
            if (!dialogueHandler.IsConversationActive) return;
            if (dialogueHandler.CurrentPlayer != player) return;

            if (this.IsOpen) this.Close();

            this.CurrentNPC = npc;
            this.CurrentPlayer = player;

            dialogueHandler.DialogueNodeChanged += this.HandleDialogueNodeChanged;

            dialogueHandler.ConversationEnded += this.HandleConversationEnded;

            base.Open();
        }

        //  2. Closing
        public override void Close()
        {
            if (!this.IsOpen) return;

            NPCDialogueHandler dialogueHandler = this.CurrentDialogueHandler;
            if (dialogueHandler != null)
            {
                dialogueHandler.DialogueNodeChanged -= this.HandleDialogueNodeChanged;

                dialogueHandler.ConversationEnded -= this.HandleConversationEnded;
            }

            dialogueHandler?.EndConversation();

            this.ClearChoiceButtons();

            this.CurrentNPC = null;
            this.CurrentPlayer = null;

            base.Close();
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
            this.ClearChoiceButtons();

            base.OnClosing();
        }

        //  2. Dialogue Events
        private void HandleDialogueNodeChanged(NPCDialogueNodeData node)
        {
            this.RefreshDialogueUI();
        }
        private void HandleConversationEnded(PlayerRootController player)
        {
            this.Close();
        }

        //  3. UI
        private void RefreshDialogueUI()
        {
            this.ClearChoiceButtons();

            NPCDialogueHandler dialogueHandler = this.CurrentDialogueHandler;
            if (dialogueHandler == null) return;

            NPCDialogueNodeData node = dialogueHandler.CurrentNode;
            if (node == null) return;

            if (this.SpeakerNameText != null) this.SpeakerNameText.text = node.SpeakerName;
            if (this.DialogueText != null) this.DialogueText.text = node.DialogueText;

            List<NPCDialogueChoiceEntry> choices = dialogueHandler.GetAvailableChoices();
            if (choices.Count == 0)
            {
                this.CreateChoiceButton("Leave", -1);
                return;
            }

            for (int i = 0; i < choices.Count; i++)
            {
                this.CreateChoiceButton(choices[i].ChoiceText, i);
            }
        }
        private void CreateChoiceButton(string text, int choiceIndex)
        {
            if (this.ChoiceButtonPrefab == null || this.ChoicesRoot == null)
            {
                return;
            }

            Button button = UnityEngine.Object.Instantiate(this.ChoiceButtonPrefab, this.ChoicesRoot);
            button.gameObject.SetActive(true);

            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label != null) label.text = text;

            int selectedIndex = choiceIndex;

            button.onClick.AddListener(() => { this.HandleChoiceSelected(selectedIndex); });

            _spawnedChoiceButtons.Add(button);
        }
        private void HandleChoiceSelected(int choiceIndex)
        {
            if (choiceIndex < 0)
            {
                this.Close();
                return;
            }

            NPCDialogueHandler dialogueHandler = this.CurrentDialogueHandler;
            if (dialogueHandler == null) return;

            if (!dialogueHandler.TrySelectChoice(choiceIndex))
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(NPCDialoguePanelController)}] Failed to execute dialogue choice.", this);
            }
        }
        private void ClearChoiceButtons()
        {
            foreach (Button button in this.SpawnedChoiceButtons)
            {
                if (button != null) UnityEngine.Object.Destroy(button.gameObject);
            }

            _spawnedChoiceButtons.Clear();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.CurrentNPC = null;
            this.CurrentPlayer = null;

            _spawnedChoiceButtons.Clear();
        }
        protected override void CleanupStates()
        {
            NPCDialogueHandler dialogueHandler = this.CurrentDialogueHandler;

            if (dialogueHandler != null)
            {
                dialogueHandler.DialogueNodeChanged -= this.HandleDialogueNodeChanged;

                dialogueHandler.ConversationEnded -= this.HandleConversationEnded;

                dialogueHandler.EndConversation();
            }

            this.ClearChoiceButtons();

            this.CurrentNPC = null;
            this.CurrentPlayer = null;

            base.CleanupStates();
        }
    }
}