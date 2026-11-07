using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCDialogueChoiceEntry
    {
        // --- Data / Configs 
        //  1. Presentation
        [SerializeField] private string _choiceText;
        public string ChoiceText => _choiceText;

        //  2. Requirements
        [Header("Requirements")]
        [SerializeField]
        private List<NPCConditionRequirementEntry> _requirements = new();

        //  3. Actions
        [Header("Actions")]
        [SerializeField] private List<NPCDialogueActionEntry> _actions = new();

        //  4. Navigation
        [Header("Navigation")]
        [SerializeField] private NPCDialogueNodeData _nextNode;
        public NPCDialogueNodeData NextNode => _nextNode;

        // --- Public APIs ---
        public bool IsAvailable(GameConditionController conditions)
        {
            return NPCConditionRequirementEntry.AreAllSatisfied(_requirements, conditions);
        }

        public bool Execute(NPCRootController npc, PlayerRootController player)
        {
            if (_actions == null) return true;

            foreach (NPCDialogueActionEntry action in _actions)
            {
                if (action == null) return false;

                if (!action.Execute(npc, player)) return false;
            }

            return true;
        }
    }
}