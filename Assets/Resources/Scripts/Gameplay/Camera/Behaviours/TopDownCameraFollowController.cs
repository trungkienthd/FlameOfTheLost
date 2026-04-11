using UnityEngine;

namespace FlameOfTheLost
{
    public class TopDownCameraFollowController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform _target;
        public Transform Target
        {
            get => _target;
            private set => _target = value;
        }

        [Header("Follow")]
        [SerializeField] private Vector3 _offset = new Vector3(0f, 10f, -2f);
        [SerializeField] private float _smoothTime = 0.15f;
        private Vector3 Offset => _offset;
        private float SmoothTime => _smoothTime;

        [Header("Axis Control")]
        [SerializeField] private bool _followX = true;
        [SerializeField] private bool _followY = true;
        [SerializeField] private bool _followZ = true;
        private bool FollowX => _followX;
        private bool FollowY => _followY;
        private bool FollowZ => _followZ;

        private Vector3 _currentVelocity;

        private void Awake()
        {
            if (this.Target == null)
            {
                Transform playerTransform = GameObject.Find("Player").transform;
                if (playerTransform != null) this.SetTarget(playerTransform);
            }
        }
        private void Start()
        {
            Vector3 targetPosition = this.Target.position + this.Offset;
            this.transform.position = targetPosition;
        }
        private void LateUpdate()
        {
            if (this.Target == null) return;

            Vector3 targetPosition = this.Target.position + this.Offset;
            Vector3 currentPosition = this.transform.position;

            Vector3 desiredPosition = new Vector3
            (
                this.FollowX ? targetPosition.x : currentPosition.x,
                this.FollowY ? targetPosition.y : currentPosition.y,
                this.FollowZ ? targetPosition.z : currentPosition.z
            );

            this.transform.position = Vector3.SmoothDamp
            (
                currentPosition,
                desiredPosition,
                ref _currentVelocity,
                this.SmoothTime
            );
        }

        public void SetTarget(Transform target)
        {
            this.Target = target;
        }
    }
}
