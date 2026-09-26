using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Chr_AI_Ntr_Animal_Flee_New", menuName = "Data/FlameOfTheLost/Character/AI/Neutral/Animal/Brain/FleeConfig")]
    public class AnimalFleeConfig : CharacterDataAsset
    {
        [Header("Damage Threat")]
        [SerializeField, Min(0f)] private float _damageThreatForgetDuration = 5f;

        [Header("Horizontal Flee")]
        [SerializeField, Min(0f)] private float _sameHorizontalPositionThreshold = 0.05f;

        [Header("Home Boundary")]
        [SerializeField, Min(0.01f)] private float _boundaryProbeDistance = 0.25f;
        [SerializeField, Min(0f)] private float _boundaryPadding = 0.05f;

        public float DamageThreatForgetDuration => _damageThreatForgetDuration;
        public float SameHorizontalPositionThreshold => _sameHorizontalPositionThreshold;
        public float BoundaryProbeDistance => _boundaryProbeDistance;
        public float BoundaryPadding => _boundaryPadding;
    }
}