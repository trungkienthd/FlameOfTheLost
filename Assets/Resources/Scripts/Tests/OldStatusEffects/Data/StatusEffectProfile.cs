using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "StatusEffectProfile", menuName = "FlameOfTheLost/Combat/Systems/StatusEffect/StatusEffectProfile")]
    public class StatusEffectProfile : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;
        [SerializeField] private StatusTag[] _statusTags;

        public string Id => _id;
        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
        public StatusTag[] StatusTags => _statusTags;

        [Header("Lifetime")]
        [SerializeField] private float _baseDuration;
        [SerializeField] private int _maxStacks;
        [SerializeField] private StatusEffectStackingPolicy _statusEffectStackingPolicy = StatusEffectStackingPolicy.RefreshDuration;
        public float BaseDurationInSeconds => _baseDuration;
        public int MaxStacks => _maxStacks;
        public StatusEffectStackingPolicy StatusEffectStackingPolicy => _statusEffectStackingPolicy;

        [Header("Potency")]
        [SerializeField] private float _basePotency = 1f;
        [SerializeField] private bool _scaleWithSource = false;

        public float BasePotencyScale => _basePotency;
        public bool ScaleWithSource => _scaleWithSource;

        [Header("Ticking")]
        [SerializeField] private TickSchedule _tickSchedule = TickSchedule.None;
        [SerializeField] private float _tickInterval = 1f;                          // Used for FixedInterval
        [SerializeField] private PeriodicStatusEffectTickMode _tickMode;
        [SerializeField] private int _tickAmount = 1;

        public TickSchedule TickSchedule => _tickSchedule;
        public float TickInterval => _tickInterval;
        public PeriodicStatusEffectTickMode TickMode => _tickMode;
        public int TickAmount => _tickAmount;

        [Header("CC Flags")]
        [SerializeField] private bool _appliesSilence = false;
        [SerializeField] private bool _appliesFatigue = false;
        [SerializeField] private bool _appliesStun = false;

        public bool AppliesSilence => _appliesSilence;  
        public bool AppliesFatigue => _appliesFatigue;
        public bool AppliesStun => _appliesStun;

        [Header("Stat Modifiers")]
        [SerializeField] private StatModifier[] _modifiers;
        public StatModifier[] StatModifiers => _modifiers;

        [Header("Dispels")]
        [SerializeField] private DispelClass _dispelClass = DispelClass.None;
        [SerializeField] private bool _isBeneficial = false;
        [SerializeField] private bool _undispellable = false;
        public DispelClass DispelClass => _dispelClass;
        public bool IsBeneficial => _isBeneficial;
        public bool Undispellable => _undispellable;
    }
}
