using UnityEngine;

namespace FlameOfTheLost
{
    public struct CharacterRespawnRefreshContext
    {
        public readonly GameObject Character;
        public readonly Vector3 RespawnPosition;
        public readonly Quaternion RespawnRotation;
        public readonly CharacterFacingStatusType RespawnFacing;
        public readonly PlayerSpawnSourceType SpawnSourceType;

        public CharacterRespawnRefreshContext(GameObject character, Vector3 respawnPosition, Quaternion respawnRotation, CharacterFacingStatusType respawnFacing, PlayerSpawnSourceType spawnSourceType)
        {
            this.Character = character;
            this.RespawnPosition = respawnPosition;
            this.RespawnRotation = respawnRotation;
            this.RespawnFacing = respawnFacing;
            this.SpawnSourceType = spawnSourceType;
        }
    }
}
