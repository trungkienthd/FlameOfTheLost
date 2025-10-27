using UnityEngine;

namespace FlameOfTheLost
{
    [System.Serializable]
    public struct StatModifier
    {
        [SerializeField] private StatModifierKey _targetStat;
        public StatModifierKey TargetStat => _targetStat;

        [SerializeField] private StatModifierFormula _formula;
        public StatModifierFormula Formula => _formula;

        [SerializeField] private float _value;
        public float Value => _value;

        [SerializeField] StatModifierStackingPolicy _modifierStackingPolicy;
        public StatModifierStackingPolicy ModifierStackingPolicy => _modifierStackingPolicy;
    }
}
