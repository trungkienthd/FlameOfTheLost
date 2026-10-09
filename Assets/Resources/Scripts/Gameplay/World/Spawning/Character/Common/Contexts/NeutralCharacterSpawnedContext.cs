namespace FlameOfTheLost
{
    public readonly struct NeutralCharacterSpawnedContext<T> where T : NeutralCharacterSpawnEntry
    {
        public NeutralCharacterDefaultSpawnPointController DefaultSpawnPointController
        {
            get;
        }

        public T SpawnEntry
        {
            get;
        }
        public CharacterRootController Character
        {
            get;
        }

        public NeutralCharacterSpawnedContext(NeutralCharacterDefaultSpawnPointController defaultSpawnPointController, T spawnEntry, CharacterRootController character)
        {
            this.DefaultSpawnPointController = defaultSpawnPointController;

            this.SpawnEntry = spawnEntry;
            this.Character = character;
        }
    }
}