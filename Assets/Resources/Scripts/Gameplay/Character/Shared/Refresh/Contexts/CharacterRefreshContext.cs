using UnityEngine;

namespace FlameOfTheLost
{
    public readonly struct CharacterRefreshContext
    {
        public readonly CharacterRootController Character;
        public readonly CharacterRefreshTarget RefreshTarget;
        public readonly CharacterRefreshOrigin RefreshOrigin;

        public CharacterRefreshContext(CharacterRootController character, CharacterRefreshTarget refreshTarget, CharacterRefreshOrigin refreshOrigin)
        {
            this.Character = character;
            this.RefreshTarget = refreshTarget;
            this.RefreshOrigin = refreshOrigin;
        }
    }
}
