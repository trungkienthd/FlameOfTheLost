using System;
using System.Collections.Generic;

namespace FlameOfTheLost.Editor
{
    [Serializable]
    public class MapConnectionData
    {
        public string FromRegionId;
        public string ToRegionId;

        public string ConnectionType;

        public bool IsOptional;

        public List<MapCoordinateData> Path = new();
    }
}