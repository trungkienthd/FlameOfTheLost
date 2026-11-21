using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class EnvironmentDoorTileEntry
    {
        //  1. Cell
        [Header("Cell")]
        [SerializeField] private Vector3Int _cellPosition;  
        public Vector3Int CellPosition => _cellPosition;

        //  2. Closed State
        [Header("Closed State")]
        [SerializeField] private TileBase _closedTile;
        [SerializeField] private EnvironmentDoorTileSorting _closedSorting = EnvironmentDoorTileSorting.Background;
        public TileBase ClosedTile => _closedTile;
        public EnvironmentDoorTileSorting ClosedSorting => _closedSorting;

        //  3. Open State
        [Header("Open State")]
        [SerializeField] private TileBase _openTile;
        [SerializeField] private EnvironmentDoorTileSorting _openSorting = EnvironmentDoorTileSorting.Background;
        public TileBase OpenTile => _openTile;
        public EnvironmentDoorTileSorting OpenSorting => _openSorting;

        // --- Public APIs ---
        public TileBase GetTile(bool isOpen)
        {
            return isOpen ? this.OpenTile : this.ClosedTile;
        }

        public EnvironmentDoorTileSorting GetSorting(bool isOpen)
        {
            return isOpen ? this.OpenSorting : this.ClosedSorting;
        }
    }
}