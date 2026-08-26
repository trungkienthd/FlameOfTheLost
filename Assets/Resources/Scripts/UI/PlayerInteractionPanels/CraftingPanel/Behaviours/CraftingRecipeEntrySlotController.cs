using System;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class CraftingRecipeEntrySlotController : FOTLMonoBehaviour
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _iconImage;
        private Button Button => _button;
        private Image BackgroundImage => _backgroundImage;
        private Image Icon => _iconImage;

        // --- Configuration ---
        [Header("Selection")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _selectedColor = Color.red;
        private Color NormalColor => _normalColor;
        private Color SelectedColor => _selectedColor;

        // --- Runtime ---
        public CraftingRecipeProfile BoundRecipe { get; private set; }

        // --- Events ---
        public event Action<CraftingRecipeEntrySlotController> OnClicked;

        // --- Public APIs ---
        public void Bind(CraftingRecipeProfile recipe)
        {
            this.BoundRecipe = recipe;

            bool hasContent = recipe != null && recipe.ResultItem != null;

            if (!hasContent)
            {
                this.ResetBindings();
                return;
            }

            if (this.Icon != null)
            {
                this.Icon.sprite = recipe.ResultItem.Icon;
                this.Icon.enabled = recipe.ResultItem.Icon != null;
            }

            if (this.Button != null)
            {
                this.Button.interactable = true;
            }

            this.SetSelected(false);
        }

        public void ResetBindings()
        {
            this.BoundRecipe = null;

            if (this.Button != null)
            {
                this.Button.interactable = false;
            }

            if (this.Icon != null)
            {
                this.Icon.sprite = null;
                this.Icon.enabled = false;
            }

            this.SetSelected(false);
        }

        public void SetSelected(bool selected)
        {
            if (this.BackgroundImage == null) return;

            this.BackgroundImage.color = selected
                ? this.SelectedColor
                : this.NormalColor;
        }

        // --- Helpers ---
        private void HandleClick()
        {
            if (this.BoundRecipe == null) return;

            this.OnClicked?.Invoke(this);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.Button == null) _button = this.GetComponent<Button>();
            if (this.BackgroundImage == null) _backgroundImage = this.GetComponent<Image>();

            if (this.Icon == null)
            {
                Transform iconTransform = this.transform.Find("Icon");
                if (iconTransform != null) _iconImage = iconTransform.GetComponent<Image>();
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            if (this.Button != null) this.Button.onClick.AddListener(this.HandleClick);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ResetBindings();
        }
        protected override void UnbindInternalReferences()
        {
            if (this.Button != null) this.Button.onClick.RemoveListener(this.HandleClick);

            base.UnbindInternalReferences();
        }
    }
}
