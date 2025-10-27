using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class StatusEffectApplication
    {
        [Header("Status Effect")]
        [SerializeField] private StatusEffectProfile _statusEffectProfile;
        [SerializeField, Range(0f, 1f)] private float _applicationChance = 1f;

        public StatusEffectProfile StatusEffectProfile => _statusEffectProfile;
        public float ApplicationChance => _applicationChance;

        [Header("Delivery")]
        [SerializeField] private StatusEffectApplicationTiming _timing = StatusEffectApplicationTiming.OnMeleeHitConfirm;
        [SerializeField] private int _stacksToApply = 1;
        [SerializeField] private float _potency = 1f;
        [SerializeField] private float _durationOverrideInSecs = 0f;
        [SerializeField] private int _maxTargets = 1;

        public StatusEffectApplicationTiming Timing => _timing;
        public int StacksToApply => _stacksToApply;
        public float PotencyScale => _potency;
        public float DurationOverrideInSeconds => _durationOverrideInSecs;
        public int MaxTargets => _maxTargets;

        [Header("Critical Bonus")]
        [SerializeField] private bool _bonusOnCritical = false;
        [SerializeField] private int _bonusStacksOnCritical = 0;
        [SerializeField] private float _bonusPotencyOnCritical = 0f;
        [SerializeField] private float _bonusDurationOnCriticalInSecs = 0f;
        public bool BonusOnCritical => _bonusOnCritical;
        public int BonusStacksOnCritical => _bonusStacksOnCritical;
        public float BonusPotencyOnCritical => _bonusPotencyOnCritical;
        public float BonusDurationOnCriticalInSeconds => _bonusDurationOnCriticalInSecs;
    }
}
