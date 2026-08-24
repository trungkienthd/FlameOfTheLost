using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class CraftingMaterialRequirementEntry
    {
        // --- Data / Configs ---
        [SerializeField] private MaterialProfile _material;
        [SerializeField, Min(1)] private int _quantity = 1;
        public MaterialProfile Material => _material;
        public int Quantity => Mathf.Max(1, _quantity);
    }
}
