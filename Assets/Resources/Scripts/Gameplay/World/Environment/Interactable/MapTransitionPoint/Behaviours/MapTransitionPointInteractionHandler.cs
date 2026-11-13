using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlameOfTheLost
{
    public class MapTransitionPointInteractionHandler : MapTransitionPointSubBehaviourBase
    {
        // --- References ---
        [Header("References - Game Save")]
        [SerializeField] private GameSaveController _gameSaveController;
        private GameSaveController GameSaveController => _gameSaveController;

        // --- Public APIs ---
        //  1. Scene Transition
        public bool TryTransition(GameObject interactor)
        {
            if (!this.EnsureBound()) return false;

            if (interactor == null) return false;
            if (!this.SceneTransitionPoint.CanInteract(interactor)) return false;

            if (this.GameSaveController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(MapTransitionPointInteractionHandler)}] {nameof(GameSaveController)} is missing.", this);
                return false;
            }

            string sourceSceneName = SceneManager.GetActiveScene().name;

            string destinationSceneName = this.SceneTransitionPoint.DestinationSceneName;

            string destinationArrivalKey = this.SceneTransitionPoint.DestinationArrivalKey;

            // a. Validate destination scene.
            if (!Application.CanStreamedLevelBeLoaded(destinationSceneName))
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(MapTransitionPointInteractionHandler)}] Destination scene '{destinationSceneName}' cannot be loaded.", this);
                return false;
            }

            if (sourceSceneName == destinationSceneName)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(MapTransitionPointInteractionHandler)}] Source and destination scenes are identical.", this);
                return false;
            }

            // b. Save Player state before leaving the current scene.
            if (!this.GameSaveController.SaveGame())
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(MapTransitionPointInteractionHandler)}] Failed to save Player state before scene transition.", this);
                return false;
            }

            // c. Register the pending scene transition.
            if (!MapTransitionPointRequest.TryCreateRequest(sourceSceneName, destinationSceneName, destinationArrivalKey))
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(MapTransitionPointInteractionHandler)}] Failed to create the scene transition request.", this);
                return false;
            }

            // d. Load destination scene.
            try
            {
                SceneManager.LoadScene(destinationSceneName);

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);

                MapTransitionPointRequest.ClearRequest();

                return false;
            }
        }

            // --- FOTLMonoBehaviour Overrides ---
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
                Debug.LogError($"[{this.gameObject.name}.{nameof(MapTransitionPointInteractionHandler)}] {nameof(GameSaveController)} reference is missing.", this);
            }
        }
    }
}
