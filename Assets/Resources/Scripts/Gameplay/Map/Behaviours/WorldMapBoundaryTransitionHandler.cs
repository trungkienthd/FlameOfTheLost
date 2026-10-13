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
        [Header("Configs - Boundary")]
        [SerializeField] private WorldMapBoundaryDirection _boundaryDirection;
        public WorldMapBoundaryDirection BoundaryDirection => _boundaryDirection;

        [Header("Configs - Destination")]
        [SerializeField] private string _destinationSceneName;
        [SerializeField] private WorldMapBoundaryDirection _destinationBoundaryDirection;
        [SerializeField] private bool _enableTransition = true;

        // --- Public APIs ---
        public Bounds GetBoundaryBounds()
        {
            return this.BoundaryCollider.bounds;
        }

        // --- Unity Events ---
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_enableTransition) return;

            if (this.TransitionController == null) return;

            PlayerRootController player = other.GetComponentInParent<PlayerRootController>();
            if (player == null) return;

            // Avoid initiating the same transition repeatedly.
            if (this.TransitionController.IsTransitioning) return;

            float normalizedPosition = this.CalculateNormalizedBoundaryPosition(player.transform.position);

            this.TransitionController.TryTransition(_destinationSceneName, _destinationBoundaryDirection, normalizedPosition);
        }

        // --- Public APIs ---
        public Vector3 CalculateDestinationPosition(float normalizedPosition, float inwardOffset)
        {
            Bounds bounds = this.BoundaryCollider.bounds;

            float x = bounds.center.x;
            float y = bounds.center.y;

            switch (this.BoundaryDirection)
            {
                case WorldMapBoundaryDirection.Left:
                    x = bounds.max.x + inwardOffset;
                    y = Mathf.Lerp(bounds.min.y, bounds.max.y, normalizedPosition);

                    break;

                case WorldMapBoundaryDirection.Right:
                    x = bounds.min.x - inwardOffset;
                    y = Mathf.Lerp(bounds.min.y, bounds.max.y, normalizedPosition);

                    break;

                case WorldMapBoundaryDirection.Top:
                    x = Mathf.Lerp(bounds.min.x, bounds.max.x, normalizedPosition);
                    y = bounds.min.y - inwardOffset;

                    break;

                case WorldMapBoundaryDirection.Bottom:
                    x = Mathf.Lerp(bounds.min.x, bounds.max.x, normalizedPosition);
                    y = bounds.max.y + inwardOffset;

                    break;
            }

            return new Vector3(x, y, 0f);
        }

        // --- Helpers ---
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

            if (this.TransitionController == null)
            {
                _transitionController = UnityEngine.Object.FindFirstObjectByType<WorldMapTransitionController>();
            }
        }

        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.TransitionController == null)
            {
                Debug.LogError($"[{nameof(WorldMapBoundaryTransitionHandler)}] {nameof(WorldMapTransitionController)} is missing.", this);
            }
        }
    }
}