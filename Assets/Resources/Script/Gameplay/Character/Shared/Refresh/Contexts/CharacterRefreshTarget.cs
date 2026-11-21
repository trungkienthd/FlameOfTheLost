using UnityEngine;

namespace FlameOfTheLost
{
    public readonly struct CharacterRefreshTarget
    {
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly CharacterFacingStatus Facing;

        public CharacterRefreshTarget(Vector3 position, Quaternion rotation, CharacterFacingStatus facing)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.Facing = facing;
        }
    }
}