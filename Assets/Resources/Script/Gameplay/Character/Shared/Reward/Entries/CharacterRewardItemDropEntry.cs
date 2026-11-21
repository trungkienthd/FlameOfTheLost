using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class CharacterRewardItemDropEntry
    {
        // --- Data / Configs ---
        //  1. Source
        [Header("Source")]
        [SerializeField] private CharacterRewardItemDropSource _source = CharacterRewardItemDropSource.Configured;
        public CharacterRewardItemDropSource Source => _source;

        //  2. Collectible
        [Header("Collectible")]
        [SerializeField] private CollectibleObjectController _collectiblePrefab;
        public CollectibleObjectController CollectiblePrefab => _collectiblePrefab;

        //  3. Quantity
        [Header("Quantity")]
        [SerializeField, Min(1)] private int _minimumQuantity = 1;
        [SerializeField, Min(1)] private int _maximumQuantity = 1;
        public int MinimumQuantity => Mathf.Max(1, _minimumQuantity);
        public int MaximumQuantity => Mathf.Max(this.MinimumQuantity, _maximumQuantity);

        //  4. Drop Chance
        [Header("Drop Chance")]
        [SerializeField, Range(0f, 1f)] private float _dropChance = 1f;
        public float DropChance => Mathf.Clamp01(_dropChance);

        // --- Public APIs ---
        public bool ShouldDrop()
        {
            return UnityEngine.Random.value < this.DropChance;
        }

        public int RollQuantity()
        {
            return UnityEngine.Random.Range
            (
                this.MinimumQuantity,
                this.MaximumQuantity + 1
            );
        }
    }
}