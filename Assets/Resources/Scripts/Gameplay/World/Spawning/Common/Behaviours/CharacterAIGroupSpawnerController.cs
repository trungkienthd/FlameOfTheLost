using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterAIGroupSpawnerController : FOTLMonoBehaviour
    {
        // --- References ---
        //  1. Player Spawner
        [Header("References - Player Spawner")]
        [SerializeField] private PlayerSpawnerController _playerSpawnerController;
        protected PlayerSpawnerController PlayerSpawnerController => _playerSpawnerController;

        //  2. Character AI Group Root
        [Header("References - Character AI Group Root")]
        [SerializeField] private Transform _characterAIGroupRoot;
        protected Transform CharacterAIGroupRoot => _characterAIGroupRoot;

        //  3. Character AI Default Spawn Points
        protected abstract IReadOnlyList<CharacterAIDefaultSpawnPointController> CharacterAIDefaultSpawnPointControllers { get; }
        protected abstract string CharacterAIGroupRootName { get; }

        // --- Configs ---
        [Header("Settings")]
        [SerializeField] private bool _isSpawnedCharactersOnStart = true;
        [SerializeField] private bool _isRespawnedCharactersWhenPlayerSpawnsOrRespawns = true;
        private bool IsSpawnedCharactersOnStart => _isSpawnedCharactersOnStart;
        private bool IsRespawnedCharactersWhenPlayerSpawnsOrRespawns => _isRespawnedCharactersWhenPlayerSpawnsOrRespawns;

        // --- Runtime ---
        //  1. Spawned Characters
        [Header("Runtime - Character AI (GameObject)")]
        [SerializeField] private List<GameObject> _spawnedCharacters = new();
        protected List<GameObject> SpawnedCharacters => _spawnedCharacters;

        //  2. Spawned Characters By Entry
        private readonly Dictionary<CharacterAISpawnEntry, List<GameObject>> _spawnedCharactersByEntry = new();

        private IReadOnlyDictionary<CharacterAISpawnEntry, List<GameObject>> SpawnedCharactersByEntry => _spawnedCharactersByEntry;

        //  3. External Binding
        private bool _hasBoundExternalReferences;
        private bool HasBoundExternalReferences
        {
            get => _hasBoundExternalReferences;
            set => _hasBoundExternalReferences = value;
        }

        // --- Events ---
        public event Action<CharacterAISpawnEntry, CharacterRootController> CharacterAISpawned;

        // --- Unity Callbacks ---
        private void OnEnable()
        {
            if (!this.HasBoundExternalReferences) return;

            this.SubscribeToPlayerSpawner();
        }
        private void OnDisable()
        {
            this.UnsubscribeFromPlayerSpawner();
        }

        // --- Public APIs ---
        //  1. Spawn Entry Queries
        public bool TryGetFirstCharacterAISpawnEntry(out CharacterAISpawnEntry spawnEntry)
        {
            spawnEntry = null;

            if (this.CharacterAIDefaultSpawnPointControllers == null) return false;

            for (int i = 0; i < this.CharacterAIDefaultSpawnPointControllers.Count; i++)
            {
                CharacterAIDefaultSpawnPointController spawnPoint = this.CharacterAIDefaultSpawnPointControllers[i];

                if (spawnPoint == null) continue;

                foreach (CharacterAISpawnEntry candidate in this.GetSpawnEntries(spawnPoint))
                {
                    if (candidate == null) continue;
                    if (!candidate.IsValid) continue;

                    spawnEntry = candidate;
                    return true;
                }
            }

            return false;
        }

        //  2. Runtime Character Queries
        public bool TryGetFirstSpawnedCharacter
        (
            CharacterAISpawnEntry spawnEntry,
            out CharacterRootController character
        )
        {
            character = null;

            if (spawnEntry == null) return false;
            if (!this.SpawnedCharactersByEntry.TryGetValue(spawnEntry, out List<GameObject> spawnedCharacters)) return false;

            for (int i = 0; i < spawnedCharacters.Count; i++)
            {
                GameObject spawnedCharacter = spawnedCharacters[i];

                if (spawnedCharacter == null) continue;

                CharacterRootController rootController = spawnedCharacter.GetComponentInChildren<CharacterRootController>(true);

                if (rootController == null) continue;

                character = rootController;
                return true;
            }

            return false;
        }

        //  3. Spawning
        public void SpawnCharactersOnStart()
        {
            this.SpawnCharacters(spawnEntry => spawnEntry.SpawnOnStart);
        }

        public void SpawnCharactersAfterPlayerRespawn()
        {
            this.SpawnCharacters(spawnEntry => spawnEntry.SpawnWhenPlayerRespawns);
        }

        // --- Helpers ---
        //  0. Hooks 
        protected virtual IEnumerable<CharacterAISpawnEntry> GetSpawnEntries(CharacterAIDefaultSpawnPointController spawnPoint)
        {
            if (spawnPoint == null) yield break;
            if (spawnPoint.CharacterAISpawnEntries == null) yield break;

            IReadOnlyList<CharacterAISpawnEntry> spawnEntries = spawnPoint.CharacterAISpawnEntries;

            for (int i = 0; i < spawnEntries.Count; i++)
            {
                CharacterAISpawnEntry spawnEntry = spawnEntries[i];

                if (spawnEntry == null) continue;

                yield return spawnEntry;
            }
        }
        protected virtual void ConfigureSpawnedCharacter(GameObject spawnedCharacter, CharacterAIDefaultSpawnPointController spawnPoint, CharacterAISpawnEntry spawnEntry)
        {
            if (spawnedCharacter == null) return;
            if (spawnPoint == null) return;
            if (spawnEntry == null) return;

            this.ConfigureSpawnedCharacterInventory(spawnedCharacter, spawnEntry);
        }
        protected virtual void NotifyCharacterAISpawned(CharacterAIDefaultSpawnPointController spawnPoint, CharacterAISpawnEntry spawnEntry, CharacterRootController character)
        {
            if (spawnEntry == null) return;
            if (character == null) return;

            this.CharacterAISpawned?.Invoke(spawnEntry, character);
        }

        //  1. General Spawning
        private void SpawnCharacters(Predicate<CharacterAISpawnEntry> predicate)
        {
            if (this.CharacterAIDefaultSpawnPointControllers == null) return;

            foreach (CharacterAIDefaultSpawnPointController spawnPoint in this.CharacterAIDefaultSpawnPointControllers)
            {
                if (spawnPoint == null) continue;

                foreach (CharacterAISpawnEntry spawnEntry in this.GetSpawnEntries(spawnPoint))
                {
                    if (spawnEntry == null) continue;
                    if (predicate != null && !predicate(spawnEntry)) continue;

                    this.SpawnFromEntry(spawnPoint, spawnEntry);
                }
            }
        }

        //  2. Entry Spawning
        protected virtual void SpawnFromEntry(CharacterAIDefaultSpawnPointController spawnPoint, CharacterAISpawnEntry spawnEntry)
        {
            if (spawnPoint == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(CharacterAIGroupSpawnerController)}] Character AI default spawn point is null.", this);
                return;
            }

            if (spawnEntry == null || !spawnEntry.IsValid)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(CharacterAIGroupSpawnerController)}] Invalid Character AI spawn entry.", this);
                return;
            }

            this.PruneDestroyedSpawnedCharacters();

            int currentSpawnedCount = this.GetSpawnedCharacterCount(spawnEntry);
            int remainingSpawnCount = spawnEntry.SpawnCount - currentSpawnedCount;
            if (remainingSpawnCount <= 0) return;

            for (int i = 0; i < remainingSpawnCount; i++)
            {
                this.SpawnSingleCharacter(spawnPoint, spawnEntry);
            }
        }
        private GameObject SpawnSingleCharacter(CharacterAIDefaultSpawnPointController spawnPoint, CharacterAISpawnEntry spawnEntry)
        {
            if (spawnPoint == null) return null;
            if (spawnEntry == null) return null;

            CharacterAIVariantProfile variant = spawnEntry.CharacterAIVariant;

            if (variant == null) return null;
            if (variant.CharacterProfile == null) return null;
            if (variant.CharacterProfile.Prefab == null) return null;

            GameObject spawnedCharacter = UnityEngine.Object.Instantiate(variant.CharacterProfile.Prefab, this.CharacterAIGroupRoot);

            if (spawnedCharacter == null) return null;

            this.RegisterSpawnedCharacter(spawnEntry, spawnedCharacter);

            this.ConfigureSpawnedCharacter(spawnedCharacter, spawnPoint, spawnEntry);

            CharacterRefreshHandler refreshHandler = spawnedCharacter.GetComponentInChildren<CharacterRefreshHandler>(true);

            if (refreshHandler == null)
            {
                Debug.LogWarning($"[{spawnedCharacter.name}.{nameof(CharacterAIGroupSpawnerController)}] Spawned Character AI has no {nameof(CharacterRefreshHandler)}.", spawnedCharacter);

                this.NotifySpawnedCharacter(spawnPoint, spawnEntry, spawnedCharacter);

                return spawnedCharacter;
            }

            CharacterRefreshTarget spawnResult = spawnEntry.ResolveSpawnResult(spawnPoint);

            refreshHandler.Refresh(spawnResult, CharacterRefreshOrigin.DefaultSpawnPoint);

            this.NotifySpawnedCharacter(spawnPoint, spawnEntry, spawnedCharacter);

            return spawnedCharacter;
        }

        //  3. Configuration
        private void ConfigureSpawnedCharacterInventory(GameObject spawnedCharacter, CharacterAISpawnEntry spawnEntry)
        {
            CharacterRootController rootController = spawnedCharacter.GetComponentInChildren<CharacterRootController>(true);
            if (rootController == null)
            {
                Debug.LogWarning($"[{spawnedCharacter.name}.{nameof(CharacterAIGroupSpawnerController)}] Spawned Character AI has no {nameof(CharacterRootController)}.", spawnedCharacter);
                return;
            }

            InventoryController inventoryController = rootController.InventoryController;
            if (inventoryController == null)
            {
                Debug.LogWarning($"[{spawnedCharacter.name}.{nameof(CharacterAIGroupSpawnerController)}] Spawned Character AI has no {nameof(InventoryController)}.", spawnedCharacter);
                return;
            }

            CharacterAIVariantProfile variant = spawnEntry.CharacterAIVariant;
            if (variant == null) return;

            inventoryController.InitializeInventoryEntries(variant.InitialInventoryEntries);
        }

        //  4. Notification
        private void NotifySpawnedCharacter(CharacterAIDefaultSpawnPointController spawnPoint, CharacterAISpawnEntry spawnEntry, GameObject spawnedCharacter)
        {
            if (spawnPoint == null) return;
            if (spawnEntry == null) return;
            if (spawnedCharacter == null) return;

            CharacterRootController character = spawnedCharacter.GetComponentInChildren<CharacterRootController>(true);
            if (character == null)
            {
                Debug.LogWarning($"[{spawnedCharacter.name}.{nameof(CharacterAIGroupSpawnerController)}] Spawned Character AI has no {nameof(CharacterRootController)}.", spawnedCharacter);
                return;
            }

            this.NotifyCharacterAISpawned(spawnPoint, spawnEntry, character);
        }

        //  5. Runtime Registration
        protected void ClearSpawnedCharacters()
        {
            for (int i = this.SpawnedCharacters.Count - 1; i >= 0; i--)
            {
                GameObject spawnedCharacter = this.SpawnedCharacters[i];

                if (spawnedCharacter != null) UnityEngine.Object.Destroy(spawnedCharacter);
            }

            this.SpawnedCharacters.Clear();
            _spawnedCharactersByEntry.Clear();
        }
        private void RegisterSpawnedCharacter(CharacterAISpawnEntry spawnEntry, GameObject spawnedCharacter)
        {
            if (spawnEntry == null) return;
            if (spawnedCharacter == null) return;

            this.SpawnedCharacters.Add(spawnedCharacter);

            if (!this.SpawnedCharactersByEntry.TryGetValue(spawnEntry, out List<GameObject> spawnedCharacters))
            {
                spawnedCharacters = new List<GameObject>();

                _spawnedCharactersByEntry.Add(spawnEntry, spawnedCharacters);
            }

            spawnedCharacters.Add(spawnedCharacter);
        }
        private int GetSpawnedCharacterCount(CharacterAISpawnEntry spawnEntry)
        {
            if (spawnEntry == null) return 0;
            if (!this.SpawnedCharactersByEntry.TryGetValue(spawnEntry, out List<GameObject> spawnedCharacters)) return 0;
            
            spawnedCharacters.RemoveAll(spawnedCharacter => spawnedCharacter == null);

            return spawnedCharacters.Count;
        }
        private void PruneDestroyedSpawnedCharacters()
        {
            this.SpawnedCharacters.RemoveAll(spawnedCharacter => spawnedCharacter == null);

            List<CharacterAISpawnEntry> emptyEntries = new();

            foreach (KeyValuePair<CharacterAISpawnEntry, List<GameObject>> pair in this.SpawnedCharactersByEntry)
            {
                pair.Value.RemoveAll (spawnedCharacter => spawnedCharacter == null);

                if (pair.Value.Count <= 0)
                {
                    emptyEntries.Add(pair.Key);
                }
            }

            foreach (CharacterAISpawnEntry emptyEntry in emptyEntries)
            {
                _spawnedCharactersByEntry.Remove(emptyEntry);
            }
        }

        //  6. Player Respawn
        private void HandlePlayerSpawnedOrRespawned()
        {
            if (!this.IsRespawnedCharactersWhenPlayerSpawnsOrRespawns) return;

            this.SpawnCharactersAfterPlayerRespawn();
        }
        private void SubscribeToPlayerSpawner()
        {
            if (this.PlayerSpawnerController == null) return;

            this.PlayerSpawnerController.OnPlayerSpawnedOrRespawned -= this.HandlePlayerSpawnedOrRespawned;
            this.PlayerSpawnerController.OnPlayerSpawnedOrRespawned += this.HandlePlayerSpawnedOrRespawned;
        }
        private void UnsubscribeFromPlayerSpawner()
        {
            if (this.PlayerSpawnerController == null) return;

            this.PlayerSpawnerController.OnPlayerSpawnedOrRespawned -= this.HandlePlayerSpawnedOrRespawned;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.SpawnedCharacters.Clear();
            _spawnedCharactersByEntry.Clear();

            this.HasBoundExternalReferences = false;
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.PlayerSpawnerController == null)
            {
                if (this.transform.parent           != null) _playerSpawnerController = this.transform.parent.GetComponentInChildren<PlayerSpawnerController>();
                if (this.PlayerSpawnerController    == null) _playerSpawnerController = UnityEngine.Object.FindFirstObjectByType<PlayerSpawnerController>();
            }

            if (this.CharacterAIGroupRoot == null)
            {
                GameObject groupRoot = GameObject.Find(this.CharacterAIGroupRootName);
                _characterAIGroupRoot = groupRoot != null
                    ? groupRoot.transform
                    : this.transform;
            }
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.PlayerSpawnerController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CharacterAIGroupSpawnerController)}] No {nameof(FlameOfTheLost.PlayerSpawnerController)} assigned.", this);
            }

            if (this.CharacterAIGroupRoot == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(CharacterAIGroupSpawnerController)}] No Character AI group root assigned.", this);
            }
        }
        protected override void BindExternalReferences()
        {
            base.BindExternalReferences();

            this.HasBoundExternalReferences = true;
            this.SubscribeToPlayerSpawner();
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            if (!this.IsSpawnedCharactersOnStart) return;

            this.SpawnCharactersOnStart();
        }
    }
}