using UnityEngine;

namespace FlameOfTheLost
{
    public interface IPlayerInteractionPromptSource
    {
        public bool CanShowInteractionPrompt(GameObject interactor);
        public string GetInteractionPromptText(GameObject interactor);
    }
}