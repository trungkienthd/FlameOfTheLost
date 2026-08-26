using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(CraftingPanelSelectionHandler))]
    public class CraftingPanelController : PlayerInteractionBasePanelController
    {
        // --- References ---
        [Header("References")]

        //  1. Sub-behaviours
        [SerializeField] private CraftingPanelSelectionHandler _selectionHandler;
        [SerializeField] private CraftingPanelBackgroundClickClearAreaHandler _backgroundClickClearAreaHandler;
        [SerializeField] private CraftingPanelRecipeEntryListSubPanelController _recipeEntryListSubPanelController;
        [SerializeField] private CraftingPanelSelectedRecipeEntryDetailsDisplayController _selectedRecipeEntryDetailsDisplayController;
        private CraftingPanelSelectionHandler SelectionHandler => _selectionHandler;
        private CraftingPanelBackgroundClickClearAreaHandler BackgroundClickClearAreaHandler => _backgroundClickClearAreaHandler;
        private CraftingPanelRecipeEntryListSubPanelController RecipeEntryListSubPanelController => _recipeEntryListSubPanelController;
        private CraftingPanelSelectedRecipeEntryDetailsDisplayController SelectedRecipeEntryDetailsDisplayController => _selectedRecipeEntryDetailsDisplayController;

        //  2. Player-related
        private PlayerCraftingController _playerCraftingController;
        public PlayerCraftingController PlayerCraftingController => _playerCraftingController;

        public InventoryController InventoryController => this.PlayerCraftingController != null ? this.PlayerCraftingController.InventoryController : null;

        // --- Runtime ---
        public CraftingRecipeProfile CurrentSelectedRecipe  => this.SelectionHandler != null ? this.SelectionHandler.CurrentSelectedRecipe  : null;
        public int CurrentCraftCount                        => this.SelectionHandler != null ? this.SelectionHandler.CurrentCraftCount      : 1;

        // --- Public APIs ---
        //  0. PlayerInteractionBasePanelController Overrides
        public void Open(PlayerCraftingController playerCraftingController)
        {
            if (playerCraftingController == null) return;
            if (!playerCraftingController.IsCrafting) return;

            _playerCraftingController = playerCraftingController;

            this.ClearSelection();

            base.Open();

            if (this.RecipeEntryListSubPanelController != null)
            {
                this.RecipeEntryListSubPanelController.ResetPagination();
                this.RecipeEntryListSubPanelController.RefreshRecipes();
            }

            this.SelectFirstAvailableRecipe();
        }
        public override void Close()
        {
            this.ClearSelection();

            if (this.RecipeEntryListSubPanelController != null)
            {
                this.RecipeEntryListSubPanelController.Clear();
            }

            _playerCraftingController = null;

            base.Close();
        }

        //  1. Selection
        public void SelectRecipe(CraftingRecipeProfile recipe)
        {
            this.SelectionHandler?.Select(recipe);
        }
        public void ClearSelection()
        {
            this.SelectionHandler?.Clear();
        }
        public void IncreaseCraftCount()
        {
            this.SelectionHandler?.IncreaseCraftCount();
        }
        public void DecreaseCraftCount()
        {
            this.SelectionHandler?.DecreaseCraftCount();
        }

        //  2. Crafting
        public bool TryCraftSelectedRecipe()
        {
            if (this.PlayerCraftingController == null) return false;
            if (this.CurrentSelectedRecipe == null) return false;

            bool crafted = this.PlayerCraftingController.TryCraft(this.CurrentSelectedRecipe, this.CurrentCraftCount);

            this.RefreshAfterCraftingAttempt();

            return crafted;
        }
        public CraftingEvaluationRecord EvaluateCurrentCrafting()
        {
            if (this.PlayerCraftingController == null)
            {
                return CraftingEvaluationRecord.Failure(CraftingFailureReason.CraftingUnavailable);
            }

            return this.PlayerCraftingController.EvaluateCrafting(this.CurrentSelectedRecipe, this.CurrentCraftCount);
        }

        // --- Internal APIs ---
        //  1. Recipe Collection
        internal IReadOnlyList<CraftingRecipeProfile> GetAvailableRecipes()
        {
            if (this.PlayerCraftingController == null) return System.Array.Empty<CraftingRecipeProfile>();

            return this.PlayerCraftingController.GetAvailableCraftingRecipes();
        }
        internal bool ContainsRecipe(CraftingRecipeProfile recipe)
        {
            if (recipe == null) return false;

            IReadOnlyList<CraftingRecipeProfile> recipes = this.GetAvailableRecipes();

            for (int i = 0; i < recipes.Count; i++)
            {
                if (UnityEngine.Object.ReferenceEquals(recipes[i], recipe)) return true;
            }

            return false;
        }

        //  2. Selection Visuals
        internal void RefreshSelectionVisuals()
        {
            this.RecipeEntryListSubPanelController?.RefreshSelection();

            if (this.SelectedRecipeEntryDetailsDisplayController == null) return;

            if (this.CurrentSelectedRecipe != null)
            {
                this.SelectedRecipeEntryDetailsDisplayController.RefreshDisplay(this.CurrentSelectedRecipe, this.CurrentCraftCount);
            }
            else
            {
                this.SelectedRecipeEntryDetailsDisplayController.ClearDisplay();
            }
        }

        // --- Helpers ---
        private void SelectFirstAvailableRecipe()
        {
            IReadOnlyList<CraftingRecipeProfile> recipes = this.GetAvailableRecipes();

            if (recipes == null || recipes.Count <= 0)
            {
                this.ClearSelection();
                return;
            }

            for (int i = 0; i < recipes.Count; i++)
            {
                CraftingRecipeProfile recipe = recipes[i];
                if (recipe == null) continue;

                this.SelectRecipe(recipe);
                return;
            }

            this.ClearSelection();
        }

        private void RefreshAfterCraftingAttempt()
        {
            this.RecipeEntryListSubPanelController?.RefreshRecipes();

            if (this.SelectionHandler != null)
            {
                this.SelectionHandler.ValidateSelection();
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.SelectionHandler                               == null) _selectionHandler                              = this.GetComponent<CraftingPanelSelectionHandler>();
            if (this.BackgroundClickClearAreaHandler                == null) _backgroundClickClearAreaHandler               = this.GetComponentInChildren<CraftingPanelBackgroundClickClearAreaHandler>(true);
            if (this.RecipeEntryListSubPanelController              == null) _recipeEntryListSubPanelController             = this.GetComponentInChildren<CraftingPanelRecipeEntryListSubPanelController>(true);
            if (this.SelectedRecipeEntryDetailsDisplayController    == null) _selectedRecipeEntryDetailsDisplayController   = this.GetComponentInChildren<CraftingPanelSelectedRecipeEntryDetailsDisplayController>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.SelectionHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] " + $"{nameof(CraftingPanelSelectionHandler)} reference is missing.", this);
            }
            if (this.BackgroundClickClearAreaHandler == null)
            {
                Debug.LogError ($"[{this.gameObject.name}.{this.GetType().Name}] " + $"{nameof(CraftingPanelBackgroundClickClearAreaHandler)} reference is missing.", this);
            }
            if (this.RecipeEntryListSubPanelController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] " + $"{nameof(CraftingPanelRecipeEntryListSubPanelController)} reference is missing.", this);
            }
            if (this.SelectedRecipeEntryDetailsDisplayController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] " + $"{nameof(CraftingPanelSelectedRecipeEntryDetailsDisplayController)} reference is missing.", this);
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.SelectionHandler?.Bind(this);
            this.BackgroundClickClearAreaHandler?.Bind(this);
            this.RecipeEntryListSubPanelController?.Bind(this);
            this.SelectedRecipeEntryDetailsDisplayController?.Bind(this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _playerCraftingController = null;
        }
        protected override void UnbindInternalReferences()
        {
            this.SelectionHandler?.Unbind(this);
            this.BackgroundClickClearAreaHandler?.Unbind(this);
            this.RecipeEntryListSubPanelController?.Unbind(this);
            this.SelectedRecipeEntryDetailsDisplayController?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
