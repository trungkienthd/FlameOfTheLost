using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class NeutralCharacterRootController<TProfile> : NeutralCharacterRootController
        where TProfile : NeutralCharacterProfile
    {
        [Header("Data")]
        [SerializeField] private TProfile _profile;
        public TProfile TypedNeutralCharacterProfile => _profile;

        public sealed override CharacterProfile Profile => this.TypedNeutralCharacterProfile;

        public sealed override CharacterAIProfile CharacterAIProfile => this.TypedNeutralCharacterProfile;

        public sealed override NeutralCharacterProfile NeutralCharacterProfile => this.TypedNeutralCharacterProfile;
    }
}