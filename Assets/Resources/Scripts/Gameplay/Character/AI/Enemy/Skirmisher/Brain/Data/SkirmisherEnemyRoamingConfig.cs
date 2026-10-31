using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Roam_Variant_Chr_AI_Enemy_Skr_New", menuName = "Data/FlameOfTheLost/Character/AI/Enemy/Skirmisher/RoamingConfig")]
    public class SkirmisherEnemyRoamingConfig : CharacterDataAsset
    {
        //  1. Walk Duration
        [Header("Walk Duration")]
        [SerializeField, Min(0f)] private float _minRoamWalkDuration = 2f;
        [SerializeField, Min(0f)] private float _maxRoamWalkDuration = 5f;
        public float MinRoamWalkDuration => _minRoamWalkDuration;
        public float MaxRoamWalkDuration => _maxRoamWalkDuration;

        //  2. Idle Duration 
        [Header("Idle Duration")]
        [SerializeField, Min(0f)] private float _minRoamIdleDuration = 1.5f;
        [SerializeField, Min(0f)] private float _maxRoamIdleDuration = 3.5f;
        public float MinRoamIdleDuration => _minRoamIdleDuration;
        public float MaxRoamIdleDuration => _maxRoamIdleDuration;

        // --- Public APIs ---
        public float GetRandomWalkDuration()
        {
            return Random.Range(this.MinRoamWalkDuration, Mathf.Max(this.MinRoamWalkDuration, this.MaxRoamWalkDuration));
        }
        public float GetRandomIdleDuration()
        {
            return Random.Range(this.MinRoamIdleDuration, Mathf.Max(this.MinRoamIdleDuration, this.MaxRoamIdleDuration));
        }
    }
}