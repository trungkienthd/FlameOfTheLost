using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerCraftingExecutionHandler : PlayerCraftingSubHandlerBase
    {
        // --- Public APIs ---
        //  1. Crafting Execution
        internal bool TryCraft(CraftingRecipeProfile recipe, int craftCount = 1)
        {
            if (!this.EnsureBound()) return false;

            CraftingEvaluationRecord evaluation = this.CraftingController.EvaluateCrafting(recipe, craftCount);

            if (!evaluation.CanCraft)
            {
                this.CraftingController.NotifyCraftingFailed(recipe, evaluation.FailureReason);
                return false;
            }

            PlayerCraftingValidationHandler validationHandler = this.CraftingController.ValidationHandler;

            if (validationHandler == null)
            {
                this.CraftingController.NotifyCraftingFailed(recipe, CraftingFailureReason.CraftingUnavailable);

                return false;
            }

            if (!validationHandler.TryBuildMaterialRequirementTotals(recipe, craftCount, out Dictionary<MaterialProfile, int> materialRequirements))
            {
                this.CraftingController.NotifyCraftingFailed(recipe, CraftingFailureReason.InvalidRecipe);
                return false;
            }

            if (!validationHandler.TryGetScaledValue(recipe.ResultQuantity, craftCount, out int resultQuantity))
            {
                this.CraftingController.NotifyCraftingFailed(recipe, CraftingFailureReason.InvalidRecipe);
                return false;
            }

            if (!validationHandler.TryGetScaledValue(recipe.LostEmbersCost, craftCount, out int lostEmbersCost))
            {
                this.CraftingController.NotifyCraftingFailed(recipe, CraftingFailureReason.InvalidRecipe);
                return false;
            }

            InventoryController inventoryController = this.CraftingController.InventoryController;
            CharacterRewardWalletController rewardWalletController = this.CraftingController.RewardWalletController;

            if (inventoryController == null || rewardWalletController == null)
            {
                this.CraftingController.NotifyCraftingFailed(recipe, CraftingFailureReason.CraftingUnavailable);
                return false;
            }

            // Remove materials first. This can free inventory entries that the resulting item may need.
            if (!this.TryConsumeMaterials(inventoryController, materialRequirements, out Dictionary<MaterialProfile, int> consumedMaterials))
            {
                this.CraftingController.NotifyCraftingFailed(recipe, CraftingFailureReason.TransactionFailed);
                return false;
            }

            // Material removal succeeded, so Lost Embers should also succeed after the earlier evaluation.
            if (lostEmbersCost > 0 && !rewardWalletController.TrySpend(lostEmbersCost))
            {
                this.RestoreMaterials(inventoryController, consumedMaterials);

                this.CraftingController.NotifyCraftingFailed(recipe, CraftingFailureReason.TransactionFailed);
                return false;
            }

            // Capacity is checked after ingredient removal because crafting may itself free inventory slots.
            if (!inventoryController.TryAddItem(recipe.ResultItem, resultQuantity))
            {
                this.RestoreMaterials(inventoryController, consumedMaterials);

                if (lostEmbersCost > 0)
                {
                    rewardWalletController.Add(lostEmbersCost);
                }

                this.CraftingController.NotifyCraftingFailed(recipe, CraftingFailureReason.InsufficientInventorySpace);
                return false;
            }

            this.CraftingController.NotifyCraftingCompleted(recipe, craftCount);
            return true;
        }

        // --- Helpers ---
        //  1. Material Transaction
        private bool TryConsumeMaterials(InventoryController inventoryController, IReadOnlyDictionary<MaterialProfile, int> requirements, out Dictionary<MaterialProfile, int> consumedMaterials)
        {
            consumedMaterials = new Dictionary<MaterialProfile, int>();

            foreach (KeyValuePair<MaterialProfile, int> requirement in requirements)
            {
                if (!inventoryController.TryRemoveItem(requirement.Key, requirement.Value))
                {
                    this.RestoreMaterials(inventoryController, consumedMaterials);
                    return false;
                }

                consumedMaterials.Add(requirement.Key, requirement.Value);
            }

            return true;
        }
        private void RestoreMaterials(InventoryController inventoryController, IReadOnlyDictionary<MaterialProfile, int> requirements)
        {
            foreach (KeyValuePair<MaterialProfile, int> requirement in requirements)
            {
                bool restored = inventoryController.TryAddItem(requirement.Key, requirement.Value);

                if (!restored)
                {
                    Debug.LogError($"[{this.CraftingController.CharacterName}.{nameof(PlayerCraftingExecutionHandler)}] " + $"Failed to restore crafting material '{requirement.Key.name}' x{requirement.Value}.", this);
                }
            }
        }
    }
}
