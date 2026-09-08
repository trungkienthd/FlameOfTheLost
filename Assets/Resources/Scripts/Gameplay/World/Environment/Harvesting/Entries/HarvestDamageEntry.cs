using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class HarvestDamageEntry
    {
        [SerializeField] private bool _isEnabled;
        [SerializeField] private HarvestImpactType _impactType = HarvestImpactType.None;
        public bool IsEnabled => _isEnabled && _impactType != HarvestImpactType.None;
        public HarvestImpactType ImpactType => this.IsEnabled
            ? _impactType
            : HarvestImpactType.None;

        [SerializeField, Min(0f)] private float _combatDamageMultiplier = 0f;
        [SerializeField, Min(0)] private int _flatHarvestDamage = 1;
        [SerializeField, Min(0)] private int _minimumHarvestDamage = 1;
        [SerializeField] private bool _triggerSelfOnHitStatusEffects = false;
        public float CombatDamageMultiplier => Mathf.Max(0f, _combatDamageMultiplier);
        public int FlatHarvestDamage => Mathf.Max(0, _flatHarvestDamage);
        public int MinimumHarvestDamage => Mathf.Max(0, _minimumHarvestDamage);
        public bool TriggerSelfOnHitStatusEffects => _triggerSelfOnHitStatusEffects;
    }
}
