using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerSpawnSessionHandler : PlayerSpawnerSubBehaviourBase
    {
        // --- Configs ---
        [Header("Configs - Spawn")]
        [SerializeField] private bool _spawnPlayerOnStart = true;
        private bool SpawnPlayerOnStart => _spawnPlayerOnStart;

        // --- Runtime ---
        private bool _hasEnteredGameplay;
        private bool _isSessionInitializing;
        public bool HasEnteredGameplay => _hasEnteredGameplay;
        public bool IsSessionInitializing => _isSessionInitializing;

        // --- Public APIs ---
        //  1. Session State Initialization
        public void InitializeSessionState()
        {
            _hasEnteredGameplay = false;

            _isSessionInitializing = MainMenuSessionRequest.HasPendingRequest || WorldMapTransitionRequest.HasPendingRequest;
        }

        //  2. Pregameplay Initialization
        public void InitializePregameplay()
        {
            if (!this.EnsureBound()) return;

            // Continue Journey and World Map Transition restore their corresponding spawn records.
            bool isContinuingJourney = MainMenuSessionRequest.PendingJourneyType == MainMenuJourneyType.Continue;
            bool isWorldMapTransition = WorldMapTransitionRequest.HasPendingRequest;
            if (isContinuingJourney || isWorldMapTransition) return;

            if (this.PlayerSpawner.PlayerSpawnRecord.SpawnPlacementSourceType == CharacterRefreshOrigin.Checkpoint) return;

            PlayerDefaultSpawnPointController firstDefaultSpawnPoint = this.PlayerSpawner.GetFirstDefaultSpawnPoint();
            if (firstDefaultSpawnPoint == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No default spawn point found during initialization.", this);
                return;
            }

            this.PlayerSpawner.ActivateSpawnPoint(firstDefaultSpawnPoint);
        }

        //  3. Gameplay Initialization
        public void StartGameplay()
        {
            if (!this.EnsureBound()) return;

            bool isContinuingJourney = MainMenuSessionRequest.PendingJourneyType == MainMenuJourneyType.Continue;
            bool isWorldMapTransition = WorldMapTransitionRequest.HasPendingRequest;

            if (isContinuingJourney || isWorldMapTransition)
            {
                this.PlayerSpawner.PlayerInputHandler?.BlockAllPlayerInputs();
            }
            else if (this.SpawnPlayerOnStart)
            {
                this.PlayerSpawner.SpawnPlayer();
            }

            // Allow automatic saving only after initial gameplay setup.
            _hasEnteredGameplay = true;
        }

        //  4. Session Initialization Completion
        public void CompleteSessionInitialization()
        {
            if (!this.EnsureBound()) return;

            _isSessionInitializing = false;

            this.PlayerSpawner.PlayerInputHandler?.UnblockAllPlayerInputs();
        }
    }
}