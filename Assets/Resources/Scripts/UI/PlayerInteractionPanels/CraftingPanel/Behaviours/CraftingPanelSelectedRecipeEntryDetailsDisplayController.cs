using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class CraftingPanelSelectedRecipeEntryDetailsDisplayController : CraftingPanelSubHandlerBase
    {
        private const int MAX_MATERIAL_SLOT_COUNT = 3;

        // --- References ---
        //  1. Result
        [Header("References (Result)")]
        [SerializeField] private Image _resultIconImage;
        [SerializeField] private TMP_Text _resultNameAndQuantityText;
        private Image ResultIcon => _resultIconImage;
        private TMP_Text ResultNameAndQuantityText => _resultNameAndQuantityText;

        //  2. Ingredients
        [Header("References (Ingredients)")]
        [SerializeField] private CraftingMaterialRequirementSlotController[] _materialSlots = new CraftingMaterialRequirementSlotController[CraftingPanelSelectedRecipeEntryDetailsDisplayController.MAX_MATERIAL_SLOT_COUNT];
        private CraftingMaterialRequirementSlotController[] MaterialSlots => _materialSlots;

        //  3. Craft Quantity
        [Header("References (Craft Quantity)")]
        [SerializeField] private Button _buttonDecrease;
        [SerializeField] private Button _buttonIncrease;
        [SerializeField] private TMP_Text _quantityText;
        private Button ButtonDecrease => _buttonDecrease;
        private Button ButtonIncrease => _buttonIncrease;
        private TMP_Text QuantityText => _quantityText;

        //  4. Craft
        [Header("References (Craft)")]
        [SerializeField] private Button _buttonCraft;
        private Button ButtonCraft => _buttonCraft;

        // --- Public APIs ---
        public void RefreshDisplay(CraftingRecipeProfile recipe, int craftCount)
        {
            if (recipe == null ||
                recipe.ResultItem == null ||
                craftCount <= 0)
            {
                this.ClearDisplay();
                return;
            }

            this.RefreshResult(recipe, craftCount);
            this.RefreshIngredients(recipe, craftCount);
            this.RefreshCraftQuantity(craftCount);
            this.RefreshCraftButton(recipe, craftCount);
        }
        public void ClearDisplay()
        {
            if (this.ResultIcon != null)
            {
                this.ResultIcon.sprite = null;
                this.ResultIcon.enabled = false;
            }

            if (this.ResultNameAndQuantityText != null)
            {
                this.ResultNameAndQuantityText.text = string.Empty;
            }

            if (this.MaterialSlots != null)
            {
                for (int i = 0; i < this.MaterialSlots.Length; i++)
                {
                    this.MaterialSlots[i]?.Clear();
                }
            }

            if (this.QuantityText != null)
            {
                this.QuantityText.text = "1";
            }

            if (this.ButtonDecrease != null)
            {
                this.ButtonDecrease.interactable = false;
            }

            if (this.ButtonIncrease != null)
            {
                this.ButtonIncrease.interactable = false;
            }

            if (this.ButtonCraft != null)
            {
                this.ButtonCraft.interactable = false;
            }
        }

        // --- Helpers ---
        //  1. Result
        private void RefreshResult(CraftingRecipeProfile recipe, int craftCount)
        {
            int totalResultQuantity =this.GetScaledQuantity(recipe.ResultQuantity, craftCount);

            if (this.ResultIcon != null)
            {
                this.ResultIcon.sprite = recipe.ResultItem.Icon;

                this.ResultIcon.enabled = recipe.ResultItem.Icon != null;
            }

            if (this.ResultNameAndQuantityText != null)
            {
                this.ResultNameAndQuantityText.text = $"{recipe.ResultItem.DisplayName} ({totalResultQuantity}x)";
            }
        }

        //  2. Ingredients
        private void RefreshIngredients(CraftingRecipeProfile recipe, int craftCount)
        {
            for (int i = 0; i < this.MaterialSlots.Length; i++)
            {
                this.MaterialSlots[i]?.Clear();
            }

            IReadOnlyList<CraftingMaterialRequirementEntry> requirements = recipe.MaterialRequirements;

            if (requirements == null) return;

            InventoryController inventoryController = this.CraftingPanelController.InventoryController;

            int displayCount = Mathf.Min(requirements.Count, CraftingPanelSelectedRecipeEntryDetailsDisplayController.MAX_MATERIAL_SLOT_COUNT);

            for (int i = 0; i < displayCount; i++)
            {
                CraftingMaterialRequirementEntry requirement = requirements[i];

                if (requirement == null || requirement.Material == null)
                {
                    continue;
                }

                int requiredQuantity = this.GetScaledQuantity(requirement.Quantity, craftCount);

                int ownedQuantity = inventoryController != null ? inventoryController.GetItemCount(requirement.Material) : 0;

                this.MaterialSlots[i]?.Bind(requirement.Material, ownedQuantity, requiredQuantity);
            }
        }

        //  3. Craft Quantity
        private void RefreshCraftQuantity(int craftCount)
        {
            if (this.QuantityText != null)
            {
                this.QuantityText.text = craftCount.ToString();
            }

            if (this.ButtonDecrease != null)
            {
                this.ButtonDecrease.interactable = craftCount > 1;
            }

            if (this.ButtonIncrease != null)
            {
                this.ButtonIncrease.interactable = craftCount < int.MaxValue;
            }
        }

        //  4. Craft Button
        private void RefreshCraftButton(CraftingRecipeProfile recipe, int craftCount)
        {
            if (this.ButtonCraft == null) return;

            PlayerCraftingController craftingController = this.CraftingPanelController.PlayerCraftingController;

            if (craftingController == null)
            {
                this.ButtonCraft.interactable = false;
                return;
            }

            CraftingEvaluationRecord evaluation = craftingController.EvaluateCrafting(recipe, craftCount);

            this.ButtonCraft.interactable = evaluation.CanCraft;
        }

        //  5. Scaling
        private int GetScaledQuantity(int quantity, int craftCount)
        {
            long scaledQuantity = (long)quantity * craftCount;

            return scaledQuantity > int.MaxValue 
                ? int.MaxValue 
                : (int)scaledQuantity;
        }

        //  6. Button Callbacks
        private void HandleDecreaseClicked()
        {
            if (!this.EnsureBound()) return;

            this.CraftingPanelController.DecreaseCraftCount();
        }
        private void HandleIncreaseClicked()
        {
            if (!this.EnsureBound()) return;

            this.CraftingPanelController.IncreaseCraftCount();
        }
        private void HandleCraftClicked()
        {
            if (!this.EnsureBound()) return;

            this.CraftingPanelController.TryCraftSelectedRecipe();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            Transform resultDisplayTransform = this.transform.Find("ResultDisplay");
            if (resultDisplayTransform != null)
            {
                if (this.ResultIcon == null)
                {
                    Transform iconTransform = resultDisplayTransform.Find("ResultSlot/Icon");
                    if (iconTransform != null) _resultIconImage = iconTransform.GetComponent<Image>();
                }

                if (this.ResultNameAndQuantityText == null)
                {
                    Transform textTransform = resultDisplayTransform.Find("ResultNameAndQuantity");
                    if (textTransform != null) _resultNameAndQuantityText = textTransform.GetComponent<TMP_Text>();
                }
            }

            if (this.MaterialSlots == null || this.MaterialSlots.Length != CraftingPanelSelectedRecipeEntryDetailsDisplayController.MAX_MATERIAL_SLOT_COUNT)
            {
                _materialSlots = new CraftingMaterialRequirementSlotController[CraftingPanelSelectedRecipeEntryDetailsDisplayController.MAX_MATERIAL_SLOT_COUNT];
            }

            Transform ingredientListTransform = this.transform.Find("IngredientEntryList");

            if (ingredientListTransform != null)
            {
                for (int i = 0; i < CraftingPanelSelectedRecipeEntryDetailsDisplayController.MAX_MATERIAL_SLOT_COUNT; i++)
                {
                    if (this.MaterialSlots[i] != null) continue;

                    Transform slotTransform = ingredientListTransform.Find($"MaterialSlot ({i + 1})");

                    if (slotTransform != null)
                    {
                        _materialSlots[i] = slotTransform.GetComponent<CraftingMaterialRequirementSlotController>();
                    }
                }
            }

            Transform craftQuantityTransform = this.transform.Find("CraftQuantity");

            if (craftQuantityTransform != null)
            {
                if (this.ButtonDecrease == null)
                {
                    Transform buttonTransform = craftQuantityTransform.Find("ButtonDecrease");
                    if (buttonTransform != null) _buttonDecrease = buttonTransform.GetComponent<Button>();
                }

                if (this.QuantityText == null)
                {
                    Transform quantityTransform = craftQuantityTransform.Find("QuantityText");
                    if (quantityTransform != null) _quantityText = quantityTransform.GetComponent<TMP_Text>();
                }
                if (this.ButtonIncrease == null)
                {
                    Transform buttonTransform = craftQuantityTransform.Find("ButtonIncrease");
                    if (buttonTransform != null) _buttonIncrease = buttonTransform.GetComponent<Button>();
                }
            }

            if (this.ButtonCraft == null)
            {
                Transform craftButtonTransform = this.transform.Find("ButtonCraft");
                if (craftButtonTransform != null) _buttonCraft = craftButtonTransform.GetComponent<Button>();
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            if (this.ButtonDecrease != null) this.ButtonDecrease.onClick.AddListener(this.HandleDecreaseClicked);
            if (this.ButtonIncrease != null) this.ButtonIncrease.onClick.AddListener(this.HandleIncreaseClicked);
            if (this.ButtonCraft    != null) this.ButtonCraft.onClick.AddListener(this.HandleCraftClicked);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ClearDisplay();
        }
        protected override void UnbindInternalReferences()
        {
            if (this.ButtonDecrease != null) this.ButtonDecrease.onClick.RemoveListener (this.HandleDecreaseClicked);
            if (this.ButtonIncrease != null) this.ButtonIncrease.onClick.RemoveListener(this.HandleIncreaseClicked);
            if (this.ButtonCraft    != null) this.ButtonCraft.onClick.RemoveListener(this.HandleCraftClicked);

            base.UnbindInternalReferences();
        }
    }
}
