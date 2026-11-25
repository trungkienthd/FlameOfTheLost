using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCBrainController : CharacterBehaviourControllerBase
    {
        // --- References ---
        //  1. NPC Root
        public NPCRootController NPCRootController => this.CharacterRootController as NPCRootController;

        //  2. Sub-behaviours
        [Header("References - Sub-Behaviours")]
        [SerializeField] private NPCAttackReactionHandler _attackReactionHandler;
        internal NPCAttackReactionHandler AttackReactionHandler => _attackReactionHandler;

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.AttackReactionHandler == null) _attackReactionHandler = this.GetComponentInChildren<NPCAttackReactionHandler>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.NPCRootController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCBrainController)}] {nameof(this.NPCRootController)} reference is missing.", this);
            }
        }
    }
}