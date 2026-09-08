namespace FlameOfTheLost
{
    public readonly struct EnemySpawnedContext<T> where T : EnemySpawnEntry
    {
        public EnemyDefaultSpawnPointController DefaultSpawnPointController { get; }
        public T SpawnEntry { get; }
        public CharacterRootController Character { get; }

        public EnemySpawnedContext(EnemyDefaultSpawnPointController defaultSpawnPointController, T spawnEntry, CharacterRootController character)
        {
            this.DefaultSpawnPointController = defaultSpawnPointController;
            this.SpawnEntry = spawnEntry;
            this.Character = character;
        }
    }
}