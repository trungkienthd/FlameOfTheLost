using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class ItemContainerGroupRefreshController : FOTLMonoBehaviour, IWorldRefreshable
    {
        // --- References ---
        //  1. Item Containers
        [Header("References - Item Containers")]
        [SerializeField] private List<ItemContainerController> _itemContainers = new();
        private IReadOnlyList<ItemContainerController> ItemContainers => _itemContainers;

        // --- Data / Configs ---
        //  1. World Refresh
        public int WorldRefreshPriority => 30;

        // --- Public APIs ---
        //  1. IWorldRefreshable
        public void RefreshWorldContent(WorldRefreshContext context)
        {
            if (!context.IsPlayerRespawn && !context.IsSaveReload) return;

            this.RefreshItemContainers();
        }

        //  2. Item Container Refresh
        public void RefreshItemContainers()
        {
            if (this.ItemContainers == null) return;

            for (int i = 0; i < this.ItemContainers.Count; i++)
            {
                ItemContainerController itemContainer = this.ItemContainers[i];

                if (itemContainer == null) continue;

                itemContainer.RefreshInventory();
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.ItemContainers == null || this.ItemContainers.Count == 0)
            {
                //Debug.LogWarning($"[{this.gameObject.name}.{nameof(ItemContainerGroupRefreshController)}] No {nameof(ItemContainerController)} references are configured.", this);
                return;
            }

            for (int i = 0; i < this.ItemContainers.Count; i++)
            {
                if (this.ItemContainers[i] != null) continue;

                Debug.LogWarning($"[{this.gameObject.name}.{nameof(ItemContainerGroupRefreshController)}] Item Container reference at index {i} is missing.", this);
            }
        }
    }
}