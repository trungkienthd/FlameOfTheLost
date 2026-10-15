using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class RuntimeCollectibleRefreshController : FOTLMonoBehaviour,
        IWorldRefreshable
    {
        // --- Runtime ---
        //  1. Runtime-Generated Collectibles
        private readonly HashSet<CollectibleObjectController> _runtimeCollectibles = new();

        // --- Public APIs ---
        //  1. Registration
        public void RegisterRuntimeCollectible(CollectibleObjectController collectible)
        {
            if (collectible == null) return;

            _runtimeCollectibles.Add(collectible);
        }

        public void UnregisterRuntimeCollectible(CollectibleObjectController collectible)
        {
            if (collectible == null) return;

            _runtimeCollectibles.Remove(collectible);
        }

        //  2. IWorldRefreshable implementation
        public int WorldRefreshPriority => 10;
        public void RefreshWorldContent(WorldRefreshContext context)
        {
            if (!context.IsPlayerRespawn && !context.IsSaveReload) return;

            foreach (CollectibleObjectController collectible in _runtimeCollectibles)
            {
                if (collectible == null) continue;

                collectible.gameObject.SetActive(false);

                UnityEngine.Object.Destroy(collectible.gameObject);
            }

            _runtimeCollectibles.Clear();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _runtimeCollectibles.Clear();
        }
    }
}