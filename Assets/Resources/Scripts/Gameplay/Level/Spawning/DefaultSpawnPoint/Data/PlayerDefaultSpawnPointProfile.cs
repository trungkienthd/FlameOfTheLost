using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "DSp_Player_New", menuName = "Data/FlameOfTheLost/Level/Spawning/DefaultSpawnPoints/PlayerDefaultSpawnPointProfile")]
    public sealed class PlayerDefaultSpawnPointProfile : CharacterDefaultSpawnPointProfile
    {
        // --- Data / Configs ---
        //  1. Activation Settings
        [Header("Activation Settings")]
        [SerializeField] private bool _activateOnPlayerEnter = true;
        public bool ActivateOnPlayerEnter => _activateOnPlayerEnter;
    }
}