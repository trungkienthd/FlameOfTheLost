using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class WorldMapBoundaryTransitionHandler : FOTLMonoBehaviour
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private WorldMapTransitionController _transitionController;
        private WorldMapTransitionController TransitionController => _transitionController;

        private BoxCollider2D _boundaryCollider;
        private BoxCollider2D BoundaryCollider => _boundaryCollider;

        // --- Configs ---
        //  1. Boundary
        [Header("Configs - Boundary")]
        [SerializeField] private WorldMapBoundaryDirection _boundaryDirection;
        public WorldMapBoundaryDirection BoundaryDirection => _boundaryDirection;

        //  2. Destination
        [Header("Configs - Destination")]
        [SerializeField] private string _destinationSceneName;
        [SerializeField] private WorldMapBoundaryDirection _destinationBoundaryDirection;
        [SerializeField] private bool _enableTransition = true;

        // --- Unity Events ---
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_enableTransition) return;

            if (this.BoundaryCollider == null) return;
            if (!this.BoundaryCollider.isTrigger) return;

            if (this.TransitionController == null) return;
            if (this.TransitionController.IsTransitioning) return;

            // Ignore secondary trigger colliders.
            if (other.isTrigger) return;

            PlayerRootController player = other.GetComponentInParent<PlayerRootController>();
            if (player == null) return;

            // Only the player's primary collider can trigger a transition.
            Collider2D playerMainCollider = player.GetComponent<Collider2D>();

            if (playerMainCollider == null) return;

            if (other != playerMainCollider) return;

            float normalizedPosition = this.CalculateNormalizedBoundaryPosition(player.transform.position);

            this.TransitionController.TryTransition(_destinationSceneName, _destinationBoundaryDirection, normalizedPosition);
        }

        // --- Public APIs ---
        public Bounds GetBoundaryBounds()
        {
            return this.BoundaryCollider.bounds;
        }
        public Vector3 CalculateDestinationPosition(float normalizedPosition, float inwardOffset, float alongEdgeClearance = 0f)
        {
            Bounds bounds = this.BoundaryCollider.bounds;

            float x = bounds.center.x;
            float y = bounds.center.y;

            float t = Mathf.Clamp01(normalizedPosition);

            float clearance = Mathf.Max(0f, alongEdgeClearance);

            switch (this.BoundaryDirection)
            {
                case WorldMapBoundaryDirection.Left:
                {
                    x = bounds.max.x + inwardOffset;

                    float minY = bounds.min.y + clearance;
                    float maxY = bounds.max.y - clearance;

                    y = Mathf.Lerp(minY, maxY, t);

                    break;
                }

                case WorldMapBoundaryDirection.Right:
                {
                    x = bounds.min.x - inwardOffset;

                    float minY = bounds.min.y + clearance;
                    float maxY = bounds.max.y - clearance;

                    y = Mathf.Lerp(minY, maxY, t);

                    break;
                }

                case WorldMapBoundaryDirection.Top:
                {
                    float minX = bounds.min.x + clearance;
                    float maxX = bounds.max.x - clearance;

                    x = Mathf.Lerp(minX, maxX, t);

                    y = bounds.min.y - inwardOffset;

                    break;
                }

                case WorldMapBoundaryDirection.Bottom:
                {
                    float minX = bounds.min.x + clearance;
                    float maxX = bounds.max.x - clearance;

                    x = Mathf.Lerp(minX, maxX, t);

                    y = bounds.max.y + inwardOffset;

                    break;
                }
            }

            return new Vector3(x, y, 0f);
        }

        // --- Helpers ---
        //  1. Boundary Position
        private float CalculateNormalizedBoundaryPosition(Vector3 position)
        {
            Bounds bounds = this.BoundaryCollider.bounds;

            switch (this.BoundaryDirection)
            {
                case WorldMapBoundaryDirection.Left:
                case WorldMapBoundaryDirection.Right:
                    return Mathf.InverseLerp(bounds.min.y, bounds.max.y, position.y);

                case WorldMapBoundaryDirection.Top:
                case WorldMapBoundaryDirection.Bottom:
                    return Mathf.InverseLerp(bounds.min.x, bounds.max.x, position.x);
            }

            return 0.5f;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            _boundaryCollider = this.GetComponent<BoxCollider2D>();
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.TransitionController == null) _transitionController = UnityEngine.Object.FindFirstObjectByType<WorldMapTransitionController>();
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.TransitionController == null)
            {
                Debug.LogError($"[{nameof(WorldMapBoundaryTransitionHandler)}] {nameof(WorldMapTransitionController)} is missing.", this);
            }

            if (this.BoundaryCollider == null)
            {
                Debug.LogError($"[{nameof(WorldMapBoundaryTransitionHandler)}] Boundary collider is missing.", this);
                return;
            }

            if (_enableTransition && !this.BoundaryCollider.isTrigger)
            {
                Debug.LogWarning($"[{nameof(WorldMapBoundaryTransitionHandler)}] An enabled transition boundary must be a trigger.", this);
            }
        }
    }
}