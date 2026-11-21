using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class PolygonColliderPathConfigurationEntry
    {
        [SerializeField] private List<Vector2> _points = new();
        public IReadOnlyList<Vector2> Points => _points;
    }
}