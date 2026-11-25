using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCLocomotionVisualModeProvider : CharacterBehaviourControllerBase,
        ICharacterLocomotionVisualModeProvider,
        ICharacterRefreshable
    {
        // --- Runtime ---
        [Header("Runtime - Locomotion")]
        [SerializeField] private CharacterLocomotionMode _locomotionMode = CharacterLocomotionMode.Default;
        private CharacterLocomotionMode LocomotionMode => _locomotionMode;

        // --- Events ---
        public event Action OnLocomotionVisualModeChanged;

        // --- Public APIs ---
        //  1. ICharacterLocomotionVisualModeProvider implementation
        public CharacterLocomotionMode GetLocomotionMode()
        {
            return this.LocomotionMode;
        }
        public CharacterCarryItemType GetCarryItemType()
        {
            return CharacterCarryItemType.None;
        }
        public LightSourceType GetLightSourceType()
        {
            return LightSourceType.None;
        }

        //  2. Locomotion Mode
        public void SetLocomotionMode(CharacterLocomotionMode locomotionMode)
        {
            if (_locomotionMode == locomotionMode) return;

            _locomotionMode = locomotionMode;

            this.OnLocomotionVisualModeChanged?.Invoke();
        }

        //  3. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 40;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.SetLocomotionMode(CharacterLocomotionMode.Default);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _locomotionMode = CharacterLocomotionMode.Default;
        }
    }
}