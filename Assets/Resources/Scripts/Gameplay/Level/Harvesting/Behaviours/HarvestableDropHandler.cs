using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(HarvestableObjectController))]
    public sealed class HarvestableDropHandler : FOTLMonoBehaviour
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private HarvestableObjectController _harvestableObjectController;
        private HarvestableObjectController HarvestableObjectController => _harvestableObjectController;

        [SerializeField] private Transform _defaultDropOrigin;
        private Transform DefaultDropOrigin => _defaultDropOrigin != null
            ? _defaultDropOrigin
            : this.transform;

        [SerializeField] private List<Transform> _dropAnchors = new();
        private IReadOnlyList<Transform> DropAnchors => _dropAnchors;

        // --- Configs ---
        [Header("Drops")]
        [SerializeField] private List<HarvestableDropEntry> _dropEntries = new();
        private IReadOnlyList<HarvestableDropEntry> DropEntries => _dropEntries;

        [SerializeField, Min(0f)] private float _randomPositionRadius = 0.35f;
        private float RandomPositionRadius => Mathf.Max(0f, _randomPositionRadius);

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

                int spawnCount = entry.RollSpawnCount();

                for (int j = 0; j < spawnCount; j++)
                {
                    Vector3 spawnPosition = this.ResolveSpawnPosition(anchorCursor);

                    anchorCursor++;

                    UnityEngine.Object.Instantiate
                    (
                        original: entry.CollectiblePrefab,
                        position: spawnPosition,
                        rotation: Quaternion.identity
                    );
                }
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

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.HarvestableObjectController == null) _harvestableObjectController = this.GetComponent<HarvestableObjectController>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.HarvestableObjectController == null)
            {
                Debug.LogError
                (
                    $"[{this.gameObject.name}." +
                    $"{nameof(HarvestableDropHandler)}] " +
                    $"{nameof(HarvestableObjectController)} " +
                    $"reference is not assigned.",
                    this
                );
            }

            for (int i = 0; i < this.DropEntries.Count; i++)
            {
                HarvestableDropEntry entry =
                    this.DropEntries[i];

                if (entry == null)
                {
                    Debug.LogWarning
                    (
                        $"[{this.gameObject.name}." +
                        $"{nameof(HarvestableDropHandler)}] " +
                        $"Drop entry [{i}] is null.",
                        this
                    );

                    continue;
                }

                if (entry.CollectiblePrefab == null)
                {
                    Debug.LogWarning
                    (
                        $"[{this.gameObject.name}." +
                        $"{nameof(HarvestableDropHandler)}] " +
                        $"Drop entry [{i}] has no collectible prefab.",
                        this
                    );
                }
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            if (this.HarvestableObjectController != null)
            {
                this.HarvestableObjectController.Broken += this.HandleBroken;
            }
        }
        protected override void UnbindInternalReferences()
        {
            if (this.HarvestableObjectController != null)
            {
                this.HarvestableObjectController.Broken -= this.HandleBroken;
            }

            base.UnbindInternalReferences();
        }
    }
}
