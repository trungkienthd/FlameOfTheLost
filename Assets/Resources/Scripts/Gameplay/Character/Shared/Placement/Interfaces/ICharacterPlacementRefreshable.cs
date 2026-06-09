namespace FlameOfTheLost
{
    public interface ICharacterPlacementRefreshable
    {
        public int PlacementConfigurePriority { get; }

        public void RefreshAfterCharacterPlacement(CharacterPlacementContext context);
    }
}