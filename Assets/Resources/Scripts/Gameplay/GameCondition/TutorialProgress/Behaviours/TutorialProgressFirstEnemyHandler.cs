using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class TutorialProgressFirstEnemyHandler : TutorialProgressSubBehaviourBase
    {
        // --- References ---
        //  1. First Enemy Source
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

        // --- Helpers ---
        //  1. TutorialProgressSubBehaviourBase Overrides
        //      1.1. Validation
        protected override bool ValidateGameplayReferences()
        {
            if (!this.ValidatePlayerReference()) return false;

            bool isValid = true;

            if (this.Player.DamageReceivingHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressFirstEnemyHandler)}] {nameof(this.Player)} has no {nameof(DamageReceivingHandler)}.", this);
                isValid = false;
            }

            if (this.FirstEnemySpawner == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressFirstEnemyHandler)}] {nameof(this.FirstEnemySpawner)} could not be resolved.", this);
                isValid = false;
            }
            if (this.FirstEnemySpawnEntry == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressFirstEnemyHandler)}] {nameof(this.FirstEnemySpawnEntry)} could not be resolved.", this);
                isValid = false;
            }

            return isValid;
        }

        //      1.2. Others
        protected override void PrepareGameplayReferences()
        {
            base.PrepareGameplayReferences();

            this.ResolveFirstEnemySource();
        }
        protected override void BindGameplayReferencesInternal()
        {
            base.BindGameplayReferencesInternal();

            this.Player.DamageReceivingHandler.OnDamageApplied += this.HandleDamageApplied;

            this.FirstEnemySpawner.EnemySpawned += this.HandleEnemySpawned;
        }
        protected override void UnbindGameplayReferencesInternal()
        {
            if (this.Player?.DamageReceivingHandler != null)
            {
                this.Player.DamageReceivingHandler.OnDamageApplied -= this.HandleDamageApplied;
            }

            if (this.FirstEnemySpawner != null)
            {
                this.FirstEnemySpawner.EnemySpawned -= this.HandleEnemySpawned;
            }

            this.UnbindFirstEnemy();

            _firstEnemySpawner = null;
            _firstEnemySpawnEntry = null;

            base.UnbindGameplayReferencesInternal();
        }
        protected override void OnGameplayReferencesBound()
        {
            base.OnGameplayReferencesBound();

            this.TryBindExistingFirstEnemy();
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

            if (!this.FirstEnemySpawner.TryGetFirstSpawnedEnemy(this.FirstEnemySpawnEntry, out CharacterRootController firstEnemy)) return;

            this.BindFirstEnemy(firstEnemy);
        }
        private void BindFirstEnemy(CharacterRootController firstEnemy)
        {
            if (firstEnemy == null) return;

            this.UnbindFirstEnemy();

            _firstEnemy = firstEnemy;

            if (this.FirstEnemy.DeathHandler != null) this.FirstEnemy.DeathHandler.OnTechnicalDeath += this.HandleFirstEnemyDeathStarted;
        }
        private void UnbindFirstEnemy()
        {
            if (this.FirstEnemy != null && this.FirstEnemy.DeathHandler != null)
            {
                this.FirstEnemy.DeathHandler.OnTechnicalDeath -= this.HandleFirstEnemyDeathStarted;
            }

            _firstEnemy = null;
        }

        //  4. Event Handling
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

            this.TutorialProgressController.MarkPlayerHitByFirstEnemy();
        }

        private void HandleFirstEnemyDeathStarted(CharacterDeathContext context)
        {
            if (context.Character != this.FirstEnemy) return;

            this.TutorialProgressController.MarkFirstEnemyDefeated();

            this.UnbindFirstEnemy();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _firstEnemy = null;
            _firstEnemySpawner = null;
            _firstEnemySpawnEntry = null;
        }
    }
}