using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "DSP_Generic_New", menuName = "Data/FlameOfTheLost/Level/Spawning/DefaultSpawnPoints/CharacterDefaultSpawnPointProfile")]
    public class CharacterDefaultSpawnPointProfile : FOTLWorldObjectProfile
    {
        // --- Data / Configs ---
        //  1. Spawn Settings
        [Header("Spawn Settings")]
        [SerializeField] private CharacterFacingStatus _defaultFacing = CharacterFacingStatus.Down;
        public CharacterFacingStatus DefaultFacing => _defaultFacing;
    }
}
