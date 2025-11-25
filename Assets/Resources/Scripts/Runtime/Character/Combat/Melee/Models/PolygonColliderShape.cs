using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public struct PolygonColliderShape
    {
        [SerializeField] private List<Vector2> _points;
        [SerializeField] private Vector2 _offset;

        public List<Vector2> Points
        {
            get => _points;
            set => _points = value;
        }
        public Vector2 Offset
        {
            get => _offset;
            set => _offset = value;
        }
    }
}
