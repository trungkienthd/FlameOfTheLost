using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FlameOfTheLost
{
    public class EnvironmentDoorController : FOTLMonoBehaviour
    {
        // --- Data / Configs ---
        [Header("Initial State")]
        [SerializeField] private bool _initiallyOpen = false;

        [Header("Door Tiles")]
        [SerializeField] private List<EnvironmentDoorTileEntry> _tileEntries = new();
        public IReadOnlyList<EnvironmentDoorTileEntry> TileEntries => _tileEntries;

        // --- References ---
        [Header("Tilemaps")]
        [SerializeField] private Tilemap _backgroundTilemap;
        [SerializeField] private Tilemap _foregroundTilemap;
        private Tilemap BackgroundTilemap => _backgroundTilemap;
        private Tilemap ForegroundTilemap => _foregroundTilemap;

        [Header("Blocking Colliders")]
        [SerializeField] private List<Collider2D> _blockingColliders = new();
        public IReadOnlyList<Collider2D> BlockingColliders => _blockingColliders;

        // --- Runtime ---
        private bool _isOpen;

        public bool IsOpen => _isOpen;
        public bool IsClosed => !_isOpen;

        // --- Events ---
        public event Action<bool> OnDoorStateChanged;

        // --- Public APIs ---
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
            this.SetOpen(!this.IsOpen);
        }

        public void SetOpen(bool isOpen)
        {
            if (this.IsOpen == isOpen) return;

            this.SetOpenInternal(isOpen, true);
        }

        // --- Helpers ---
        //  1. Door state
        private void SetOpenInternal(bool isOpen, bool notify)
        {
            _isOpen = isOpen;

            this.RefreshDoorTiles();
            this.RefreshDoorCollision();

            this.LogDebug($"Door state changed: {(this.IsOpen ? "Open" : "Closed")}");

            if (notify)
            {
                this.OnDoorStateChanged?.Invoke(this.IsOpen);
            }
        }

        //  2. Door tiles
        private void RefreshDoorTiles()
        {
            if (this.BackgroundTilemap == null) return;
            if (this.ForegroundTilemap == null) return;

            // Clear only the cells managed by this door.
            foreach (EnvironmentDoorTileEntry entry in this.TileEntries)
            {
                if (entry == null) continue;

                Vector3Int cellPosition = entry.CellPosition;

                this.BackgroundTilemap.SetTile(cellPosition, null);
                this.ForegroundTilemap.SetTile(cellPosition, null);
            }

            // Repaint the tiles for the current state.
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

        //  3. Door collision
        private void RefreshDoorCollision()
        {
            foreach (Collider2D blockingCollider in this.BlockingColliders)
            {
                if (blockingCollider == null) continue;

                blockingCollider.isTrigger = false;
                blockingCollider.enabled = this.IsClosed;
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.BackgroundTilemap == null) Debug.LogError($"[{this.name}] Background Tilemap is missing.", this);
            if (this.ForegroundTilemap == null) Debug.LogError($"[{this.name}] Foreground Tilemap is missing.", this);

            if (this.BackgroundTilemap != null && this.BackgroundTilemap == this.ForegroundTilemap)
            {
                Debug.LogError($"[{this.name}] Background and Foreground " + "Tilemaps must be different.", this);
            }

            if (this.TileEntries.Count == 0)
            {
                Debug.LogWarning($"[{this.name}] No door tile entries configured.", this);
            }

            if (this.BlockingColliders.Count == 0)
            {
                Debug.LogWarning($"[{this.name}] No blocking colliders configured.", this);
            }

            HashSet<Vector3Int> configuredCells = new();
            foreach (EnvironmentDoorTileEntry entry in this.TileEntries)
            {
                if (entry == null) continue;

                if (!configuredCells.Add(entry.CellPosition))
                {
                    Debug.LogError($"[{this.name}] Duplicate door cell: {entry.CellPosition}.", this);
                }
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _isOpen = _initiallyOpen;
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            // Apply the default state when the scene starts.
            this.SetOpenInternal(_initiallyOpen, false);
        }
        protected override void CleanupStates()
        {
            base.CleanupStates();

            this.OnDoorStateChanged = null;
        }
    }
}
