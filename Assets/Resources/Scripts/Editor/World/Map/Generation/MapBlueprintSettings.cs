using System;
using UnityEngine;

namespace FlameOfTheLost.Editor
{
    [Serializable]
    public class MapBlueprintSettings
    {
        public string MapName = "GreenvaleCrossroads";

        public int Seed = 12345;

        [Min(120)] public int Width = 200;
        [Min(110)] public int Height = 160;

        [Min(0.1f)] public float CellSize = 1f;

        [Range(0, 4)] public int RegionPositionVariation = 2;

        [Range(1, 7)] public int RoadWidth = 3;

        [Range(0f, 1f)] public float EnvironmentDensity = 0.35f;

        public bool GenerateEnvironmentPlacements = true;
    }    
}
