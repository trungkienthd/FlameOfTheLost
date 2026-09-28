namespace FlameOfTheLost
{
    public static class CharacterAITargetValidationUtility
    {
        public static bool CanTarget(CharacterFaction selfFaction, CharacterFaction targetFaction)
        {
            if (selfFaction == targetFaction) return false;

            return selfFaction switch
            {
                CharacterFaction.Ally =>
                    targetFaction == CharacterFaction.Enemy,

                CharacterFaction.Enemy =>
                    targetFaction == CharacterFaction.Player ||
                    targetFaction == CharacterFaction.Ally,

                CharacterFaction.Neutral =>
                    targetFaction == CharacterFaction.Player ||
                    targetFaction == CharacterFaction.Enemy ||
                    targetFaction == CharacterFaction.Ally,

                CharacterFaction.Player =>
                    targetFaction == CharacterFaction.Enemy,

                _ => false,
            };
        }
    }
}
