using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class InventoryNavigationSubPanelController : MonoBehaviour
    {
        [SerializeField] private List<InventoryNavigationButtonUIController> _buttons = new();
        private IReadOnlyList<InventoryNavigationButtonUIController> ButtonControllers => _buttons;

        public ItemType CurrentSelectedItemType { get; private set; } = ItemType.Weapon;

        public event Action<ItemType> OnSelectedItemTypeChanged;

        private void Awake()
        {
            for (int i = 0; i < this.ButtonControllers.Count; i++)
            {
                InventoryNavigationButtonUIController button = this.ButtonControllers[i];
                if (button == null) continue;

                button.OnClicked += this.HandleButtonClicked;
            }
        }
        private void Start()
        {
            this.RefreshButtonStates();
        }
        private void OnDestroy()
        {
            for (int i = 0; i < this.ButtonControllers.Count; i++)
            {
                InventoryNavigationButtonUIController button = this.ButtonControllers[i];
                if (button == null) continue;

                button.OnClicked -= this.HandleButtonClicked;
            }
        }

        public void SetSelectedItemType(ItemType itemType, bool notify = false)
        {
            this.CurrentSelectedItemType = itemType;
            this.RefreshButtonStates();

            if (notify)
            {
                this.OnSelectedItemTypeChanged?.Invoke(itemType);
            }
        }

        private void HandleButtonClicked(InventoryNavigationButtonUIController clickedButton)
        {
            if (clickedButton == null) return;
            if (this.CurrentSelectedItemType == clickedButton.Category) return;

            this.CurrentSelectedItemType = clickedButton.Category;
            this.RefreshButtonStates();
            this.OnSelectedItemTypeChanged?.Invoke(this.CurrentSelectedItemType);
        }

        private void RefreshButtonStates()
        {
            for (int i = 0; i < this.ButtonControllers.Count; i++)
            {
                InventoryNavigationButtonUIController button = this.ButtonControllers[i];
                if (button == null) continue;

                button.SetSelected(button.Category == this.CurrentSelectedItemType);
            }
        }
    }
}