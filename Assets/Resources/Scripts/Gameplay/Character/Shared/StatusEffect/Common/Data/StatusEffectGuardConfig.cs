using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "SEGuard_Character", menuName = "Data/FlameOfTheLost/Character/Shared/StatusEffects/CharacterGuardConfig")]
    public class StatusEffectGuardConfig : ScriptableObject
    {
        [Header("Global Floors & Ceilings (Multipliers)")]
        [SerializeField, Min(0f)] private float _minMovementSpeedScale = 0f;     // Allows 0 for Stunned effects
        [SerializeField, Min(0f)] private float _maxMovementSpeedScale = 3f;
        public float MinMovementSpeedScale => _minMovementSpeedScale;
        public float MaxMovementSpeedScale => _maxMovementSpeedScale;

        [Header("Global Guards (Add Flat)")]
        [SerializeField] private bool _allowNegativeRegen = false;          // If false, Clamp >= 0
        public bool AllowNegativeRegen => _allowNegativeRegen;

        [Header("Damage/Cost Guards (Multipliers)")]
        [SerializeField, Min(0f)] private float _minDamageTakenScale = 0f;  // prevent negative multipliers
        [SerializeField, Min(0f)] private float _minDamageDealtScale = 0f;
        [SerializeField, Min(0f)] private float _minKnockbackTakenScale = 0f;
        [SerializeField, Min(0f)] private float _minKnockbackDealtScale = 0f;
        [SerializeField, Min(0f)] private float _minCostScale = 0f;        // stamina/fp cost cannot go < 0

        public float MinDamageTakenScale => _minDamageTakenScale;
        public float MinDamageDealtScale => _minDamageDealtScale;
        public float MinKnockbackTakenScale => _minKnockbackTakenScale;
        public float MinKnockbackDealtScale => _minKnockbackDealtScale;
        public float MinCostScale => _minCostScale;

        [Header("Periodic Tick Guards")]
        [SerializeField, Min(0.01f)] private float _minTickInterval = 0.05f; // avoid 0 or crazy spam
        [SerializeField, Min(0.01f)] private float _maxTickInterval = 10f;   // safety
        public float MinTickInterval => _minTickInterval;
        public float MaxTickInterval => _maxTickInterval;


        [Header("Periodic Amount Guards (per second)")] 
        [SerializeField, Min(0f)] private float _maxHPPerSecond = 9999f;
        [SerializeField, Min(0f)] private float _maxFPPerSecond = 9999f;
        [SerializeField, Min(0f)] private float _maxStaminaPerSecond = 9999f;

        public float MaxHPPerSecond => _maxHPPerSecond;
        public float MaxFPPerSecond => _maxFPPerSecond;
        public float MaxStaminaPerSecond => _maxStaminaPerSecond;
    }
}
