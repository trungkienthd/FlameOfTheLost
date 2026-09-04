using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Mm_New", menuName = "Data/FlameOfTheLost/Character/Shared/Movement/MovementConfig")]
    public class CharacterMovementConfig : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField, Min(0f)] private float _movementSpeed = 6f;
        [SerializeField, Min(0f)] private float _accel = 60f;
        [SerializeField, Min(0f)] private float _decel = 80f;

        public float MovementSpeed => _movementSpeed;
        public float Accel => _accel;
        public float Decel => _decel;
    }
}
