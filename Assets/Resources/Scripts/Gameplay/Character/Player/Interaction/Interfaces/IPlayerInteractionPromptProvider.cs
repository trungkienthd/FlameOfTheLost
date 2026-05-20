using UnityEngine;

namespace FlameOfTheLost
{
    public interface IPlayerInteractionPromptProvider
    {
        public string GetInteractionPromptText(GameObject interactor);
    }
}