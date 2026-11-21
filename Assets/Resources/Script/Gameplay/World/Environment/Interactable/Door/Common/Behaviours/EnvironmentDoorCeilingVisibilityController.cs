
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FlameOfTheLost
{
    public class EnvironmentDoorCeilingVisibilityController : EnvironmentDoorSubBehaviourBase
    {
        // --- Data / Configs ---
        //  1. Player Detection
        [Header("Player Detection")]
        [SerializeField] private string _playerTag = "Player";

        [SerializeField, Min(0.1f)] private float _playerSearchInterval = 0.5f;

        // --- References ---
        //  1. Ceiling
        [Header("Ceiling")]
        [SerializeField] private TilemapRenderer _ceilingTilemapRenderer;
        private TilemapRenderer CeilingTilemapRenderer => _ceilingTilemapRenderer;

        //  2. Player
        private GameObject _player;
        private GameObject Player => _player;

        // --- Runtime ---
        private bool _isCeilingVisible = true;
        public bool IsCeilingVisible => _isCeilingVisible;

        private float _nextPlayerSearchTime;
        private bool _playerTagErrorLogged;

        // --- Unity Callbacks ---
        private void LateUpdate()
        {
            if (!this.IsBound) return;

            this.TryResolvePlayer();
            this.RefreshCeilingVisibility();
        }

        private void OnDisable()
        {
            this.ResetCeilingVisibility();
        }

        // --- Public APIs ---
        //  1. Player
        public void SetPlayer(GameObject player)
        {
            _player = player;

            this.RefreshCeilingVisibility();
        }

        //  2. Ceiling Visibility
        public void RefreshCeilingVisibility()
        {
            if (!this.IsBound) return;
            if (!this.isActiveAndEnabled) return;
            if (this.CeilingTilemapRenderer == null) return;

            bool shouldBeVisible = true;

            if (this.DoorController.isActiveAndEnabled && this.Player != null && this.Player.activeInHierarchy)
            {
                bool positionResolved = this.DoorController.TryGetGatePositionState(this.Player, out EnvironmentDoorGatePositionState positionState);
                if (positionResolved)
                {
                    shouldBeVisible = positionState != EnvironmentDoorGatePositionState.OppositeSide;
                }
            }

            this.SetCeilingVisible(shouldBeVisible);
        }

        // --- Helpers ---
        //  1. Player Detection
        private void TryResolvePlayer()
        {
            if (this.Player != null) return;
            if (string.IsNullOrWhiteSpace(_playerTag)) return;

            if (Time.unscaledTime < _nextPlayerSearchTime) return;

            _nextPlayerSearchTime = Time.unscaledTime + _playerSearchInterval;

            try
            {
                _player = GameObject.FindGameObjectWithTag(_playerTag);
            }
            catch (UnityException exception)
            {
                if (_playerTagErrorLogged) return;

                _playerTagErrorLogged = true;

                Debug.LogError($"[{this.name}] Player lookup failed for tag '{_playerTag}': {exception.Message}", this);
            }
        }

        //  2. Ceiling Visibility
        private void SetCeilingVisible(bool visible)
        {
            if (this.CeilingTilemapRenderer == null) return;

            if (_isCeilingVisible == visible && this.CeilingTilemapRenderer.enabled == visible)
            {
                return;
            }

            _isCeilingVisible = visible;

            this.CeilingTilemapRenderer.enabled = visible;

            this.LogDebug($"Ceiling visibility changed: {(visible ? "Visible" : "Hidden")}.");
        }

        //  3. State Reset
        private void ResetCeilingVisibility()
        {
            this.SetCeilingVisible(true);
        }

        // --- FOTLSubBehaviour Overrides ---
        protected override void OnBound(EnvironmentDoorController brainBehaviour)
        {
            base.OnBound(brainBehaviour);

            _nextPlayerSearchTime = 0f;
            _playerTagErrorLogged = false;

            this.RefreshCeilingVisibility();
        }
        protected override void OnUnbinding(EnvironmentDoorController brainBehaviour)
        {
            this.ResetCeilingVisibility();

            base.OnUnbinding(brainBehaviour);
        }
        protected override void OnUnbound(EnvironmentDoorController brainBehaviour)
        {
            _player = null;
            _nextPlayerSearchTime = 0f;

            base.OnUnbound(brainBehaviour);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.CeilingTilemapRenderer == null)
            {
                Debug.LogError($"[{this.name}] Ceiling Tilemap Renderer is missing.", this);
            }

            if (string.IsNullOrWhiteSpace(_playerTag))
            {
                Debug.LogWarning($"[{this.name}] Player Tag is empty. A Player reference must be assigned through SetPlayer().", this);
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _isCeilingVisible = this.CeilingTilemapRenderer == null || this.CeilingTilemapRenderer.enabled;

            _nextPlayerSearchTime = 0f;
            _playerTagErrorLogged = false;
        }
        protected override void InitializeGameplay()
        {
            base.InitializeGameplay();

            this.TryResolvePlayer();
            this.RefreshCeilingVisibility();
        }
        protected override void CleanupStates()
        {
            this.ResetCeilingVisibility();

            _player = null;

            base.CleanupStates();
        }
    }
}
