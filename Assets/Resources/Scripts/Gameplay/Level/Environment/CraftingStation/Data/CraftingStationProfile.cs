using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "CftStn_New", menuName = "Data/FlameOfTheLost/Environment/Crafting Station")]
    public class CraftingStationProfile : FOTLWorldObjectProfile
    {
        // --- Data / Configs ---
        //  1. Recipes
        [Header("Crafting")]
        [SerializeField] private List<CraftingRecipeProfile> _craftingRecipes = new();
        public IReadOnlyList<CraftingRecipeProfile> CraftingRecipes => _craftingRecipes;

        //  2. Prompt
        [Header("Prompt Settings")]
        [SerializeField] private int _interactionPromptPriority = 100;
        public int InteractionPromptPriority => _interactionPromptPriority;
    }
}