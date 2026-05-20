using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class SkillStatusEffectApplication : StatusEffectApplication
    {
        [Header("Who")]
        [SerializeField] private SkillStatusEffectReceiverType _receiver = SkillStatusEffectReceiverType.Target;
        public SkillStatusEffectReceiverType Receiver => _receiver;

        [Header("When")]
        [SerializeField] private SkillStatusEffectTriggerType _trigger = SkillStatusEffectTriggerType.OnAttackStart;
        public SkillStatusEffectTriggerType Trigger => _trigger;

        [Header("Application Limits")]
        [SerializeField] private SkillStatusEffectApplicationLimitType _limit = SkillStatusEffectApplicationLimitType.EveryValidHit;
        public SkillStatusEffectApplicationLimitType Limit => _limit;

        public override bool IsValid => base.IsValid;
    }
}