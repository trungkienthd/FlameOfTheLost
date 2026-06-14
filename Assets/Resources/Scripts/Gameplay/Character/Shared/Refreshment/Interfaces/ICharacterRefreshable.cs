namespace FlameOfTheLost
{
    public interface ICharacterRefreshable
    {
        public int RefreshmentConfigurePriority { get; }

        public void RefreshCharacter(CharacterPlacementContext context);
    }
}