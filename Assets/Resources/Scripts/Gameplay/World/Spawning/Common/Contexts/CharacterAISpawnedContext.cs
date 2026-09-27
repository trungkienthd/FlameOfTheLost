namespace FlameOfTheLost
{
    public readonly struct CharacterAISpawnedContext<TSpawnPoint, TSpawnEntry>
        where TSpawnPoint : CharacterAIDefaultSpawnPointController
        where TSpawnEntry : CharacterAISpawnEntry
    {
        public TSpawnPoint DefaultSpawnPointController { get; }
        public TSpawnEntry SpawnEntry { get; }
        public CharacterRootController Character { get; }

        public CharacterAISpawnedContext(TSpawnPoint defaultSpawnPointController, TSpawnEntry spawnEntry, CharacterRootController character)
        {
            this.DefaultSpawnPointController = defaultSpawnPointController;

            this.SpawnEntry = spawnEntry;
            this.Character = character;
        }
    }
}