using UnityEngine;

namespace FlameOfTheLost
{
    public interface IPlayerHoldInteractable : IPlayerInteractable
    {
        public PlayerInteractionType InteractionType { get; }

        public float HoldDuration { get; }

        public bool CanContinueHoldInteraction(GameObject interactor);
    }
}