using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Chr_AI_Ntr_Animal_Roam_New", menuName = "Data/FlameOfTheLost/Character/AI/Neutral/Animal/RoamingConfig")]
    public class AnimalRoamingConfig : CharacterDataAsset
    {
        [Header("Walk Duration")]
        [SerializeField, Min(0f)] private float _minWalkDuration = 1.5f;
        [SerializeField, Min(0f)] private float _maxWalkDuration = 4f;
        public float MinWalkDuration => _minWalkDuration;
        public float MaxWalkDuration => _maxWalkDuration;

        public float GetRandomWalkDuration()
        {
            return Random.Range
            (
                Mathf.Min(this.MinWalkDuration, this.MaxWalkDuration),
                Mathf.Max(this.MinWalkDuration, this.MaxWalkDuration)
            );
        }

        [Header("Idle Duration")]
        [SerializeField, Min(0f)] private float _minIdleDuration = 2f;
        [SerializeField, Min(0f)] private float _maxIdleDuration = 5f;
        public float MinIdleDuration => _minIdleDuration;
        public float MaxIdleDuration => _maxIdleDuration;

        public float GetRandomIdleDuration()
        {
            return Random.Range
            (
                Mathf.Min(this.MinIdleDuration, this.MaxIdleDuration),
                Mathf.Max(this.MinIdleDuration, this.MaxIdleDuration)
            );
        }
    }
}