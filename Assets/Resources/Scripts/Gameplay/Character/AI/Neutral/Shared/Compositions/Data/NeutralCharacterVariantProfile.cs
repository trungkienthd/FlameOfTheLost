namespace FlameOfTheLost
{
    public abstract class NeutralCharacterVariantProfile : CharacterAIVariantProfile
    {
        public abstract NeutralCharacterProfile NeutralCharacterProfile { get; }

        public sealed override CharacterAIProfile CharacterProfile => this.NeutralCharacterProfile;
    }
}