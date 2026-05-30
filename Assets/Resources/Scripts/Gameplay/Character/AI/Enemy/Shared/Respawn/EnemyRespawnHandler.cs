using UnityEngine;

namespace FlameOfTheLost
{
    public class EnemyRespawnHandler : CharacterBehaviourBaseController
    {
        [Header("Respawn Settings")]
        [SerializeField] private bool _respawnWhenPlayerRespawns = true;
        [SerializeField] private Transform _fixedRespawnPoint;
        [SerializeField] private CharacterFacingStatus _respawnFacing = CharacterFacingStatus.Down;
        public bool RespawnWhenPlayerRespawns => _respawnWhenPlayerRespawns;
        private Transform DefaultRespawnPoint => _fixedRespawnPoint;
        private CharacterFacingStatus RespawnFacing => _respawnFacing;

        public void RespawnEnemy()
        {
            if (this.DefaultRespawnPoint == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.EnemyRespawnHandler] Missing fixed respawn point.", this);
                return;
            }

            CharacterRespawnUtility.RespawnCharacter(character: this.CharacterRootController.gameObject,
                position: this.DefaultRespawnPoint.position,
                rotation: this.DefaultRespawnPoint.rotation,
                facing: this.RespawnFacing,
                spawnSourceType: PlayerSpawnSourceType.SpawnPoint
            );
        }
    }
}
