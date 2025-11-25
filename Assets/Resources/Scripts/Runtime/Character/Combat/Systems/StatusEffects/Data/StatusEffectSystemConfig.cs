using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "StatusEffectSystemConfig", menuName = "Data/FlameOfTheLost/Combat/Systems/StatusEffect/StatusEffectSystemConfig")]
    public class StatusEffectSystemConfig : ScriptableObject
    {
        [Header("Global Floors & Ceilings")]
        [SerializeField, Min(0f)]       private float _minMovementSpeedScale = 0f;     // Allows 0 for Stunned effects
        [SerializeField, Min(0f)]       private float _maxMovementSpeedScale = 3f;
        [SerializeField, Min(0.01f)]    private float _minCooldownSeconds = 0.05f;
        public float MinMovementSpeedScale => _minMovementSpeedScale;
        public float MaxMovementSpeedScale => _maxMovementSpeedScale;
        public float MinCooldownSeconds => _minCooldownSeconds;

        [Header("Regen Guards")]
        [SerializeField] private bool _allowNegativeRegen = false;          // If false, Clamp >= 0
        public bool AllowNegativeRegen => _allowNegativeRegen;

        [Header("Max Health Behavior")]
        [SerializeField] private bool _proportionalAdjustCurrentHP = true;  // If true, current HP is adjusted proportionally when max HP changes
        public bool ProportionalAdjustCurrentHP => _proportionalAdjustCurrentHP;

        [Header("DoT/HoT Defaults")]
        [SerializeField] private bool _dotBypassesDamageTaken = true;       // If true, DoT ignores damage taken multipliers by default; This can be overridden per-effect by TreatAsIncomingDamage
        public bool DamageOverTimeBypassesDamageTaken => _dotBypassesDamageTaken;
    }
}
