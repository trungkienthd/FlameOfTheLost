using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class EquippedLoadoutSlotActionTooltipController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private TMP_Text _headerText;
        [SerializeField] private Button _unequipButton;
        [SerializeField] private Button _replaceButton;
        [SerializeField] private Button _changeSkillButton;
        private RectTransform RectTransform => _rectTransform;
        private TMP_Text HeaderText => _headerText;
        private Button UnequipButton => _unequipButton;
        private Button ReplaceButton => _replaceButton;
        private Button ChangeSkillButton => _changeSkillButton;

        private EquippedLoadoutSlotUIController _boundSlot;
        public EquippedLoadoutSlotUIController BoundSlot => _boundSlot;

        public event Action<EquippedLoadoutSlotUIController> OnUnequipClicked;
        public event Action<EquippedLoadoutSlotUIController> OnReplaceClicked;
        public event Action<EquippedLoadoutSlotUIController> OnChangeSkillClicked;

        private void Awake()
        {
            if (this.RectTransform == null) _rectTransform = this.GetComponent<RectTransform>();
            if (this.HeaderText == null)
            {
                GameObject headerText = this.transform.Find("HeaderText")?.gameObject;
                if (headerText != null) _headerText = headerText.GetComponent<TMP_Text>();
            }
            if (this.UnequipButton == null || this.ReplaceButton == null || this.ChangeSkillButton == null)
            {
                GameObject buttonListRoot = this.transform.Find("ButtonListRoot")?.gameObject;
                GameObject unequipButton        = buttonListRoot?.transform.Find("UnequipButton")?.gameObject;
                GameObject replaceButton        = buttonListRoot?.transform.Find("ReplaceButton")?.gameObject;
                GameObject changeSkillButton    = buttonListRoot?.transform.Find("ChangeSkillButton")?.gameObject;
                if (unequipButton != null)      _unequipButton      = unequipButton.GetComponent<Button>();
                if (replaceButton != null)      _replaceButton      = replaceButton.GetComponent<Button>();
                if (changeSkillButton != null)  _changeSkillButton  = changeSkillButton.GetComponent<Button>();
            }

            // this.Hide();
        }
        private void OnEnable()
        {
            if (this.UnequipButton != null)
            {
                this.UnequipButton.onClick.RemoveListener(this.HandleUnequipClicked);
                this.UnequipButton.onClick.AddListener(this.HandleUnequipClicked);
            }
            if (this.ReplaceButton != null)
            {
                this.ReplaceButton.onClick.RemoveListener(this.HandleReplaceClicked);
                this.ReplaceButton.onClick.AddListener(this.HandleReplaceClicked);
            }
            if (this.ChangeSkillButton != null)
            {
                this.ChangeSkillButton.onClick.RemoveListener(this.HandleChangeSkillClicked);
                this.ChangeSkillButton.onClick.AddListener(this.HandleChangeSkillClicked);
            }
        }
        private void OnDestroy()
        {
            if (this.UnequipButton != null)     this.UnequipButton.onClick.RemoveListener(this.HandleUnequipClicked);
            if (this.ReplaceButton != null)     this.ReplaceButton.onClick.RemoveListener(this.HandleReplaceClicked);
            if (this.ChangeSkillButton != null) this.ChangeSkillButton.onClick.RemoveListener(this.HandleChangeSkillClicked);
        }

        public void ShowForSlotAtScreenPosition(EquippedLoadoutSlotUIController slot, RectTransform tooltipParent, Vector2 screenPosition, Vector2 offset, bool changeSkillBtnIsActive = false)
        {
            _boundSlot = slot;

            if (slot == null || tooltipParent == null)
            {
                this.Hide();
                return;
            }

            if (this.RectTransform == null)
            {
                this.Hide();
                return;
            }

            this.RefreshTexts(slot);
            this.RefreshButtons(slot, changeSkillBtnIsActive);

            if (this.RectTransform.parent != tooltipParent)
            {
                this.RectTransform.SetParent(tooltipParent, worldPositionStays: false);
            }

            this.gameObject.SetActive(true);
            if (!this.gameObject.activeInHierarchy)
            {
                Debug.LogWarning($"{nameof(EquippedLoadoutSlotActionTooltipController)} could not be shown because one of its parent GameObjects is inactive. Try again...");
                this.gameObject.SetActive(true);
            }

            this.RectTransform.SetAsLastSibling();

            Canvas parentCanvas = tooltipParent.GetComponentInParent<Canvas>();
            Camera eventCamera = parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? parentCanvas.worldCamera
                : null;

            RectTransformUtility.ScreenPointToLocalPointInRectangle
            (
                tooltipParent,
                screenPosition,
                eventCamera,
                out Vector2 localPoint
            );

            this.RectTransform.anchoredPosition = localPoint + offset;
        }

        public void Hide()
        {
            _boundSlot = null;
            this.gameObject.SetActive(false);
        }

        private void RefreshTexts(EquippedLoadoutSlotUIController slot)
        {
            if (this.HeaderText == null) return;

            this.HeaderText.text = this.GetHeaderText(slot);
        }

        private void RefreshButtons(EquippedLoadoutSlotUIController slot, bool changeSkillBtnIsActive)
        {
            bool hasContent = slot != null && slot.HasContent();

            if (this.UnequipButton != null)
            {
                this.UnequipButton.gameObject.SetActive(true);
                this.UnequipButton.interactable = hasContent;
            }
            if (this.ReplaceButton != null) this.ReplaceButton.gameObject.SetActive(true);
            if (this.ChangeSkillButton != null) this.ChangeSkillButton.gameObject.SetActive(changeSkillBtnIsActive);
        }

        private string GetHeaderText(EquippedLoadoutSlotUIController slot)
        {
            if (slot == null) return string.Empty;

            switch (slot.SlotType)
            {
                case LoadoutPanelSlotType.Weapon:
                    return slot.BoundWeaponEntry != null && slot.BoundWeaponEntry.Item != null
                        ? slot.BoundWeaponEntry.Item.DisplayName
                        : "Empty Weapon Slot";

                case LoadoutPanelSlotType.Consumable:
                    return slot.BoundConsumableEntry != null && slot.BoundConsumableEntry.Item != null
                        ? $"{slot.BoundConsumableEntry.Item.DisplayName} ({slot.BoundConsumableEntry.Quantity})"
                        : "Empty Consumable Item Slot";


                case LoadoutPanelSlotType.CarriableItem:
                    return slot.BoundCarriableItemEntry != null && slot.BoundCarriableItemEntry.Item != null
                        ? slot.BoundCarriableItemEntry.Item.DisplayName
                        : "Empty Carriable Item Slot";

                default:
                    return string.Empty;
            }
        }

        private void HandleUnequipClicked()
        {
            if (this.BoundSlot == null) return;
            this.OnUnequipClicked?.Invoke(this.BoundSlot);
        }

        private void HandleReplaceClicked()
        {
            if (this.BoundSlot == null) return;
            this.OnReplaceClicked?.Invoke(this.BoundSlot);
        }

        private void HandleChangeSkillClicked()
        {
            if (this.BoundSlot == null) return;
            this.OnChangeSkillClicked?.Invoke(this.BoundSlot);
        }
    }
}
