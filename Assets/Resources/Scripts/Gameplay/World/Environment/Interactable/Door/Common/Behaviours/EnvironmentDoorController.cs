using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FlameOfTheLost
{
    public class EnvironmentDoorController : FOTLWorldObjectProfileBehaviour<EnvironmentDoorProfile>
    {
        // --- Data / Configs ---
        public IReadOnlyList<EnvironmentDoorTileEntry> TileEntries => this.Profile != null
            ? this.Profile.TileEntries
            : Array.Empty<EnvironmentDoorTileEntry>();

        // --- References ---
        //  1. Tilemaps
        [Header("Tilemaps")]
        [SerializeField] private Tilemap _backgroundTilemap;
        [SerializeField] private Tilemap _foregroundTilemap;
        private Tilemap BackgroundTilemap => _backgroundTilemap;
        private Tilemap ForegroundTilemap => _foregroundTilemap;

        //  2. Blocking Colliders
        [Header("Blocking Colliders")]
        [SerializeField] private List<Collider2D> _blockingColliders = new();
        public IReadOnlyList<Collider2D> BlockingColliders => _blockingColliders;

        //  3. Gate Area
        [Header("Gate Area")]
        [SerializeField] private BoxCollider2D _gateAreaCollider;
        private BoxCollider2D GateAreaCollider => _gateAreaCollider;

        public EnvironmentDoorGateDirection GateDirection => this.Profile != null
            ? this.Profile.GateDirection
            : EnvironmentDoorGateDirection.LeftToRight;

        // --- Runtime ---
        private bool _isOpen;

        public bool IsOpen => _isOpen;
        public bool IsClosed => !_isOpen;

        // --- Events ---
        public event Action<bool> OnDoorStateChanged;

        // --- Public APIs ---
        //  1. Door State
        public void Open()
        {
            this.SetOpen(true);
        }
        public void Close()
        {
            this.SetOpen(false);
        }

        public void Toggle()
        {
            Debug.Log("TOGGLED!");
            this.SetOpen(!this.IsOpen);
        }

        public void SetOpen(bool isOpen)
        {
            if (this.Profile == null) return;
            if (this.IsOpen == isOpen) return;

            this.SetOpenInternal(isOpen, true);
        }

        //  2. Gate Position
        public bool TryGetGatePositionState(GameObject character, out EnvironmentDoorGatePositionState state)
        {
            state = default;

            if (character == null) return false;

            if (this.GateAreaCollider == null) return false;
            if (!this.GateAreaCollider.enabled) return false;
            if (!this.GateAreaCollider.gameObject.activeInHierarchy) return false;

            if (!this.GateAreaCollider.isTrigger) return false;

            Collider2D characterCollider = this.GetCharacterBodyCollider(character);
            if (characterCollider == null) return false;

            // Check overlap before determining the side.
            ColliderDistance2D distance = this.GateAreaCollider.Distance(characterCollider);
            if (!distance.isValid) return false;
            if (distance.isOverlapped || distance.distance <= 0f)
            {
                state = EnvironmentDoorGatePositionState.Inside;
                return true;
            }

            // Determine which local axis separates the two sides.
            Vector2 gateDirection = this.GetGateDirection();

            Vector2 gateCenter = this.GateAreaCollider.bounds.center;
            Vector2 characterCenter = characterCollider.bounds.center;

            Vector2 displacement = characterCenter - gateCenter;

            float signedDistance = Vector2.Dot(displacement, gateDirection);

            state = signedDistance < 0f
                ? EnvironmentDoorGatePositionState.Outside
                : EnvironmentDoorGatePositionState.OppositeSide;

            return true;
        }

        // --- Helpers ---
        //  1. Door State
        private void SetOpenInternal(bool isOpen, bool notify)
        {
            _isOpen = isOpen;

            this.RefreshDoorTiles();
            this.RefreshDoorCollision();

            this.LogDebug($"Door state changed: {(this.IsOpen ? "Open" : "Closed")}");

            if (notify) this.OnDoorStateChanged?.Invoke(this.IsOpen);
        }

        //  2. Door Tiles
        private void RefreshDoorTiles()
        {
            if (this.BackgroundTilemap == null) return;
            if (this.ForegroundTilemap == null) return;

            // Clear only cells managed by this door.
            foreach (EnvironmentDoorTileEntry entry in this.TileEntries)
            {
                if (entry == null) continue;

                Vector3Int cellPosition = entry.CellPosition;

                this.BackgroundTilemap.SetTile(cellPosition, null);
                this.ForegroundTilemap.SetTile(cellPosition, null);
            }

            // Repaint cells according to the current door state.
            foreach (EnvironmentDoorTileEntry entry in this.TileEntries)
            {
                if (entry == null) continue;

                TileBase targetTile = entry.GetTile(this.IsOpen);
                if (targetTile == null) continue;

                Tilemap targetTilemap = this.GetTargetTilemap(entry.GetSorting(this.IsOpen));
                if (targetTilemap == null) continue;

                targetTilemap.SetTile(entry.CellPosition, targetTile);
            }
        }

        private Tilemap GetTargetTilemap(EnvironmentDoorTileSorting sorting)
        {
            switch (sorting)
            {
                case EnvironmentDoorTileSorting.Background:
                    return this.BackgroundTilemap;

                case EnvironmentDoorTileSorting.Foreground:
                    return this.ForegroundTilemap;

                default:
                    return this.BackgroundTilemap;
            }
        }

        //  3. Door Collision
        private void RefreshDoorCollision()
        {
            foreach (Collider2D blockingCollider in this.BlockingColliders)
            {
                if (blockingCollider == null) continue;

                blockingCollider.isTrigger = false;
                blockingCollider.enabled = this.IsClosed;
            }
        }

        //  4. Gate Position
        private Vector2 GetGateDirection()
        {
            if (this.GateAreaCollider == null) return Vector2.zero;

            Transform gateTransform = this.GateAreaCollider.transform;

            Vector2 direction = this.GateDirection switch
            {
                EnvironmentDoorGateDirection.LeftToRight =>     gateTransform.right,
                EnvironmentDoorGateDirection.RightToLeft =>    -gateTransform.right,
                EnvironmentDoorGateDirection.BottomToTop =>     gateTransform.up,
                EnvironmentDoorGateDirection.TopToBottom =>    -gateTransform.up,

                _ => Vector2.zero
            };

            return direction.normalized;
        }

        private Collider2D GetCharacterBodyCollider(GameObject character)
        {
            Collider2D[] colliders = character.GetComponentsInChildren<Collider2D>();

            foreach (Collider2D collider in colliders)
            {
                if (collider == null) continue;
                if (!collider.enabled) continue;
                if (!collider.gameObject.activeInHierarchy) continue;
                if (collider.isTrigger) continue;

                return collider;
            }

            return null;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.BackgroundTilemap == null) Debug.LogError($"[{this.name}] Background Tilemap is missing.", this);
            if (this.ForegroundTilemap == null) Debug.LogError($"[{this.name}] Foreground Tilemap is missing.", this);

            if (this.BackgroundTilemap != null && this.BackgroundTilemap == this.ForegroundTilemap)
            {
                Debug.LogError($"[{this.name}] Background and Foreground Tilemaps must be different.", this);
            }

            if (this.BlockingColliders.Count == 0)
            {
                Debug.LogWarning($"[{this.name}] No blocking colliders configured.", this);
            }

            foreach (Collider2D blockingCollider in this.BlockingColliders)
            {
                if (blockingCollider == null)
                {
                    Debug.LogWarning($"[{this.name}] A blocking collider reference is missing.", this);
                }
            }

            if (this.GateAreaCollider == null)
            {
                Debug.LogError($"[{this.name}] Gate Area Collider is missing.", this);
                return;
            }
            if (!this.GateAreaCollider.isTrigger)   Debug.LogError($"[{this.name}] Gate Area Collider must be a trigger.", this);
            if (!this.GateAreaCollider.enabled)     Debug.LogError($"[{this.name}] Gate Area Collider must remain enabled.", this);
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.Profile == null) return;

            if (this.TileEntries.Count == 0)
            {
                Debug.LogWarning($"[{this.name}] No door tile entries configured in profile '{this.Profile.name}'.", this);
                return;
            }

            HashSet<Vector3Int> configuredCells = new();

            foreach (EnvironmentDoorTileEntry entry in this.TileEntries)
            {
                if (entry == null)
                {
                    Debug.LogWarning($"[{this.name}] A door tile entry is null.", this);
                    continue;
                }

                if (!configuredCells.Add(entry.CellPosition))
                {
                    Debug.LogError($"[{this.name}] Duplicate door cell: {entry.CellPosition}.", this);
                }
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _isOpen = this.Profile != null && this.Profile.InitiallyOpen;
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            if (this.Profile == null) return;

            this.SetOpenInternal(this.Profile.InitiallyOpen, false);
        }
        protected override void CleanupStates()
        {
            this.OnDoorStateChanged = null;

            base.CleanupStates();
        }
    }
}
