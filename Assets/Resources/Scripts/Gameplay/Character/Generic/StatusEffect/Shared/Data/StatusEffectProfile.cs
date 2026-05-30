using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "StatusEffectProfile", menuName = "Data/FlameOfTheLost/Character/StatusEffects/StatusEffect")]
    public class StatusEffectProfile : ScriptableObject, IIdentityProvider, IIconProvider
    {
        // --- States ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _id;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(5, 5)] private string _description;
        [SerializeField] private Sprite _icon;
        public int ID => _id;
        public string DisplayName => _displayName;
        public string Description => _description;
        public Sprite Icon => _icon;

        //  2. Classification
        [Header("Classification")]
        [SerializeField] private StatusEffectAffinityType _affinity = StatusEffectAffinityType.Debuff;
        public StatusEffectAffinityType Affinity => _affinity;

        //  3. Configurations
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

        //  4. Contributions
        //      4.1. Flag
        [Header("Flag")]
        [SerializeField] private List<StatusEffectFlagContribution> _flags = new();
        public IReadOnlyList<StatusEffectFlagContribution> Flags => _flags;

        //      4.2. Stat Modifier
        [Header("Stat Modifier")]
        [SerializeField] private List<StatusEffectStatModifierContribution> _statModifiers = new();
        public IReadOnlyList<StatusEffectStatModifierContribution> StatModifiers => _statModifiers;

        [SerializeField] private StatusEffectStatModifierCombinationType _combinationType = StatusEffectStatModifierCombinationType.AddStacks;
        public StatusEffectStatModifierCombinationType StatModifierCombinationType => _combinationType;

        //      4.3. Stat Periodic
        [Header("Stat Periodic")]
        [SerializeField] private List<StatusEffectStatPeriodicContribution> _statPeriodics = new();
        public IReadOnlyList<StatusEffectStatPeriodicContribution> StatPeriodics => _statPeriodics;

        //  5. VFX
        [Header("VFX")]
        [SerializeField] private AnimationClip _vfxClip;
        [SerializeField] private int _vfxSortingOrderOffset = 5;
        public AnimationClip VFXClip => _vfxClip;
        public int VFXSortingOrderOffset => _vfxSortingOrderOffset;
    }
}
