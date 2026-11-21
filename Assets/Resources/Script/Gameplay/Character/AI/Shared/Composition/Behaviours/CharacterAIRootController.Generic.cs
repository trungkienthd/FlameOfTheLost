using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterAIRootController<TProfile> : CharacterAIRootController
        where TProfile : CharacterAIProfile
    {
        [Header("Data")]
        [SerializeField] private TProfile _profile;
        public TProfile TypedProfile => _profile;

        public sealed override CharacterProfile Profile => this.TypedProfile;
        public sealed override CharacterAIProfile CharacterAIProfile => this.TypedProfile;
    }
}
