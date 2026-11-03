using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCDialoguePanelController : PlayerInteractionBasePanelController
    {
        // --- Runtime ---
        //  1. Current Interaction
        public NPCRootController CurrentNPC { get; private set; }
        public PlayerRootController CurrentPlayer { get; private set; }

        //  2. Dialogue
        private NPCDialogueHandler CurrentDialogueHandler => this.CurrentNPC?.NPCBrainController?.DialogueHandler;

        // --- Public APIs ---
        //  1. Opening
        public void Open(NPCRootController npc, PlayerRootController player)
        {
            if (npc == null) return;
            if (player == null) return;

            NPCDialogueHandler dialogueHandler = npc.NPCBrainController?.DialogueHandler;

            if (dialogueHandler == null) return;
            if (!dialogueHandler.IsConversationActive) return;
            if (dialogueHandler.CurrentPlayer != player) return;

            this.CurrentNPC = npc;
            this.CurrentPlayer = player;

            base.Open();
        }

        //  2. Closing
        public override void Close()
        {
            this.EndCurrentConversation();

            this.CurrentNPC = null;
            this.CurrentPlayer = null;

            base.Close();
        }

        // --- Helpers ---
        private void EndCurrentConversation()
        {
            this.CurrentDialogueHandler?.EndConversation();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.CurrentNPC = null;
            this.CurrentPlayer = null;
        }
        protected override void CleanupStates()
        {
            this.EndCurrentConversation();

            this.CurrentNPC = null;
            this.CurrentPlayer = null;

            base.CleanupStates();
        }
    }
}