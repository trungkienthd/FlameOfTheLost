using UnityEngine;

namespace FlameOfTheLost
{
    public static class Vector2Extensions
    {
        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            float newX = v.x * cos - v.y * sin;
            float newY = v.x * sin + v.y * cos;

            return new Vector2(newX, newY);
        }
    }
}
