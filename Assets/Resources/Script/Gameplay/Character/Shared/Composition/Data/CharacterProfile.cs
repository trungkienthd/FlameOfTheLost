using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterProfile : FOTLScriptableObjectProfile
    {
        //  0. Prefab
        [Header("Prefab")]
        [SerializeField] private GameObject _prefab;
        public GameObject Prefab => _prefab;

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

        //  3. Dodge
        [Header("Dodge")]
        [SerializeField] private CharacterDodgeConfig _dodgeConfig;
        public CharacterDodgeConfig DodgeConfig => _dodgeConfig;

        //  4. Visual 
        [Header("Visual")]
        [SerializeField] private CharacterVisualConfig _defaultVisualConfig;
        public CharacterVisualConfig DefaultVisualConfig => _defaultVisualConfig;

        //  5. Combat 
        [Header("Combat")]
        [SerializeField] private CharacterCombatConfig _combatConfig;
        public CharacterCombatConfig CombatConfig => _combatConfig;

        //  6. Status Effects
        [Header("Status Effects")]
        [SerializeField] private StatusEffectGuardConfig _statusEffectGuardConfig;
        public StatusEffectGuardConfig StatusEffectGuardConfig => _statusEffectGuardConfig;

        //  7.  Progression
        [Header("Progression")]
        [SerializeField] private CharacterProgressionConfig _progressionConfig;
        public CharacterProgressionConfig ProgressionConfig => _progressionConfig;
    }
}
