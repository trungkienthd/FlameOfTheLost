using UnityEngine;

namespace FlameOfTheLost
{
    public class CraftingPanelSelectionHandler : CraftingPanelSubHandlerBase
    {
        // --- Runtime ---
        internal CraftingRecipeProfile CurrentSelectedRecipe { get; private set; }

        [SerializeField] private int _currentCraftCount = 1;
        internal int CurrentCraftCount 
        { 
            get => _currentCraftCount; 
            private set => _currentCraftCount = value; 
        }

        // --- Internal APIs ---
        //  1. Select Crafting Recipe
        internal void Select(CraftingRecipeProfile recipe)
        {
            if (!this.EnsureBound()) return;

            if (recipe == null)
            {
                this.Clear();
                return;
            }

            if (!this.CraftingPanelController.ContainsRecipe(recipe))
            {
                this.Clear();
                return;
            }

            this.CurrentSelectedRecipe = recipe;
            this.CurrentCraftCount = 1;

            this.CraftingPanelController.RefreshSelectionVisuals();
        }

        //  2. Clear Selected Crafting Recipe
        internal void Clear()
        {
            this.CurrentSelectedRecipe = null;
            this.CurrentCraftCount = 1;

            if (!this.IsBound) return;

            this.CraftingPanelController.RefreshSelectionVisuals();
        }

        //  3. Validation
        internal bool ValidateSelection()
        {
            if (!this.EnsureBound()) return false;

            if (this.CurrentSelectedRecipe == null)
            {
                this.CraftingPanelController.RefreshSelectionVisuals();
                return false;
            }

            if (!this.CraftingPanelController.ContainsRecipe(this.CurrentSelectedRecipe))
            {
                this.Clear();
                return false;
            }

            this.CraftingPanelController.RefreshSelectionVisuals();

            return true;
        }

        //  4. Craft Count
        internal void IncreaseCraftCount()
        {
            if (!this.EnsureBound()) return;
            if (this.CurrentSelectedRecipe == null) return;
            if (this.CurrentCraftCount >= int.MaxValue) return;

            this.CurrentCraftCount++;

            this.CraftingPanelController.RefreshSelectionVisuals();
        }
        internal void DecreaseCraftCount()
        {
            if (!this.EnsureBound()) return;
            if (this.CurrentSelectedRecipe == null) return;
            if (this.CurrentCraftCount <= 1) return;

            this.CurrentCraftCount--;

            this.CraftingPanelController.RefreshSelectionVisuals();
        }
        internal void ResetCraftCount()
        {
            this.CurrentCraftCount = 1;

            if (!this.IsBound) return;

            this.CraftingPanelController.RefreshSelectionVisuals();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.CurrentSelectedRecipe = null;
            this.CurrentCraftCount = 1;
        }
        protected override void OnUnbinding(CraftingPanelController craftingPanelController)
        {
            this.CurrentSelectedRecipe = null;
            this.CurrentCraftCount = 1;

            base.OnUnbinding(craftingPanelController);
        }
    }
}
