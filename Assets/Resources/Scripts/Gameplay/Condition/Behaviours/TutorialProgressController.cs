using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class TutorialProgressController : FOTLMonoBehaviour
    {
        // --- References ---
        //  1. Condition Systems
        [Header("References (Systems)")]
        [SerializeField] private GameConditionController _gameConditionController;
        private GameConditionController GameConditionController => _gameConditionController;

        //  2. Characters
        [Header("References (Characters)")]
        [SerializeField] private CharacterRootController _player;
        private CharacterRootController Player => _player;

        //  3. First Enemy (Spawner)
        [Header("References (First Enemy Source)")]
        [SerializeField] private EnemyGroupSpawnerController _preferredFirstEnemySpawner;
        private EnemyGroupSpawnerController PreferredFirstEnemySpawner => _preferredFirstEnemySpawner;

        // --- Runtime ---
        //  1. First Enemy
        [Header("Runtime (First Enemy)")]
        [SerializeField] private CharacterRootController _firstEnemy;
        private CharacterRootController FirstEnemy => _firstEnemy;

        private EnemyGroupSpawnerController _firstEnemySpawner;
        private EnemySpawnEntry _firstEnemySpawnEntry;

        private EnemyGroupSpawnerController FirstEnemySpawner => _firstEnemySpawner;
        private EnemySpawnEntry FirstEnemySpawnEntry => _firstEnemySpawnEntry;

        // --- Public APIs ---
        public void MarkPlayerHitByFirstEnemy()
        {
            if (this.GameConditionController == null) return;

            this.GameConditionController.SatisfyCondition(GameConditionKey.PlayerHitByFirstEnemy);
        }
        public void MarkFirstEnemyDefeated()
        {
            if (this.GameConditionController == null) return;

            this.GameConditionController.SatisfyCondition(GameConditionKey.FirstEnemyDefeated);
        }

        public void ResetTutorialProgress()
        {
            if (this.GameConditionController == null) return;

            this.GameConditionController.ResetConditions();

            this.RefreshPlayerHasMultipleWeaponsCondition();
            this.RefreshPlayerReadyToCycleWeaponsCondition();
        }

        // --- Helpers ---
        //  1. Player Equipment Progress
        private void RefreshPlayerHasMultipleWeaponsCondition()
        {
            if (this.GameConditionController == null) return;
            if (this.Player == null) return;
            if (this.Player.InventoryController == null) return;

            IReadOnlyList<InventoryItemEntry> weaponEntries = this.Player.InventoryController.AllWeaponEntries;

            bool playerHasMultipleWeapons = weaponEntries != null && weaponEntries.Count >= 2;

            this.GameConditionController.SetConditionState(GameConditionKey.PlayerHasMultipleWeapons, playerHasMultipleWeapons);
        }
        private void RefreshPlayerReadyToCycleWeaponsCondition()
        {
            if (this.GameConditionController == null) return;
            if (this.Player == null) return;
            if (this.Player.CombatLoadoutController == null) return;

            CharacterCombatLoadoutController combatLoadoutController = this.Player.CombatLoadoutController;

            IReadOnlyList<InventoryItemEntry> equippedWeaponEntries = combatLoadoutController.EquippedWeaponEntries;
            InventoryItemEntry activeWeaponEntry = combatLoadoutController.ActiveEquippedWeaponEntry;

            int equippedWeaponCount = 0;
            bool activeWeaponIsEquipped = false;

            if (equippedWeaponEntries != null)
            {
                for (int i = 0; i < equippedWeaponEntries.Count; i++)
                {
                    InventoryItemEntry equippedWeaponEntry = equippedWeaponEntries[i];

                    if (equippedWeaponEntry == null) continue;

                    equippedWeaponCount++;

                    if (UnityEngine.Object.ReferenceEquals(equippedWeaponEntry, activeWeaponEntry))
                    {
                        activeWeaponIsEquipped = true;
                    }
                }
            }

            bool playerReadyToCycleWeapons = equippedWeaponCount >= 2 && activeWeaponEntry != null && activeWeaponIsEquipped;

            this.GameConditionController.SetConditionState(GameConditionKey.PlayerReadyToCycleWeapons, playerReadyToCycleWeapons);
        }

        //  2. First Enemy Source Resolution
        private void ResolveFirstEnemySource()
        {
            _firstEnemySpawner = null;
            _firstEnemySpawnEntry = null;

            if (this.TryUseFirstEnemySpawner(this.PreferredFirstEnemySpawner)) return;

            EnemyGroupSpawnerController[] spawners = UnityEngine.Object.FindObjectsByType<EnemyGroupSpawnerController>(FindObjectsSortMode.None);

            for (int i = 0; i < spawners.Length; i++)
            {
                EnemyGroupSpawnerController spawner = spawners[i];

                if (spawner == null) continue;
                if (spawner == this.PreferredFirstEnemySpawner) continue;

                if (this.TryUseFirstEnemySpawner(spawner)) return;
            }
        }
        private bool TryUseFirstEnemySpawner(EnemyGroupSpawnerController spawner)
        {
            if (spawner == null) return false;

            if (!spawner.TryGetFirstSpawnEntry(out EnemySpawnEntry spawnEntry)) return false;

            _firstEnemySpawner = spawner;
            _firstEnemySpawnEntry = spawnEntry;

            return true;
        }

        //  3. First Enemy Binding
        private void TryBindExistingFirstEnemy()
        {
            if (this.FirstEnemySpawner == null) return;
            if (this.FirstEnemySpawnEntry == null) return;
            if (this.FirstEnemy != null) return;

            if (!this.FirstEnemySpawner.TryGetFirstSpawnedEnemy(this.FirstEnemySpawnEntry, out CharacterRootController firstEnemy))
            {
                return;
            }

            this.BindFirstEnemy(firstEnemy);
        }
        private void BindFirstEnemy(CharacterRootController firstEnemy)
        {
            if (firstEnemy == null) return;

            this.UnbindFirstEnemy();

            _firstEnemy = firstEnemy;

            if (this.FirstEnemy.DeathHandler != null)
            {
                this.FirstEnemy.DeathHandler.OnTechnicalDeath += this.HandleFirstEnemyDeathStarted;
            }
        }
        private void UnbindFirstEnemy()
        {
            if (this.FirstEnemy != null && this.FirstEnemy.DeathHandler != null)
            {
                this.FirstEnemy.DeathHandler.OnTechnicalDeath -= this.HandleFirstEnemyDeathStarted;
            }

            _firstEnemy = null;
        }

        //  4. Event handling
        private void HandlePlayerItemAdded(ItemProfile item, int quantity)
        {
            if (item is not WeaponProfile) return;

            this.RefreshPlayerHasMultipleWeaponsCondition();
        }
        private void HandlePlayerItemRemoved(ItemProfile item, int quantity)
        {
            if (item is not WeaponProfile) return;

            this.RefreshPlayerHasMultipleWeaponsCondition();
        }
        private void HandleEquippedWeaponEntriesChanged()
        {
            this.RefreshPlayerReadyToCycleWeaponsCondition();
        }
        private void HandleActiveEquippedWeaponChanged(InventoryItemEntry activeWeaponEntry)
        {
            this.RefreshPlayerReadyToCycleWeaponsCondition();
        }
        private void HandleEnemySpawned(EnemySpawnEntry spawnEntry, CharacterRootController character)
        {
            if (spawnEntry != this.FirstEnemySpawnEntry) return;
            if (character == null) return;
            if (this.FirstEnemy != null) return;

            this.BindFirstEnemy(character);
        }
        private void HandleDamageApplied(DamageAppliedContext context)
        {
            if (context.Attacker != this.FirstEnemy) return;
            if (context.Victim != this.Player) return;
            if (context.DamageAmount <= 0) return;

            this.MarkPlayerHitByFirstEnemy();
        }
        private void HandleFirstEnemyDeathStarted(CharacterDeathContext context)
        {
            if (context.Character != this.FirstEnemy) return;

            this.MarkFirstEnemyDefeated();

            this.UnbindFirstEnemy();
        }

        // --- FOTLMonoBehaviour Overrides  ----
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _firstEnemy = null;
            _firstEnemySpawner = null;
            _firstEnemySpawnEntry = null;
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.GameConditionController == null) _gameConditionController = UnityEngine.Object.FindFirstObjectByType<GameConditionController>();
            if (this.Player == null)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null) _player = player.GetComponent<CharacterRootController>();
            }

            this.ResolveFirstEnemySource();
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.GameConditionController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressController)}] {nameof(this.GameConditionController)} reference is not assigned.", this);
            }

            if (this.Player == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressController)}] {nameof(this.Player)} reference is not assigned.", this);
            }
            else
            {
                if (this.Player.DamageReceivingHandler  == null) Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressController)}] {nameof(this.Player)} has no {nameof(DamageReceivingHandler)}.", this);
                if (this.Player.InventoryController     == null) Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressController)}] {nameof(this.Player)} has no {nameof(InventoryController)}.", this);
                if (this.Player.CombatLoadoutController == null) Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressController)}] {nameof(this.Player)} has no {nameof(CharacterCombatLoadoutController)}.", this);
            }

            if (this.FirstEnemySpawner == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressController)}] {nameof(this.FirstEnemySpawner)} could not be resolved.", this);
            }

            if (this.FirstEnemySpawnEntry == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressController)}] {nameof(this.FirstEnemySpawnEntry)} could not be resolved.", this);
            }
        }
        protected override void BindExternalReferences()
        {
            base.BindExternalReferences();

            if (this.Player?.DamageReceivingHandler != null) this.Player.DamageReceivingHandler.OnDamageApplied += this.HandleDamageApplied;
            if (this.Player?.InventoryController != null)
            {
                this.Player.InventoryController.OnItemAdded     += this.HandlePlayerItemAdded;
                this.Player.InventoryController.OnItemRemoved   += this.HandlePlayerItemRemoved;
            }
            if (this.Player?.CombatLoadoutController != null)
            {
                this.Player.CombatLoadoutController.OnEquippedWeaponEntriesChanged  += this.HandleEquippedWeaponEntriesChanged;
                this.Player.CombatLoadoutController.OnActiveEquippedWeaponChanged   += this.HandleActiveEquippedWeaponChanged;
            }
            if (this.FirstEnemySpawner != null) this.FirstEnemySpawner.EnemySpawned += this.HandleEnemySpawned;

            this.TryBindExistingFirstEnemy();

            this.RefreshPlayerHasMultipleWeaponsCondition();
            this.RefreshPlayerReadyToCycleWeaponsCondition();
        }
        protected override void UnbindExternalReferences()
        {
            if (this.Player?.DamageReceivingHandler != null) this.Player.DamageReceivingHandler.OnDamageApplied -= this.HandleDamageApplied;
            if (this.Player?.InventoryController != null)
            {
                this.Player.InventoryController.OnItemAdded     -= this.HandlePlayerItemAdded;
                this.Player.InventoryController.OnItemRemoved   -= this.HandlePlayerItemRemoved;
            }
            if (this.Player?.CombatLoadoutController != null)
            {
                this.Player.CombatLoadoutController.OnEquippedWeaponEntriesChanged  -= this.HandleEquippedWeaponEntriesChanged;
                this.Player.CombatLoadoutController.OnActiveEquippedWeaponChanged   -= this.HandleActiveEquippedWeaponChanged;
            }
            if (this.FirstEnemySpawner != null) this.FirstEnemySpawner.EnemySpawned -= this.HandleEnemySpawned;

            this.UnbindFirstEnemy();

            base.UnbindExternalReferences();
        }
    }
}
