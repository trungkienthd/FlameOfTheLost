using UnityEngine;

namespace FlameOfTheLost
{
    public struct CharacterRespawnRefreshContext
    {
        public readonly GameObject Character;
        public readonly Vector3 RespawnPosition;
        public readonly Quaternion RespawnRotation;
        public readonly CharacterFacingStatus RespawnFacing;
        public readonly CharacterSpawnSourceType SpawnSourceType;

        public CharacterRespawnRefreshContext(GameObject character, Vector3 respawnPosition, Quaternion respawnRotation, CharacterFacingStatus respawnFacing, CharacterSpawnSourceType spawnSourceType)
        {
            this.Character = character;
            this.RespawnPosition = respawnPosition;
            this.RespawnRotation = respawnRotation;
            this.RespawnFacing = respawnFacing;
            this.SpawnSourceType = spawnSourceType;
        }
    }
}
