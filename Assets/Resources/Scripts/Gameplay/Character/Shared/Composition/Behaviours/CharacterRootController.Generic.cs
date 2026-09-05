using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterRootController<TProfile> : CharacterRootController
        where TProfile : CharacterProfile
    {
        // --- Data / Configs ---
        [Header("Data")]
        [SerializeField] private TProfile _profile;
        public TProfile TypedProfile => _profile;

        public sealed override CharacterProfile Profile => this.TypedProfile;
    }
}