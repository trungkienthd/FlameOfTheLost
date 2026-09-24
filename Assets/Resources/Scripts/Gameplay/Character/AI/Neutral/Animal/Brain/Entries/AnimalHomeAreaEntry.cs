using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public struct AnimalHomeAreaEntry
    {
        [SerializeField] private Vector2 _bottomLeftPosition;
        [SerializeField] private Vector2 _topRightPosition;
        [SerializeField] private float _returnReachDistance;
        public Vector2 BottomLeftPosition => _bottomLeftPosition;
        public Vector2 TopRightPosition => _topRightPosition;
        public float ReturnReachDistance => _returnReachDistance;

        public AnimalHomeAreaEntry(Vector2 bottomLeftPosition, Vector2 topRightPosition, float returnReachDistance)
        {
            _bottomLeftPosition = bottomLeftPosition;
            _topRightPosition = topRightPosition;
            _returnReachDistance = returnReachDistance;
        }

        public bool HasValidArea()
        {
            Vector2 min = this.GetMin();
            Vector2 max = this.GetMax();

            return Mathf.Abs(max.x - min.x) > 0.01f && Mathf.Abs(max.y - min.y) > 0.01f;
        }

        public Vector2 GetMin()
        {
            return new Vector2
            (
                Mathf.Min(this.BottomLeftPosition.x, this.TopRightPosition.x),
                Mathf.Min(this.BottomLeftPosition.y, this.TopRightPosition.y)
            );
        }

        public Vector2 GetMax()
        {
            return new Vector2
            (
                Mathf.Max(this.BottomLeftPosition.x, this.TopRightPosition.x),
                Mathf.Max(this.BottomLeftPosition.y, this.TopRightPosition.y)
            );
        }

        public bool Contains(Vector2 position)
        {
            Vector2 min = this.GetMin();
            Vector2 max = this.GetMax();

            return position.x >= min.x &&
                   position.x <= max.x &&
                   position.y >= min.y &&
                   position.y <= max.y;
        }

        public Vector2 GetRandomPointInside()
        {
            Vector2 min = this.GetMin();
            Vector2 max = this.GetMax();

            return new Vector2
            (
                UnityEngine.Random.Range(min.x, max.x),
                UnityEngine.Random.Range(min.y, max.y)
            );
        }
    }
}