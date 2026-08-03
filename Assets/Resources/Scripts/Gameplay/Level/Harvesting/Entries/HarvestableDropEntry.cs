using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class HarvestableDropEntry
    {
        [SerializeField] private CollectibleItemController _collectiblePrefab;
        public CollectibleItemController CollectiblePrefab => _collectiblePrefab;

        [SerializeField, Min(0)] private int _minimumSpawnCount = 1;
        [SerializeField, Min(0)] private int _maximumSpawnCount = 1;
        public int MinimumSpawnCount => Mathf.Max(0, _minimumSpawnCount);
        public int MaximumSpawnCount => Mathf.Max(this.MinimumSpawnCount, _maximumSpawnCount);

        [SerializeField, Range(0f, 1f)] private float _dropChance = 1f;
        public float DropChance => Mathf.Clamp01(_dropChance);

        public bool ShouldDrop()
        {
            return UnityEngine.Random.value < this.DropChance;
        }

        public int RollSpawnCount()
        {
            return UnityEngine.Random.Range(this.MinimumSpawnCount, this.MaximumSpawnCount + 1);
        }
    }
}
