using UnityEngine;

namespace FlameOfTheLost
{
    public static class AimHelper
    {
        public static Vector2 TryGetMouseAimDir2D(Transform origin, Camera camera, float deadRadius, out bool valid)
        {
            valid = false;
            if (!origin || !camera) return Vector2.zero;

            Vector3 mousePos = Input.mousePosition;
            Vector3 world = camera.ScreenToWorldPoint(mousePos);
            world.z = origin.position.z;

            Vector2 delta = (Vector2)(world - origin.position);
            if (delta.sqrMagnitude < deadRadius * deadRadius) return Vector2.zero;

            valid = true;
            return delta.normalized;
        }

        public static FacingStatus GetFacingFromVector(Vector2 dir, bool eightWay, out bool flipX)
        {
            // Your FacingStatus = DOWN/UP/SIDE; you flip sprite for left vs right
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // -180..180, 0 = right
            flipX = angle < -90f || angle > 90f; // left if pointing to the left half-plane

            if (!eightWay)
            {
                // 4-way snap (Right/Left -> SIDE; UP; DOWN)
                float a = Mathf.Abs(angle);
                if (a <= 45f || a >= 135f) return FacingStatus.Side;
                return angle > 0 ? FacingStatus.Up : FacingStatus.Down;
            }
            else
            {
                // 8-way (optional if you add diagonal anims later)
                float a = (angle + 360f) % 360f;
                if (a < 22.5f || a >= 337.5f) return FacingStatus.Side;       
                if (a < 67.5f) return FacingStatus.Up;                     
                if (a < 112.5f) return FacingStatus.Up;                        
                if (a < 157.5f) return FacingStatus.Up;                       
                if (a < 202.5f) return FacingStatus.Side;                      
                if (a < 247.5f) return FacingStatus.Down;                       
                if (a < 292.5f) return FacingStatus.Down;                       
                return FacingStatus.Down;                                      
            }
        }
    }
}
