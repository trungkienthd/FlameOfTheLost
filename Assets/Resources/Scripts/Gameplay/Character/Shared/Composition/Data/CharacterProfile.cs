using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterProfile : FOTLScriptableObjectProfile
    {
        //  1. Classification
        [Header("Classification")]
        [SerializeField] private CharacterType _characterType;
        [SerializeField] private CharacterFaction _faction;
        [SerializeField] private CharacterTraversalType _traversalType;
        public CharacterType CharacterType => _characterType;
        public CharacterFaction Faction => _faction;
        public CharacterTraversalType TraversalType => _traversalType;

        //  2. Movement
        [Header("Movement")]
        [SerializeField] private CharacterMovementConfig _movementConfig;
        public CharacterMovementConfig MovementConfig => _movementConfig;

        //  3. Visual 
        [Header("Visual")]
        [SerializeField] private CharacterVisualConfig _visualConfig;
        public CharacterVisualConfig VisualConfig => _visualConfig;

        //  4. Combat 
        [Header("Combat")]
        [SerializeField] private CharacterCombatConfig _combatConfig;
        public CharacterCombatConfig CombatConfig => _combatConfig;

        //  5. Status Effects
        [Header("Status Effects")]
        [SerializeField] private StatusEffectGuardConfig _statusEffectGuardConfig;
        public StatusEffectGuardConfig StatusEffectGuardConfig => _statusEffectGuardConfig;

        //  6.  Progression
        [Header("Progression")]
        [SerializeField] private CharacterProgressionConfig _progressionConfig;
        public CharacterProgressionConfig ProgressionConfig => _progressionConfig;

        //  7. Reward
        [Header("Reward")]
        [SerializeField] private CharacterRewardDropProfile _rewardDropProfile;
        public CharacterRewardDropProfile RewardDropProfile => _rewardDropProfile;
    }
}
