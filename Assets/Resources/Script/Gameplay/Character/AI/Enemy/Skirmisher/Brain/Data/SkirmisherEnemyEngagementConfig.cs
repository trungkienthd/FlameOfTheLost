using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Egm_Variant_Chr_AI_Enemy_Sks_New", menuName = "Data/FlameOfTheLost/Character/AI/Enemy/Skirmisher/EngagementConfig")]
    public class SkirmisherEnemyEngagementConfig : CharacterData
    {
        //  1. Alert
        [Header("Alert")]
        [SerializeField, Min(0f)] private float _alertDuration = 0.5f;
        public float AlertDuration => _alertDuration;

        //  2. Target Retention
        [Header("Target Retention")]
        [SerializeField, Min(0f)] private float _targetLostGraceDuration = 1f;
        public float TargetLostGraceDuration => _targetLostGraceDuration;

        //  3. Pursuit 
        [Header("Pursuit")]
        [SerializeField, Min(0f)] private float _maximumPursuitDistance = 6f;
        [SerializeField, Min(0f)] private float _chaseSpeedMultiplier = 1.1f;
        public float MaximumPursuitDistance => _maximumPursuitDistance;
        public float ChaseSpeedMultiplier => _chaseSpeedMultiplier;
    }
}
