using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCDialogueHandler : NPCBehaviourControllerBase, ICharacterRefreshable
    {
        // --- Data / Configs ---
        [Header("Configs - Dialogue")]
        [SerializeField] private NPCDialogueProfile _dialogueProfile;
        public NPCDialogueProfile DialogueProfile => _dialogueProfile;

        // --- Runtime ---
        [Header("Runtime - Conversation")]
        [SerializeField] private bool _isConversationActive;
        public bool IsConversationActive => _isConversationActive;
        private PlayerRootController _currentPlayer;
        public PlayerRootController CurrentPlayer => _currentPlayer;
        private NPCDialogueNodeData _currentNode;
        public NPCDialogueNodeData CurrentNode => _currentNode;

        // --- Events ---
        public event Action<PlayerRootController> ConversationStarted;
        public event Action<PlayerRootController> ConversationEnded;
        public event Action<NPCDialogueNodeData> DialogueNodeChanged;

        // --- Public APIs ---
        //  1. Conversation Validation
        public bool CanBeginConversation(PlayerRootController player)
        {
            if (!this.isActiveAndEnabled) return false;
            if (this.NPCRootController == null) return false;
            if (player == null) return false;
            if (this.IsConversationActive) return false;
            if (this.DialogueProfile == null) return false;

            return true;
        }

        //  2. Conversation Lifecycle
        public bool TryBeginConversation(PlayerRootController player)
        {
            if (!this.CanBeginConversation(player)) return false;

            GameConditionController conditions = this.GetGameConditionController();
            if (conditions == null) return false;

            NPCDialogueNodeData startingNode = this.DialogueProfile.SelectStartingNode(conditions);
            if (startingNode == null) return false;

            _currentPlayer = player;
            _currentNode = startingNode;
            _isConversationActive = true;

            this.ConversationStarted?.Invoke(player);

            this.DialogueNodeChanged?.Invoke(this.CurrentNode);

            return true;
        }

        public void EndConversation()
        {
            if (!this.IsConversationActive) return;

            PlayerRootController previousPlayer = this.CurrentPlayer;

            _isConversationActive = false;
            _currentPlayer = null;
            _currentNode = null;

            this.ConversationEnded?.Invoke(previousPlayer);
        }

        //  3. Available Choices
        public List<NPCDialogueChoiceEntry> GetAvailableChoices()
        {
            List<NPCDialogueChoiceEntry> result = new();

            if (!this.IsConversationActive || this.CurrentNode == null)
            {
                return result;
            }

            GameConditionController conditions = this.GetGameConditionController();
            if (conditions == null) return result;

            IReadOnlyList<NPCDialogueChoiceEntry> choices = this.CurrentNode.Choices;
            if (choices == null) return result;

            foreach (NPCDialogueChoiceEntry choice in choices)
            {
                if (choice == null) continue;

                if (choice.IsAvailable(conditions)) result.Add(choice);
            }

            return result;
        }

        //  4. Choice Selection
        public bool TrySelectChoice(int choiceIndex)
        {
            if (!this.IsConversationActive) return false;
            if (this.CurrentNode == null) return false;
            if (this.CurrentPlayer == null) return false;

            List<NPCDialogueChoiceEntry> choices = this.GetAvailableChoices();

            if (choiceIndex < 0 || choiceIndex >= choices.Count)
            {
                return false;
            }

            NPCDialogueChoiceEntry selectedChoice = choices[choiceIndex];
            if (selectedChoice == null) return false;

            if (!selectedChoice.Execute(this.NPCRootController, this.CurrentPlayer))
            {
                return false;
            }

            NPCDialogueNodeData nextNode = selectedChoice.NextNode;
            if (nextNode == null)
            {
                this.EndConversation();
                return true;
            }

            _currentNode = nextNode;

            this.DialogueNodeChanged?.Invoke(this.CurrentNode);

            return true;
        }

        //  5. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 110;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.EndConversation();
        }

        // --- Helpers ---
        private GameConditionController GetGameConditionController()
        {
            return UnityEngine.Object.FindFirstObjectByType<GameConditionController>(FindObjectsInactive.Include);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _isConversationActive = false;
            _currentPlayer = null;
        }
        protected override void CleanupStates()
        {
            this.EndConversation();

            base.CleanupStates();
        }
    }
}
