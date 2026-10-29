using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class SkirmisherLocomotionVisualModeProvider : CharacterBehaviourControllerBase, ICharacterLocomotionVisualModeProvider, ICharacterRefreshable
    {
        // --- References ---
        private SkirmisherEnemyBrainController SkirmisherEnemyBrainController
        {
            get
            {
                if (this.CharacterRootController is EnemyRootController enemyRootController)
                {
                    if (enemyRootController.EnemyBrainController is SkirmisherEnemyBrainController skirmisherEnemyBrainController)
                    {
                        return skirmisherEnemyBrainController;
                    }
                }

                return null;
            }
        }

        // --- Configs ---
        [Header("Locomotion Mode Mapping")]
        [SerializeField] private CharacterLocomotionMode _idleMode              = CharacterLocomotionMode.Default;
        [SerializeField] private CharacterLocomotionMode _wanderingMode         = CharacterLocomotionMode.Default;
        [SerializeField] private CharacterLocomotionMode _alertMode             = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _chasingMode           = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _strafingMode          = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _normalAttackingMode   = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _specialAttackingMode  = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _recoveringMode        = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _retreatingMode        = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _returningMode         = CharacterLocomotionMode.Default;
        [SerializeField] private CharacterLocomotionMode _deadMode              = CharacterLocomotionMode.Default;
        private CharacterLocomotionMode IdleMode                => _idleMode;
        private CharacterLocomotionMode WanderingMode           => _wanderingMode;
        private CharacterLocomotionMode AlertMode               => _alertMode;
        private CharacterLocomotionMode ChasingMode             => _chasingMode;
        private CharacterLocomotionMode StrafingMode            => _strafingMode;
        private CharacterLocomotionMode NormalAttackingMode     => _normalAttackingMode;
        private CharacterLocomotionMode SpecialAttackingMode    => _specialAttackingMode;
        private CharacterLocomotionMode RecoveringMode          => _recoveringMode;
        private CharacterLocomotionMode RetreatingMode          => _retreatingMode;
        private CharacterLocomotionMode ReturningMode           => _returningMode;
        private CharacterLocomotionMode DeadMode                => _deadMode;

        // --- Runtime ---
        [Header("Runtime")]
        [SerializeField] private CharacterLocomotionMode _currentResolvedMode = CharacterLocomotionMode.Default;
        private CharacterLocomotionMode CurrentResolvedMode
        {
            get => _currentResolvedMode;
            set => _currentResolvedMode = value;
        }

        // --- Events ---
        public event Action OnLocomotionVisualModeChanged;

        // --- Unity Callbacks ---
        private void OnEnable()
        {
            this.ResolveCurrentModeAndNotifyIfChanged(forceNotify: true);
        }
        private void Update()
        {
            this.ResolveCurrentModeAndNotifyIfChanged(forceNotify: false);
        }

        // --- Public APIs ---
        //  1. ICharacterLocomotionVisualModeProvider implementation
        public CharacterLocomotionMode GetLocomotionMode()
        {
            return this.CurrentResolvedMode;
        }
        public CharacterCarryItemType GetCarryItemType()
        {
            return CharacterCarryItemType.None;
        }
        public LightSourceType GetLightSourceType()
        {
            return LightSourceType.None;
        }

        //  2. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 92;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.CurrentResolvedMode = this.IdleMode;
            this.OnLocomotionVisualModeChanged?.Invoke();
        }

        // --- Helpers ---
        private void ResolveCurrentModeAndNotifyIfChanged(bool forceNotify)
        {
            CharacterLocomotionMode nextMode = this.ResolveLocomotionMode();

            if (!forceNotify && this.CurrentResolvedMode == nextMode) return;

            this.CurrentResolvedMode = nextMode;
            this.OnLocomotionVisualModeChanged?.Invoke();
        }

        private CharacterLocomotionMode ResolveLocomotionMode()
        {
            if (this.SkirmisherEnemyBrainController == null) return this.IdleMode;

            return this.SkirmisherEnemyBrainController.CurrentAIState switch
            {
                SkirmisherEnemyState.Idle               => this.IdleMode,
                SkirmisherEnemyState.Wandering          => this.WanderingMode,
                SkirmisherEnemyState.Alert              => this.AlertMode,
                SkirmisherEnemyState.Chasing            => this.ChasingMode,
                SkirmisherEnemyState.Strafing           => this.StrafingMode,
                SkirmisherEnemyState.NormalAttacking    => this.NormalAttackingMode,
                SkirmisherEnemyState.SpecialAttacking   => this.SpecialAttackingMode,
                SkirmisherEnemyState.Recovering         => this.RecoveringMode,
                SkirmisherEnemyState.Retreating         => this.RetreatingMode,
                SkirmisherEnemyState.Returning          => this.ReturningMode,
                SkirmisherEnemyState.Dead               => this.DeadMode,
                _ => this.IdleMode,
            };
        }
    }
}
