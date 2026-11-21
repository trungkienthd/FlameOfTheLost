using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public sealed class CraftingPanelCraftExecutionHandler : CraftingPanelSelectedRecipeEntryDetailsDisplaySubHandlerBase
    {        
        // --- References ---
        [Header("References")]
        [SerializeField] private Button _buttonCraft;
        private Button ButtonCraft => _buttonCraft;

        // --- Public APIs ---
        public void RefreshDisplay(CraftingRecipeProfile recipe, int craftCount)
        {
            if (this.ButtonCraft == null) return;

            if (!this.EnsureBound() || recipe == null || craftCount <= 0)
            {
                this.ButtonCraft.interactable = false;
                return;
            }

            CraftingEvaluationRecord evaluation = this.RecipeDisplayController.EvaluateCrafting(recipe, craftCount);

            this.ButtonCraft.interactable = evaluation.CanCraft;
        }
        public void ClearDisplay()
        {
            if (this.ButtonCraft != null)
            {
                this.ButtonCraft.interactable = false;
            }
        }

        // --- Helpers ---
        private void HandleCraftClicked()
        {
            if (!this.EnsureBound()) return;

            this.RecipeDisplayController.TryCraftSelectedRecipe();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.ButtonCraft == null) _buttonCraft = this.GetComponent<Button>();
        }

        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            if (this.ButtonCraft != null) this.ButtonCraft.onClick.AddListener(this.HandleCraftClicked);
        }

        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ClearDisplay();
        }

        protected override void UnbindInternalReferences()
        {
            if (this.ButtonCraft != null) this.ButtonCraft.onClick.RemoveListener(this.HandleCraftClicked);

            base.UnbindInternalReferences();
        }
    }
}
