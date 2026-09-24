using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class NeutralCharacterVariantProfile<TProfile> : NeutralCharacterVariantProfile
        where TProfile : NeutralCharacterProfile
    {
        [Header("Character")]
        [SerializeField] private TProfile _characterProfile;

        public TProfile TypedNeutralCharacterProfile => _characterProfile;

        public sealed override NeutralCharacterProfile NeutralCharacterProfile => this.TypedNeutralCharacterProfile;
    }
}