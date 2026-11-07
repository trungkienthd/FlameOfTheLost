using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class NPCDialogueEntry
    {
        [SerializeField] private int _priority;
        public int Priority => _priority;

        [SerializeField] private NPCDialogueNodeData _startingNode;
        public NPCDialogueNodeData StartingNode => _startingNode;

        [SerializeField] private List<NPCConditionRequirementEntry> _requirements = new();

        public bool IsAvailable(GameConditionController conditions)
        {
            if (_startingNode == null) return false;

            return NPCConditionRequirementEntry.AreAllSatisfied(_requirements, conditions);
        }
    }
}
