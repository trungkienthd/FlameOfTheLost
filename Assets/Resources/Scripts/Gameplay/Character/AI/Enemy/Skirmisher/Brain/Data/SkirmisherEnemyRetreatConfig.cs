using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Ret_Variant_Chr_AI_Enemy_Skr_New", menuName = "Data/FlameOfTheLost/Character/AI/Enemy/Skirmisher/RetreatConfig")]
    public class SkirmisherEnemyRetreatConfig : CharacterDataAsset
    {
        //  1. Retreat Trigger
        [Header("Retreat Trigger")]
        [SerializeField, Range(0f, 1f)] private float _healthThreshold = 0.3f;
        [SerializeField, Min(0)] private int _maxRetreatsPerEngagement = 1;
        public float HealthThreshold => _healthThreshold;
        public int MaxRetreatsPerEngagement => _maxRetreatsPerEngagement;

        //  2. Retreat Movement
        [Header("Retreat Movement")]
        [SerializeField, Min(0f)] private float _retreatSpeedMultiplier = 1.2f;
        [SerializeField, Min(0f)] private float _preferredSafeDistance = 3.5f;
        public float RetreatSpeedMultiplier => _retreatSpeedMultiplier;
        public float PreferredSafeDistance => _preferredSafeDistance;

        //  3. Retreat Duration
        [Header("Retreat Duration")]
        [SerializeField, Min(0f)] private float _minimumRetreatDuration = 1f;
        [SerializeField, Min(0f)] private float _maximumRetreatDuration = 2.5f;
        public float MinimumRetreatDuration => _minimumRetreatDuration;
        public float MaximumRetreatDuration => _maximumRetreatDuration;
    }
}