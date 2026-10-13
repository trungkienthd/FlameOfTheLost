using System;
using System.Collections;
using UnityEngine;

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
        private GameConditionController GameConditionController => _gameConditionController;
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

            GameSaveData saveData = this.CaptureSaveData();

            bool success = this.Repository.Save(saveData);
            if (!success) return false;
            
            this.GameSaved?.Invoke();

            return true;
        }

        //  2. Loading
        public bool LoadGame()
        {
            if (this.Repository == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(GameSaveController)}] {nameof(this.Repository)} is not initialized.", this);
                return false;
            }
            if (!this.Repository.TryLoad(out GameSaveData saveData)) return false;
            if (!this.ValidateSaveData(saveData)) return false;

            if (!this.CanRestore())
            {
                Debug.LogError($"[{nameof(GameSaveController)}] Cannot restore saved game because required systems are missing.", this);
                return false;
            }

            this.RestoreSaveData(saveData);

            this.GameLoaded?.Invoke();

            return true;
        }

        //  3. Save File Queries
        public bool HasSaveGame()
        {
            return this.Repository.HasSave();
        }

        //  3. Save File Deletion
        public void DeleteSaveGame()
        {
            this.Repository.DeleteSave();
        }

        // --- Helpers ---
        //  1. Capture
        private GameSaveData CaptureSaveData()
        {
            GameSaveData saveData = new();

            // a. Game Condition
            saveData.GameCondition = this.GameConditionController?.CaptureSaveData();

            // b. Spawn
            saveData.PlayerSpawn = this.PlayerSpawnerController?.CaptureSaveData() ?? new PlayerSpawnSaveData();

            // c. Persistent Player systems
            saveData.Player.Progression     = this.Player.ProgressionController?.CaptureSaveData();
            saveData.Player.Reward          = this.Player.RewardWalletController?.CaptureSaveData();
            saveData.Player.Inventory       = this.Player.InventoryController?.CaptureSaveData();
            saveData.Player.CombatLoadout   = this.Player.CombatLoadoutController?.CaptureSaveData();
            saveData.Player.CarryLoadout    = this.Player.PlayerCarryLoadoutController?.CaptureSaveData();

            // d. Player's world location
            saveData.WorldLocation = new PlayerWorldLocationSaveData
            {
                SceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                Position = this.Player.transform.position
            };

            return saveData;
        }

        //  2. Restore
        private void RestoreSaveData(GameSaveData saveData)
        {
            if (saveData == null) return;

            // a. Game Condition
            this.GameConditionController?.RestoreSaveData(saveData.GameCondition);

            // b. Spawn
            this.PlayerSpawnerController?.RestoreSaveData(saveData.PlayerSpawn);

            PlayerSaveData playerData = saveData.Player ?? new PlayerSaveData();

            // c. Persistent Player systems
            this.Player.ProgressionController?.RestoreSaveData(playerData.Progression);

            //  c.1. Reward
            this.Player.RewardWalletController?.RestoreSaveData(playerData.Reward);

            //  c.2. Inventory (MUST be before Loadouts)
            this.Player.InventoryController?.RestoreSaveData(playerData.Inventory);

            //  c.3. Combat Loadout
            this.Player.CombatLoadoutController?.RestoreSaveData(playerData.CombatLoadout);

            //  c.4. Carry Loadout
            this.Player.PlayerCarryLoadoutController?.RestoreSaveData(playerData.CarryLoadout);

            // d. Respawn + transient Character refresh
            this.PlayerSpawnerController?.ReloadPlayerFromSave();

            // e. Reset ordinary world content
            this.WorldRefreshController?.RefreshForSaveReload();
        }

        //  3. Validation
        private bool CanSave()
        {
            if (this.Repository == null) return false;
            if (this.GameConditionController == null) return false;
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
            if (this.GameConditionController == null) return false;
            if (this.PlayerSpawnerController == null) return false;
            if (this.WorldRefreshController == null) return false;
            if (this.Player == null) return false;

            if (this.Player.ProgressionController == null) return false;
            if (this.Player.RewardWalletController == null) return false;
            if (this.Player.InventoryController == null) return false;
            if (this.Player.CombatLoadoutController == null) return false;
            if (this.Player.PlayerCarryLoadoutController == null) return false;

            return true;
        }

        //  4. Gameplay Session Initialization
        private IEnumerator InitializeGameplaySessionRoutine()
        {
            MainMenuJourneyType journeyType = MainMenuSessionRequest.PendingJourneyType;
            if (journeyType == MainMenuJourneyType.None) yield break;

            Debug.Log($"[{nameof(GameSaveController)}] Initializing gameplay session: {journeyType}", this);

            // Wait until the scene's initial Start callbacks have completed.
            yield return null;

            bool success = false;

            switch (journeyType)
            {
                case MainMenuJourneyType.New:
                    success = this.InitializeNewJourney();
                    break;

                case MainMenuJourneyType.Continue:
                    success = this.InitializeContinuedJourney();
                    break;

                default:
                    Debug.LogError($"[{nameof(GameSaveController)}] Unsupported journey type: {journeyType}", this);
                    break;
            }

            if (success)
            {
                this.PlayerSpawnerController?.CompleteSessionInitialization();

                Debug.Log($"[{nameof(GameSaveController)}] Gameplay session initialized successfully: {journeyType}", this);
            }
            else
            {
                Debug.LogError($"[{nameof(GameSaveController)}] Gameplay session initialization failed: {journeyType}", this);
            }

            MainMenuSessionRequest.ClearRequest();
        }

        private bool InitializeNewJourney()
        {
            Debug.Log($"[{nameof(GameSaveController)}] Starting a New Journey.", this);

            if (this.Repository == null)
            {
                Debug.LogError($"[{nameof(GameSaveController)}] Repository is missing.", this);
                return false;
            }

            this.Repository.DeleteSave();

            return true;
        }

        private bool InitializeContinuedJourney()
        {
            Debug.Log($"[{nameof(GameSaveController)}] Loading previous Journey.", this);

            if (!this.LoadGame())
            {
                Debug.LogError($"[{nameof(GameSaveController)}] Failed to restore the previous Journey. The existing save file will be preserved.", this);
                return false;
            }

            Debug.Log($"[{nameof(GameSaveController)}] Previous Journey restored successfully.", this);

            return true;
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

            if (this.GameConditionController == null) _gameConditionController = UnityEngine.Object.FindFirstObjectByType<GameConditionController>();
            if (this.PlayerSpawnerController == null) _playerSpawnerController = UnityEngine.Object.FindFirstObjectByType<PlayerSpawnerController>();
            if (this.Player == null) _player = UnityEngine.Object.FindFirstObjectByType<PlayerRootController>();
            if (this.WorldRefreshController == null) _worldRefreshController = UnityEngine.Object.FindFirstObjectByType<WorldRefreshController>();
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.GameConditionController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(GameSaveController)}] {nameof(this.GameConditionController)} is missing.", this);
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
