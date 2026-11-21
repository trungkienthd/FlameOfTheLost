using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "CrfRcp_New", menuName = "Data/FlameOfTheLost/Character/Player/Crafting/CraftingRecipeProfile")]
    public class CraftingRecipeProfile : FOTLScriptableObjectProfile
    {
        // --- Data / Configs ---
        //  1. Result
        [Header("Result")]
        [SerializeField] private ItemProfile _resultItem;
        [SerializeField, Min(1)] private int _resultQuantity = 1;
        public ItemProfile ResultItem => _resultItem;
        public int ResultQuantity => Mathf.Max(1, _resultQuantity);

        //  2. Material Requirements
        [Header("Material Requirements")]
        [SerializeField] private List<CraftingMaterialRequirementEntry> _materialRequirements = new();
        public IReadOnlyList<CraftingMaterialRequirementEntry> MaterialRequirements => _materialRequirements;

        //  3. Lost Embers Cost
        [Header("Lost Embers Cost")]
        [SerializeField, Min(0)] private int _lostEmbersCost;
        public int LostEmbersCost => Mathf.Max(0, _lostEmbersCost);
    }
}
