using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerSpawnAutoSaveHandler : PlayerSpawnerSubBehaviourBase
    {
        // --- Public APIs ---
        //  1. Automatic Saving
        public void TryAutoSaveAfterActivation()
        {
            if (!this.EnsureBound()) return;

            PlayerSpawnerController spawner = this.PlayerSpawner;

            // Automatic saving must not occur during initial gameplay setup, player respawning, or session initialization.
            if (!spawner.HasEnteredGameplay || spawner.IsRespawning || spawner.IsSessionInitializing) return;

            GameSaveController gameSaveController = spawner.GameSaveController;

            if (gameSaveController == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Cannot autosave: {nameof(GameSaveController)} is missing.", this);
                return;
            }

            if (!gameSaveController.SaveGame())
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Autosave failed after spawn/checkpoint activation.", this);
            }
        }
    }
}