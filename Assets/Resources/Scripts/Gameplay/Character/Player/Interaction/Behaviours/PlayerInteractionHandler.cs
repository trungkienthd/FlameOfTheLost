using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    public class PlayerInteractionHandler : CharacterBehaviourBaseController
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private GameObject _playerRoot;
        [SerializeField] private PlayerInteractionPromptPanelController _promptPanelController;
        [SerializeField] private PlayerMenuManager _playerMenuManager;
        [SerializeField] private PlayerInteractionPanelListManager _playerInteractionPanelListManager;
        private GameObject PlayerRoot => _playerRoot;
        private PlayerInteractionPromptPanelController PromptPanelController => _promptPanelController;
        private PlayerMenuManager PlayerMenuManager => _playerMenuManager;
        private PlayerInteractionPanelListManager PlayerInteractionPanelListManager => _playerInteractionPanelListManager;

        //  2. Input display
        [Header("Input Display")]
        [SerializeField] private InputActionReference _interactActionReference;
        private InputActionReference InteractActionReference => _interactActionReference;
        private InputAction InteractAction
        {
            get
            {
                return this.InteractActionReference != null ? this.InteractActionReference.action : null;
            }
        }

        //  3. Runtime: Nearby Interaction state
        private readonly List<MonoBehaviour> _nearbyInteractableBehaviours = new();
        private IReadOnlyList<MonoBehaviour> NearbyInteractableBehaviours => _nearbyInteractableBehaviours;

        //  4. Runtime: Prompt blockers
        private readonly HashSet<IPlayerInteractionPromptBlocker> _activePromptBlockers = new();
        public IReadOnlyCollection<IPlayerInteractionPromptBlocker> ActivePromptBlockers => _activePromptBlockers;
        private bool IsPromptBlocked => this.ActivePromptBlockers.Count > 0;

        // --- Unity Callbacks ---
        protected override void Awake()
        {
            base.Awake();

            this.RegisterPromptBlocker(this.PlayerMenuManager);
            this.RegisterPromptBlocker(this.PlayerInteractionPanelListManager);

            this.HidePrompt();
        }
        private void OnDestroy()
        {
            this.UnregisterPromptBlocker(this.PlayerMenuManager);
            this.UnregisterPromptBlocker(this.PlayerInteractionPanelListManager);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.isTrigger) return;

            IPlayerInteractable interactable = other.GetComponentInParent<IPlayerInteractable>();
            if (interactable == null) return;
            if (interactable is not MonoBehaviour behaviour) return;
            if (_nearbyInteractableBehaviours.Contains(behaviour)) return;

            GameObject interactor = this.ResolveInteractor();
            if (!interactable.CanInteract(interactor)) return;

            Debug.Log($"[{this.PlayerRoot.name}.PlayerInteractionHandler] Detected nearby interactable: {behaviour.gameObject.name}", behaviour);

            _nearbyInteractableBehaviours.Add(behaviour);
            this.RefreshPrompt();
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.isTrigger) return;

            IPlayerInteractable interactable = other.GetComponentInParent<IPlayerInteractable>();
            if (interactable == null) return;
            if (interactable is not MonoBehaviour behaviour) return;

            _nearbyInteractableBehaviours.Remove(behaviour);
            this.RefreshPrompt();
        }

        // --- Public APIs ---
        public bool TryInteractNearest()
        {
            if (this.IsPromptBlocked)
            {
                this.HidePrompt();
                return false;
            }

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

        public void RegisterPromptBlocker(IPlayerInteractionPromptBlocker blocker)
        {
            if (blocker == null) return;

            blocker.PromptBlockStarted += this.HandlePromptBlockStarted;
            blocker.PromptBlockEnded += this.HandlePromptBlockEnded;

            if (blocker.IsBlockingPlayerInteractionPrompt)
            {
                this.HandlePromptBlockStarted(blocker);
            }
        }
        public void UnregisterPromptBlocker(IPlayerInteractionPromptBlocker blocker)
        {
            if (blocker == null) return;

            blocker.PromptBlockStarted -= this.HandlePromptBlockStarted;
            blocker.PromptBlockEnded -= this.HandlePromptBlockEnded;

            if (_activePromptBlockers.Remove(blocker))
            {
                this.RefreshPrompt();
            }
        }

        // --- Helpers ---

        private void RefreshPrompt()
        {
            if (this.IsPromptBlocked)
            {
                this.HidePrompt();
                return;
            }

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

            this.ShowPrompt($"[{this.GetInteractInputDisplayName()}] {promptText}");
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
            if (this.PromptPanelController == null) return;
            this.PromptPanelController.Show(text);
        }

        private string GetInteractInputDisplayName()
        {
            InputAction interactAction = this.InteractAction;

            if (interactAction == null) return "I";

            for (int i = 0; i < interactAction.bindings.Count; i++)
            {
                InputBinding binding = interactAction.bindings[i];

                if (binding.isComposite || binding.isPartOfComposite) continue;
                if (string.IsNullOrWhiteSpace(binding.effectivePath)) continue;
                if (!binding.effectivePath.Contains("<Keyboard>")) continue;

                string displayName = interactAction.GetBindingDisplayString(i, InputBinding.DisplayStringOptions.DontIncludeInteractions);

                return string.IsNullOrWhiteSpace(displayName) ? "E" : displayName;
            }

            return "I";
        }

        private void HidePrompt()
        {
            if (this.PromptPanelController == null) return;
            this.PromptPanelController.Hide();
        }

        private void HandlePromptBlockStarted(IPlayerInteractionPromptBlocker blocker)
        {
            if (blocker == null) return;

            _activePromptBlockers.Add(blocker);
            this.HidePrompt();
        }

        private void HandlePromptBlockEnded(IPlayerInteractionPromptBlocker blocker)
        {
            if (blocker == null) return;

            _activePromptBlockers.Remove(blocker);
            this.RefreshPrompt();
        }

        protected override void CacheReferences()
        {
            if (this.PlayerRoot == null) _playerRoot = this.transform.root.gameObject;

            GameObject canvas = GameObject.Find("Canvas");
            if (this.PromptPanelController == null)
            {
                if (canvas != null) _promptPanelController = canvas.GetComponentInChildren<PlayerInteractionPromptPanelController>(true);
            }
            if (this.PlayerMenuManager == null)
            {
                if (canvas != null) _playerMenuManager = canvas.GetComponentInChildren<PlayerMenuManager>(true);
            }
            if (this.PlayerInteractionPanelListManager == null)
            {
                if (canvas != null) _playerInteractionPanelListManager = canvas.GetComponentInChildren<PlayerInteractionPanelListManager>(true);
            }

            if (this.InteractActionReference == null)
            {
                PlayerInput playerInput = this.GetComponentInParent<PlayerInput>();
                if (playerInput != null)
                {
                    InputAction interactAction = playerInput.actions.FindAction("Interact", true);
                    if (interactAction != null) _interactActionReference = InputActionReference.Create(interactAction);
                }
            }
        }
    }
}