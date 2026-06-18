using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class TerritorialLocomotionVisualModeProvider : CharacterBehaviourControllerBase,
        ICharacterLocomotionVisualModeProvider,
        ICharacterRefreshable
    {
        // --- States ---
        //  1. References
        private TerritorialEnemyBrainController TerritorialEnemyBrainController
        {
            get
            {
                if (this.CharacterRootController is EnemyRootController enemyRootController)
                {
                    if (enemyRootController.EnemyBrainController is TerritorialEnemyBrainController territorialEnemyBrainController)
                    {
                        return territorialEnemyBrainController;
                    }
                    return null;
                }
                return null;
            }
        }
           // this.CharacterRootController is EnemyRootController enemyRootController ? enemyRootController.EnemyBrainController : null; }

        //  2. Locomotion Mode Mapping
        [Header("Locomotion Mode Mapping")]
        [SerializeField] private CharacterLocomotionMode _roamingMode = CharacterLocomotionMode.Default;
        [SerializeField] private CharacterLocomotionMode _alertMode = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _readyToAttackMode = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _preAttackMode = CharacterLocomotionMode.PreAttack;
        [SerializeField] private CharacterLocomotionMode _attackingMode = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _recoveringMode = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _deadMode = CharacterLocomotionMode.Default;
        private CharacterLocomotionMode RoamingMode => _roamingMode;
        private CharacterLocomotionMode AlertMode => _alertMode;
        private CharacterLocomotionMode ReadyToAttackMode => _readyToAttackMode;
        private CharacterLocomotionMode PreAttackMode => _preAttackMode;
        private CharacterLocomotionMode AttackingMode => _attackingMode;
        private CharacterLocomotionMode RecoveringMode => _recoveringMode;
        private CharacterLocomotionMode DeadMode => _deadMode;

        //  3. Runtime Cache
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

        //  2. ICharacterPlacementRefreshable implementation
        public int RefreshmentConfigurePriority => 92;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.CurrentResolvedMode = this.RoamingMode;
            this.OnLocomotionVisualModeChanged?.Invoke();
        }

        // --- Helpers ---
        private void ResolveCurrentModeAndNotifyIfChanged(bool forceNotify)
        {
            CharacterLocomotionMode nextMode = this.ResolveLocomotionMode();

            if (!forceNotify && this.CurrentResolvedMode == nextMode)
            {
                return;
            }

            this.CurrentResolvedMode = nextMode;
            this.OnLocomotionVisualModeChanged?.Invoke();
        }

        private CharacterLocomotionMode ResolveLocomotionMode()
        {
            if (this.TerritorialEnemyBrainController == null)
            {
                return this.RoamingMode;
            }

            return this.TerritorialEnemyBrainController.CurrentAIState switch
            {
                TerritorialEnemyState.Roaming => this.RoamingMode,
                TerritorialEnemyState.Alert => this.AlertMode,
                TerritorialEnemyState.ReadyToAttack => this.ReadyToAttackMode,
                TerritorialEnemyState.PreAttack => this.PreAttackMode,
                TerritorialEnemyState.Attacking => this.AttackingMode,
                TerritorialEnemyState.Recovering => this.RecoveringMode,
                TerritorialEnemyState.Dead => this.DeadMode,
                _ => this.RoamingMode,
            };
        }
    }
}
