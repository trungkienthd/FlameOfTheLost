using System.Collections.Generic;

namespace FlameOfTheLost
{
    public static class CharacterCapabilityRequirementUtility
    {
        public static bool AreRequirementsSatisfied(CharacterProgressionController progressionController, IReadOnlyList<CharacterCapabilityRequirementEntry> requirements)
        {
            if (requirements == null || requirements.Count == 0) return true;
            if (progressionController == null) return false;

            foreach (CharacterCapabilityRequirementEntry requirement in requirements)
            {
                if (requirement == null) continue;

                if (!requirement.IsSatisfiedBy(progressionController))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
