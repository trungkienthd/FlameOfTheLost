namespace FlameOfTheLost
{
    public interface INPCRewardGrantService
    {
        public bool TryGrantReward(PlayerRootController player, NPCRewardEntry reward);
    }
}
