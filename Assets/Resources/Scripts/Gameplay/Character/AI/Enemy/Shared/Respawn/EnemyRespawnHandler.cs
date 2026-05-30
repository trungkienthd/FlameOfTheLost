using UnityEngine;

namespace FlameOfTheLost
{
    public class EnemyRespawnHandler : CharacterRespawnHandler
    {
        [Header("Respawn Settings")]
        [SerializeField] private bool _respawnWhenPlayerRespawns = true;
        [SerializeField] private CharacterDefaultSpawnPointController _defaultSpawnPoint;
        public bool RespawnWhenPlayerRespawns => _respawnWhenPlayerRespawns;
        private CharacterDefaultSpawnPointController DefaultSpawnPoint => _defaultSpawnPoint;

        public override void Respawn()
        {
            if (this.DefaultSpawnPoint == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.EnemyRespawnHandler] Missing default spawn point.", this);
                return;
            }

            CharacterRespawnUtility.RespawnCharacter(character: this.CharacterRootController.gameObject,
                position: this.DefaultSpawnPoint.SpawnPosition,
                rotation: this.DefaultSpawnPoint.SpawnRotation,
                facing: this.DefaultSpawnPoint.DefaultFacing,
                spawnSourceType: CharacterSpawnSourceType.DefaultSpawnPoint
            );
        }
    }
}
