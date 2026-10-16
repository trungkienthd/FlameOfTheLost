using System;
using System.Collections.Generic;

namespace FlameOfTheLost.Editor
{
    [Serializable]
    public class MapBlueprint
    {
        public string MapName;
        public int Seed;

        public int Width;
        public int Height;

        public float CellSize;

        public List<MapRegionData> Regions = new();
        public List<MapConnectionData> Connections = new();

        public List<MapCellData> Cells = new();
        public List<MapObjectPlacementData> Placements = new();

        public List<string> ValidationWarnings = new();
    }
}
