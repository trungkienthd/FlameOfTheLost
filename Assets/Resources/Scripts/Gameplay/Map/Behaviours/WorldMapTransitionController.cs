using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlameOfTheLost
{
    public class WorldMapTransitionController : FOTLMonoBehaviour
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private GameSaveController _gameSaveController;
        private GameSaveController GameSaveController => _gameSaveController;

        // --- Runtime ---
        private bool _isTransitioning;
        public bool IsTransitioning => _isTransitioning;

        // --- Public APIs ---
        public bool TryTransition(string destinationSceneName, WorldMapBoundaryDirection destinationBoundary, float normalizedBoundaryPosition)
        {
            if (this.IsTransitioning) return false;

            if (string.IsNullOrWhiteSpace(destinationSceneName))
            {
                Debug.LogError($"[{nameof(WorldMapTransitionController)}] Destination scene name is empty.", this);
                return false;
            }
            if (this.GameSaveController == null)
            {
                Debug.LogError($"[{nameof(WorldMapTransitionController)}] {nameof(GameSaveController)} is missing.", this);
                return false;
            }
            if (!Application.CanStreamedLevelBeLoaded(destinationSceneName))
            {
                Debug.LogError($"[{nameof(WorldMapTransitionController)}] Destination scene '{destinationSceneName}' cannot be loaded.", this);
                return false;
            }

            _isTransitioning = true;

            // Preserve the player's current supported save state.
            if (!this.GameSaveController.SaveGame())
            {
                Debug.LogError($"[{nameof(WorldMapTransitionController)}] Failed to save game before transitioning.", this);
                _isTransitioning = false;
                return false;
            }

            string sourceSceneName = SceneManager.GetActiveScene().name;

            WorldMapTransitionRequest request = new WorldMapTransitionRequest(sourceSceneName, destinationSceneName, destinationBoundary, normalizedBoundaryPosition);

            WorldMapTransitionRequest.SetRequest(request);

            SceneManager.LoadScene(destinationSceneName);

            return true;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _isTransitioning = false;
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.GameSaveController == null) _gameSaveController = UnityEngine.Object.FindFirstObjectByType<GameSaveController>();
        }
    }
}