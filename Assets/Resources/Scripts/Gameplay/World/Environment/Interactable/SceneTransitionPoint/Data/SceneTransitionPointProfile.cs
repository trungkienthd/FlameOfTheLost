using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "SceneTransitionPointProfile", menuName = "Scriptable Objects/SceneTransitionPointProfile")]
    public class SceneTransitionPointProfile : FOTLWorldObjectProfile
    {
        // --- Data / Configs ---
        //  1. Transition Destination
        [Header("Configs - Transition Destination")]
        [SerializeField] private string _destinationSceneName;
        [SerializeField] private string _destinationArrivalKey;
        public string DestinationSceneName => _destinationSceneName;
        public string DestinationArrivalKey => _destinationArrivalKey;

        //  2. Interaction
        [Header("Configs - Interaction")]
        [SerializeField] private string _interactionPromptText = "Enter";
        public string InteractionPromptText => _interactionPromptText;
    }
}
