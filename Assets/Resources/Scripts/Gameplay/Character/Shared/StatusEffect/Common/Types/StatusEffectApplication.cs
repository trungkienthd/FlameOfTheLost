using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class StatusEffectApplication
    {
        [Header("What")]
        [SerializeField] private StatusEffectProfile _statusEffect;
        public StatusEffectProfile StatusEffect => _statusEffect;

        [Header("Chance / Strength")]
        [SerializeField, Range(0f, 1f)] private float _chance = 1f;
        [SerializeField, Min(0f)] private float _potency = 1f;
        public float Chance => Mathf.Clamp01(_chance);
        public float Potency => Mathf.Max(0f, _potency);

        public virtual bool IsValid => this.StatusEffect != null && this.Chance > 0f && this.Potency >= 0f;
    }
}
