using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class ToxicschroomLocomotionVisualModeProvider : CharacterBehaviourBaseController,
        ICharacterLocomotionVisualModeProvider,
        ICharacterRespawnRefreshable
    {
        // --- States ---
        //  1. References
        private EnemyBrainController EnemyBrainController => this.CharacterRootController is EnemyRootController enemyRootController ? enemyRootController.EnemyBrainController : null;

        //  2. Locomotion Mode Mapping
        [Header("Locomotion Mode Mapping")]
        [SerializeField] private CharacterLocomotionMode _roamingMode = CharacterLocomotionMode.Default;
        [SerializeField] private CharacterLocomotionMode _alertMode = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _readyToAttackMode = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _attackingMode = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _recoveringMode = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _deadMode = CharacterLocomotionMode.Default;
        private CharacterLocomotionMode RoamingMode => _roamingMode;
        private CharacterLocomotionMode AlertMode => _alertMode;
        private CharacterLocomotionMode ReadyToAttackMode => _readyToAttackMode;
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

        //  2. ICharacterRespawnRefreshable implementation
        public int RespawnRefreshPriority => 92;
        public void RefreshAfterRespawn(CharacterRespawnRefreshContext context)
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
            if (this.EnemyBrainController == null)
            {
                return this.RoamingMode;
            }

            return this.EnemyBrainController.CurrentAIState switch
            {
                CharacterAIState.Roaming => this.RoamingMode,
                CharacterAIState.Alert => this.AlertMode,
                CharacterAIState.ReadyToAttack => this.ReadyToAttackMode,
                CharacterAIState.Attacking => this.AttackingMode,
                CharacterAIState.Recovering => this.RecoveringMode,
                CharacterAIState.Dead => this.DeadMode,
                _ => this.RoamingMode,
            };
        }
    }
}
