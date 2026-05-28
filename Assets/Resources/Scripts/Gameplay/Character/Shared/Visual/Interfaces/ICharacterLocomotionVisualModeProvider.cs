using System;

namespace FlameOfTheLost
{
    public interface ICharacterLocomotionVisualModeProvider
    {
        public event Action OnLocomotionVisualModeChanged;

        public CharacterLocomotionMode GetLocomotionMode();
        public CharacterCarryItemType GetCarryItemType();
        public LightSourceType GetLightSourceType();
    }
}