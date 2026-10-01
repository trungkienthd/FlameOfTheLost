using UnityEngine;

namespace FlameOfTheLost
{
    public class CheckpointPanelController : PlayerInteractionBasePanelController
    {
        // --- References ---
        [Header("References")]

        //  1. Sub-Panels
        [SerializeField] private CheckpointMainSubPanelController           _mainSubPanelController;
        [SerializeField] private CheckpointUpgradePlayerSubPanelController  _upgradePlayerSubPanelController;
        [SerializeField] private CheckpointLeaveSubPanelController          _leaveSubPanelController;
        private CheckpointMainSubPanelController            MainSubPanelController          => _mainSubPanelController;
        private CheckpointUpgradePlayerSubPanelController   UpgradePlayerSubPanelController => _upgradePlayerSubPanelController;
        private CheckpointLeaveSubPanelController           LeaveSubPanelController         => _leaveSubPanelController;

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

            this.CurrentCheckpointController = checkpointController;
            this.PlayerRootController = playerRootController;

            this.ShowMainSubPanel();

            base.Open();
        }
        public override void Close()
        {
            this.ResetSubPanels();

            this.CurrentCheckpointController = null;
            this.PlayerRootController = null;

            base.Close();
        }

        //  1. Sub-Panel Navigation
        public void ShowMainSubPanel()
        {
            this.MainSubPanelController?.Show();

            this.UpgradePlayerSubPanelController?.Hide();
            this.LeaveSubPanelController?.Hide();
        }
        public void ShowUpgradePlayerSubPanel()
        {
            this.MainSubPanelController?.SetInteractionEnabled(false);

            this.UpgradePlayerSubPanelController?.Show();
            this.LeaveSubPanelController?.Hide();
        }
        public void ShowLeaveSubPanel()
        {
            this.MainSubPanelController?.SetInteractionEnabled(false);

            this.UpgradePlayerSubPanelController?.Hide();
            this.LeaveSubPanelController?.Show();
        }

        // --- Helpers ---
        //  1. Sub-Panel State
        private void ResetSubPanels()
        {
            this.MainSubPanelController?.SetInteractionEnabled(true);
            this.MainSubPanelController?.Show();

            this.UpgradePlayerSubPanelController?.Hide();
            this.LeaveSubPanelController?.Hide();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.MainSubPanelController             == null) _mainSubPanelController            = this.GetComponentInChildren<CheckpointMainSubPanelController>(true);
            if (this.UpgradePlayerSubPanelController    == null) _upgradePlayerSubPanelController   = this.GetComponentInChildren<CheckpointUpgradePlayerSubPanelController>(true);
            if (this.LeaveSubPanelController            == null) _leaveSubPanelController           = this.GetComponentInChildren<CheckpointLeaveSubPanelController>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.MainSubPanelController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(CheckpointMainSubPanelController)} reference is missing.", this);
            }
            if (this.UpgradePlayerSubPanelController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(CheckpointUpgradePlayerSubPanelController)} reference is missing.", this);
            }
            if (this.LeaveSubPanelController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(CheckpointLeaveSubPanelController)} reference is missing.", this);
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.MainSubPanelController?.Bind(this);
            this.UpgradePlayerSubPanelController?.Bind(this);
            this.LeaveSubPanelController?.Bind(this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.CurrentCheckpointController = null;
            this.PlayerRootController = null;

            this.ResetSubPanels();
        }
        protected override void UnbindInternalReferences()
        {
            this.MainSubPanelController?.Unbind(this);
            this.UpgradePlayerSubPanelController?.Unbind(this);
            this.LeaveSubPanelController?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}