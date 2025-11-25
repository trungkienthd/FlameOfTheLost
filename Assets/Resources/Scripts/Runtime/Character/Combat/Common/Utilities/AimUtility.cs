using UnityEngine;

namespace FlameOfTheLost
{
    public static class AimUtility
    {
        public static Vector2 ApplyDeadzone(Vector2 vector, float deadzone)
        {
            if (vector.magnitude <= deadzone) return Vector2.zero;

            return vector;
        }

        public static Vector2 ClampLength(Vector2 vector, float maxLength)
        {
            if (maxLength <= 0f) return vector;

            float magnitude = vector.magnitude;
            return (magnitude > maxLength) ? vector * (maxLength / magnitude) : vector;
        }

        // Returns 0=Down, 1=Side, 2=Up given a vector and snap mode
        public static int FacingFromDirection(Vector2 direction, FacingSnapType mode)
        {
            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y)) return 1; // Side

            return (direction.y > 0f) ? 2 : 0; // Up/Down
        }

        // Snaps to 8-way
        public static Vector2 SnapDirection(Vector2 direction, FacingSnapType mode)
        {
            if (direction == Vector2.zero) return direction;
            if (mode == FacingSnapType.FourWay)
            {
                // choose axis of dominance
                return Mathf.Abs(direction.x) >= Mathf.Abs(direction.y)
                    ? new Vector2(Mathf.Sign(direction.x), 0f)
                    : new Vector2(0f, Mathf.Sign(direction.y));
            }
            // 8-way (simple octant)
            float ang = Mathf.Atan2(direction.y, direction.x);
            float step = Mathf.PI / 4f; // 45°
            int bucket = Mathf.RoundToInt(ang / step);
            float snapped = bucket * step;
            return new Vector2(Mathf.Cos(snapped), Mathf.Sin(snapped)).normalized;
        }
    }
}
