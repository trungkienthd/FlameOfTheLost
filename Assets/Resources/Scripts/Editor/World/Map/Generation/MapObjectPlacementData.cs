using System;

namespace FlameOfTheLost.Editor
{
    [Serializable]
    public class MapObjectPlacementData
    {
        public string Id;
        public string Name;
        public string Category;

        public string RegionId;

        public int X;
        public int Y;

        public bool IsRequired;

        public string Notes;
    }
}