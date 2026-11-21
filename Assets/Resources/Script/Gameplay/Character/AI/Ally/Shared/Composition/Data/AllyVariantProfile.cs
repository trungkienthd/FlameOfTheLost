using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class AllyVariantProfile : CharacterAIVariantProfile
    {
        // 1. Character
        [Header("Character")]
        [SerializeField] private AllyProfile _characterProfile;
        public AllyProfile AllyProfile => _characterProfile;

        public override CharacterAIProfile CharacterProfile => AllyProfile;
    }
}