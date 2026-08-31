using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class InventoryNavigationSubPanelController : InventoryPanelSubBehaviourBase
    {
        // --- References ---
        [SerializeField] private List<InventoryNavigationButtonUIController> _buttons = new();
        private IReadOnlyList<InventoryNavigationButtonUIController> ButtonControllers => _buttons;

        // --- Runtime ---
        public ItemType? CurrentSelectedItemType { get; private set; }

        // --- Events ---
        public event Action<ItemType?> OnSelectedItemTypeChanged;

        // --- Public APIs ---
        public void SetSelectedItemType(ItemType itemType, bool notify = false)
        {
            this.CurrentSelectedItemType = itemType;
            this.RefreshButtonStates();

            if (notify)
            {
                this.OnSelectedItemTypeChanged?.Invoke(itemType);
            }
        }

        // --- Helpers ---
        //  1. Event Handling
        private void HandleButtonClicked(InventoryNavigationButtonUIController clickedButton)
        {
            if (clickedButton == null) return;

            this.CurrentSelectedItemType = this.CurrentSelectedItemType == clickedButton.Category
                ? null
                : clickedButton.Category;

            this.RefreshButtonStates();
            this.OnSelectedItemTypeChanged?.Invoke(this.CurrentSelectedItemType);
        }

        //  2. Refresh
        private void RefreshButtonStates()
        {
            for (int i = 0; i < this.ButtonControllers.Count; i++)
            {
                InventoryNavigationButtonUIController button = this.ButtonControllers[i];
                if (button == null) continue;

                button.SetSelected(this.CurrentSelectedItemType.HasValue && button.Category == this.CurrentSelectedItemType);
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            for (int i = 0; i < this.ButtonControllers.Count; i++)
            {
                InventoryNavigationButtonUIController button = this.ButtonControllers[i];
                if (button == null) continue;

                button.OnClicked += this.HandleButtonClicked;
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.CurrentSelectedItemType = null;
        }
        protected override void InitializeGameplay()
        {
            base.InitializeGameplay();

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

            base.UnbindInternalReferences();
        }
    }
}