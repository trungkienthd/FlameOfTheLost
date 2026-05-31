namespace FlameOfTheLost
{
    public interface ICharacterRespawnRefreshable
    {
        public int RespawnRefreshPriority { get; }

        public void RefreshAfterRespawn(CharacterRespawnRefreshContext context);
    }
}