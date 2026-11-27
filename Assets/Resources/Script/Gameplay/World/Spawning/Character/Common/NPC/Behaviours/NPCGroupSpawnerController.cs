using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCGroupSpawnerController : FOTLMonoBehaviour, IWorldRefreshable
    {
        // --- References ---
        //  1. NPC Group Root
        [Header("References - NPC Group Root")]
        [SerializeField] private Transform _npcGroupRoot;
        protected Transform NPCGroupRoot => _npcGroupRoot;

        //  2. NPC Default Spawn Points
        [Header("References - NPC Default Spawn Points")]
        [SerializeField]
        private List<NPCDefaultSpawnPointController> _npcDefaultSpawnPointControllers = new();
        protected IReadOnlyList<NPCDefaultSpawnPointController> NPCDefaultSpawnPointControllers => _npcDefaultSpawnPointControllers;

        // --- Configs ---
        [Header("Settings")]
        [SerializeField] private bool _isSpawnedNPCsOnStart = true;
        private bool IsSpawnedNPCsOnStart => _isSpawnedNPCsOnStart;

        // --- Runtime ---
        //  1. Spawned NPCs
        [Header("Runtime - NPCs")]
        [SerializeField] private List<GameObject> _spawnedNPCs = new();
        public IReadOnlyList<GameObject> SpawnedNPCs => _spawnedNPCs;

        //  2. NPCs By Identity
        private readonly Dictionary<int, NPCRootController> _spawnedNPCsById = new();

        //  3. Finalized NPCs
        private readonly HashSet<int> _initializedNPCIds = new();

        //  4. Pending Initialization
        private readonly Dictionary<int, Coroutine> _pendingInitializationCoroutines = new();

        private bool _isClearingNPCs;

        // --- Events ---
        public event Action<NPCSpawnedContext> NPCSpawned;
        public event Action SpawnedNPCsClearing;

        // --- Public APIs ---
        //  0. IWorldRefreshable Implementation
        public virtual int WorldRefreshPriority => 100;
        public void RefreshWorldContent(WorldRefreshContext context)
        {
            if (!context.IsPlayerRespawn && !context.IsSaveReload) return;

            this.SpawnNPCs(spawnEntry => this.ShouldSpawnEntryAfterWorldRefresh(spawnEntry, context));
        }

        //  1. Spawning
        public void SpawnNPCsOnStart()
        {
            this.SpawnNPCs(spawnEntry => spawnEntry.SpawnOnStart);
        }
        public bool TryEnsureNPCForWorldRefresh(int npcId, WorldRefreshContext context, out NPCRootController npc)
        {
            npc = null;

            if (!context.IsSaveReload && !context.IsPlayerRespawn) return false;

            // Preserve an existing NPC instance.
            if (this.TryGetSpawnedNPC(npcId, out npc)) return true;

            if (!this.TryGetSpawnEntryByNPCId(npcId, out NPCDefaultSpawnPointController spawnPoint, out NPCSpawnEntry spawnEntry)) return false;

            if (!this.ShouldSpawnEntryAfterWorldRefresh(spawnEntry, context)) return false;

            return this.TrySpawnFromEntry(spawnPoint, spawnEntry, out npc);
        }

        //  2. Runtime NPC Queries
        public bool TryGetSpawnedNPC(int npcId,  out NPCRootController npc)
        {
            npc = null;

            if (npcId <= 0) return false;
            if (!_spawnedNPCsById.TryGetValue(npcId, out NPCRootController spawnedNPC)) return false;

            if (spawnedNPC == null)
            {
                _spawnedNPCsById.Remove(npcId);
                _initializedNPCIds.Remove(npcId);

                return false;
            }

            npc = spawnedNPC;

            return true;
        }
        public bool TryGetInitializedNPC
        (int npcId, out NPCRootController npc)
        {
            npc = null;

            if (!this.TryGetSpawnedNPC(npcId, out NPCRootController spawnedNPC)) return false;
            if (!_initializedNPCIds.Contains(npcId)) return false;

            npc = spawnedNPC;

            return true;
        }

        //  3. Spawn Entry Queries
        public bool TryGetSpawnEntryByNPCId(int npcId, out NPCSpawnEntry spawnEntry)
        {
            return this.TryGetSpawnEntryByNPCId(npcId,out _, out spawnEntry);
        }
        public bool TryGetSpawnEntryByNPCId(int npcId, out NPCDefaultSpawnPointController spawnPoint, out NPCSpawnEntry spawnEntry)
        {
            spawnPoint = null;
            spawnEntry = null;

            if (npcId <= 0) return false;

            return this.TryGetUniqueSpawnEntry
            (
                (point, entry) => entry.NPCProfile.Id == npcId,

                out spawnPoint,
                out spawnEntry
            );
        }
        public bool TryGetSpawnEntryBySpawnPointId(int spawnPointId, out NPCSpawnEntry spawnEntry)
        {
            spawnEntry = null;

            if (spawnPointId <= 0) return false;

            return this.TryGetUniqueSpawnEntry
            (
                (point, entry) => point.Id == spawnPointId,
                out _,
                out spawnEntry
            );
        }

        // --- Helpers ---
        //  1. Spawn Entry Queries
        private bool TryGetUniqueSpawnEntry(Func<NPCDefaultSpawnPointController, NPCSpawnEntry, bool> predicate, out NPCDefaultSpawnPointController spawnPoint, out NPCSpawnEntry spawnEntry)
        {
            spawnPoint = null;
            spawnEntry = null;

            if (predicate == null) return false;
            if (this.NPCDefaultSpawnPointControllers == null) return false;

            foreach (NPCDefaultSpawnPointController candidatePoint in this.NPCDefaultSpawnPointControllers)
            {
                if (candidatePoint == null) continue;
                if (candidatePoint.NPCSpawnEntries == null) continue;

                foreach (NPCSpawnEntry candidateEntry in candidatePoint.NPCSpawnEntries)
                {
                    if (candidateEntry == null) continue;
                    if (!candidateEntry.IsValid) continue;

                    if (!predicate(candidatePoint, candidateEntry)) continue;

                    // Queries must identify a unique entry.
                    if (spawnEntry != null)
                    {
                        Debug.LogError($"[{this.gameObject.name}.{nameof(NPCGroupSpawnerController)}] Multiple matching NPC spawn entries were found.", this);

                        spawnPoint = null;
                        spawnEntry = null;

                        return false;
                    }

                    spawnPoint = candidatePoint;
                    spawnEntry = candidateEntry;
                }
            }

            return spawnEntry != null;
        }

        //  2. General Spawning
        private void SpawnNPCs(Predicate<NPCSpawnEntry> predicate)
        {
            if (_isClearingNPCs) return;

            if (this.NPCGroupRoot == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCGroupSpawnerController)}] Cannot spawn NPCs because NPC Group Root is missing.", this);
                return;
            }

            if (this.NPCDefaultSpawnPointControllers == null) return;

            this.PruneDestroyedNPCs();

            foreach (NPCDefaultSpawnPointController spawnPoint in this.NPCDefaultSpawnPointControllers)
            {
                if (spawnPoint == null) continue;
                if (spawnPoint.NPCSpawnEntries == null) continue;

                foreach (NPCSpawnEntry spawnEntry in spawnPoint.NPCSpawnEntries)
                {
                    if (spawnEntry == null) continue;

                    if (!spawnEntry.IsValid)
                    {
                        Debug.LogWarning($"[{this.gameObject.name}.{nameof(NPCGroupSpawnerController)}] Invalid NPC spawn entry.", spawnPoint);
                        continue;
                    }

                    if (predicate != null && !predicate(spawnEntry)) continue;

                    this.TrySpawnFromEntry(spawnPoint, spawnEntry, out _);
                }
            }
        }

        //  3. Entry Spawning
        protected virtual bool TrySpawnFromEntry(NPCDefaultSpawnPointController spawnPoint, NPCSpawnEntry spawnEntry, out NPCRootController npc)
        {
            npc = null;

            if (_isClearingNPCs) return false;

            if (spawnPoint == null) return false;
            if (spawnEntry == null) return false;
            if (!spawnEntry.IsValid) return false;

            if (this.NPCGroupRoot == null) return false;

            if (!this.isActiveAndEnabled || !this.NPCGroupRoot.gameObject.activeInHierarchy)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCGroupSpawnerController)}] NPC spawner and group root must be active.", this);
                return false;
            }

            NPCProfile npcProfile = spawnEntry.NPCProfile;

            int npcId = npcProfile.Id;

            // Do not create another instance of an existing NPC.
            if (this.TryGetSpawnedNPC(npcId, out npc)) return true;

            // Reject ambiguous duplicate spawn configurations.
            if (!this.TryGetSpawnEntryByNPCId(npcId, out NPCDefaultSpawnPointController configuredPoint, out NPCSpawnEntry configuredEntry)) return false;

            if (configuredPoint != spawnPoint || configuredEntry != spawnEntry) return false;

            GameObject npcPrefab = npcProfile.Prefab;

            // The NPC root must be on the prefab root.
            NPCRootController prefabRoot = npcPrefab.GetComponent<NPCRootController>();
            if (prefabRoot == null)
            {
                Debug.LogError($"[{npcPrefab.name}] Missing {nameof(NPCRootController)} on prefab root.", npcPrefab);
                return false;
            }

            // NPC Profiles are serialized on their own prefabs.
            if (prefabRoot.NPCProfile != npcProfile)
            {
                Debug.LogError($"[{npcPrefab.name}] NPC Profile does not match the assigned NPC Spawn Entry.", npcPrefab);
                return false;
            }

            if (!npcPrefab.activeSelf || !prefabRoot.enabled)
            {
                Debug.LogError($"[{npcPrefab.name}] NPC prefab and root controller must be active.", npcPrefab);
                return false;
            }

            if (npcPrefab.GetComponentInChildren<CharacterRefreshHandler>(true) == null)
            {
                Debug.LogError($"[{npcPrefab.name}] Missing {nameof(CharacterRefreshHandler)}.", npcPrefab);
                return false;
            }

            // Ensure the SpawnAnchor is ready before reading it.
            spawnPoint.EnsureSpawnAnchorInitialized();

            CharacterRefreshTarget spawnResult = spawnEntry.ResolveSpawnResult(spawnPoint);

            // Instantiate at the correct world-space transform.
            GameObject spawnedNPC = UnityEngine.Object.Instantiate
            (
                original:   npcPrefab, 
                position:   spawnResult.Position, 
                rotation:   spawnResult.Rotation, 
                parent:     this.NPCGroupRoot
            );

            if (spawnedNPC == null) return false;
            spawnedNPC.name = this.GetSpawnedNPCGameObjectName(npcPrefab);

            npc = spawnedNPC.GetComponent<NPCRootController>();
            if (npc == null)
            {
                spawnedNPC.SetActive(false);
                UnityEngine.Object.Destroy(spawnedNPC);

                return false;
            }

            this.RegisterSpawnedNPC(npcId, spawnedNPC, npc);

            this.ConfigureSpawnedNPC(npc, spawnPoint, spawnEntry);

            this.ScheduleNPCInitialization(spawnPoint, spawnEntry, npc);

            return true;
        }

        //  4. Spawned NPC Configuration
        protected virtual void ConfigureSpawnedNPC(NPCRootController npc, NPCDefaultSpawnPointController spawnPoint, NPCSpawnEntry spawnEntry)
        {
            // Reserved for future NPC-specific spawn configuration.
            //
            // No variant-specific visual or inventory configuration is required for the current NPC system.
        }

        private string GetSpawnedNPCGameObjectName(GameObject npcPrefab)
        {
            if (npcPrefab == null) return "GO_Chr_NPC";

            string npcName = npcPrefab.name;

            const string prefabPrefix = "Prefab_";
            const string gameObjectPrefix = "GO_";

            if (npcName.StartsWith(prefabPrefix, StringComparison.Ordinal))
            {
                npcName = npcName.Substring(prefabPrefix.Length);
            }
            else if (npcName.StartsWith(gameObjectPrefix, StringComparison.Ordinal))
            {
                npcName = npcName.Substring(gameObjectPrefix.Length);
            }

            return $"{gameObjectPrefix}{npcName}";
        }

        //  5. Spawn Initialization
        private void ScheduleNPCInitialization(NPCDefaultSpawnPointController spawnPoint, NPCSpawnEntry spawnEntry, NPCRootController npc)
        {
            if (npc == null) return;

            int npcId = npc.Id;

            if (_pendingInitializationCoroutines.TryGetValue(npcId, out Coroutine previousCoroutine))
            {
                if (previousCoroutine != null)
                {
                    this.StopCoroutine(previousCoroutine);
                }

                _pendingInitializationCoroutines.Remove(npcId);
            }

            Coroutine initializationCoroutine = this.StartCoroutine(this.InitializeSpawnedNPC(spawnPoint, spawnEntry, npc));

            _pendingInitializationCoroutines[npcId] = initializationCoroutine;
        }
        private IEnumerator InitializeSpawnedNPC(NPCDefaultSpawnPointController spawnPoint, NPCSpawnEntry spawnEntry, NPCRootController npc)
        {
            int npcId = npc.Id;

            // Give the newly spawned NPC an opportunity to complete
            // its Start callbacks and external reference caching.
            yield return null;

            _pendingInitializationCoroutines.Remove(npcId);

            if (npc == null) yield break;
            if (spawnPoint == null) yield break;
            if (spawnEntry == null) yield break;

            if (!this.TryGetSpawnedNPC(npcId, out NPCRootController registeredNPC)) yield break;

            if (registeredNPC != npc) yield break;

            CharacterRefreshHandler refreshHandler = npc.RefreshHandler;
            if (refreshHandler == null) refreshHandler = npc.GetComponentInChildren<CharacterRefreshHandler>(true);
            if (refreshHandler == null)
            {
                Debug.LogError($"[{npc.gameObject.name}] Missing {nameof(CharacterRefreshHandler)}.", npc);

                this.RemoveFailedNPC(npcId, npc);

                yield break;
            }

            spawnPoint.EnsureSpawnAnchorInitialized();

            CharacterRefreshTarget spawnResult = spawnEntry.ResolveSpawnResult(spawnPoint);

            refreshHandler.Refresh(spawnResult, CharacterRefreshOrigin.DefaultSpawnPoint);

            if (npc == null) yield break;
            if (!this.TryGetSpawnedNPC(npcId, out registeredNPC) || registeredNPC != npc) yield break;

            _initializedNPCIds.Add(npcId);

            this.NotifyNPCSpawned(spawnPoint, spawnEntry, npc);
        }

        //  6. Spawn Notification
        protected virtual void NotifyNPCSpawned(NPCDefaultSpawnPointController spawnPoint, NPCSpawnEntry spawnEntry, NPCRootController npc)
        {
            if (spawnPoint == null) return;
            if (spawnEntry == null) return;
            if (npc == null) return;

            NPCSpawnedContext context = new NPCSpawnedContext(spawnPoint, spawnEntry, npc);

            this.NPCSpawned?.Invoke(context);
        }

        //  7. Runtime Registration
        private void RegisterSpawnedNPC(int npcId, GameObject spawnedNPC, NPCRootController npc)
        {
            if (spawnedNPC == null) return;
            if (npc == null) return;

            _spawnedNPCs.Add(spawnedNPC);

            _spawnedNPCsById[npcId] = npc;

            _initializedNPCIds.Remove(npcId);
        }
        private void RemoveFailedNPC(int npcId, NPCRootController npc)
        {
            _spawnedNPCsById.Remove(npcId);
            _initializedNPCIds.Remove(npcId);

            if (npc == null) return;

            GameObject npcObject = npc.gameObject;

            _spawnedNPCs.Remove(npcObject);

            npcObject.SetActive(false);

            UnityEngine.Object.Destroy(npcObject);
        }
        private void PruneDestroyedNPCs()
        {
            _spawnedNPCs.RemoveAll(spawnedNPC => spawnedNPC == null);

            List<int> destroyedNPCIds = new();

            foreach (KeyValuePair<int, NPCRootController> pair in _spawnedNPCsById)
            {
                if (pair.Value != null) continue;

                destroyedNPCIds.Add(pair.Key);
            }

            foreach (int npcId in destroyedNPCIds)
            {
                _spawnedNPCsById.Remove(npcId);

                _initializedNPCIds.Remove(npcId);

                if (_pendingInitializationCoroutines.TryGetValue(npcId, out Coroutine coroutine))
                {
                    if (coroutine != null)
                    {
                        this.StopCoroutine(coroutine);
                    }

                    _pendingInitializationCoroutines.Remove(npcId);
                }
            }
        }
        protected void ClearSpawnedNPCs()
        {
            if (_isClearingNPCs) return;

            _isClearingNPCs = true;

            try
            {
                // Notify observers before destroying NPC instances.
                this.SpawnedNPCsClearing?.Invoke();

                foreach (Coroutine coroutine in _pendingInitializationCoroutines.Values)
                {
                    if (coroutine == null) continue;

                    this.StopCoroutine(coroutine);
                }

                _pendingInitializationCoroutines.Clear();

                for (int i = _spawnedNPCs.Count - 1; i >= 0; i--)
                {
                    GameObject spawnedNPC = _spawnedNPCs[i];

                    if (spawnedNPC == null) continue;

                    spawnedNPC.SetActive(false);

                    UnityEngine.Object.Destroy(spawnedNPC);
                }

                _spawnedNPCs.Clear();
                _spawnedNPCsById.Clear();
                _initializedNPCIds.Clear();
            }
            finally
            {
                _isClearingNPCs = false;
            }
        }

        //  8. World Refresh
        protected virtual bool ShouldSpawnEntryAfterWorldRefresh(NPCSpawnEntry spawnEntry, WorldRefreshContext context)
        {
            if (spawnEntry == null) return false;
            if (!spawnEntry.IsValid) return false;

            if (context.IsSaveReload) return spawnEntry.SpawnWhenSaveReloads;

            if (context.IsPlayerRespawn) return spawnEntry.SpawnWhenPlayerRespawns;

            return false;
        }

        //  9. Validation Helper
        private void ValidateUniqueNPCIds()
        {
            if (this.NPCDefaultSpawnPointControllers == null) return;

            HashSet<int> npcIds = new();

            foreach (NPCDefaultSpawnPointController spawnPoint in this.NPCDefaultSpawnPointControllers)
            {
                if (spawnPoint == null) continue;
                if (spawnPoint.NPCSpawnEntries == null) continue;

                foreach (NPCSpawnEntry spawnEntry in spawnPoint.NPCSpawnEntries)
                {
                    if (spawnEntry == null) continue;
                    if (!spawnEntry.IsValid) continue;

                    int npcId = spawnEntry.NPCProfile.Id;

                    if (npcIds.Add(npcId)) continue;

                    Debug.LogError($"[{this.gameObject.name}.{nameof(NPCGroupSpawnerController)}] Duplicate NPC Profile ID: {npcId}.", this);
                }
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.NPCDefaultSpawnPointControllers == null || this.NPCDefaultSpawnPointControllers.Count == 0)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(NPCGroupSpawnerController)}] No NPC Default Spawn Points assigned.", this);
            }

            this.ValidateUniqueNPCIds();
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _spawnedNPCs.Clear();
            _spawnedNPCsById.Clear();
            _initializedNPCIds.Clear();
            _pendingInitializationCoroutines.Clear();

            // Runs during Awake, not Start.
            if (!this.IsSpawnedNPCsOnStart) return;

            this.SpawnNPCsOnStart();
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.NPCGroupRoot == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCGroupSpawnerController)}] NPC Group Root is missing.", this);
            }
        }
        protected override void CleanupStates()
        {
            this.ClearSpawnedNPCs();

            base.CleanupStates();
        }
    }
}