using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterDeathHandler : CharacterBehaviourControllerBase, ICharacterRefreshable
    {
        // --- States ---
        //  1. References
        [Header("References - Player Spawner")]
        [SerializeField] private PlayerSpawnerController _playerSpawnerController;
        private PlayerSpawnerController PlayerSpawnerController => _playerSpawnerController;
        private HPController HPController => this.CharacterRootController.HPController;
        private CharacterVisualController VisualController => this.CharacterRootController.VisualController;

        //  2. Runtime Death Cause
        private CharacterDeathCauseEntry LastDeathCause => this.HPController != null
            ? this.HPController.LastDeathCause
            : null;

        //  3. Runtime States
        private bool _hasHandledTechnicalDeath = false;
        private bool _hasHandledDeathAnimationFinished = false;
        private bool _hasDistributedReward = false;
        private bool HasHandledTechnicalDeath
        {
            get => _hasHandledTechnicalDeath;
            set => _hasHandledTechnicalDeath = value;
        }
        private bool HasHandledDeathAnimationFinished
        {
            get => _hasHandledDeathAnimationFinished;
            set => _hasHandledDeathAnimationFinished = value;
        }
        private bool HasDistributedReward
        {
            get => _hasDistributedReward;
            set => _hasDistributedReward = value;
        }

        // --- Events ---
        public event Action<CharacterDeathContext> OnTechnicalDeath;

        // --- Unity Callbacks ---
        private void OnEnable()
        {
            this.CacheInternalReferences();
            this.BindEvents();
        }
        private void OnDisable()
        {
            this.UnbindEvents();
        }

        // --- Public Methods ---
        //  1. ICharacterPlacementRefreshable Implementation
        public int RefreshmentConfigurePriority => 10;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.HasHandledTechnicalDeath = false;
            this.HasHandledDeathAnimationFinished = false;
            this.HasDistributedReward = false;
        }

        // --- Helpers ---
        //  1. Event Binding
        private void BindEvents()
        {
            if (this.HPController == null) return;

            this.HPController.OnDeath -= this.HandleTechnicalDeath;
            this.HPController.OnDeath += this.HandleTechnicalDeath;

            if (this.VisualController == null) return;

            this.VisualController.OnDeathAnimationFinished -= this.HandleDeathAnimationFinished;
            this.VisualController.OnDeathAnimationFinished += this.HandleDeathAnimationFinished;
        }
        private void UnbindEvents()
        {
            if (this.HPController != null)
            {
                this.HPController.OnDeath -= this.HandleTechnicalDeath;
            }

            if (this.VisualController != null)
            {
                this.VisualController.OnDeathAnimationFinished -= this.HandleDeathAnimationFinished;
            }
        }

        //  2. Event Handlers
        //      2.1. Stage 1: Handle Death (technically)
        private void HandleTechnicalDeath(CharacterDeathCauseEntry deathCause)
        {
            if (this.HasHandledTechnicalDeath) return;
            this.HasHandledTechnicalDeath = true;

            CharacterDeathContext context = new CharacterDeathContext
            (
                character: this.CharacterRootController,
                deathCause: deathCause
            );

            this.OnTechnicalDeath?.Invoke(context);

            Debug.Log
            (
                $"[{this.CharacterName}.{nameof(CharacterDeathHandler)}] Character is technically dead. " +
                $"CauseType: {(deathCause != null ? deathCause.CauseType.ToString() : "None")}. " +
                $"SourceCharacter: {(deathCause?.SourceCharacter != null ? deathCause.SourceCharacter.name : "None")}.",
                this
            );

            this.DisableDeathDisableableSystems(context);
        }

        private void DisableDeathDisableableSystems(CharacterDeathContext context)
        {
            if (this.CharacterRootController == null) return;

            IEnumerable<ICharacterDeathDisableable> disableableSystems = this.CharacterRootController
                .GetComponentsInChildren<MonoBehaviour>(true)
                .OfType<ICharacterDeathDisableable>()
                .OrderBy(system => system.DeathDisablePriority);

            foreach (ICharacterDeathDisableable disableableSystem in disableableSystems)
            {
                disableableSystem.DisableAfterCharacterDeath(context);
            }
        }

        //      2.2. Stage 2: Handle Death Animation Finished
        private void HandleDeathAnimationFinished()
        {
            if (this.HasHandledDeathAnimationFinished) return;
            this.HasHandledDeathAnimationFinished = true;

            CharacterDeathCauseEntry deathCause = this.LastDeathCause;

            switch (this.CharacterRootController.Faction)
            {
                case CharacterFaction.Player:
                    this.RespawnPlayer();
                    break;

                case CharacterFaction.Enemy:
                case CharacterFaction.Ally:
                    this.TryDistributeRewardAfterDeath(deathCause);
                    this.DestroyCharacter();
                    break;

                case CharacterFaction.Neutral:
                default:
                    this.TryDistributeRewardAfterDeath(deathCause);
                    this.DestroyCharacter();
                    break;
            }
        }

        private bool TryDistributeRewardAfterDeath(CharacterDeathCauseEntry deathCause)
        {
            if (this.HasDistributedReward) return true;
            this.HasDistributedReward = true;

            if (deathCause == null) return false;
            if (deathCause.CauseType != CharacterDeathCauseType.OtherCharacter) return false;

            CharacterRootController receiverCharacter = deathCause.SourceCharacter;
            CharacterRootController defeatedCharacter = this.CharacterRootController;

            if (receiverCharacter == null) return false;
            if (defeatedCharacter == null) return false;
            if (receiverCharacter == defeatedCharacter) return false;

            bool distributed = CharacterRewardDistributionUtility.TryDistributeRewardFromDefeatedCharacter
            (
                receiverCharacter: receiverCharacter,
                defeatedCharacter: defeatedCharacter
            );

            //Debug.Log
            //(
            //    $"[{this.CharacterName}.{nameof(CharacterDeathHandler)}] Reward distributed. " +
            //    $"Receiver: {receiverCharacter.name}. " +
            //    $"Defeated: {defeatedCharacter.name}.",
            //    this
            //);

            return distributed;
        }

        private void RespawnPlayer()
        {
            if (this.PlayerSpawnerController == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterDeathHandler)}] Player died, but no {nameof(PlayerSpawnerController)} was found.", this);
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
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PlayerSpawnerController == null) _playerSpawnerController = UnityEngine.Object.FindFirstObjectByType<PlayerSpawnerController>();
        }

        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.CharacterRootController is PlayerRootController && this.PlayerSpawnerController == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterDeathHandler)}] No {nameof(PlayerSpawnerController)} reference found. Player characters will not respawn on death.", this);
            }
        }
    }
}
