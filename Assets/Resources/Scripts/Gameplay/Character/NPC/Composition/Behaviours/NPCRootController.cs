using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCRootController : CharacterRootController<NPCProfile>
    {
        // --- Data / Configs ---
        public NPCProfile NPCProfile => this.TypedProfile;

        // --- References ---
        //  1. NPC Brain
        [Header("References - NPC Brain")]
        [SerializeField] private NPCBrainController _npcBrainController;
        public NPCBrainController NPCBrainController => _npcBrainController;

        //  2. NPC Interaction
        [Header("References - NPC Interaction")]
        [SerializeField] private NPCInteractionHandler _npcInteractionHandler;
        public NPCInteractionHandler NPCInteractionHandler => _npcInteractionHandler;

        //  3. NPC Input
        public NPCInputHandler NPCInputHandler => this.InputHandler as NPCInputHandler;

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.NPCBrainController     == null) _npcBrainController    = this.GetComponentInChildren<NPCBrainController>(true);
            if (this.NPCInteractionHandler  == null) _npcInteractionHandler = this.GetComponentInChildren<NPCInteractionHandler>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.NPCBrainController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCRootController)}] {nameof(this.NPCBrainController)} reference is not assigned.", this);
            }
            if (this.NPCInteractionHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCRootController)}] {nameof(NPCInteractionHandler)} reference is not assigned.", this);
            }
            if (this.NPCInputHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCRootController)}] {nameof(NPCInputHandler)} reference is not assigned.", this);
            }

            if (this.NPCProfile != null && this.NPCProfile.Faction != CharacterFaction.None)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCRootController)}] NPC faction must be {CharacterFaction.None}.", this);
            }
        }
    }
}
