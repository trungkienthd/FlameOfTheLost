using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PickupItemController : MonoBehaviour, IPlayerInteractable
    {
        // --- States ---
        [Header("Item")]
        [SerializeField] private ItemProfile _item;
        [SerializeField, Min(1)] private int _quantity = 1;
        public ItemProfile Item => _item;
        public int Quantity => _quantity;

        [Header("Visual")]
        [SerializeField] private bool _useItemIconAsSprite = true;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private bool UseItemIconAsSprite => _useItemIconAsSprite && this.Item != null;
        private SpriteRenderer SpriteRenderer => _spriteRenderer;

        [Header("Pickup")]
        [SerializeField] private bool _destroyAfterPickup = true;
        private bool DestroyAfterPickup => _destroyAfterPickup;

        public event Action<PickupItemController> OnPickedUp;

        // --- Unity Callbacks ---
        private void Awake()
        {
            this.CacheReferences();
            this.RefreshVisual();
        }

//#if UNITY_EDITOR
//        private void OnValidate()
//        {
//            if (this.Quantity < 1)
//            {
//                _quantity = 1;
//            }

//            this.CacheReferences();
//            this.RefreshVisual();
//        }
//#endif

        public bool CanInteract(GameObject interactor)
        {
            if (interactor == null) return false;
            if (this.Item == null) return false;
            if (this.Quantity <= 0) return false;

            InventoryController inventoryController = interactor.GetComponentInChildren<InventoryController>();
            return inventoryController != null;
        }

        public bool TryInteract(GameObject interactor)
        {
            return this.TryPickup(interactor);
        }

        public bool TryPickup(GameObject picker)
        {
            if (picker == null) return false;
            if (this.Item == null) return false;
            if (this.Quantity <= 0) return false;

            InventoryController inventoryController = picker.GetComponentInChildren<InventoryController>();
            if (inventoryController == null) return false;

            bool added = inventoryController.TryAddItem(this.Item, this.Quantity);
            if (!added) return false;

            this.OnPickedUp?.Invoke(this);

            if (this.DestroyAfterPickup)
            {
                UnityEngine.Object.Destroy(this.gameObject);
            }
            else
            {
                this.gameObject.SetActive(false);
            }

            return true;
        }

        private void CacheReferences()
        {
            if (this.SpriteRenderer == null) _spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        }

        private void RefreshVisual()
        {
            if (!this.UseItemIconAsSprite) return;
            if (this.SpriteRenderer == null) return;
            if (this.Item == null) return;
            if (this.Item.Icon == null) return;

            this.SpriteRenderer.sprite = this.Item.Icon;
        }
    }
}
