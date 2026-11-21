using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace FlameOfTheLost
{
    public class EnvironmentObjectGroupSpawnerController<TProfile, TEntry> : FOTLWorldObjectProfileBehaviour<TProfile>, IWorldRefreshable
        where TProfile : EnvironmentObjectGroupSpawnerProfile<TEntry>
        where TEntry : EnvironmentObjectSpawnEntry
    {
        // --- References ---
        //  1. Environment Object Group Root
        [Header("References - Environment Object Group Root")]
        [SerializeField] private Transform _environmentObjectGroupRoot;
        protected Transform EnvironmentObjectGroupRoot => _environmentObjectGroupRoot;

        // --- Data / Configs ---
        //  1. Spawn Entries
        protected IReadOnlyList<TEntry> SpawnEntries => this.Profile != null
            ? this.Profile.SpawnEntries
            : null;

        //  2. Spawn Settings
        protected bool SpawnObjectsOnStart => this.Profile != null
            && this.Profile.SpawnObjectsOnStart;

        // --- Runtime ---
        //  1. Spawned Objects
        [Header("Runtime - Spawned Objects")]
        [SerializeField] private List<GameObject> _spawnedObjects = new();
        protected List<GameObject> SpawnedObjects => _spawnedObjects;

        //  2. Spawned Objects By Entry
        private readonly Dictionary<TEntry, GameObject> _spawnedObjectsByEntry = new();
        protected IReadOnlyDictionary<TEntry, GameObject> SpawnedObjectsByEntry => _spawnedObjectsByEntry;

        // --- Events ---
        public event Action<TEntry, GameObject> ObjectSpawned;

        // --- Public APIs ---
        //  1. IWorldRefreshable implementation
        public virtual int WorldRefreshPriority => 20;
        public void RefreshWorldContent(WorldRefreshContext context)
        {
            this.ClearSpawnedObjects();

            if (!this.ShouldSpawnObjectsAfterWorldRefresh(context)) return;

            this.SpawnObjects(entry => this.ShouldSpawnEntryAfterWorldRefresh(entry, context));
        }

        //  2. Initial Spawning
        public void SpawnObjectsAtStart()
        {
            this.ClearSpawnedObjects();

            this.SpawnObjects(entry => entry.SpawnOnStart);
        }

        //  3. Spawned Object Queries
        public bool TryGetSpawnedObject(TEntry spawnEntry, out GameObject spawnedObject)
        {
            spawnedObject = null;

            if (spawnEntry == null) return false;
            if (!this.SpawnedObjectsByEntry.TryGetValue(spawnEntry, out GameObject candidate)) return false;
            if (candidate == null) return false;

            spawnedObject = candidate;
            return true;
        }

        // --- Helpers ---
        //  1. General Spawning
        private void SpawnObjects(Predicate<TEntry> predicate)
        {
            if (this.Profile == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Cannot spawn environment objects because no profile is assigned.", this);
                return;
            }
            if (this.EnvironmentObjectGroupRoot == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Cannot spawn environment objects because no object group root is assigned.", this);
                return;
            }
            if (this.SpawnEntries == null) return;

            for (int i = 0; i < this.SpawnEntries.Count; i++)
            {
                TEntry spawnEntry = this.SpawnEntries[i];

                if (spawnEntry == null) continue;
                if (!spawnEntry.IsValid)
                {
                    Debug.LogWarning($"[{this.gameObject.name}.{this.GetType().Name}] Invalid environment object spawn entry at index {i}.", this);
                    continue;
                }

                if (predicate != null && !predicate(spawnEntry)) continue;

                this.SpawnFromEntry(spawnEntry);
            }
        }

        //  2. Entry Spawning
        protected virtual GameObject SpawnFromEntry(TEntry spawnEntry)
        {
            if (spawnEntry == null || !spawnEntry.IsValid) return null;
            if (this.EnvironmentObjectGroupRoot == null) return null;

            // Avoid creating duplicate instances for the same spawn entry.
            if (this.TryGetSpawnedObject(spawnEntry, out GameObject existingObject)) return existingObject;

            GameObject spawnedObject = UnityEngine.Object.Instantiate
            (
                original: spawnEntry.Prefab,
                position: spawnEntry.InitialPosition,
                rotation: spawnEntry.InitialRotation,
                parent: this.EnvironmentObjectGroupRoot
            );

            if (spawnedObject == null) return null;

            // Apply the initial scale configured in the spawn entry.
            spawnedObject.transform.localScale = spawnEntry.InitialLocalScale;

            // Register the instance before calling configuration hooks.
            this.RegisterSpawnedObject(spawnEntry, spawnedObject);

            // Allow derived spawners to configure object-specific properties.
            this.ConfigureSpawnedObject(spawnedObject, spawnEntry);

            this.NotifyObjectSpawned(spawnEntry, spawnedObject);

            return spawnedObject;
        }

        //  3. Spawned Object Configuration
        protected virtual void ConfigureSpawnedObject(GameObject spawnedObject, TEntry spawnEntry)
        {
        }

        //  4. Spawn Notification
        protected virtual void NotifyObjectSpawned(TEntry spawnEntry, GameObject spawnedObject)
        {
            if (spawnEntry == null) return;
            if (spawnedObject == null) return;

            this.ObjectSpawned?.Invoke(spawnEntry, spawnedObject);
        }

        //  5. Runtime Registration
        private void RegisterSpawnedObject(TEntry spawnEntry, GameObject spawnedObject)
        {
            if (spawnEntry == null) return;
            if (spawnedObject == null) return;

            this.SpawnedObjects.Add(spawnedObject);
            _spawnedObjectsByEntry[spawnEntry] = spawnedObject;
        }

        //  6. Cleanup
        protected void ClearSpawnedObjects()
        {
            if (this.EnvironmentObjectGroupRoot != null)
            {
                for (int i = this.EnvironmentObjectGroupRoot.childCount - 1; i >= 0; i--)
                {
                    Transform child = this.EnvironmentObjectGroupRoot.GetChild(i);
                    if (child == null) continue;

                    GameObject childObject = child.gameObject;

                    // Immediately disable the object to prevent interactions.
                    childObject.SetActive(false);

                    // Detach from the group root before destruction.
                    child.SetParent(null, true);

                    // Destroy at the end of the current frame.
                    UnityEngine.Object.Destroy(childObject);
                }
            }

            // Clear runtime registrations.
            this.SpawnedObjects.Clear();
            _spawnedObjectsByEntry.Clear();
        }

        //  7. Refresh Hooks
        protected virtual bool ShouldSpawnObjectsAfterWorldRefresh(WorldRefreshContext context)
        {
            return context.IsPlayerRespawn || context.IsSaveReload;
        }
        protected virtual bool ShouldSpawnEntryAfterWorldRefresh(TEntry spawnEntry, WorldRefreshContext context)
        {
            if (spawnEntry == null) return false;

            return spawnEntry.SpawnOnWorldRefresh;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            if (_spawnedObjects == null)
            {
                _spawnedObjects = new List<GameObject>();
            }
            else
            {
                _spawnedObjects.Clear();
            }

            _spawnedObjectsByEntry.Clear();
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.EnvironmentObjectGroupRoot == null) Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(this.EnvironmentObjectGroupRoot)} reference is missing.", this);
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            if (!this.SpawnObjectsOnStart) return;

            this.SpawnObjectsAtStart();
        }
    }
}
