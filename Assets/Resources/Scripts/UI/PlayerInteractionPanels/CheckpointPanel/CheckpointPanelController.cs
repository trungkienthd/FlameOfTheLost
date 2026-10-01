using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlameOfTheLost
{
    public class CheckpointPanelController : PlayerInteractionBasePanelController
    {
        // --- References ---
        [Header("References")]

        //  1. UI
        [SerializeField] private TMP_Text _mapNameText;
        private TMP_Text MapNameText => _mapNameText;

        // --- Runtime ---
        //  1. Active Checkpoint / Player
        public CheckpointController CurrentCheckpointController { get; private set; }
        public PlayerRootController PlayerRootController { get; private set; }

        //  2. Resting
        public bool IsResting => this.IsOpen;

        // --- Public APIs ---
        //  0. PlayerInteractionBasePanelController Overrides
        public void Open(CheckpointController checkpointController, PlayerRootController playerRootController)
        {
            if (checkpointController == null) return;
            if (playerRootController == null) return;

            base.Open();

            this.CurrentCheckpointController = checkpointController;
            this.PlayerRootController = playerRootController;

            this.RefreshMapName();
        }
        public override void Close()
        {
            this.CurrentCheckpointController = null;
            this.PlayerRootController = null;

            base.Close();
        }

        // --- Helpers ---
        //  1. Map Name
        private void RefreshMapName()
        {
            if (this.MapNameText == null) return;

            Scene activeScene = SceneManager.GetActiveScene();

            this.MapNameText.text = activeScene.name;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.MapNameText == null) _mapNameText = this.GetComponentInChildren<TMP_Text>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.MapNameText == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(TMP_Text)} reference for Map Name is missing.", this);
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.CurrentCheckpointController = null;
            this.PlayerRootController = null;
        }
    }
}
