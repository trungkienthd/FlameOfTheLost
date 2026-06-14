using UnityEngine;

namespace FlameOfTheLost
{
    public readonly struct CharacterPlacementContext
    {
        public readonly GameObject Character;
        public readonly Vector3 PlacementPosition;
        public readonly Quaternion PlacementRotation;
        public readonly CharacterFacingStatus PlacementFacing;
        public readonly CharacterPlacementSourceType PlacementSourceType;

        public CharacterPlacementContext(GameObject character, Vector3 placementPosition, Quaternion placementRotation, CharacterFacingStatus placementFacing, CharacterPlacementSourceType placementSourceType)
        {
            this.Character = character;
            this.PlacementPosition = placementPosition;
            this.PlacementRotation = placementRotation;
            this.PlacementFacing = placementFacing;
            this.PlacementSourceType = placementSourceType;
        }
    }
}
