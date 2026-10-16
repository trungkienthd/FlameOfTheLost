using System;
using UnityEngine;

namespace FlameOfTheLost.Editor
{
    [Serializable]
    public class MapRegionData
    {
        public string Id;
        public string Name;
        public string Category;

        public int CenterX;
        public int CenterY;

        public int Width;
        public int Height;

        public bool IsMandatory;
        public bool IsHandcrafted;

        public string Description;

        public int MinX => CenterX - Width / 2;
        public int MinY => CenterY - Height / 2;

        public int MaxX => MinX + Width - 1;
        public int MaxY => MinY + Height - 1;

        public int Area => Width * Height;

        public bool Contains(int x, int y)
        {
            return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
        }
    }
}
