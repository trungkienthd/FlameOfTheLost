using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Chr_AI_Ntr_Animal_Flee_New", menuName = "Data/FlameOfTheLost/Character/AI/Neutral/Animal/Brain/FleeConfig")]
    public class AnimalFleeConfig : CharacterData
    {
        [Header("Horizontal Flee")]
        [SerializeField, Min(0f)] private float _sameHorizontalPositionThreshold = 0.05f;
        public float SameHorizontalPositionThreshold => _sameHorizontalPositionThreshold;
    }
}