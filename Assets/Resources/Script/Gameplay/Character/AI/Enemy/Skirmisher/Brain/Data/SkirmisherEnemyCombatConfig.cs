using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Cbt_Variant_Chr_AI_Enemy_Sks_New", menuName = "Data/FlameOfTheLost/Character/AI/Enemy/Skirmisher/CombatConfig")]
    public class SkirmisherEnemyCombatConfig : CharacterData
    {
        //  1. Attack Decision
        [Header("Attack Decision")]
        [SerializeField, Min(0f)] private float _minAttackDecisionInterval = 0.4f;
        [SerializeField, Min(0f)] private float _maxAttackDecisionInterval = 0.8f;
        public float MinAttackDecisionInterval => _minAttackDecisionInterval;
        public float MaxAttackDecisionInterval => _maxAttackDecisionInterval;

        //  2. Skill Selection
        [Header("Skill Selection")]
        [SerializeField, Min(0)] private int _normalAttackWeight = 70;
        [SerializeField, Min(0)] private int _specialAttackWeight = 30;
        public int NormalAttackWeight => _normalAttackWeight;
        public int SpecialAttackWeight => _specialAttackWeight;

        //  3. Normal Attack
        [Header("Normal Attack")]
        [SerializeField] private AttackSide _normalAttackSide = AttackSide.Left;
        [SerializeField, Min(0f)] private float _normalAttackStateDuration = 0.7f;
        [SerializeField, Min(0f)] private float _normalAttackRecoveryDuration = 0.5f;
        public AttackSide NormalAttackSide => _normalAttackSide;
        public float NormalAttackStateDuration => _normalAttackStateDuration;
        public float NormalAttackRecoveryDuration => _normalAttackRecoveryDuration;

        //      3.1. AI Normal Attack Distance
        [Header("Normal Attack AI Range")]
        [SerializeField, Min(0f)] private float _normalAttackMinimumDistance = 0f;
        [SerializeField, Min(0f)] private float _normalAttackMaximumDistance = 1.5f;
        public float NormalAttackMinimumDistance => _normalAttackMinimumDistance;
        public float NormalAttackMaximumDistance => _normalAttackMaximumDistance;

        //  4. Special Attack
        [Header("Special Attack")]
        [SerializeField, Min(0f)] private float _specialAttackStateDuration = 1f;
        [SerializeField, Min(0f)] private float _specialAttackRecoveryDuration = 0.9f;
        public float SpecialAttackStateDuration => _specialAttackStateDuration;
        public float SpecialAttackRecoveryDuration => _specialAttackRecoveryDuration;

        //      4.1. AI Special Attack Distance
        [Header("Special Attack AI Range")]
        [SerializeField, Min(0f)] private float _specialAttackMinimumDistance = 0f;
        [SerializeField, Min(0f)] private float _specialAttackMaximumDistance = 3f;
        public float SpecialAttackMinimumDistance => _specialAttackMinimumDistance;
        public float SpecialAttackMaximumDistance => _specialAttackMaximumDistance;

        // --- Unity Callbacks ---
#if UNITY_EDITOR
        private void OnValidate()
        {
            _normalAttackMaximumDistance    = Mathf.Max(this.NormalAttackMinimumDistance,   this.NormalAttackMaximumDistance);
            _specialAttackMaximumDistance   = Mathf.Max(this.SpecialAttackMinimumDistance,  this.SpecialAttackMaximumDistance);
        }
#endif

        // --- Public APIs ---
        //  1. AI Attack Range Validation
        public bool IsNormalAttackInRange(float distance)
        {
            return distance >= this.NormalAttackMinimumDistance && distance <= this.NormalAttackMaximumDistance;
        }
        public bool IsSpecialAttackInRange(float distance)
        {
            return distance >= this.SpecialAttackMinimumDistance && distance <= this.SpecialAttackMaximumDistance;
        }

        //  2. Attack Decision Interval
        public float GetRandomAttackDecisionInterval()
        {
            return Random.Range(this.MinAttackDecisionInterval, Mathf.Max(this.MinAttackDecisionInterval, this.MaxAttackDecisionInterval));
        }
    }
}