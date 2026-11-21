using UnityEngine;

namespace FlameOfTheLost
{
    public class InputVectorAimingProvider : MonoBehaviour, ICombatAimingProvider
    {
        [SerializeField] private Vector2 _rawInputVector;
        public Vector2 RawInputVector
        {
            get => _rawInputVector;
            private set => _rawInputVector = value;
        }

        public bool TryGetAimDirection(Transform ownerTransform, out Vector2 aimDirection)
        {
            aimDirection = Vector2.zero;
            if (this.RawInputVector.sqrMagnitude < Mathf.Epsilon) return false;

            aimDirection = this.RawInputVector.normalized;
            return true;
        }
    }
}
