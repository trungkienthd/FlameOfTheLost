using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class SkillStatusEffectApplication
    {
        [Header("What")]
        [SerializeField] private StatusEffectProfile _statusEffect;
        public StatusEffectProfile StatusEffect => _statusEffect;

        [Header("Who")]
        [SerializeField] private SkillStatusEffectReceiverType _receiver = SkillStatusEffectReceiverType.Target;
        public SkillStatusEffectReceiverType Receiver => _receiver;

        [Header("When")]
        [SerializeField] private SkillStatusEffectTriggerType _trigger = SkillStatusEffectTriggerType.OnAttackStart;
        public SkillStatusEffectTriggerType Trigger => _trigger;

        [Header("Chance / Strength")]
        [SerializeField, Range(0f, 1f)] private float _chance = 1f;
        [SerializeField, Min(0f)] private float _potency = 1f;
        public float Chance => Mathf.Clamp01(_chance);
        public float Potency => Mathf.Max(0f, _potency);

        [Header("Application Limits")]
        [SerializeField] private SkillStatusEffectApplicationLimitType _limit = SkillStatusEffectApplicationLimitType.EveryValidHit;
        public SkillStatusEffectApplicationLimitType Limit => _limit;

        public bool IsValid => this.StatusEffect != null && this.Chance > 0f && this.Potency >= 0f;
    }
}