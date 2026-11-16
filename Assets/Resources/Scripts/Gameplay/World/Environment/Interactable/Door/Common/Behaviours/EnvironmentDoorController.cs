
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
