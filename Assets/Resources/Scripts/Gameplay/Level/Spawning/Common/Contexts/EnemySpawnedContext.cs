namespace FlameOfTheLost
{
    public readonly struct EnemySpawnedContext<T> where T : EnemySpawnEntry
    {
        public T SpawnEntry { get; }
        public CharacterRootController Character { get; }

        public EnemySpawnedContext(T spawnEntry, CharacterRootController character)
        {
            this.SpawnEntry = spawnEntry;
            this.Character = character;
        }
    }
}