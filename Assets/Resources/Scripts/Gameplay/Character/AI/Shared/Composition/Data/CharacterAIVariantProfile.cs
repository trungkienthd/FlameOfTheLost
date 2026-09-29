using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterAIVariantProfile : CharacterDataAsset
    {
        //   1. Character
        public abstract CharacterAIProfile CharacterProfile { get; }

        //  2. Visual
        [Header("Visual")]
        [SerializeField] private CharacterVisualConfig _visualConfig;
        public CharacterVisualConfig VisualConfig => _visualConfig;
        public CharacterVisualConfig ResolvedVisualConfig => this.VisualConfig != null
            ? this.VisualConfig
            : (this.CharacterProfile != null ? this.CharacterProfile.DefaultVisualConfig : null);

        //  3. Initial Inventory
        [Header("Initial Inventory")]
        [SerializeField] private List<InventoryItemEntry> _initialInventoryEntries = new();
        public IReadOnlyList<InventoryItemEntry> InitialInventoryEntries => _initialInventoryEntries;

        public virtual bool IsValid => CharacterProfile != null;
    }
}
