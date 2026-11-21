using UnityEngine;

namespace FlameOfTheLost
{
    public interface IPlayerInteractable
    {
        public bool CanInteract(GameObject interactor);
        public bool TryInteract(GameObject interactor);
    }
}