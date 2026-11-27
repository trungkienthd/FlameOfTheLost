namespace FlameOfTheLost
{
    public interface INPCSpawnPlacementResolver
    {
        public int NPCId { get; }

        public bool TryResolveSpawnTarget(NPCSpawnEntry spawnEntry, CharacterRefreshTarget defaultTarget, out CharacterRefreshTarget resolvedTarget);
    }
}
