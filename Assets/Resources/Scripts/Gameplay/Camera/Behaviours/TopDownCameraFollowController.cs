using UnityEngine;

namespace FlameOfTheLost
{
    public class TopDownCameraFollowController : MonoBehaviour
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _target;
        private Camera Camera => _camera;
        private Transform Target => _target;

        //  2. Follow Settings
        [Header("Follow")]
        [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, -10f);
        [SerializeField] private float _smoothTime = 0.15f;
        private Vector3 Offset => _offset;
        private float SmoothTime => _smoothTime;

        //  3. Axis Control
        [Header("Axis Control")]
        [SerializeField] private bool _followX = true;
        [SerializeField] private bool _followY = true;
        [SerializeField] private bool _followZ = true;
        private bool FollowX => _followX;
        private bool FollowY => _followY;
        private bool FollowZ => _followZ;

        //  4. Boundary
        [Header("Boundary")]
        [SerializeField] private bool _useBoundary = true;
        [SerializeField] private Vector2 _minBoundary;                      // World-space minimum boundary of the camera center.
        [SerializeField] private Vector2 _maxBoundary;                      // World-space maximum boundary of the camera center.
        [SerializeField] private bool _clampUsingCameraViewSize = true;     // If true, the camera view size is considered, so the visible camera area never leaves the boundary.
        private bool UseBoundary => _useBoundary;
        private Vector2 MinBoundary => _minBoundary;
        private Vector2 MaxBoundary => _maxBoundary;
        private bool ClampUsingCameraViewSize => _clampUsingCameraViewSize;

        //  5. Runtime State
        private Vector3 _currentVelocity;

        // --- Unity Callbacks ---
        private void Awake()
        {
            if (this.Camera == null) _camera = this.GetComponent<Camera>();

            if (this.Target == null)
            {
                Transform playerTransform = GameObject.Find("Player").transform;
                if (playerTransform != null) _target = playerTransform;
            }
        }
        private void Start()
        {
            if (this.Target == null) return;

            Vector3 targetPosition = this.Target.position + this.Offset;
            Vector3 desiredPosition = this.ApplyAxisControl(targetPosition);
            desiredPosition = this.ClampPositionToBoundary(desiredPosition);

            this.transform.position = desiredPosition;
        }
        private void LateUpdate()
        {
            if (this.Target == null) return;

            Vector3 targetPosition = this.Target.position + this.Offset;
            Vector3 desiredPosition = this.ApplyAxisControl(targetPosition);
            desiredPosition = this.ClampPositionToBoundary(desiredPosition);

            this.transform.position = Vector3.SmoothDamp
            (
                this.transform.position,
                desiredPosition,
                ref _currentVelocity,
                this.SmoothTime
            );
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!this.UseBoundary) return;

            Vector3 center = new Vector3
            (
                (this.MinBoundary.x + this.MaxBoundary.x) * 0.5f,
                (this.MinBoundary.y + this.MaxBoundary.y) * 0.5f,
                this.transform.position.z
            );

            Vector3 size = new Vector3
            (
                this.MaxBoundary.x - this.MinBoundary.x,
                this.MaxBoundary.y - this.MinBoundary.y,
                0f
            );

            Gizmos.DrawWireCube(center, size);
        }
#endif

        // --- Helpers ---
        private Vector3 ApplyAxisControl(Vector3 targetPosition)
        {
            Vector3 currentPosition = this.transform.position;

            return new Vector3
            (
                this.FollowX ? targetPosition.x : currentPosition.x,
                this.FollowY ? targetPosition.y : currentPosition.y,
                this.FollowZ ? targetPosition.z : currentPosition.z
            );
        }

        private Vector3 ClampPositionToBoundary(Vector3 position)
        {
            if (!this.UseBoundary) return position;

            float minX = this.MinBoundary.x;
            float maxX = this.MaxBoundary.x;
            float minY = this.MinBoundary.y;
            float maxY = this.MaxBoundary.y;

            if (this.ClampUsingCameraViewSize && this.Camera != null && this.Camera.orthographic)
            {
                float cameraHalfHeight = this.Camera.orthographicSize;
                float cameraHalfWidth = cameraHalfHeight * this.Camera.aspect;

                minX += cameraHalfWidth;
                maxX -= cameraHalfWidth;
                minY += cameraHalfHeight;
                maxY -= cameraHalfHeight;
            }

            if (minX > maxX)
            {
                float middleX = (this.MinBoundary.x + this.MaxBoundary.x) * 0.5f;
                minX = middleX;
                maxX = middleX;
            }

            if (minY > maxY)
            {
                float middleY = (this.MinBoundary.y + this.MaxBoundary.y) * 0.5f;
                minY = middleY;
                maxY = middleY;
            }

            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.y = Mathf.Clamp(position.y, minY, maxY);

            return position;
        }
    }
}
