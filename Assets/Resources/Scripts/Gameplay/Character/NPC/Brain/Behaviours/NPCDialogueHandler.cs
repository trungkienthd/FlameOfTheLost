using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCDialogueHandler : NPCBrainSubBehaviourBase, ICharacterRefreshable
    {
        // --- Runtime ---
        [Header("Runtime - Conversation")]
        [SerializeField] private bool _isConversationActive;
        public bool IsConversationActive => _isConversationActive;
        private PlayerRootController _currentPlayer;
        public PlayerRootController CurrentPlayer => _currentPlayer;

        // --- Events ---
        public event Action<PlayerRootController> ConversationStarted;
        public event Action<PlayerRootController> ConversationEnded;

        // --- Public APIs ---
        //  1. Conversation Validation
        public bool CanBeginConversation(PlayerRootController player)
        {
            if (!this.EnsureBound()) return false;
            if (!this.isActiveAndEnabled) return false;
            if (this.NPCRootController == null) return false;
            if (player == null) return false;
            if (this.IsConversationActive) return false;

            return true;
        }

        //  2. Conversation Lifecycle
        public bool TryBeginConversation(PlayerRootController player)
        {
            if (!this.CanBeginConversation(player)) return false;

            _currentPlayer = player;
            _isConversationActive = true;

            this.ConversationStarted?.Invoke(player);

            return true;
        }

        public void EndConversation()
        {
            if (!this.IsConversationActive) return;

            PlayerRootController previousPlayer = this.CurrentPlayer;

            _isConversationActive = false;
            _currentPlayer = null;

            this.ConversationEnded?.Invoke(previousPlayer);
        }

        //  3. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 110;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.EndConversation();
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
