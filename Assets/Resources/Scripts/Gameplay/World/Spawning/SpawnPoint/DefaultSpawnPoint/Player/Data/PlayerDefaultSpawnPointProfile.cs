using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "DSP_Player_New", menuName = "Data/FlameOfTheLost/World/Spawning/DefaultSpawnPoints/PlayerDefaultSpawnPointProfile")]
    public sealed class PlayerDefaultSpawnPointProfile : CharacterDefaultSpawnPointProfile
    {
        // --- Data / Configs ---
        //  1. Activation Settings
        [Header("Activation Settings")]
        [SerializeField] private bool _activateOnPlayerEnter = true;
        public bool ActivateOnPlayerEnter => _activateOnPlayerEnter;
    }
}