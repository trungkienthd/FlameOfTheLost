using System;

namespace FlameOfTheLost.Editor
{
    [Serializable]
    public class MapCoordinateData
    {
        public int X;
        public int Y;

        public MapCoordinateData()
        {
        }

        public MapCoordinateData(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}