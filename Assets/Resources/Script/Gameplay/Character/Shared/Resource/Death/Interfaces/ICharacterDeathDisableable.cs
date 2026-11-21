namespace FlameOfTheLost
{
    public interface ICharacterDeathDisableable
    {
        public int DeathDisablePriority { get; }

        public void DisableAfterCharacterDeath(CharacterDeathContext context);
    }
}
