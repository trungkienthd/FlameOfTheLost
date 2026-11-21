using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class AnimalLocomotionVisualModeProvider : CharacterBehaviourControllerBase,
        ICharacterLocomotionVisualModeProvider,
        ICharacterRefreshable
    {
        // --- References ---
        private AnimalBrainController AnimalBrainController
        {
            get
            {
                if (this.CharacterRootController is not AnimalRootController animalRootController) return null;

                return animalRootController.AnimalBrainController as AnimalBrainController;
            }
        }

        // --- Locomotion Mode Mapping ---
        [Header("Locomotion Mode Mapping")]
        [SerializeField] private CharacterLocomotionMode _idleMode = CharacterLocomotionMode.Default;
        [SerializeField] private CharacterLocomotionMode _wanderingMode = CharacterLocomotionMode.Default;

        [SerializeField] private CharacterLocomotionMode _grazeMode = CharacterLocomotionMode.Graze;
        [SerializeField] private CharacterLocomotionMode _lieDownMode = CharacterLocomotionMode.LieDown;
        [SerializeField] private CharacterLocomotionMode _restMode = CharacterLocomotionMode.Rest;
        [SerializeField] private CharacterLocomotionMode _sleepMode = CharacterLocomotionMode.Sleep;
        [SerializeField] private CharacterLocomotionMode _standUpMode = CharacterLocomotionMode.StandUp;

        [SerializeField] private CharacterLocomotionMode _alertMode = CharacterLocomotionMode.Alert;
        [SerializeField] private CharacterLocomotionMode _fleeingMode = CharacterLocomotionMode.Default;

        [SerializeField] private CharacterLocomotionMode _deadMode = CharacterLocomotionMode.Default;

        private CharacterLocomotionMode IdleMode => _idleMode;
        private CharacterLocomotionMode WanderingMode => _wanderingMode;

        private CharacterLocomotionMode GrazeMode => _grazeMode;
        private CharacterLocomotionMode LieDownMode => _lieDownMode;
        private CharacterLocomotionMode RestMode => _restMode;
        private CharacterLocomotionMode SleepMode => _sleepMode;
        private CharacterLocomotionMode StandUpMode => _standUpMode;

        private CharacterLocomotionMode AlertMode => _alertMode;
        private CharacterLocomotionMode FleeingMode => _fleeingMode;

        private CharacterLocomotionMode DeadMode => _deadMode;

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
        //  1. ICharacterLocomotionVisualModeProvider
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

        //  2. ICharacterRefreshable
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

            this.LogDebug($"Mode: {nextMode.ToString()} - {this.AnimalBrainController.CurrentState.ToString()}");

            if (!forceNotify && this.CurrentResolvedMode == nextMode) return;

            this.CurrentResolvedMode = nextMode;
            this.OnLocomotionVisualModeChanged?.Invoke();
        }

        private CharacterLocomotionMode ResolveLocomotionMode()
        {
            if (this.AnimalBrainController == null) return this.IdleMode;

            return this.AnimalBrainController.CurrentState switch
            {
                AnimalState.Idle => this.IdleMode,
                AnimalState.Wandering => this.WanderingMode,

                AnimalState.Graze => this.GrazeMode,
                AnimalState.LieDown => this.LieDownMode,
                AnimalState.Rest => this.RestMode,
                AnimalState.Sleep => this.SleepMode,
                AnimalState.StandUp => this.StandUpMode,

                AnimalState.Alert => this.AlertMode,
                AnimalState.Fleeing => this.FleeingMode,

                AnimalState.Dead => this.DeadMode,

                _ => this.IdleMode,
            };
        }
    }
}