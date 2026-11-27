namespace FlameOfTheLost
{
    public readonly struct NPCSpawnedContext
    {
        public NPCDefaultSpawnPointController DefaultSpawnPointController { get; }

        public NPCSpawnEntry SpawnEntry { get; }

        public NPCRootController NPC { get; }

        public NPCSpawnedContext(NPCDefaultSpawnPointController defaultSpawnPointController, NPCSpawnEntry spawnEntry, NPCRootController npc)
        {
            this.DefaultSpawnPointController = defaultSpawnPointController;

            this.SpawnEntry = spawnEntry;

            this.NPC = npc;
        }
    }
}