using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Cpt_New", menuName = "Data/FlameOfTheLost/World/Spawning/SpawnPoints/Checkpoints/CheckpointProfile")]
    public sealed class CheckpointProfile : CharacterSpawnPointProfile
    {
        // --- Data / Configs ---
        //  1. Interaction
        [Header("Interaction")]
        [SerializeField] private string _interactionPromptText = "Rest";
        [SerializeField] private int _interactionPromptPriority = 0;
        public string InteractionPromptText => _interactionPromptText;
        public int InteractionPromptPriority => _interactionPromptPriority;
    }
}