using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CraftingPanelRecipeEntryListSubPanelController : CraftingPanelSubHandlerBase
    {
        // --- References ---
        //  1. Pagination
        [Header("References (Pagination)")]
        [SerializeField] private ItemEntryListPaginationHandler _paginationHandler;
        private ItemEntryListPaginationHandler PaginationHandler => _paginationHandler;

        //  2. Recipe Entry List
        [Header("References (Recipe Entry List)")]
        [SerializeField] private RectTransform _recipeEntryListParent;
        [SerializeField] private GameObject _slotPrefab;
        private RectTransform RecipeEntryListParent => _recipeEntryListParent;
        private GameObject SlotPrefab => _slotPrefab;

        // --- Configs ---
        [SerializeField] private Vector2 _startPosition = new(-450f, 200f);
        [SerializeField] private Vector2 _step = new(150f, 150f);
        private Vector2 StartPosition => _startPosition;
        private Vector2 Step => _step;

        // --- Runtime ---
        private readonly List<CraftingRecipeEntrySlotController> _slotControllers = new();
        private readonly List<CraftingRecipeProfile> _visibleRecipes = new();
        private IReadOnlyList<CraftingRecipeEntrySlotController> SlotControllers => _slotControllers;
        private IReadOnlyList<CraftingRecipeProfile> VisibleRecipes => _visibleRecipes;

        // --- Public APIs ---
        public void ResetPagination()
        {
            if (this.PaginationHandler == null) return;

            this.PaginationHandler.SetCurrentPageWithoutNotify(1);
        }

        public void RefreshRecipes()
        {
            this.RebuildVisibleRecipes();

            bool pageChanged = false;

            if (this.PaginationHandler != null)
            {
                pageChanged = this.PaginationHandler.TrySetTotalEntryCount(this.VisibleRecipes.Count);
            }

            if (!pageChanged)
            {
                this.RefreshVisibleSlots();
            }
        }

        public void RefreshSelection()
        {
            this.RefreshVisibleSlots();
        }

        public void Clear()
        {
            _visibleRecipes.Clear();

            if (this.PaginationHandler != null)
            {
                this.PaginationHandler.SetCurrentPageWithoutNotify(1);
                this.PaginationHandler.TrySetTotalEntryCount(0);
            }

            this.RefreshVisibleSlots();
        }

        internal bool ContainsRecipe(CraftingRecipeProfile recipe)
        {
            return recipe != null && this.VisibleRecipes.Contains(recipe);
        }

        // --- Helpers ---
        private void RebuildVisibleRecipes()
        {
            _visibleRecipes.Clear();

            if (!this.EnsureBound()) return;

            IReadOnlyList<CraftingRecipeProfile> recipes = this.CraftingPanelController.GetAvailableRecipes();

            if (recipes == null) return;

            for (int i = 0; i < recipes.Count; i++)
            {
                CraftingRecipeProfile recipe = recipes[i];

                if (recipe == null) continue;
                if (recipe.ResultItem == null) continue;

                _visibleRecipes.Add(recipe);
            }
        }

        private void CreateSlots()
        {
            if (this.SlotPrefab == null) return;
            if (this.RecipeEntryListParent == null) return;
            if (this.PaginationHandler == null) return;
            if (this.SlotControllers.Count > 0) return;

            for (int i = 0; i < this.PaginationHandler.SlotsPerPage; i++)
            {
                GameObject slotObject = UnityEngine.Object.Instantiate(this.SlotPrefab, this.RecipeEntryListParent);

                CraftingRecipeEntrySlotController slotController = slotObject.GetComponent<CraftingRecipeEntrySlotController>();

                RectTransform rectTransform = slotObject.transform as RectTransform;

                int column = i % Mathf.Max(1, this.PaginationHandler.ColumnCount);
                int row = i / Mathf.Max(1, this.PaginationHandler.ColumnCount);

                float posX = this.StartPosition.x + column * this.Step.x;
                float posY = this.StartPosition.y - row * this.Step.y;

                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(posX, posY);
                }

                if (slotController == null) continue;

                slotController.OnClicked += this.HandleSlotClicked;
                slotController.ResetBindings();

                _slotControllers.Add(slotController);
            }
        }

        private void RefreshVisibleSlots()
        {
            int currentPage     = this.PaginationHandler != null ? this.PaginationHandler.CurrentPageNumber : 1;
            int slotsPerPage    = this.PaginationHandler != null ? this.PaginationHandler.SlotsPerPage      : this.SlotControllers.Count;

            int startIndex = (currentPage - 1) * slotsPerPage;

            CraftingRecipeProfile selectedRecipe = this.IsBound ? this.CraftingPanelController.CurrentSelectedRecipe : null;

            for (int i = 0; i < this.SlotControllers.Count; i++)
            {
                CraftingRecipeEntrySlotController slot = this.SlotControllers[i];

                if (slot == null) continue;

                int recipeIndex = startIndex + i;

                CraftingRecipeProfile recipe = recipeIndex >= 0 && recipeIndex < this.VisibleRecipes.Count
                    ? this.VisibleRecipes[recipeIndex]
                    : null;

                if (recipe != null)
                {
                    slot.Bind(recipe);

                    slot.SetSelected(UnityEngine.Object.ReferenceEquals(recipe, selectedRecipe));
                }
                else
                {
                    slot.ResetBindings();
                }
            }
        }

        private void HandleCurrentPageChanged(int pageNumber)
        {
            this.RefreshVisibleSlots();
        }

        private void HandleSlotClicked(CraftingRecipeEntrySlotController clickedSlot)
        {
            if (!this.EnsureBound()) return;
            if (clickedSlot == null) return;
            if (clickedSlot.BoundRecipe == null) return;

            this.CraftingPanelController.SelectRecipe(clickedSlot.BoundRecipe);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PaginationHandler == null) _paginationHandler = this.GetComponentInChildren<ItemEntryListPaginationHandler>(true);

            if (this.RecipeEntryListParent == null)
            {
                Transform recipeEntryListTransform = this.transform.Find("RecipeEntryList");
                if (recipeEntryListTransform != null) _recipeEntryListParent = recipeEntryListTransform.GetComponent<RectTransform>();
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            if (this.PaginationHandler != null)
            {
                this.PaginationHandler.OnCurrentPageChanged += this.HandleCurrentPageChanged;
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.CreateSlots();
            this.Clear();
        }
        protected override void UnbindInternalReferences()
        {
            if (this.PaginationHandler != null)
            {
                this.PaginationHandler.OnCurrentPageChanged -= this.HandleCurrentPageChanged;
            }

            for (int i = 0; i < this.SlotControllers.Count; i++)
            {
                CraftingRecipeEntrySlotController slot = this.SlotControllers[i];

                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
            }

            base.UnbindInternalReferences();
        }
    }
}