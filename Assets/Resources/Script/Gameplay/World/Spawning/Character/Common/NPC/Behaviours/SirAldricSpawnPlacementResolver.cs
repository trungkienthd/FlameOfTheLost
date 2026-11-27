
using UnityEngine;

namespace FlameOfTheLost
{
    public class SirAldricSpawnPlacementResolver : FOTLMonoBehaviour,
        INPCSpawnPlacementResolver
    {
        // --- Constants ---
        private const int SIR_ALDRIC_NPC_ID = 14201;

        // --- Configs ---
        [Header("Configs - Sir Aldric")]
        [SerializeField] private int _npcId = SIR_ALDRIC_NPC_ID;
        public int NPCId => _npcId;

        [Header("Configs - Companion Spawn")]
        [SerializeField] private Vector2 _playerSpawnOffset = new Vector2(1.5f, -0.5f);

        // --- References ---
        [Header("References - Post Boss")]
        [SerializeField] private Transform _postBossSpawnAnchor;

        private GameConditionController GameConditions => GameConditionController.Instance;

        private PlayerRootController Player => UnityEngine.Object.FindFirstObjectByType<PlayerRootController>();

        // --- Public APIs ---
        public bool TryResolveSpawnTarget(NPCSpawnEntry spawnEntry, CharacterRefreshTarget defaultTarget, out CharacterRefreshTarget resolvedTarget)
        {
            resolvedTarget = defaultTarget;

            if (spawnEntry == null || spawnEntry.NPCProfile == null) return false;
            if (spawnEntry.NPCProfile.Id != this.NPCId) return false;
            if (this.GameConditions == null) return false;

            bool accepted   = this.GameConditions.GetConditionState(GameConditionKey.QuestAnOathUnbrokenAccepted);
            bool freed      = this.GameConditions.GetConditionState(GameConditionKey.QuestAnOathUnbrokenAldricFreed);
            bool completed  = this.GameConditions.GetConditionState(GameConditionKey.QuestAnOathUnbrokenCompleted);

            // 1. Completed quest:
            // Aldric stays near the defeated Gor'Vakh.
            if (completed)
            {
                if (_postBossSpawnAnchor == null)
                {
                    Debug.LogWarning($"[{this.gameObject.name}.{nameof(SirAldricSpawnPlacementResolver)}] Post-boss spawn anchor is missing.", this);

                    return false;
                }

                resolvedTarget = new CharacterRefreshTarget(_postBossSpawnAnchor.position, _postBossSpawnAnchor.rotation, CharacterFacingStatus.Down);

                return true;
            }

            // 2. Active quest, but Aldric has not been freed: Keep the configured Forsaken Cells position.
            if (!accepted || !freed) return true;

            // 3. Active quest and freed: Spawn Aldric near the Player.
            PlayerRootController player = this.Player;
            if (player == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(SirAldricSpawnPlacementResolver)}] Player could not be found for companion placement.", this);
                return false;
            }

            Vector3 playerPosition = player.transform.position;

            Vector3 companionPosition = new Vector3(playerPosition.x + _playerSpawnOffset.x, playerPosition.y + _playerSpawnOffset.y, defaultTarget.Position.z);

            resolvedTarget = new CharacterRefreshTarget(companionPosition, Quaternion.identity, defaultTarget.Facing);

            return true;
        }
    }
}
