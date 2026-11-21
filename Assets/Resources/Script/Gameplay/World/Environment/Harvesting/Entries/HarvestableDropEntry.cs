using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class HarvestableDropEntry
    {
        [SerializeField] private CollectibleObjectController _collectiblePrefab;
        public CollectibleObjectController CollectiblePrefab => _collectiblePrefab;

        [SerializeField, Min(1)] private int _minimumQuantity = 1;
        [SerializeField, Min(1)] private int _maximumQuantity = 1;
        public int MinimumQuantity => Mathf.Max(1, _minimumQuantity);
        public int MaximumQuantity => Mathf.Max(this.MinimumQuantity, _maximumQuantity);

        // --- Public APIs ---
        [SerializeField, Range(0f, 1f)] private float _dropChance = 1f;
        public float DropChance => Mathf.Clamp01(_dropChance);

        public bool ShouldDrop()
        {
            return UnityEngine.Random.value < this.DropChance;
        }

        public int RollQuantity()
        {
            return UnityEngine.Random.Range(this.MinimumQuantity, this.MaximumQuantity + 1);
        }
    }
}
