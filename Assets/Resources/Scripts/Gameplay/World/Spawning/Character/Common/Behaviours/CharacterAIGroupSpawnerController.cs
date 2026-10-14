using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterAIGroupSpawnerController : FOTLMonoBehaviour, IWorldRefreshable
    {
        // --- References ---
        //  1. Character AI Group Root
        [Header("References - Character AI Group Root")]
        [SerializeField] private Transform _characterAIGroupRoot;
        protected Transform CharacterAIGroupRoot => _characterAIGroupRoot;

        //  2. Character AI Default Spawn Points
        protected abstract IReadOnlyList<CharacterAIDefaultSpawnPointController> CharacterAIDefaultSpawnPointControllers { get; }

        // --- Configs ---
        [Header("Settings")]
        [SerializeField] private bool _isSpawnedCharactersOnStart = true;
        private bool IsSpawnedCharactersOnStart => _isSpawnedCharactersOnStart;

        // --- Runtime ---
        //  1. Spawned Characters
        [Header("Runtime - Character AI (GameObject)")]
        [SerializeField] private List<GameObject> _spawnedCharacters = new();
        protected List<GameObject> SpawnedCharacters => _spawnedCharacters;

        //  2. Spawned Characters By Entry
        private readonly Dictionary<CharacterAISpawnEntry, List<GameObject>> _spawnedCharactersByEntry = new();

        private IReadOnlyDictionary<CharacterAISpawnEntry, List<GameObject>> SpawnedCharactersByEntry => _spawnedCharactersByEntry;

        // --- Events ---
        public event Action<CharacterAISpawnEntry, CharacterRootController> CharacterAISpawned;
        public event Action SpawnedCharactersClearing;

        // --- Public APIs ---
        //  0. IWorldRefreshable implementation
        public virtual int WorldRefreshPriority => 100;
        public void RefreshWorldContent(WorldRefreshContext context)
        {
            this.ClearSpawnedCharacters();

            if (!this.ShouldSpawnCharactersAfterWorldRefresh(context)) return;

            this.SpawnCharacters(spawnEntry => this.ShouldSpawnEntryAfterWorldRefresh(spawnEntry,context));
        }

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

            this.ConfigureSpawnedCharacterVisual(spawnedCharacter, spawnEntry);
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
            if (this.CharacterAIGroupRoot == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CharacterAIGroupSpawnerController)}] Cannot spawn Character AI because no Character AI group root is assigned.", this);
                return;
            }

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
                int spawnedCharacterIndex = currentSpawnedCount + i + 1;

                this.SpawnSingleCharacter(spawnPoint, spawnEntry, spawnedCharacterIndex);
            }
        }
        private GameObject SpawnSingleCharacter(CharacterAIDefaultSpawnPointController spawnPoint, CharacterAISpawnEntry spawnEntry, int spawnedCharacterIndex)
        {
            if (spawnPoint == null) return null;
            if (spawnEntry == null) return null;
            if (this.CharacterAIGroupRoot == null) return null;

            CharacterAIVariantProfile variant = spawnEntry.CharacterAIVariant;

            if (variant == null) return null;
            if (variant.CharacterProfile == null) return null;
            if (variant.CharacterProfile.Prefab == null) return null;

            GameObject spawnedCharacter = UnityEngine.Object.Instantiate(variant.CharacterProfile.Prefab, this.CharacterAIGroupRoot);

            if (spawnedCharacter == null) return null;

            spawnedCharacter.name = this.GetSpawnedCharacterGameObjectName(variant.CharacterProfile.Prefab, spawnedCharacterIndex);

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
        private string GetSpawnedCharacterGameObjectName(GameObject characterPrefab, int spawnedCharacterIndex)
        {
            if (characterPrefab == null) return $"GO_Chr_AI_{spawnedCharacterIndex:D2}";

            string characterName = characterPrefab.name;

            const string prefabPrefix = "Prefab_";
            const string gameObjectPrefix = "GO_";

            if (characterName.StartsWith(prefabPrefix))
            {
                characterName = characterName.Substring(prefabPrefix.Length);
            }
            else if (characterName.StartsWith(gameObjectPrefix))
            {
                characterName = characterName.Substring(gameObjectPrefix.Length);
            }

            return $"{gameObjectPrefix}{characterName}_{spawnedCharacterIndex:D2}";
        }

        //  3. Configuration
        private void ConfigureSpawnedCharacterVisual(GameObject spawnedCharacter, CharacterAISpawnEntry spawnEntry)
        {
            if (spawnedCharacter == null) return;
            if (spawnEntry == null) return;

            CharacterAIVariantProfile variant = spawnEntry.CharacterAIVariant;
            if (variant == null) return;

            CharacterVisualConfig resolvedVisualConfig = variant.ResolvedVisualConfig;
            if (resolvedVisualConfig == null)
            {
                Debug.LogWarning($"[{spawnedCharacter.name}.{nameof(CharacterAIGroupSpawnerController)}] No {nameof(CharacterVisualConfig)} could be resolved from the Character AI Variant or Character Profile.", spawnedCharacter);
                return;
            }

            CharacterRootController rootController = spawnedCharacter.GetComponentInChildren<CharacterRootController>(true);
            if (rootController == null)
            {
                Debug.LogWarning($"[{spawnedCharacter.name}.{nameof(CharacterAIGroupSpawnerController)}] Spawned Character AI has no {nameof(CharacterRootController)}.", spawnedCharacter);
                return;
            }

            CharacterVisualController visualController = rootController.VisualController;
            if (visualController == null)
            {
                Debug.LogWarning($"[{spawnedCharacter.name}.{nameof(CharacterAIGroupSpawnerController)}] Spawned Character AI has no {nameof(CharacterVisualController)}.", spawnedCharacter);
                return;
            }

            visualController.SetConfig(resolvedVisualConfig);
        }
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
            // Notify observers before old characters are deactivated and Destroy is queued.
            this.SpawnedCharactersClearing?.Invoke();

            for (int i = this.SpawnedCharacters.Count - 1; i >= 0; i--)
            {
                GameObject spawnedCharacter = this.SpawnedCharacters[i];

                if (spawnedCharacter == null) continue;

                spawnedCharacter.SetActive(false);

                UnityEngine.Object.Destroy(spawnedCharacter);
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

        //  6. 
        protected virtual bool ShouldSpawnCharactersAfterWorldRefresh(WorldRefreshContext context)
        {
            return context.IsPlayerRespawn || context.IsSaveReload;
        }
        protected virtual bool ShouldSpawnEntryAfterWorldRefresh(CharacterAISpawnEntry spawnEntry, WorldRefreshContext context)
        {
            if (spawnEntry == null) return false;

            return spawnEntry.SpawnWhenPlayerRespawns;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.SpawnedCharacters.Clear();
            _spawnedCharactersByEntry.Clear();
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.CharacterAIGroupRoot == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CharacterAIGroupSpawnerController)}] No Character AI group root assigned.", this);
            }
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            if (!this.IsSpawnedCharactersOnStart) return;

            this.SpawnCharactersOnStart();
        }
    }
}