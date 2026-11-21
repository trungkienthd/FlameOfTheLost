using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class NPCConditionRequirementEntry
    {
        [SerializeField] private GameConditionKey _conditionKey;
        [SerializeField] private bool _requiredState = true;
        public GameConditionKey ConditionKey => _conditionKey;
        public bool RequiredState => _requiredState;

        public bool IsSatisfied(GameConditionController controller)
        {
            if (controller == null) return false;

            return controller.GetConditionState(this.ConditionKey) == this.RequiredState;
        }

        public static bool AreAllSatisfied(IReadOnlyList<NPCConditionRequirementEntry> requirements, GameConditionController controller)
        {
            if (requirements == null || requirements.Count == 0) return true;

            if (controller == null) return false;

            foreach (NPCConditionRequirementEntry requirement in requirements)
            {
                if (requirement == null) return false;
                if (!requirement.IsSatisfied(controller)) return false;
            }

            return true;
        }
    }
}
