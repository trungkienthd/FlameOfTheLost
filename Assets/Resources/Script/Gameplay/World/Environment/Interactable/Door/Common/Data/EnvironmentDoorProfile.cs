using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Door_New", menuName = "Data/FlameOfTheLost/World/Environment/Interactable/Door/EnvironmentDoorProfile")]
    public class EnvironmentDoorProfile : FOTLWorldObjectProfile
    {
        // --- Data / Configs ---
        //  1. Initial State
        [Header("Initial State")]
        [SerializeField] private bool _initiallyOpen = false;
        public bool InitiallyOpen => _initiallyOpen;

        //  2. Door Tiles
        [Header("Door Tiles")]
        [SerializeField] private List<EnvironmentDoorTileEntry> _tileEntries = new();
        public IReadOnlyList<EnvironmentDoorTileEntry> TileEntries => _tileEntries;

        //  3. Gate Direction
        [Header("Gate Direction")]
        [SerializeField] private EnvironmentDoorGateDirection _gateDirection = EnvironmentDoorGateDirection.LeftToRight;
        public EnvironmentDoorGateDirection GateDirection => _gateDirection;
    }
}
