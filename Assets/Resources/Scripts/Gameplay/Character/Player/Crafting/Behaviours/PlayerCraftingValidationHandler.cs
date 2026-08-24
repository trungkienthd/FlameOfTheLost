using System.Collections.Generic;

namespace FlameOfTheLost
{
    public class PlayerCraftingValidationHandler : PlayerCraftingSubHandlerBase
    {
        // --- Public APIs ---
        //  1. Crafting Evaluation
        internal CraftingEvaluationRecord EvaluateCrafting(CraftingRecipeProfile recipe, int craftCount = 1)
        {
            if (!this.EnsureBound()) return CraftingEvaluationRecord.Failure(CraftingFailureReason.CraftingUnavailable);

            if (!this.CraftingController.CanProcessCrafting) return CraftingEvaluationRecord.Failure(CraftingFailureReason.CraftingUnavailable);

            if (!this.CraftingController.IsValidCurrentRecipeProvider()) return CraftingEvaluationRecord.Failure(CraftingFailureReason.InvalidProvider);

            if (recipe == null || craftCount <= 0) return CraftingEvaluationRecord.Failure(CraftingFailureReason.InvalidRecipe);

            if (!this.CraftingController.IsRecipeAvailable(recipe)) return CraftingEvaluationRecord.Failure(CraftingFailureReason.RecipeUnavailable);

            if (!this.IsRecipeValid(recipe)) return CraftingEvaluationRecord.Failure(CraftingFailureReason.InvalidRecipe);

            if (!this.HasRequiredMaterials(recipe, craftCount)) return CraftingEvaluationRecord.Failure(CraftingFailureReason.InsufficientMaterials);

            if (!this.HasRequiredLostEmbers(recipe, craftCount)) return CraftingEvaluationRecord.Failure(CraftingFailureReason.InsufficientLostEmbers);

            return CraftingEvaluationRecord.Success();
        }

        //  2. Recipe Validation
        internal bool IsRecipeValid(CraftingRecipeProfile recipe)
        {
            if (recipe == null) return false;
            if (recipe.ResultItem == null) return false;
            if (recipe.ResultQuantity <= 0) return false;

            IReadOnlyList<CraftingMaterialRequirementEntry> requirements = recipe.MaterialRequirements;

            if (requirements == null) return true;

            for (int i = 0; i < requirements.Count; i++)
            {
                CraftingMaterialRequirementEntry requirement = requirements[i];

                if (requirement == null) return false;
                if (requirement.Material == null) return false;
                if (requirement.Quantity <= 0) return false;
            }

            return true;
        }

        //  3. Material Requirement Calculation
        internal bool TryBuildMaterialRequirementTotals(CraftingRecipeProfile recipe, int craftCount, out Dictionary<MaterialProfile, int> requirements)
        {
            requirements = new Dictionary<MaterialProfile, int>();

            if (recipe == null || craftCount <= 0) return false;

            IReadOnlyList<CraftingMaterialRequirementEntry> sourceRequirements = recipe.MaterialRequirements;

            if (sourceRequirements == null) return true;

            for (int i = 0; i < sourceRequirements.Count; i++)
            {
                CraftingMaterialRequirementEntry entry = sourceRequirements[i];

                if (entry == null || entry.Material == null) return false;
                if (!this.TryGetScaledValue(entry.Quantity, craftCount, out int scaledQuantity)) return false;

                if (requirements.TryGetValue(entry.Material, out int existingQuantity))
                {
                    long combinedQuantity = (long)existingQuantity + scaledQuantity;

                    if (combinedQuantity > int.MaxValue)
                    {
                        return false;
                    }

                    requirements[entry.Material] = (int)combinedQuantity;
                }
                else
                {
                    requirements.Add(entry.Material, scaledQuantity);
                }
            }

            return true;
        }

        //  4. Scaling
        internal bool TryGetScaledValue(int value, int multiplier, out int result)
        {
            result = 0;

            if (value < 0 || multiplier <= 0) return false;

            long scaledValue = (long)value * multiplier;

            if (scaledValue > int.MaxValue) return false;

            result = (int)scaledValue;

            return true;
        }

        // --- Helpers ---
        //  1. Material Requirements
        private bool HasRequiredMaterials(CraftingRecipeProfile recipe, int craftCount)
        {
            if (!this.TryBuildMaterialRequirementTotals(recipe, craftCount, out Dictionary<MaterialProfile, int> requirements)) return false;

            InventoryController inventoryController = this.CraftingController.InventoryController;

            if (inventoryController == null) return false;

            foreach (KeyValuePair<MaterialProfile, int> requirement in requirements)
            {
                if (!inventoryController.HasItem(requirement.Key, requirement.Value)) return false;
            }

            return true;
        }

        //  2. Lost Embers
        private bool HasRequiredLostEmbers(CraftingRecipeProfile recipe, int craftCount)
        {
            if (!this.TryGetScaledValue(recipe.LostEmbersCost, craftCount, out int requiredAmount)) return false;

            if (requiredAmount <= 0) return true;

            CharacterRewardWalletController rewardWalletController = this.CraftingController.RewardWalletController;

            return rewardWalletController != null &&
                   rewardWalletController.CanSpend(requiredAmount);
        }
    }
}
