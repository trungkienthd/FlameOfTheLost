using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class HarvestImpactResponseEntry
    {
        // --- Data / Configs ---
        [SerializeField] private HarvestImpactType _impactType = HarvestImpactType.None;
        [SerializeField, Min(0f)] private float _damageMultiplier = 1f;

        public HarvestImpactType ImpactType => _impactType;
        public float DamageMultiplier => Mathf.Max(0f, _damageMultiplier);
    }
}