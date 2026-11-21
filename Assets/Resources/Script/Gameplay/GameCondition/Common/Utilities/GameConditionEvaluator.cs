using System.Collections.Generic;

namespace FlameOfTheLost
{
    public static class GameConditionEvaluator
    {
        // --- Public APIs ---
        public static bool AreConditionsSatisfied(IReadOnlyList<GameConditionRequirementEntry> requirements, GameConditionCombinationType combinationType, GameConditionController conditionController)
        {
            if (requirements == null || requirements.Count == 0) return true;

            if (conditionController == null) return false;

            if (combinationType == GameConditionCombinationType.Any)
            {
                return GameConditionEvaluator.IsAnyConditionSatisfied(requirements, conditionController);
            }

            return GameConditionEvaluator.AreAllConditionsSatisfied(requirements, conditionController);
        }

        // --- Helpers ---
        private static bool AreAllConditionsSatisfied(IReadOnlyList<GameConditionRequirementEntry> requirements, GameConditionController conditionController)
        {
            for (int i = 0; i < requirements.Count; i++)
            {
                GameConditionRequirementEntry requirement = requirements[i];

                if (requirement == null) continue;
                if (requirement.ConditionKey == GameConditionKey.None) continue;

                bool currentValue = conditionController.GetConditionState(requirement.ConditionKey);

                if (currentValue != requirement.RequiredValue) return false;
            }

            return true;
        }

        private static bool IsAnyConditionSatisfied(IReadOnlyList<GameConditionRequirementEntry> requirements, GameConditionController conditionController)
        {
            bool hasValidCondition = false;

            for (int i = 0; i < requirements.Count; i++)
            {
                GameConditionRequirementEntry requirement = requirements[i];

                if (requirement == null) continue;
                if (requirement.ConditionKey == GameConditionKey.None) continue;

                hasValidCondition = true;

                bool currentValue = conditionController.GetConditionState(requirement.ConditionKey);

                if (currentValue == requirement.RequiredValue) return true;
            }

            return !hasValidCondition;
        }
    }
}
