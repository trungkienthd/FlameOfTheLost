using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class EnemyVariantProfile : CharacterAIVariantProfile
    {
        // 1. Character
        [Header("Character")]
        [SerializeField] private EnemyProfile _characterProfile;
        public EnemyProfile EnemyCharacterProfile => _characterProfile;

        public override CharacterAIProfile CharacterProfile => EnemyCharacterProfile;
    }
}
