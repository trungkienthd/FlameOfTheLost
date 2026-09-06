using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterAIVariantProfile : CharacterDataAsset
    {
        //   1. Character
        public abstract CharacterAIProfile CharacterProfile { get; }

        // 2. Initial Inventory
        [Header("Initial Inventory")]
        [SerializeField] private List<InventoryItemEntry> _initialInventoryEntries = new();
        public IReadOnlyList<InventoryItemEntry> InitialInventoryEntries => _initialInventoryEntries;

        public virtual bool IsValid => CharacterProfile != null;
    }
}
