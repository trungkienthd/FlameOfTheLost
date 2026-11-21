using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(HarvestableObjectController))]
    public sealed class HarvestableDropHandler : HarvestableObjectSubHandlerBase
    {
        // --- References ---
        //  1. Drop References
        [Header("References")]
        [SerializeField] private Transform _defaultDropOrigin;
        private Transform DefaultDropOrigin => _defaultDropOrigin != null
            ? _defaultDropOrigin
            : this.transform;

        //  2. Runtime Collectible Refresh
        [SerializeField] private RuntimeCollectibleRefreshController _runtimeCollectibleRefreshController;
        private RuntimeCollectibleRefreshController RuntimeCollectibleRefreshController => _runtimeCollectibleRefreshController;

        [SerializeField] private List<Transform> _dropAnchors = new();
        private IReadOnlyList<Transform> DropAnchors => _dropAnchors;

        // --- Data / Configs ---
        private IReadOnlyList<HarvestableDropEntry> DropEntries => this.Profile?.DropEntries;
        private float RandomPositionRadius                      => this.Profile != null ? this.Profile.RandomPositionRadius : 0.35f;

        // --- Helpers ---
        //  1. Drop Spawn
        private void SpawnDrops(HarvestDamageAppliedContext context)
        {
            if (this.DropEntries == null) return;

            int anchorCursor = 0;

            for (int i = 0; i < this.DropEntries.Count; i++)
            {
                HarvestableDropEntry entry = this.DropEntries[i];

                if (entry == null) continue;
                if (entry.CollectiblePrefab == null) continue;
                if (!entry.ShouldDrop()) continue;

                int quantity = entry.RollQuantity();

                Vector3 spawnPosition = this.ResolveSpawnPosition(anchorCursor);

                anchorCursor++;

                CollectibleObjectController collectibleController = UnityEngine.Object.Instantiate
                (
                    original: entry.CollectiblePrefab,
                    position: spawnPosition,
                    rotation: Quaternion.identity
                );

                collectibleController.ConfigureQuantity(quantity);

                this.RuntimeCollectibleRefreshController?.RegisterRuntimeCollectible(collectibleController);
            }
        }
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

        //  2. Event Handling
        private void HandleBroken(HarvestDamageAppliedContext context)
        {
            this.SpawnDrops(context);
        }

        //  3. Binding Hooks
        protected override void OnBound(HarvestableObjectController controller)
        {
            base.OnBound(controller);

            controller.Broken += this.HandleBroken;
        }

        protected override void OnUnbinding(HarvestableObjectController controller)
        {
            controller.Broken -= this.HandleBroken;

            base.OnUnbinding(controller);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.RuntimeCollectibleRefreshController == null) _runtimeCollectibleRefreshController = UnityEngine.Object.FindFirstObjectByType<RuntimeCollectibleRefreshController>();
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.RuntimeCollectibleRefreshController == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(HarvestableDropHandler)}] {nameof(RuntimeCollectibleRefreshController)} reference is missing. Generated collectibles will not be registered for World Refresh.", this);
            }
        }
    }
}
