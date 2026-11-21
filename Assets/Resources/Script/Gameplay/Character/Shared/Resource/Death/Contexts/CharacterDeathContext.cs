namespace FlameOfTheLost
{
    public struct CharacterDeathContext
    {
        public CharacterRootController Character { get; }
        public CharacterDeathCauseEntry DeathCause { get; }

        public CharacterDeathContext(CharacterRootController character, CharacterDeathCauseEntry deathCause)
        {
            this.Character = character;
            this.DeathCause = deathCause;
        }
    }
}
