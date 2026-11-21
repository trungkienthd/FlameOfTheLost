using System.Collections.Generic;

namespace FlameOfTheLost
{
    public interface ICraftingRecipeProvider
    {
        public IReadOnlyList<CraftingRecipeProfile> GetAvailableCraftingRecipes(CharacterRootController crafter);
    }
}
