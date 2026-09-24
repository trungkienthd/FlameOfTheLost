using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Chr_AI_Ntr_Animal_Flee_New", menuName = "Data/FlameOfTheLost/Character/AI/Neutral/Animal/FleeConfig")]
    public class AnimalFleeConfig : CharacterDataAsset
    {
        [Header("Flee")]
        [SerializeField, Min(0f)] private float _minimumFleeDuration = 3f;
        [SerializeField, Min(0f)] private float _threatForgetDuration = 5f;

        [Header("Distance")]
        [SerializeField, Min(0f)] private float _minimumSafeDistance = 4f;

        public float MinimumFleeDuration => _minimumFleeDuration;
        public float ThreatForgetDuration => _threatForgetDuration;
        public float MinimumSafeDistance => _minimumSafeDistance;
    }
}