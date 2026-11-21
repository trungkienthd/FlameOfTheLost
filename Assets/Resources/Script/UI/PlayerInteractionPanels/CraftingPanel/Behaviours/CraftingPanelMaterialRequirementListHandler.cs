using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CraftingPanelMaterialRequirementListHandler : CraftingPanelSelectedRecipeEntryDetailsDisplaySubHandlerBase
    {
        // --- Constants ---
        private const int MAX_MATERIAL_SLOT_COUNT = 3;

        // --- References ---
        [Header("References")]
        [SerializeField]
        private CraftingMaterialRequirementSlotController[] _materialSlots = new CraftingMaterialRequirementSlotController[CraftingPanelMaterialRequirementListHandler.MAX_MATERIAL_SLOT_COUNT];
        private CraftingMaterialRequirementSlotController[] MaterialSlots => _materialSlots;

        // --- Public APIs ---
        public void RefreshDisplay(CraftingRecipeProfile recipe, int craftCount)
        {
            this.ClearDisplay();

            if (!this.EnsureBound()) return;
            if (recipe == null || craftCount <= 0) return;

            IReadOnlyList<CraftingMaterialRequirementEntry> requirements = recipe.MaterialRequirements;

            if (requirements == null) return;

            InventoryController inventoryController = this.RecipeDisplayController.InventoryController;

            int displayCount = Mathf.Min(requirements.Count, CraftingPanelMaterialRequirementListHandler.MAX_MATERIAL_SLOT_COUNT);
            for (int i = 0; i < displayCount; i++)
            {
                CraftingMaterialRequirementEntry requirement = requirements[i];

                if (requirement == null || requirement.Material == null) continue;

                int requiredQuantity = this.RecipeDisplayController.GetScaledQuantity(requirement.Quantity, craftCount);

                int ownedQuantity = inventoryController != null
                    ? inventoryController.GetItemCount(requirement.Material)
                    : 0;

                this.MaterialSlots[i]?.Bind(requirement.Material, ownedQuantity, requiredQuantity);
            }
        }
        public void ClearDisplay()
        {
            if (this.MaterialSlots == null) return;

            for (int i = 0; i < this.MaterialSlots.Length; i++)
            {
                this.MaterialSlots[i]?.Clear();
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.MaterialSlots == null || this.MaterialSlots.Length != CraftingPanelMaterialRequirementListHandler.MAX_MATERIAL_SLOT_COUNT)
            {
                _materialSlots = new CraftingMaterialRequirementSlotController[CraftingPanelMaterialRequirementListHandler.MAX_MATERIAL_SLOT_COUNT];
            }

            for (int i = 0; i < CraftingPanelMaterialRequirementListHandler.MAX_MATERIAL_SLOT_COUNT; i++)
            {
                if (this.MaterialSlots[i] != null) continue;

                Transform slotTransform = this.transform.Find($"MaterialSlot ({i + 1})");
                if (slotTransform != null) _materialSlots[i] = slotTransform.GetComponent<CraftingMaterialRequirementSlotController>();
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ClearDisplay();
        }
    }
}
