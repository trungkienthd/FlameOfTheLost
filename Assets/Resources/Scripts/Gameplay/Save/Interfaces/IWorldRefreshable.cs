namespace FlameOfTheLost
{
    public interface IWorldRefreshable
    {
        public int WorldRefreshPriority { get; }

        public void RefreshWorldContent(WorldRefreshContext context);
    }
}