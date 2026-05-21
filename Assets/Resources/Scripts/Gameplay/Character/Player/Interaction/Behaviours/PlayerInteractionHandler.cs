using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerInteractionHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _playerRoot;
        [SerializeField] private PlayerInteractionPromptPanelController _promptUIController;
        private GameObject PlayerRoot => _playerRoot;
        private PlayerInteractionPromptPanelController PromptUIController => _promptUIController;

        [Header("Input Display")]
        [SerializeField] private KeyCode _interactInputKey = KeyCode.E;
        private KeyCode InteractInputKey => _interactInputKey;
        private string InteractInputDisplayName => this.InteractInputKey.ToString();

        private readonly List<MonoBehaviour> _nearbyInteractableBehaviours = new();
        private IReadOnlyList<MonoBehaviour> NearbyInteractableBehaviours => _nearbyInteractableBehaviours;

        private void Awake()
        {
            this.CacheReferences();
            this.HidePrompt();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            IPlayerInteractable interactable = other.GetComponentInParent<IPlayerInteractable>();
            if (interactable == null) return;
            if (interactable is not MonoBehaviour behaviour) return;
            if (_nearbyInteractableBehaviours.Contains(behaviour)) return;

            GameObject interactor = this.ResolveInteractor();
            if (!interactable.CanInteract(interactor)) return;

            _nearbyInteractableBehaviours.Add(behaviour);
            this.RefreshPrompt();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            IPlayerInteractable interactable = other.GetComponentInParent<IPlayerInteractable>();
            if (interactable == null) return;
            if (interactable is not MonoBehaviour behaviour) return;

            _nearbyInteractableBehaviours.Remove(behaviour);
            this.RefreshPrompt();
        }

        public bool TryInteractNearest()
        {
            this.RemoveInvalidInteractables();

            IPlayerInteractable nearestInteractable = this.FindNearestInteractable();
            if (nearestInteractable == null)
            {
                this.RefreshPrompt();
                return false;
            }

            bool result = nearestInteractable.TryInteract(this.ResolveInteractor());

            this.RemoveInvalidInteractables();
            this.RefreshPrompt();

            return result;
        }

        private IPlayerInteractable FindNearestInteractable()
        {
            GameObject interactor = this.ResolveInteractor();
            Vector3 origin = interactor != null ? interactor.transform.position : this.transform.position;

            IPlayerInteractable nearestInteractable = null;
            float nearestSqrDistance = float.MaxValue;

            for (int i = 0; i < this.NearbyInteractableBehaviours.Count; i++)
            {
                MonoBehaviour behaviour = this.NearbyInteractableBehaviours[i];
                if (behaviour == null) continue;
                if (!behaviour.gameObject.activeInHierarchy) continue;
                if (behaviour is not IPlayerInteractable interactable) continue;
                if (!interactable.CanInteract(interactor)) continue;

                float sqrDistance = (behaviour.transform.position - origin).sqrMagnitude;
                if (sqrDistance >= nearestSqrDistance) continue;

                nearestSqrDistance = sqrDistance;
                nearestInteractable = interactable;
            }

            return nearestInteractable;
        }

        private void RefreshPrompt()
        {
            this.RemoveInvalidInteractables();

            IPlayerInteractable nearestInteractable = this.FindNearestInteractable();
            if (nearestInteractable == null)
            {
                this.HidePrompt();
                return;
            }

            string promptText = "Interact";

            if (nearestInteractable is IPlayerInteractionPromptProvider promptProvider)
            {
                promptText = promptProvider.GetInteractionPromptText(this.ResolveInteractor());
            }

            this.ShowPrompt($"[{this.InteractInputDisplayName}] {promptText}");
        }

        private void RemoveInvalidInteractables()
        {
            GameObject interactor = this.ResolveInteractor();

            for (int i = this.NearbyInteractableBehaviours.Count - 1; i >= 0; i--)
            {
                MonoBehaviour behaviour = this.NearbyInteractableBehaviours[i];

                if (behaviour == null)
                {
                    _nearbyInteractableBehaviours.RemoveAt(i);
                    continue;
                }

                if (!behaviour.gameObject.activeInHierarchy)
                {
                    _nearbyInteractableBehaviours.RemoveAt(i);
                    continue;
                }

                if (behaviour is not IPlayerInteractable interactable)
                {
                    _nearbyInteractableBehaviours.RemoveAt(i);
                    continue;
                }

                if (!interactable.CanInteract(interactor))
                {
                    _nearbyInteractableBehaviours.RemoveAt(i);
                }
            }
        }

        private GameObject ResolveInteractor()
        {
            return this.PlayerRoot != null ? this.PlayerRoot : this.transform.root.gameObject;
        }

        private void ShowPrompt(string text)
        {
            if (this.PromptUIController == null) return;
            this.PromptUIController.Show(text);
        }

        private void HidePrompt()
        {
            if (this.PromptUIController == null) return;
            this.PromptUIController.Hide();
        }

        private void CacheReferences()
        {
            if (this.PlayerRoot == null) _playerRoot = this.transform.root.gameObject;

            if (this.PromptUIController == null)
            {
                GameObject canvas = GameObject.Find("Canvas");
                _promptUIController = canvas.GetComponentInChildren<PlayerInteractionPromptPanelController>(true);
            }
        }
    }
}