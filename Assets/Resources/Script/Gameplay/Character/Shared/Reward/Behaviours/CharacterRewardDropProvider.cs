using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterRewardDropProvider : CharacterConfiguredBehaviourBase<CharacterRewardDropProfile>
    {
        // --- References ---
        //  1. Drop References
        [Header("References")]
        [SerializeField] private Transform _defaultDropOrigin;
        private Transform DefaultDropOrigin => _defaultDropOrigin != null 
            ? _defaultDropOrigin 
            : this.transform;

        [SerializeField] private List<Transform> _dropAnchors = new();
        private IReadOnlyList<Transform> DropAnchors => _dropAnchors;

        // --- Data / Configs ---
        public CharacterRewardDropProfile DropProfile => this.Config;

        //  1. Lost Embers
        public int RewardAmount => this.DropProfile != null ? this.DropProfile.Amount : 0;
        public bool HasReward => this.RewardAmount > 0;

        //  2. Item Drops
        public IReadOnlyList<CharacterRewardItemDropEntry> ItemDropEntries => this.DropProfile?.ItemDropEntries;
        private float RandomPositionRadius => this.DropProfile != null
            ? this.DropProfile.RandomPositionRadius
            : 0.35f;
        public bool HasItemDrops
            => this.ItemDropEntries != null
            && this.ItemDropEntries.Count > 0;

        // --- Public APIs ---
        public bool TryDropItems()
        {
            if (this.ItemDropEntries == null) return false;

            bool droppedAny = false;
            int anchorCursor = 0;

            for (int i = 0; i < this.ItemDropEntries.Count; i++)
            {
                CharacterRewardItemDropEntry entry = this.ItemDropEntries[i];

                if (entry == null) continue;
                if (entry.CollectiblePrefab == null) continue;
                if (!entry.ShouldDrop()) continue;

                if (!this.TryResolveDropQuantity(entry, out int quantity)) continue;

                Vector3 spawnPosition = this.ResolveSpawnPosition(anchorCursor);

                CollectibleObjectController collectibleController = UnityEngine.Object.Instantiate(entry.CollectiblePrefab, spawnPosition, Quaternion.identity);

                if (collectibleController == null) continue;

                // Only remove inventory items once the collectible was
                // successfully created.
                if (entry.Source == CharacterRewardItemDropSource.Inventory)
                {
                    if (!this.TryConsumeInventoryDrop(entry, quantity))
                    {
                        UnityEngine.Object.Destroy(collectibleController.gameObject);
                        continue;
                    }
                }

                collectibleController.ConfigureQuantity(quantity);

                anchorCursor++;
                droppedAny = true;
            }

            return droppedAny;
        }

        // --- Helpers ---
        //  1. Drop Quantity
        private bool TryResolveDropQuantity(CharacterRewardItemDropEntry entry, out int quantity)
        {
            quantity = 0;

            if (entry == null) return false;
            if (entry.CollectiblePrefab == null) return false;

            int rolledQuantity = entry.RollQuantity();
            if (rolledQuantity <= 0) return false;

            switch (entry.Source)
            {
                case CharacterRewardItemDropSource.Configured:
                    quantity = rolledQuantity;
                    return true;

                case CharacterRewardItemDropSource.Inventory:
                    return this.TryResolveInventoryDropQuantity(entry, rolledQuantity, out quantity);

                default:
                    return false;
            }
        }

        private bool TryResolveInventoryDropQuantity(CharacterRewardItemDropEntry entry, int requestedQuantity, out int quantity)
        {
            quantity = 0;

            CharacterRootController character = this.GetCharacter();
            if (character == null) return false;

            InventoryController inventoryController = character.InventoryController;
            if (inventoryController == null) return false;

            ItemProfile item = entry.CollectiblePrefab.Item;
            if (item == null) return false;

            int availableQuantity = inventoryController.GetItemCount(item);
            if (availableQuantity <= 0) return false;

            quantity = Mathf.Min(requestedQuantity, availableQuantity);

            return quantity > 0;
        }

        private bool TryConsumeInventoryDrop(CharacterRewardItemDropEntry entry, int quantity)
        {
            if (entry == null) return false;
            if (entry.CollectiblePrefab == null) return false;
            if (quantity <= 0) return false;

            CharacterRootController character = this.GetCharacter();
            if (character == null) return false;

            InventoryController inventoryController = character.InventoryController;
            if (inventoryController == null) return false;

            ItemProfile item = entry.CollectiblePrefab.Item;
            if (item == null) return false;

            return inventoryController.TryRemoveItem(item, quantity);
        }

        //  2. Drop Position
        private Vector3 ResolveSpawnPosition(int spawnIndex)
        {
            Transform anchor = this.GetValidAnchor(spawnIndex);

            Vector3 origin = anchor != null
                ? anchor.position
                : this.DefaultDropOrigin.position;

            if (this.RandomPositionRadius <= 0f) return origin;

            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * this.RandomPositionRadius;

            return origin + new Vector3(randomOffset.x, randomOffset.y, 0f);
        }

        private Transform GetValidAnchor(int spawnIndex)
        {
            if (this.DropAnchors == null) return null;
            if (this.DropAnchors.Count == 0) return null;

            int startIndex = Mathf.Abs(spawnIndex) % this.DropAnchors.Count;

            for (int offset = 0; offset < this.DropAnchors.Count; offset++)
            {
                int index = (startIndex + offset) % this.DropAnchors.Count;

                Transform anchor = this.DropAnchors[index];

                if (anchor != null)
                {
                    return anchor;
                }
            }

            return null;
        }

        // --- FOTLMonoBehavior Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.DropProfile == null) Debug.LogError($"[{this.CharacterName}.{this.GetType().Name}] No Drop Profile assigned. Please assign a valid Drop Profile.", this);
        }
    }
}
