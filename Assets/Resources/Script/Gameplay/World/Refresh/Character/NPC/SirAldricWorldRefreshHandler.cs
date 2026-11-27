using UnityEngine;

namespace FlameOfTheLost
{
    public class SirAldricWorldRefreshHandler : FOTLMonoBehaviour, IWorldRefreshable
    {
        // --- References ---
        //  1. NPC Group Spawner
        [Header("References - NPC Spawning")]
        [SerializeField]
        private NPCGroupSpawnerController _npcGroupSpawner;

        private NPCGroupSpawnerController NPCGroupSpawner
            => _npcGroupSpawner;

        //  2. Sir Aldric
        [Header("References - Sir Aldric")]
        [SerializeField] private SirAldricBrainController _sirAldricBrain;
        public SirAldricBrainController SirAldricBrain => _sirAldricBrain;

        // --- Constants ---
        private const int SIR_ALDRIC_NPC_ID = 14201;

        // --- Runtime ---
        [Header("Runtime")]
        [SerializeField] private bool _isSynchronizationPending;
        public bool IsSynchronizationPending => _isSynchronizationPending;

        private NPCGroupSpawnerController _boundNPCGroupSpawner;

        // --- Public APIs ---
        //  0. IWorldRefreshable Implementation
        public int WorldRefreshPriority => 40;
        public void RefreshWorldContent(WorldRefreshContext context)
        {
            if (!context.IsSaveReload && !context.IsPlayerRespawn) return;

            this.ResolveNPCGroupSpawnerReference();

            if (this.NPCGroupSpawner == null) return;

            this.BindNPCGroupSpawnerEvents();

            if (!this.NPCGroupSpawner.TryEnsureNPCForWorldRefresh(SirAldricWorldRefreshHandler.SIR_ALDRIC_NPC_ID, context, out NPCRootController npc))
            {
                _sirAldricBrain = null;
                _isSynchronizationPending = false;

                return;
            }

            this.ResolveSirAldricBrain(npc);

            // If full Character Refresh is still pending, synchronize when the NPCSpawned event is raised.
            if (!this.NPCGroupSpawner.TryGetInitializedNPC(SirAldricWorldRefreshHandler.SIR_ALDRIC_NPC_ID, out NPCRootController initializedNPC))
            {
                _isSynchronizationPending = true;

                return;
            }

            this.ResolveSirAldricBrain(initializedNPC);

            this.SynchronizeSirAldric();
        }

        // --- Helpers ---
        //  1. Reference Resolution
        private void ResolveNPCGroupSpawnerReference()
        {
            if (this.NPCGroupSpawner != null) return;

            _npcGroupSpawner = UnityEngine.Object.FindFirstObjectByType<NPCGroupSpawnerController>(FindObjectsInactive.Include);
        }
        private void ResolveSirAldricBrain()
        {
            _sirAldricBrain = null;

            if (this.NPCGroupSpawner == null) return;
            if (!this.NPCGroupSpawner.TryGetSpawnedNPC(SirAldricWorldRefreshHandler.SIR_ALDRIC_NPC_ID, out NPCRootController npc)) return;

            this.ResolveSirAldricBrain(npc);
        }
        private void ResolveSirAldricBrain(NPCRootController npc)
        {
            _sirAldricBrain = null;

            if (npc == null) return;
            if (npc.Id != SirAldricWorldRefreshHandler.SIR_ALDRIC_NPC_ID) return;

            _sirAldricBrain = npc.NPCBrainController as SirAldricBrainController;
            if (this.SirAldricBrain == null) _sirAldricBrain = npc.GetComponentInChildren<SirAldricBrainController>(true);
        }

        //  2. World Synchronization
        private void SynchronizeSirAldric()
        {
            _isSynchronizationPending = false;

            if (this.SirAldricBrain == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(SirAldricWorldRefreshHandler)}] Cannot synchronize Sir Aldric because his Brain Controller is missing.", this);
                return;
            }

            this.SirAldricBrain.SynchronizeAfterWorldRestore();
        }

        //  3. NPC Spawning Events
        private void HandleNPCSpawned(NPCSpawnedContext context)
        {
            NPCRootController npc = context.NPC;

            if (npc == null) return;
            if (npc.Id != SirAldricWorldRefreshHandler.SIR_ALDRIC_NPC_ID) return;

            this.ResolveSirAldricBrain(npc);

            if (!this.IsSynchronizationPending) return;

            this.SynchronizeSirAldric();
        }
        private void HandleSpawnedNPCsClearing()
        {
            _sirAldricBrain = null;

            _isSynchronizationPending = false;
        }

        //  4. Event Binding
        private void BindNPCGroupSpawnerEvents()
        {
            if (_boundNPCGroupSpawner == this.NPCGroupSpawner) return;

            this.UnbindNPCGroupSpawnerEvents();

            if (this.NPCGroupSpawner == null) return;

            _boundNPCGroupSpawner = this.NPCGroupSpawner;
            _boundNPCGroupSpawner.NPCSpawned += this.HandleNPCSpawned;
            _boundNPCGroupSpawner.SpawnedNPCsClearing += this.HandleSpawnedNPCsClearing;
        }
        private void UnbindNPCGroupSpawnerEvents()
        {
            if (_boundNPCGroupSpawner != null)
            {
                _boundNPCGroupSpawner.NPCSpawned -= this.HandleNPCSpawned;
                _boundNPCGroupSpawner.SpawnedNPCsClearing -= this.HandleSpawnedNPCsClearing;
            }

            _boundNPCGroupSpawner = null;
        }


        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.SirAldricBrain == null)
            {
                _sirAldricBrain = UnityEngine.Object.FindFirstObjectByType<SirAldricBrainController>(FindObjectsInactive.Include);
            }
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.SirAldricBrain == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(SirAldricWorldRefreshHandler)}] Sir Aldric Brain reference is missing.", this);
            }
        }
    }
}