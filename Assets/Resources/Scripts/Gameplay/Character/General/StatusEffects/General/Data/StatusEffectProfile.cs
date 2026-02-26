using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "StatusEffectProfile", menuName = "Data/FlameOfTheLost/Combat/Systems/StatusEffect/StatusEffectProfile")]
    public class StatusEffectProfile : ScriptableObject, IIdentityProvider, IIconProvider
    {
        // --- Identity
        [Header("Identity")]
        [SerializeField] private int _id;
        [SerializeField] private string _displayName;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;
        public int ID => _id;
        public string DisplayName => _displayName;
        public string Description => _description;
        public Sprite Icon => _icon;

        // --- Classification
        [Header("Classification")]
        [SerializeField] private StatusEffectAffinityType _affinity = StatusEffectAffinityType.Debuff;
        public StatusEffectAffinityType Affinity => _affinity;

        // --- Mechanics
        [Header("Lifetime & Stacking")]
        [SerializeField, Min(0.01f)] private float _duration = 5f; // seconds
        [SerializeField, Min(1f)] private int _maxStacks = 1;
        [SerializeField] private StatusEffectDurationRefreshType _durationRefreshType = StatusEffectDurationRefreshType.Extend;
        public float Duration
        {
            get => _duration;
            private set => _duration = Mathf.Max(0.01f, value);
        }
        public int MaxStacks
        {
            get => _maxStacks;
            private set => _maxStacks = Mathf.Max(1, value);
        }
        public StatusEffectDurationRefreshType DurationRefreshType
        {
            get => _durationRefreshType;
        }

        [Header("Flag")]
        [SerializeField] private List<StatusEffectFlagContribution> _flags = new();
        public IReadOnlyList<StatusEffectFlagContribution> Flags => _flags;

        [Header("Stat Modifier")]
        [SerializeField] private List<StatusEffectStatModifierContribution> _statModifiers = new();
        public IReadOnlyList<StatusEffectStatModifierContribution> StatModifiers => _statModifiers;

        [SerializeField] private StatusEffectStatModifierCombinationType _combinationType = StatusEffectStatModifierCombinationType.AddStacks;
        public StatusEffectStatModifierCombinationType StatModifierCombinationType => _combinationType;

        [Header("Stat Periodic")]
        [SerializeField] private List<StatusEffectStatPeriodicContribution> _statPeriodics = new();
        public IReadOnlyList<StatusEffectStatPeriodicContribution> StatPeriodics => _statPeriodics;
    }
}
