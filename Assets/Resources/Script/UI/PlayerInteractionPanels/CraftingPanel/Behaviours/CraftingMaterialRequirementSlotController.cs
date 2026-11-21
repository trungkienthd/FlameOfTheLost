using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class CraftingMaterialRequirementSlotController : FOTLMonoBehaviour
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private GameObject _subSlotRoot;
        [SerializeField] private TMP_Text _quantityText;
        private Image Icon => _iconImage;
        private GameObject SubSlotRoot => _subSlotRoot;
        private TMP_Text QuantityText => _quantityText;

        // --- Public APIs ---
        public void Bind(MaterialProfile material, int ownedQuantity, int requiredQuantity)
        {
            if (material == null || requiredQuantity <= 0)
            {
                this.Clear();
                return;
            }

            if (this.Icon != null)
            {
                this.Icon.sprite = material.Icon;
                this.Icon.enabled = material.Icon != null;
            }

            if (this.SubSlotRoot != null)
            {
                this.SubSlotRoot.SetActive(true);
            }

            if (this.QuantityText != null)
            {
                this.QuantityText.text = $"{ownedQuantity}/{requiredQuantity}";
            }
        }
        public void Clear()
        {
            if (this.Icon != null)
            {
                this.Icon.sprite = null;
                this.Icon.enabled = false;
            }

            if (this.QuantityText != null)
            {
                this.QuantityText.text = string.Empty;
            }

            if (this.SubSlotRoot != null)
            {
                this.SubSlotRoot.SetActive(false);
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.Icon == null)
            {
                Transform iconTransform = this.transform.Find("Icon");
                if (iconTransform != null) _iconImage = iconTransform.GetComponent<Image>();
            }

            if (this.SubSlotRoot == null)
            {
                Transform subSlotTransform = this.transform.Find("SubSlot");
                if (subSlotTransform != null) _subSlotRoot = subSlotTransform.gameObject;
            }

            if (this.QuantityText == null && this.SubSlotRoot != null) _quantityText = this.SubSlotRoot.GetComponentInChildren<TMP_Text>(true);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.Clear();
        }
    }
}
