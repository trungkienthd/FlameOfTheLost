using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "StatusEffectProfile", menuName = "Data/FlameOfTheLost/Combat/Systems/StatusEffect/StatusEffectProfile")]
    public class StatusEffectProfile : ScriptableObject, IIconProvider
    {
        [Header("Identity & UI")]
        [SerializeField] private int _id;
        [SerializeField] private string _displayName;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private Color _iconTint = Color.white;
        [SerializeField] private DispelClassType _dispelClass = DispelClassType.None;
        [SerializeField] private List<string> _tags = new();
        public int ID => _id;
        public string DisplayName => _displayName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public Color IconTint => _iconTint;
        public DispelClassType DispelClass => _dispelClass;
        public IReadOnlyList<string> Tags => _tags;

        [Header("Lifetime & Stacking")]
        [SerializeField, Min(0.01f)] private float _duration = 5f; // seconds
        [SerializeField, Min(1f)] private int _maxStacks = 1;
        [SerializeField] private StatusEffectCombinationType _combinationType = StatusEffectCombinationType.StackAll;
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
        public StatusEffectCombinationType CombinationType => _combinationType;
        public StatusEffectDurationRefreshType DurationRefreshType => _durationRefreshType;

        [Header("Modifiers (Stat)")]
        [SerializeField] private List<StatusEffectModifierContribution> _modifiers = new();
        public IReadOnlyList<StatusEffectModifierContribution> Modifiers => _modifiers;

        [Header("Over-Time (Direct HP change)")]
        [SerializeField] private StatusEffectPeriodicContribution _periodic = new();
        public StatusEffectPeriodicContribution Periodic => _periodic;

        // Validation helpers (editor-time suggested, but safe at runtime)
        public bool ValidateOperation(StatusEffectModifierTargetType target, StatusEffectModifierOperationType operation)
        {
            switch (target)
            {
                case StatusEffectModifierTargetType.MaxHealthDelta:
                case StatusEffectModifierTargetType.StaminaRegenRateDelta:
                case StatusEffectModifierTargetType.FPRegenRateDelta:
                case StatusEffectModifierTargetType.StaminaRegenDelayDelta:
                case StatusEffectModifierTargetType.FPRegenDelayDelta:
                    return operation == StatusEffectModifierOperationType.AddFlat;

                case StatusEffectModifierTargetType.MovementSpeedScale:
                case StatusEffectModifierTargetType.CooldownScale:
                case StatusEffectModifierTargetType.AbilityCostScaleStamina:
                case StatusEffectModifierTargetType.AbilityCostScaleFP:
                    return operation == StatusEffectModifierOperationType.Multiply;

                case StatusEffectModifierTargetType.DamageTaken:
                case StatusEffectModifierTargetType.KnockbackTaken:
                case StatusEffectModifierTargetType.DamageDealt:
                    return operation == StatusEffectModifierOperationType.AddFlat || operation == StatusEffectModifierOperationType.Multiply;

                default:
                    return false;
            }
        }
    }
}
