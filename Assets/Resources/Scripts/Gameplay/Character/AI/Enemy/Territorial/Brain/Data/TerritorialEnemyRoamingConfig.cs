using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "AI_Enm_Trt_Roam_New", menuName = "Data/FlameOfTheLost/Character/Enemy/Territorial/RoamingConfig")]
    public class TerritorialEnemyRoamingConfig : ScriptableObject
    {
        [Header("Walk Duration")]
        [SerializeField] private float _minRoamWalkDuration = 1.2f;
        [SerializeField] private float _maxRoamWalkDuration = 2.5f;
        public float MinRoamWalkDuration => _minRoamWalkDuration;
        public float MaxRoamWalkDuration => _maxRoamWalkDuration;

        [Header("Idle Duration")]
        [SerializeField] private float _minRoamIdleDuration = 0.8f;
        [SerializeField] private float _maxRoamIdleDuration = 1.8f;
        public float MinRoamIdleDuration => _minRoamIdleDuration;
        public float MaxRoamIdleDuration => _maxRoamIdleDuration;

        public float GetRandomWalkDuration()
        {
            return Random.Range(this.MinRoamWalkDuration, this.MaxRoamWalkDuration);
        }
        public float GetRandomIdleDuration()
        {
            return Random.Range(this.MinRoamIdleDuration, this.MaxRoamIdleDuration);
        }
    }
}
