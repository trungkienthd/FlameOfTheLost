using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class CraftingPanelSelectedRecipeEntryDetailsDisplayController : CraftingPanelSubHandlerBase
    {
        // --- References ---
        //  1. Sub-behaviours
        [Header("References")]
        [SerializeField] private CraftingPanelResultDisplayHandler _resultDisplayHandler;
        [SerializeField] private CraftingPanelMaterialRequirementListHandler _materialRequirementListHandler;
        [SerializeField] private CraftingPanelLostEmbersCostDisplayHandler _lostEmbersCostDisplayHandler;
        [SerializeField] private CraftingPanelCraftQuantityHandler _craftQuantityHandler;
        [SerializeField] private CraftingPanelCraftExecutionHandler _craftExecutionHandler;
        private CraftingPanelResultDisplayHandler ResultDisplayHandler => _resultDisplayHandler;
        private CraftingPanelMaterialRequirementListHandler MaterialRequirementListHandler => _materialRequirementListHandler;
        private CraftingPanelLostEmbersCostDisplayHandler LostEmbersCostDisplayHandler => _lostEmbersCostDisplayHandler;
        private CraftingPanelCraftQuantityHandler CraftQuantityHandler => _craftQuantityHandler;
        private CraftingPanelCraftExecutionHandler CraftExecutionHandler => _craftExecutionHandler;

        //  2. Internal Accessors
        internal InventoryController InventoryController => this.IsBound ? this.CraftingPanelController.InventoryController : null;

        // --- Public APIs ---
        public void RefreshDisplay(CraftingRecipeProfile recipe, int craftCount)
        {
            if (recipe == null || recipe.ResultItem == null || craftCount <= 0)
            {
                this.ClearDisplay();
                return;
            }

            this.ResultDisplayHandler?.RefreshDisplay(recipe, craftCount);
            this.MaterialRequirementListHandler?.RefreshDisplay(recipe, craftCount);
            this.LostEmbersCostDisplayHandler?.RefreshDisplay(recipe, craftCount);
            this.CraftQuantityHandler?.RefreshDisplay(craftCount);
            this.CraftExecutionHandler?.RefreshDisplay(recipe, craftCount);
        }
        public void ClearDisplay()
        {
            this.ResultDisplayHandler?.ClearDisplay();
            this.MaterialRequirementListHandler?.ClearDisplay();
            this.LostEmbersCostDisplayHandler?.ClearDisplay();
            this.CraftQuantityHandler?.ClearDisplay();
            this.CraftExecutionHandler?.ClearDisplay();
        }

        // --- Internal APIs ---
        //  1. Craft Quantity
        internal void IncreaseCraftCount()
        {
            if (!this.EnsureBound()) return;

            this.CraftingPanelController.IncreaseCraftCount();
        }
        internal void DecreaseCraftCount()
        {
            if (!this.EnsureBound()) return;

            this.CraftingPanelController.DecreaseCraftCount();
        }

        //  2. Crafting
        internal bool TryCraftSelectedRecipe()
        {
            if (!this.EnsureBound()) return false;

            return this.CraftingPanelController.TryCraftSelectedRecipe();
        }
        internal CraftingEvaluationRecord EvaluateCrafting(CraftingRecipeProfile recipe, int craftCount)
        {
            if (!this.EnsureBound()) return CraftingEvaluationRecord.Failure(CraftingFailureReason.CraftingUnavailable);

            PlayerCraftingController playerCraftingController = this.CraftingPanelController.PlayerCraftingController;
            if (playerCraftingController == null) return CraftingEvaluationRecord.Failure(CraftingFailureReason.CraftingUnavailable);

            return playerCraftingController.EvaluateCrafting(recipe, craftCount);
        }

        //  3. Quantity Scaling
        internal int GetScaledQuantity(int quantity, int craftCount)
        {
            long scaledQuantity = (long)quantity * craftCount;

            return scaledQuantity > int.MaxValue
                ? int.MaxValue
                : (int)scaledQuantity;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.ResultDisplayHandler           == null) _resultDisplayHandler              = this.GetComponentInChildren<CraftingPanelResultDisplayHandler>(true);
            if (this.MaterialRequirementListHandler == null) _materialRequirementListHandler    = this.GetComponentInChildren<CraftingPanelMaterialRequirementListHandler>(true);
            if (this.LostEmbersCostDisplayHandler   == null) _lostEmbersCostDisplayHandler      = this.GetComponentInChildren<CraftingPanelLostEmbersCostDisplayHandler>(true);
            if (this.CraftQuantityHandler           == null) _craftQuantityHandler              = this.GetComponentInChildren<CraftingPanelCraftQuantityHandler>(true);
            if (this.CraftExecutionHandler          == null) _craftExecutionHandler             = this.GetComponentInChildren<CraftingPanelCraftExecutionHandler>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.ResultDisplayHandler           == null) Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(CraftingPanelResultDisplayHandler)} reference is missing.", this);
            if (this.MaterialRequirementListHandler == null) Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(CraftingPanelMaterialRequirementListHandler)} reference is missing.", this);
            if (this.LostEmbersCostDisplayHandler   == null) Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(CraftingPanelLostEmbersCostDisplayHandler)} reference is missing.", this);
            if (this.CraftQuantityHandler           == null) Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(CraftingPanelCraftQuantityHandler)} reference is missing.", this);
            if (this.CraftExecutionHandler          == null) Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(CraftingPanelCraftExecutionHandler)} reference is missing.", this);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.ResultDisplayHandler?.Bind(this);
            this.MaterialRequirementListHandler?.Bind(this);
            this.LostEmbersCostDisplayHandler?.Bind(this);
            this.CraftQuantityHandler?.Bind(this);
            this.CraftExecutionHandler?.Bind(this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ClearDisplay();
        }
        protected override void UnbindInternalReferences()
        {
            this.ResultDisplayHandler?.Unbind(this);
            this.MaterialRequirementListHandler?.Unbind(this);
            this.LostEmbersCostDisplayHandler?.Unbind(this);
            this.CraftQuantityHandler?.Unbind(this);
            this.CraftExecutionHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
