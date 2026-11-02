namespace FlameOfTheLost
{
    public class NPCAttackReactionHandler : NPCBrainSubBehaviourBase
    {
        // --- Data / Configs ---
        protected NPCHostilityResponseType HostilityResponseType => this.NPCProfile != null
            ? this.NPCProfile.HostilityResponseType
            : NPCHostilityResponseType.None;
    }
}
