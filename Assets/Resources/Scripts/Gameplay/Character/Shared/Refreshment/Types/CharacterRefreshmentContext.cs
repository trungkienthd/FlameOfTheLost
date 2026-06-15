using UnityEngine;

namespace FlameOfTheLost
{
    public readonly struct CharacterRefreshmentContext
    {
        public readonly CharacterRootController Character;
        public readonly Vector3 RefreshmentPosition;
        public readonly Quaternion RefreshmentRotation;
        public readonly CharacterFacingStatus RefreshmentFacing;
        public readonly CharacterPlacementSource PlacementSource;

        public CharacterRefreshmentContext(CharacterRootController character, Vector3 refreshmentPosition, Quaternion refreshmentRotation, CharacterFacingStatus refreshmentFacing, CharacterPlacementSource placementSource)
        {
            this.Character = character;
            this.RefreshmentPosition = refreshmentPosition;
            this.RefreshmentRotation = refreshmentRotation;
            this.RefreshmentFacing = refreshmentFacing;
            this.PlacementSource = placementSource;
        }
    }
}
