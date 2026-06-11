using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterDeathHandler : CharacterBehaviourControllerBase
    {
        // --- States ---
        //  1. References
        [Header("References - Player Spawner")]
        [SerializeField] private PlayerSpawnerController _playerSpawnerController;
        private PlayerSpawnerController PlayerSpawnerController => _playerSpawnerController;
        private CharacterIdentityController IdentityController => this.CharacterRootController.IdentityController;
        private HPController HPController => this.CharacterRootController.HPController;

        //  2. Runtime Death Cause
        private CharacterDeathCauseEntry LastDeathCause => this.HPController != null
            ? this.HPController.LastDeathCause
            : null;

        // --- Unity Callbacks ---
        private void OnEnable()
        {
            this.CacheReferences();
            this.BindEvents();
        }
        private void OnDisable()
        {
            this.UnbindEvents();
        }

        // --- Helpers ---
        //  1. Event Binding
        private void BindEvents()
        {
            if (this.HPController == null) return;

            this.HPController.OnDeathAnimationFinished -= this.HandleDeathAnimationFinished;
            this.HPController.OnDeathAnimationFinished += this.HandleDeathAnimationFinished;
        }
        private void UnbindEvents()
        {
            if (this.HPController == null) return;

            this.HPController.OnDeathAnimationFinished -= this.HandleDeathAnimationFinished;
        }

        //  2. Event Handlers
        private void HandleDeathAnimationFinished()
        {
            CharacterDeathCauseEntry deathCause = this.LastDeathCause;
            Debug.Log
            (
                $"[{this.CharacterName}.{nameof(CharacterDeathHandler)}] Resolving death. " +
                $"CauseType: {(deathCause != null ? deathCause.CauseType.ToString() : "None")}. " +
                $"SourceCharacter: {(deathCause?.SourceCharacter != null ? deathCause.SourceCharacter.name : "None")}."
            );

            if (this.IdentityController == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterDeathHandler)}] No CharacterIdentityController found. Destroying character by fallback.", this);
                this.DestroyCharacter();
                return;
            }

            switch (this.IdentityController.Faction)
            {
                case CharacterFaction.Player:
                    this.RespawnPlayer();
                    break;

                case CharacterFaction.Enemy:
                case CharacterFaction.Ally:
                    this.DestroyCharacter();
                    break;

                case CharacterFaction.Neutral:
                default:
                    this.DestroyCharacter();
                    break;
            }
        }

        private void RespawnPlayer()
        {
            if (this.PlayerSpawnerController == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterDeathHandler)}] Player died, but no PlayerSpawnerController was found.", this);
                return;
            }

            this.PlayerSpawnerController.RespawnPlayer();
        }

        private void DestroyCharacter()
        {
            if (this.CharacterRootController == null)
            {
                UnityEngine.Object.Destroy(this.gameObject);
                return;
            }

            UnityEngine.Object.Destroy(this.CharacterRootController.gameObject);
        }

        //  3. Cache & Validate References
        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.PlayerSpawnerController == null) _playerSpawnerController = UnityEngine.Object.FindFirstObjectByType<PlayerSpawnerController>();
        }

        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.IdentityController == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterDeathHandler)}] No CharacterIdentityController reference found.", this);
                return;
            }

            if (this.IdentityController.IsPlayer && this.PlayerSpawnerController == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterDeathHandler)}] No PlayerSpawnerController reference found. Player characters will not respawn on death.", this);
            }
        }
    }
}
