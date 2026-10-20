using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlameOfTheLost
{
    public class GameSaveController : FOTLMonoBehaviour
    {
        // --- References ---
        [Header("References - Game")]
        [SerializeField] private GameConditionController _gameConditionController;
        [SerializeField] private PlayerSpawnerController _playerSpawnerController;
        [SerializeField] private PlayerRootController _player;
        [SerializeField] private WorldRefreshController _worldRefreshController;
        private GameConditionController GameConditions
        {
            get
            {
                if (GameConditionController.Instance != null) return GameConditionController.Instance;

                return _gameConditionController;
            }
        }
        private PlayerSpawnerController PlayerSpawnerController => _playerSpawnerController;
        private PlayerRootController Player => _player;
        private WorldRefreshController WorldRefreshController => _worldRefreshController;

        // --- Runtime ---
        private GameSaveFileRepository _repository;
        private GameSaveFileRepository Repository => _repository;

        // --- Events ---
        public event Action GameSaved;
        public event Action GameLoaded;


        // --- Public APIs ---
        //  1. Saving
        public bool SaveGame()
        {
            if (!this.CanSave()) return false;

            // Load previous data to preserve records from other scenes.
            GameSaveData previousSaveData = null;
            if (this.Repository.HasSave())
            {
                if (!this.Repository.TryLoad(out previousSaveData))
                {
                    Debug.LogError($"[{nameof(GameSaveController)}] An existing save file could not be loaded. Saving was aborted to prevent data loss.", this);
                    return false;
                }

                if (!this.ValidateSaveData(previousSaveData)) return false;
            }

            GameSaveData saveData = this.CaptureSaveData(previousSaveData);

            bool success = this.Repository.Save(saveData);
            if (!success) return false;
            
            this.GameSaved?.Invoke();

            return true;
        }

        //  2. Loading
        public bool LoadGame()
        {
            if (!this.TryReadSave(out GameSaveData saveData)) return false;

            if (!this.CanRestore()) return false;

            string currentSceneName = SceneManager.GetActiveScene().name;

            // Prevent loading saved data into the wrong scene.
            if (saveData.Version >= 2 && saveData.WorldLocation != null && !string.IsNullOrEmpty(saveData.WorldLocation.SceneName) && saveData.WorldLocation.SceneName != currentSceneName)
            {
                Debug.LogError($"[{nameof(GameSaveController)}] Saved game belongs to scene '{saveData.WorldLocation.SceneName}', not '{currentSceneName}'.", this);
                return false;
            }

            if (!this.RestoreIntoCurrentScene(saveData, null)) return false;

            this.GameLoaded?.Invoke();

            return true;
        }

        //  3. Save File Queries
        public bool HasSaveGame()
        {
            return this.Repository != null && this.Repository.HasSave();
        }
        public static bool TryGetSavedSceneName(out string sceneName)
        {
            sceneName = null;

            GameSaveFileRepository repository = new GameSaveFileRepository();

            if (!repository.TryLoad(out GameSaveData saveData)) return false;

            if (saveData == null) return false;
            if (saveData.Version > GameSaveData.CurrentVersion) return false;

            sceneName = saveData.Version >= 2
                ? saveData.WorldLocation?.SceneName
                : null;

            // Compatibility with older saves.
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                sceneName = "GreenvaleBorderPath";
            }

            return Application.CanStreamedLevelBeLoaded(sceneName);
        }

        //  4. Save File Deletion
        public void DeleteSaveGame()
        {
            this.Repository?.DeleteSave();
        }

        // --- Helpers ---
        //  1. Capture
        private GameSaveData CaptureSaveData(GameSaveData previousSaveData)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            GameSaveData saveData = new GameSaveData
            {
                // a. Game Conditions
                GameCondition = this.GameConditions.CaptureSaveData(),

                // b. Player Spawn
                PlayerSpawn =
                    this.PlayerSpawnerController.CaptureSaveData(),

                // c. Preserve other scenes' spawn records
                ScenePlayerSpawns = previousSaveData?.ScenePlayerSpawns != null ? new List<PlayerSceneSpawnSaveEntry>(previousSaveData.ScenePlayerSpawns) : new List<PlayerSceneSpawnSaveEntry>(),

                // d. Player World Location
                WorldLocation = new PlayerWorldLocationSaveData
                {
                    SceneName = currentSceneName,
                    Position = this.Player.transform.position
                }
            };

            // e. Persistent Player Systems
            saveData.Player.Progression     = this.Player.ProgressionController?.CaptureSaveData();
            saveData.Player.Reward          = this.Player.RewardWalletController?.CaptureSaveData();
            saveData.Player.Inventory       = this.Player.InventoryController?.CaptureSaveData();
            saveData.Player.CombatLoadout   = this.Player.CombatLoadoutController?.CaptureSaveData();
            saveData.Player.CarryLoadout    = this.Player.PlayerCarryLoadoutController?.CaptureSaveData();

            // Migrate a previous single-scene spawn record.
            if (previousSaveData != null && previousSaveData.Version < 3 && previousSaveData.PlayerSpawn != null && previousSaveData.WorldLocation != null && !string.IsNullOrEmpty(previousSaveData.WorldLocation.SceneName))
            {
                GameSaveController.SetSceneSpawn(saveData.ScenePlayerSpawns, previousSaveData.WorldLocation.SceneName, previousSaveData.PlayerSpawn);
            }

            // Update the current scene's spawn record.
            GameSaveController.SetSceneSpawn(saveData.ScenePlayerSpawns, currentSceneName, saveData.PlayerSpawn);

            return saveData;
        }

        //  2. Scene Spawn Records
        private static void SetSceneSpawn(List<PlayerSceneSpawnSaveEntry> entries, string sceneName, PlayerSpawnSaveData spawnData)
        {
            if (entries == null) return;

            if (string.IsNullOrWhiteSpace(sceneName)) return;

            if (spawnData == null) return;

            // Remove the previous record for the same scene.
            entries.RemoveAll( entry => entry == null || entry.SceneName == sceneName);

            // Add the latest scene record.
            var spawnSaveEntry = new PlayerSceneSpawnSaveEntry
            {
                SceneName = sceneName,
                Spawn = spawnData
            };
            entries.Add(spawnSaveEntry);
        }
        private PlayerSpawnSaveData FindSceneSpawn(GameSaveData saveData, string sceneName)
        {
            if (saveData.ScenePlayerSpawns != null)
            {
                foreach (PlayerSceneSpawnSaveEntry entry in saveData.ScenePlayerSpawns)
                {
                    if (entry == null) continue;

                    if (entry.SceneName != sceneName) continue;

                    return entry.Spawn;
                }
            }

            // Compatibility with version-1 and version-2 saves.
            if (saveData.Version < 3 && saveData.PlayerSpawn != null)
            {
                bool sameScene = saveData.WorldLocation == null || string.IsNullOrEmpty(saveData.WorldLocation.SceneName) || saveData.WorldLocation.SceneName == sceneName;
                if (sameScene)
                {
                    return saveData.PlayerSpawn;
                }
            }

            return null;
        }

        //  3. Restore
        private bool RestoreIntoCurrentScene(GameSaveData saveData, WorldMapTransitionRequest transitionRequest, SceneTransitionPointRequest sceneTransitionRequest = null)
        {
            if (!this.CanRestore()) return false;

            string currentSceneName = SceneManager.GetActiveScene().name;

            // a. Restore Game Conditions
            this.GameConditions.UnbindGameplayReferences();     // Suspend gameplay observations while saved data is being restored.

            this.GameConditions.RestoreSaveData(saveData.GameCondition);

            // b. Restore current scene's spawn record
            PlayerSpawnSaveData spawnData = this.FindSceneSpawn(saveData, currentSceneName);
            if (spawnData == null)
            {
                spawnData = this.PlayerSpawnerController.CreateSceneDefaultSpawnSaveData();
            }

            this.PlayerSpawnerController.RestoreSaveData(spawnData);

            // c. Restore persistent Player systems
            PlayerSaveData playerData = saveData.Player ?? new PlayerSaveData();

            this.Player.ProgressionController.RestoreSaveData(playerData.Progression);
            this.Player.RewardWalletController.RestoreSaveData(playerData.Reward);
            this.Player.InventoryController.RestoreSaveData(playerData.Inventory);  // IMPORTANT: Inventory must be restored before loadouts.
            this.Player.CombatLoadoutController.RestoreSaveData(playerData.CombatLoadout);
            this.Player.PlayerCarryLoadoutController.RestoreSaveData(playerData.CarryLoadout);

            // d. Resolve Player placement
            if (sceneTransitionRequest != null)
            {
                // d.1. Scene Transition Point
                SceneTransitionPointController destinationPoint = GameSaveController.FindArrivalTransitionPoint(sceneTransitionRequest.DestinationArrivalKey);

                if (destinationPoint == null)
                {
                    Debug.LogError($"[{nameof(GameSaveController)}] Cannot find destination transition point '{sceneTransitionRequest.DestinationArrivalKey}' in scene '{currentSceneName}'.", this);
                    return false;
                }

                // Refresh the Player at the configured arrival position.
                bool placed = this.PlayerSpawnerController.PlacePlayerAtWorldLocation(destinationPoint.ArrivalPosition, CharacterRefreshOrigin.WorldMapTransition, destinationPoint.ArrivalFacing);

                if (!placed) return false;
            }
            else if(transitionRequest != null)
            {
                // d.1. World Map Transition
                WorldMapBoundaryTransitionHandler boundary = GameSaveController.FindArrivalBoundary(transitionRequest.DestinationBoundary);
                if (boundary == null)
                {
                    Debug.LogError($"[{nameof(GameSaveController)}] Cannot find destination boundary '{transitionRequest.DestinationBoundary}' in scene '{currentSceneName}'.", this);
                    return false;
                }

                // Obtain Player collider dimensions.
                Collider2D playerCollider = this.Player.GetComponent<Collider2D>();

                float halfWidth = playerCollider != null
                    ? playerCollider.bounds.extents.x
                    : 0.5f;
                float halfHeight = playerCollider != null
                    ? playerCollider.bounds.extents.y
                    : 0.5f;

                bool isVerticalBoundary = transitionRequest.DestinationBoundary == WorldMapBoundaryDirection.Left || transitionRequest.DestinationBoundary == WorldMapBoundaryDirection.Right;

                // Safe inward placement distance.
                float inwardOffset = (isVerticalBoundary ? halfWidth : halfHeight) + 0.15f;

                // Clearance from the ends of the boundary.
                float edgeClearance = (isVerticalBoundary ? halfHeight : halfWidth) + 0.05f;

                Bounds bounds = boundary.GetBoundaryBounds();

                float edgeLength = isVerticalBoundary
                    ? bounds.size.y
                    : bounds.size.x;

                edgeClearance = Mathf.Min(edgeClearance, Mathf.Max(0f, edgeLength * 0.49f));

                Vector3 destinationPosition = boundary.CalculateDestinationPosition(transitionRequest.NormalizedBoundaryPosition, inwardOffset, edgeClearance);

                // Refresh the Player once at the destination.
                bool placed = this.PlayerSpawnerController.PlacePlayerAtWorldLocation(destinationPosition, CharacterRefreshOrigin.WorldMapTransition);
                if (!placed) return false;
            }
            else if (saveData.Version >= 2 && saveData.WorldLocation != null && saveData.WorldLocation.SceneName == currentSceneName
            )
            {
                // d.2. Saved World Location
                bool placed = this.PlayerSpawnerController.PlacePlayerAtWorldLocation(saveData.WorldLocation.Position, CharacterRefreshOrigin.SavedWorldLocation);
                if (!placed) return false;
            }
            else
            {
                // d.3. Legacy Save Fallback
                this.PlayerSpawnerController.ReloadPlayerFromSave();
            }

            // e. Refresh ordinary world content
            this.WorldRefreshController.RefreshForSaveReload();

            // f. Rebind GameCondition gameplay dependencies.
            if (!this.GameConditions.BindGameplayReferences())
            {
                Debug.LogError($"[{nameof(GameSaveController)}] Failed to bind GameCondition gameplay references.", this);
                return false;
            }

            return true;
        }

        //  4. Lookup
        //      4.1. Destination Boundary Lookup
        private static WorldMapBoundaryTransitionHandler FindArrivalBoundary(WorldMapBoundaryDirection direction)
        {
            WorldMapBoundaryTransitionHandler[] handlers = UnityEngine.Object.FindObjectsByType<WorldMapBoundaryTransitionHandler>(FindObjectsSortMode.None);

            foreach (WorldMapBoundaryTransitionHandler handler in handlers)
            {
                if (handler == null) continue;
                if (handler.BoundaryDirection != direction) continue;

                return handler;
            }

            return null;
        }

        //      4.2. Scene Transition Point Lookup
        private static SceneTransitionPointController FindArrivalTransitionPoint(string arrivalKey)
        {
            if (string.IsNullOrWhiteSpace(arrivalKey)) return null;

            SceneTransitionPointController[] transitionPoints = UnityEngine.Object.FindObjectsByType<SceneTransitionPointController>(FindObjectsSortMode.None);

            SceneTransitionPointController result = null;

            foreach (SceneTransitionPointController transitionPoint in transitionPoints)
            {
                if (transitionPoint == null) continue;
                if (transitionPoint.ArrivalKey != arrivalKey) continue;

                if (result != null)
                {
                    Debug.LogError($"[{nameof(GameSaveController)}] Multiple scene transition points use arrival key '{arrivalKey}'.");
                    return null;
                }

                result = transitionPoint;
            }

            return result;
        }

        //  5. Repository
        private bool TryReadSave(out GameSaveData saveData)
        {
            saveData = null;

            if (this.Repository == null) return false;
            if (!this.Repository.TryLoad(out saveData)) return false;

            return this.ValidateSaveData(saveData);
        }

        //  6. Validation
        private bool CanSave()
        {
            if (this.Repository == null) return false;
            if (this.GameConditions == null) return false;
            if (this.PlayerSpawnerController == null) return false;
            if (this.Player == null) return false;

            if (this.PlayerSpawnerController.IsSessionInitializing)
            {
                Debug.LogWarning($"[{nameof(GameSaveController)}] Cannot save while the gameplay session is initializing.", this);
                return false;
            }

            return true;
        }
        private bool ValidateSaveData(GameSaveData saveData)
        {
            if (saveData == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(GameSaveController)}] Loaded save data is null.", this);
                return false;
            }

            if (saveData.Version > GameSaveData.CurrentVersion)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(GameSaveController)}] Save data version {saveData.Version} is newer than the currently supported version {GameSaveData.CurrentVersion}.", this);
                return false;
            }

            return true;
        }
        private bool CanRestore()
        {
            if (this.GameConditions == null) return false;
            if (this.PlayerSpawnerController == null) return false;
            if (this.WorldRefreshController == null) return false;
            if (this.Player == null) return false;

            if (this.Player.ProgressionController           == null) return false;
            if (this.Player.RewardWalletController          == null) return false;
            if (this.Player.InventoryController             == null) return false;
            if (this.Player.CombatLoadoutController         == null) return false;
            if (this.Player.PlayerCarryLoadoutController    == null) return false;

            return true;
        }

        //  7. Gameplay Session Initialization
        private IEnumerator InitializeGameplaySessionRoutine()
        {
            // a. Determine the requested initialization type.
            bool isWorldMapTransition = WorldMapTransitionRequest.HasPendingRequest;
            bool isSceneTransitionPoint = SceneTransitionPointRequest.HasPendingRequest;
            MainMenuJourneyType journeyType = MainMenuSessionRequest.PendingJourneyType;

            // Wait until the scene's initial Start callbacks have completed.
            yield return null;

            if (!isWorldMapTransition && !isSceneTransitionPoint && journeyType == MainMenuJourneyType.None)
            {
                if (this.GameConditions == null)
                {
                    Debug.LogError($"[{nameof(GameSaveController)}] GameConditions is missing.", this);
                    yield break;
                }

                if (!this.GameConditions.BindGameplayReferences())
                {
                    Debug.LogError($"[{nameof(GameSaveController)}] Failed to initialize GameCondition gameplay references.", this);
                }

                yield break;
            }

            string currentSceneName = SceneManager.GetActiveScene().name;

            // b. World Map Transition
            if (isWorldMapTransition)
            {
                WorldMapTransitionRequest request = WorldMapTransitionRequest.Pending;

                // b.1. Validate transition request.
                if (request == null)
                {
                    this.HandleGameplaySessionInitializationFailure("Transition request is missing.");
                    yield break;
                }
                if (request.DestinationSceneName != currentSceneName)
                {
                    this.HandleGameplaySessionInitializationFailure($"Transition request targets '{request.DestinationSceneName}', but the current scene is '{currentSceneName}'.");
                    yield break;
                }

                // b.2. Validate destination boundary BEFORE restoring.
                WorldMapBoundaryTransitionHandler destinationBoundary =
                    GameSaveController.FindArrivalBoundary(request.DestinationBoundary);

                if (destinationBoundary == null)
                {
                    this.HandleGameplaySessionInitializationFailure($"Cannot find destination boundary '{request.DestinationBoundary}' in scene '{currentSceneName}'.");
                    yield break;
                }

                // b.3. Read departure save.
                if (!this.TryReadSave(out GameSaveData transitionSaveData))
                {
                    this.HandleGameplaySessionInitializationFailure("Failed to load the saved Player state.");
                    yield break;
                }

                // b.4. Restore Player systems and initialize destination.
                bool transitionSuccess = false;

                try
                {
                    transitionSuccess = this.RestoreIntoCurrentScene(transitionSaveData, request);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                }

                if (!transitionSuccess)
                {
                    this.HandleGameplaySessionInitializationFailure("Failed to restore Player state or initialize the destination scene.");
                    yield break;
                }

                // b.5. Complete gameplay initialization.
                this.PlayerSpawnerController.CompleteSessionInitialization();

                // b.6. Clear the transition request.
                WorldMapTransitionRequest.ClearRequest();

                // b.7. Save the Player's new scene and arrival position.
                if (!this.SaveGame())
                {
                    Debug.LogError($"[{nameof(GameSaveController)}] Map transition succeeded, but saving the destination location failed.", this);
                }

                // b.8. Notify listeners.
                this.GameLoaded?.Invoke();

                Debug.Log($"[{nameof(GameSaveController)}] World map transition completed: '{request.SourceSceneName}' → '{currentSceneName}'.", this);

                yield break;
            }

            // c. Scene Transition Point
            if (isSceneTransitionPoint)
            {
                SceneTransitionPointRequest request = SceneTransitionPointRequest.Pending;
                // c.1. Validate transition request.
                if (request == null)
                {
                    this.HandleGameplaySessionInitializationFailure("Scene transition request is missing.");
                    yield break;
                }

                if (request.DestinationSceneName != currentSceneName)
                {
                    this.HandleGameplaySessionInitializationFailure($"Scene transition request targets '{request.DestinationSceneName}', but the current scene is '{currentSceneName}'.");
                    yield break;
                }

                // c.2. Validate arrival point before restoration.
                SceneTransitionPointController arrivalPoint = GameSaveController.FindArrivalTransitionPoint(request.DestinationArrivalKey);
                if (arrivalPoint == null)
                {
                    this.HandleGameplaySessionInitializationFailure($"Cannot find arrival point '{request.DestinationArrivalKey}' in scene '{currentSceneName}'.");
                    yield break;
                }

                // c.3. Read departure save.
                if (!this.TryReadSave(out GameSaveData transitionSaveData))
                {
                    this.HandleGameplaySessionInitializationFailure("Failed to load the saved Player state.");
                    yield break;
                }

                // c.4. Restore Player systems and initialize destination.
                bool transitionSuccess = false;

                try
                {
                    transitionSuccess = this.RestoreIntoCurrentScene(transitionSaveData, null, request);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                }

                if (!transitionSuccess)
                {
                    this.HandleGameplaySessionInitializationFailure("Failed to restore Player state or initialize destination scene.");
                    yield break;
                }

                // c.5. Complete gameplay initialization.
                this.PlayerSpawnerController.CompleteSessionInitialization();

                // c.6. Clear the transition request.
                SceneTransitionPointRequest.ClearRequest();

                // c.7. Save Player's new scene and arrival position.
                if (!this.SaveGame())
                {
                    Debug.LogError($"[{nameof(GameSaveController)}] Scene transition succeeded, but saving arrival location failed.", this);
                }

                // c.8. Notify listeners.
                this.GameLoaded?.Invoke();

                Debug.Log($"[{nameof(GameSaveController)}] Scene transition completed: '{request.SourceSceneName}' → '{currentSceneName}'.", this);

                yield break;
            }

            // d. Continue Journey
            if (journeyType == MainMenuJourneyType.Continue)
            {
                // c.1. Read saved game.
                if (!this.TryReadSave(out GameSaveData saveData))
                {
                    this.HandleGameplaySessionInitializationFailure("Failed to read the saved Journey.");
                    yield break;
                }

                // c.2. Resolve saved scene.
                string savedSceneName = saveData.Version >= 2
                    ? saveData.WorldLocation?.SceneName
                    : null;
                if (string.IsNullOrWhiteSpace(savedSceneName))
                {
                    savedSceneName = "GreenvaleBorderPath";
                }

                // c.3. Redirect to the saved scene when necessary.
                if (savedSceneName != currentSceneName)
                {
                    if (!Application.CanStreamedLevelBeLoaded(savedSceneName))
                    {
                        this.HandleGameplaySessionInitializationFailure($"Saved scene '{savedSceneName}' cannot be loaded.");
                        yield break;
                    }

                    Debug.Log($"[{nameof(GameSaveController)}] Redirecting Continue Journey to '{savedSceneName}'.", this);

                    // Preserve the Continue request for the destination scene.
                    try
                    {
                        SceneManager.LoadScene(savedSceneName);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception, this);

                        this.HandleGameplaySessionInitializationFailure($"Failed to load saved scene '{savedSceneName}'.");
                    }

                    yield break;
                }

                // c.4. Restore saved gameplay state.
                bool restoreSuccess = false;

                try
                {
                    restoreSuccess = this.RestoreIntoCurrentScene(saveData, null);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                }

                if (!restoreSuccess)
                {
                    this.HandleGameplaySessionInitializationFailure("Failed to restore the saved Journey.");
                    yield break;
                }

                // c.5. Notify listeners.
                this.GameLoaded?.Invoke();

                Debug.Log($"[{nameof(GameSaveController)}] Continue Journey restored successfully in '{currentSceneName}'.", this);
            }

            // d. New Journey
            else if (journeyType == MainMenuJourneyType.New)
            {
                if (this.Repository == null)
                {
                    Debug.LogError($"[{nameof(GameSaveController)}] Cannot initialize New Journey because the repository is missing.", this);
                    MainMenuSessionRequest.ClearRequest();
                    yield break;
                }

                // d.1. Delete previous saved journey.
                this.Repository.DeleteSave();

                // d.2. Reset GameConditions.
                this.GameConditions.UnbindGameplayReferences();
                this.GameConditions.ResetConditions();

                // d.3. Bind current gameplay dependencies.
                if (!this.GameConditions.BindGameplayReferences())
                {
                    this.HandleGameplaySessionInitializationFailure("Failed to initialize GameCondition gameplay references for New Journey.");
                    yield break;
                }

                Debug.Log($"[{nameof(GameSaveController)}] New Journey initialized in '{currentSceneName}'.", this);
            }

            // e. Unsupported Journey Type
            else
            {
                Debug.LogError($"[{nameof(GameSaveController)}] Unsupported journey type: {journeyType}", this);
                MainMenuSessionRequest.ClearRequest();
                yield break;
            }

            // f. Complete session initialization
            this.PlayerSpawnerController.CompleteSessionInitialization();

            // g. Consume Main Menu request
            MainMenuSessionRequest.ClearRequest();
        }

        //  8. World Map Transition Failure
        private void HandleGameplaySessionInitializationFailure(string reason)
        {
            Debug.LogError($"[{nameof(GameSaveController)}] Gameplay session initialization failed: {reason}", this);

            WorldMapTransitionRequest.ClearRequest();
            SceneTransitionPointRequest.ClearRequest();
            MainMenuSessionRequest.ClearRequest();

            const string mainMenuSceneName = "MainMenu";

            if (!Application.CanStreamedLevelBeLoaded(mainMenuSceneName))
            {
                Debug.LogError($"[{nameof(GameSaveController)}] Cannot return to Main Menu because '{mainMenuSceneName}' is unavailable.", this);
                return;
            }

            try
            {
                SceneManager.LoadScene(mainMenuSceneName);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
        }

        // --- FOTLMonoBehaviour ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _repository = new GameSaveFileRepository();
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.GameConditions == null) _gameConditionController = GameConditionController.Instance ?? UnityEngine.Object.FindFirstObjectByType<GameConditionController>();
            if (this.PlayerSpawnerController == null) _playerSpawnerController = UnityEngine.Object.FindFirstObjectByType<PlayerSpawnerController>();
            if (this.Player == null) _player = UnityEngine.Object.FindFirstObjectByType<PlayerRootController>();
            if (this.WorldRefreshController == null) _worldRefreshController = UnityEngine.Object.FindFirstObjectByType<WorldRefreshController>();
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.GameConditions == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(GameSaveController)}] {nameof(this.GameConditions)} is missing.", this);
            }

            if (this.PlayerSpawnerController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(GameSaveController)}] {nameof(this.PlayerSpawnerController)} is missing.", this);
            }

            if (this.Player == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(GameSaveController)}] {nameof(this.Player)} is missing.", this);
            }
        }
        protected override void InitializeGameplay()
        {
            base.InitializeGameplay();

            this.StartCoroutine(this.InitializeGameplaySessionRoutine());
        }
    }
}
