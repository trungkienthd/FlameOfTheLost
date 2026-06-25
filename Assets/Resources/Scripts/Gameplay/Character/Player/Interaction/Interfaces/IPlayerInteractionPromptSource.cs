using UnityEngine;

namespace FlameOfTheLost
{
    public interface IPlayerInteractionPromptSource
    {
        public int InteractionPromptPriority { get; }

        public bool CanShowInteractionPrompt(GameObject interactor);
        public string GetInteractionPromptText(GameObject interactor);
    }
}