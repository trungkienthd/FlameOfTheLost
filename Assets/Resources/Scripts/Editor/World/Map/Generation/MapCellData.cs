using System;

namespace FlameOfTheLost.Editor
{
    [Serializable]
    public class MapCellData
    {
        public int X;
        public int Y;

        public string RegionId;
        public string TerrainType;

        public bool IsRoad;
    }
}