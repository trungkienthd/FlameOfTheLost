using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class InventoryNavigationSubPanelController : FOTLMonoBehaviour
    {
        [SerializeField] private List<InventoryNavigationButtonUIController> _buttons = new();
        private IReadOnlyList<InventoryNavigationButtonUIController> ButtonControllers => _buttons;

        public ItemType CurrentSelectedItemType { get; private set; } = ItemType.Weapon;

        public event Action<ItemType> OnSelectedItemTypeChanged;

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

        protected override void BindInternalReferences()
        {
            for (int i = 0; i < this.ButtonControllers.Count; i++)
            {
                InventoryNavigationButtonUIController button = this.ButtonControllers[i];
                if (button == null) continue;

                button.OnClicked += this.HandleButtonClicked;
            }
        }
        protected override void InitializeGameplay()
        {
            this.RefreshButtonStates();
        }
        protected override void UnbindInternalReferences()
        {
            for (int i = 0; i < this.ButtonControllers.Count; i++)
            {
                InventoryNavigationButtonUIController button = this.ButtonControllers[i];
                if (button == null) continue;

                button.OnClicked -= this.HandleButtonClicked;
            }
        }
    }
}