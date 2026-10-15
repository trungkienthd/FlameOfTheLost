using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlameOfTheLost
{
    public class WorldMapTransitionController : FOTLMonoBehaviour
    {
        // --- References ---
        [Header("References - Game Save")]
        [SerializeField] private GameSaveController _gameSaveController;
        private GameSaveController GameSaveController => _gameSaveController;

        // --- Runtime ---
        private bool _isTransitioning;
        public bool IsTransitioning => _isTransitioning;

        // --- Public APIs ---
        public bool TryTransition(string destinationSceneName, WorldMapBoundaryDirection destinationBoundary, float normalizedBoundaryPosition)
        {
            // Prevent overlapping transitions.
            if (this.IsTransitioning) return false;

            if (WorldMapTransitionRequest.HasPendingRequest) return false;

            // Validate destination.
            if (string.IsNullOrWhiteSpace(destinationSceneName))
            {
                Debug.LogError($"[{nameof(WorldMapTransitionController)}] Destination scene name is empty.", this);
                return false;
            }
            if (!Application.CanStreamedLevelBeLoaded(destinationSceneName))
            {
                Debug.LogError($"[{nameof(WorldMapTransitionController)}] Destination scene '{destinationSceneName}' cannot be loaded.", this);
                return false;
            }
            if (this.GameSaveController == null)
            {
                Debug.LogError($"[{nameof(WorldMapTransitionController)}] {nameof(GameSaveController)} is missing.", this);
                return false;
            }
            // Preserve the player's current supported save state.
            if (!this.GameSaveController.SaveGame())
            {
                Debug.LogError($"[{nameof(WorldMapTransitionController)}] Map transition aborted because saving failed.", this);
                return false;
            }

            _isTransitioning = true;

            string sourceSceneName = SceneManager.GetActiveScene().name;

            WorldMapTransitionRequest request = new WorldMapTransitionRequest(sourceSceneName, destinationSceneName, destinationBoundary, normalizedBoundaryPosition);

            WorldMapTransitionRequest.SetRequest(request);

            // Load destination scene.
            try
            {
                SceneManager.LoadScene(destinationSceneName);

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);

                WorldMapTransitionRequest.ClearRequest();

                _isTransitioning = false;

                return false;
            }
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
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.GameSaveController == null)
            {
                Debug.LogError($"[{nameof(WorldMapTransitionController)}] {nameof(GameSaveController)} is missing.", this);
            }
        }
    }
}