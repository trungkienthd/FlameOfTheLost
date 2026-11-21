namespace FlameOfTheLost
{
    public readonly struct WorldRefreshContext
    {
        public bool IsSaveReload { get; }
        public bool IsPlayerRespawn { get; }

        public WorldRefreshContext(bool isSaveReload, bool isPlayerRespawn)
        {
            this.IsSaveReload = isSaveReload;
            this.IsPlayerRespawn = isPlayerRespawn;
        }
    }
}
