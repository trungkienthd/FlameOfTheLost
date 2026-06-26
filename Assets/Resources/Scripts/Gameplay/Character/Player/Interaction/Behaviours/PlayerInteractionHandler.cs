using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    public class PlayerInteractionHandler : CharacterBehaviourControllerBase, 
        ICharacterRefreshable,
        ICharacterDeathDisableable
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

        //  4. Runtime: Nearby Prompt Source state
        private readonly List<MonoBehaviour> _nearbyPromptSourceBehaviours = new();
        private IReadOnlyList<MonoBehaviour> NearbyPromptSourceBehaviours => _nearbyPromptSourceBehaviours;

        //  5. Runtime: Prompt blockers
        private readonly HashSet<IPlayerInteractionPromptBlocker> _activePromptBlockers = new();
        public IReadOnlyCollection<IPlayerInteractionPromptBlocker> ActivePromptBlockers => _activePromptBlockers;
        private bool IsPromptBlocked => this.ActivePromptBlockers.Count > 0;

        //  6. Death Disable State (Runtime)
        [Header("Death Disable (Runtime)")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled
        {
            get => _isDeathDisabled;
            private set => _isDeathDisabled = value;
        }

        // --- Unity Callbacks ---
        private void OnDestroy()
        {
            this.UnregisterPromptBlocker(this.PlayerMenuManager);
            this.UnregisterPromptBlocker(this.PlayerInteractionPanelListManager);

            this.ClearNearbyInteractionState();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (this.IsDeathDisabled) return;
            if (!other.isTrigger) return;

            this.TryRegisterNearbyInteractable(other);
            this.TryRegisterNearbyPromptSource(other);

            this.RefreshPrompt();
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.isTrigger) return;

            this.TryUnregisterNearbyInteractable(other);
            this.TryUnregisterNearbyPromptSource(other);

            if (!this.IsDeathDisabled)
            {
                this.RefreshPrompt();
            }
        }

        // --- Public APIs ---
        //  1. (Nearest) Interaction Handling
        public bool TryInteractNearest()
        {
            if (this.IsDeathDisabled)
            {
                this.HidePrompt();
                return false;
            }

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

        //  2. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 50;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.IsDeathDisabled = false;

            this.ClearNearbyInteractionState();
            this.HidePrompt();

            // Nearby trigger cache will be rebuilt naturally when the player enters trigger areas again.
            // If later you need instant prompt after teleporting into a trigger,
            // add an overlap scan here.
        }

        //  3. ICharacterDeathDisableable implementation
        public int DeathDisablePriority => 15;
        public void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;

            this.ClearNearbyInteractionState();
            this.HidePrompt();
        }

        // --- Helpers ---
        //  1. Prompt Blocker (UI) Registration
        private void RegisterPromptBlocker(IPlayerInteractionPromptBlocker blocker)
        {
            if (blocker == null) return;

            blocker.PromptBlockStarted += this.HandlePromptBlockStarted;
            blocker.PromptBlockEnded += this.HandlePromptBlockEnded;

            if (blocker.IsBlockingPlayerInteractionPrompt)
            {
                this.HandlePromptBlockStarted(blocker);
            }
        }
        private void UnregisterPromptBlocker(IPlayerInteractionPromptBlocker blocker)
        {
            if (blocker == null) return;

            blocker.PromptBlockStarted -= this.HandlePromptBlockStarted;
            blocker.PromptBlockEnded -= this.HandlePromptBlockEnded;

            if (_activePromptBlockers.Remove(blocker))
            {
                this.RefreshPrompt();
            }
        }

        //  2. Nearby Interactable Registration
        private void TryRegisterNearbyInteractable(Collider2D other)
        {
            IPlayerInteractable interactable = other.GetComponentInParent<IPlayerInteractable>();
            if (interactable == null) return;
            if (interactable is not MonoBehaviour behaviour) return;
            if (this.NearbyInteractableBehaviours.Contains(behaviour)) return;

            GameObject interactor = this.ResolveInteractor();
            if (!interactable.CanInteract(interactor)) return;

            //Debug.Log($"[{this.PlayerRoot.name}.{this.GetType().Name}] Detected nearby interactable: {behaviour.gameObject.name}", behaviour);

            _nearbyInteractableBehaviours.Add(behaviour);
        }
        private void TryUnregisterNearbyInteractable(Collider2D other)
        {
            IPlayerInteractable interactable = other.GetComponentInParent<IPlayerInteractable>();
            if (interactable == null) return;
            if (interactable is not MonoBehaviour behaviour) return;

            _nearbyInteractableBehaviours.Remove(behaviour);
        }

        //  3. Nearby Prompt Source Registration
        private void TryRegisterNearbyPromptSource(Collider2D other)
        {
            IPlayerInteractionPromptSource promptSource = other.GetComponentInParent<IPlayerInteractionPromptSource>();
            if (promptSource == null) return;
            if (promptSource is not MonoBehaviour behaviour) return;
            if (this.NearbyPromptSourceBehaviours.Contains(behaviour)) return;

            //Debug.Log($"[{this.PlayerRoot.name}.{this.GetType().Name}] Detected nearby prompt source: {behaviour.gameObject.name}", behaviour);

            _nearbyPromptSourceBehaviours.Add(behaviour);

            if (promptSource is IPlayerInteractionPromptRefreshNotifier refreshNotifier)
            {
                refreshNotifier.PromptRefreshRequested += this.HandlePromptRefreshRequested;
            }
        }
        private void TryUnregisterNearbyPromptSource(Collider2D other)
        {
            IPlayerInteractionPromptSource promptSource = other.GetComponentInParent<IPlayerInteractionPromptSource>();
            if (promptSource == null) return;
            if (promptSource is not MonoBehaviour behaviour) return;

            if (_nearbyPromptSourceBehaviours.Remove(behaviour))
            {
                if (promptSource is IPlayerInteractionPromptRefreshNotifier refreshNotifier)
                {
                    refreshNotifier.PromptRefreshRequested -= this.HandlePromptRefreshRequested;
                }
            }
        }

        //  4. Refresh Prompt
        private void RefreshPrompt()
        {
            if (this.IsPromptBlocked)
            {
                this.HidePrompt();
                return;
            }

            this.RemoveInvalidInteractables();
            this.RemoveInvalidPromptSources();

            IPlayerInteractable nearestInteractable = this.FindNearestInteractable();
            if (nearestInteractable != null)
            {
                this.ShowInteractablePrompt(nearestInteractable);
                return;
            }

            IPlayerInteractionPromptSource nearestPromptSource = this.FindBestPromptSource();
            if (nearestPromptSource != null)
            {
                this.ShowPromptSourcePrompt(nearestPromptSource);
                return;
            }

            this.HidePrompt();
        }

        //  5. Show Prompt 
        private void ShowInteractablePrompt(IPlayerInteractable interactable)
        {
            string promptText = "Interact";

            if (interactable is IPlayerInteractionPromptSource promptSource)
            {
                promptText = promptSource.GetInteractionPromptText(this.ResolveInteractor());
            }

            this.ShowPrompt($"[{this.GetInteractInputDisplayName()}] {promptText}");
        }

        private void ShowPromptSourcePrompt(IPlayerInteractionPromptSource promptSource)
        {
            string promptText = promptSource.GetInteractionPromptText(this.ResolveInteractor());
            this.ShowPrompt(promptText);
        }

        //  6. Find Nearest
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

        private IPlayerInteractionPromptSource FindBestPromptSource()
        {
            GameObject interactor = this.ResolveInteractor();
            Vector3 origin = interactor != null ? interactor.transform.position : this.transform.position;

            IPlayerInteractionPromptSource bestPromptSource = null;
            int bestPriority = int.MinValue;
            float bestSqrDistance = float.MaxValue; // Nearest

            for (int i = 0; i < this.NearbyPromptSourceBehaviours.Count; i++)
            {
                MonoBehaviour behaviour = this.NearbyPromptSourceBehaviours[i];
                if (behaviour == null) continue;
                if (!behaviour.gameObject.activeInHierarchy) continue;
                if (behaviour is not IPlayerInteractionPromptSource promptSource) continue;
                if (!promptSource.CanShowInteractionPrompt(interactor)) continue;

                int priority = promptSource.InteractionPromptPriority;
                float sqrDistance = (behaviour.transform.position - origin).sqrMagnitude;

                bool isHigherPriority = priority > bestPriority;
                bool isSamePriorityButCloser = priority == bestPriority && sqrDistance < bestSqrDistance;

                if (!isHigherPriority && !isSamePriorityButCloser) continue;

                bestPriority = priority;
                bestSqrDistance = sqrDistance;
                bestPromptSource = promptSource;
            }

            return bestPromptSource;
        }

        //  7. Cleanup
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
        private void RemoveInvalidPromptSources()
        {
            for (int i = this.NearbyPromptSourceBehaviours.Count - 1; i >= 0; i--)
            {
                MonoBehaviour behaviour = this.NearbyPromptSourceBehaviours[i];

                if (behaviour == null)
                {
                    _nearbyPromptSourceBehaviours.RemoveAt(i);
                    continue;
                }

                if (!behaviour.gameObject.activeInHierarchy)
                {
                    if (behaviour is IPlayerInteractionPromptRefreshNotifier refreshNotifier)
                    {
                        refreshNotifier.PromptRefreshRequested -= this.HandlePromptRefreshRequested;
                    }

                    _nearbyPromptSourceBehaviours.RemoveAt(i);
                    continue;
                }

                if (behaviour is not IPlayerInteractionPromptSource promptSource)
                {
                    if (behaviour is IPlayerInteractionPromptRefreshNotifier refreshNotifier)
                    {
                        refreshNotifier.PromptRefreshRequested -= this.HandlePromptRefreshRequested;
                    }

                    _nearbyPromptSourceBehaviours.RemoveAt(i);
                    continue;
                }
            }
        }
        private void ClearNearbyInteractionState()
        {
            for (int i = 0; i < this.NearbyPromptSourceBehaviours.Count; i++)
            {
                MonoBehaviour behaviour = this.NearbyPromptSourceBehaviours[i];
                if (behaviour == null) continue;

                if (behaviour is IPlayerInteractionPromptRefreshNotifier refreshNotifier)
                {
                    refreshNotifier.PromptRefreshRequested -= this.HandlePromptRefreshRequested;
                }
            }

            _nearbyInteractableBehaviours.Clear();
            _nearbyPromptSourceBehaviours.Clear();
        }

        //  8. Resolve Interactor
        private GameObject ResolveInteractor()
        {
            return this.PlayerRoot;
        }

        //  9. Prompt Display (Show/Hide)
        private void ShowPrompt(string text)
        {
            if (this.PromptPanelController == null) return;

            this.PromptPanelController.Show(text);
        }
        private void HidePrompt()
        {
            if (this.PromptPanelController == null) return;

            this.PromptPanelController.Hide();
        }

        //  10. Get Interact Input Display Name
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

                return string.IsNullOrWhiteSpace(displayName) ? "I" : displayName;
            }

            return "I";
        }

        //  11. Handler Events 
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
        private void HandlePromptRefreshRequested()
        {
            if (this.IsDeathDisabled)
            {
                this.HidePrompt();
                return;
            }

            this.RefreshPrompt();
        }

        //  12. FOTL Base Behaviour Overrides 
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PlayerRoot == null) _playerRoot = this.transform.root.gameObject;
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.PlayerRoot == null) Debug.LogError($"[{this.CharacterName}.{nameof(PlayerInteractionHandler)}] PlayerRoot reference is not assigned.", this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.IsDeathDisabled = false;

            _nearbyInteractableBehaviours.Clear();
            _nearbyPromptSourceBehaviours.Clear();
            _activePromptBlockers.Clear();
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            GameObject canvas = GameObject.Find("Canvas");

            if (this.PromptPanelController              == null && canvas != null) _promptPanelController               = canvas.GetComponentInChildren<PlayerInteractionPromptPanelController>(true);
            if (this.PlayerMenuManager                  == null && canvas != null) _playerMenuManager                   = canvas.GetComponentInChildren<PlayerMenuManager>(true);
            if (this.PlayerInteractionPanelListManager  == null && canvas != null) _playerInteractionPanelListManager   = canvas.GetComponentInChildren<PlayerInteractionPanelListManager>(true);

            if (this.InteractActionReference == null)
            {
                PlayerInput playerInput = this.GetComponentInParent<PlayerInput>();
                if (playerInput != null)
                {
                    InputAction interactAction = playerInput.actions.FindAction("Interact", false);
                    if (interactAction != null) _interactActionReference = InputActionReference.Create(interactAction);
                }
            }
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.PromptPanelController == null) Debug.LogError($"[{this.CharacterName}.{nameof(PlayerInteractionHandler)}] Prompt Panel Controller reference is not assigned.", this);
            if (this.PlayerMenuManager == null) Debug.LogError($"[{this.CharacterName}.{nameof(PlayerInteractionHandler)}] Player Menu Manager reference is not assigned.", this);
            if (this.PlayerInteractionPanelListManager == null) Debug.LogError($"[{this.CharacterName}.{nameof(PlayerInteractionHandler)}] Player Interaction Panel List Manager reference is not assigned.", this);
            if (this.InteractActionReference == null || this.InteractActionReference.action == null) Debug.LogError($"[{this.CharacterName}.{nameof(PlayerInteractionHandler)}] Interact Action Reference is not assigned or has no action.", this);
        }
        protected override void BindExternalReferences()
        {
            base.BindExternalReferences();

            this.RegisterPromptBlocker(this.PlayerMenuManager);
            this.RegisterPromptBlocker(this.PlayerInteractionPanelListManager);
        }
        protected override void InitializeAfterBinding()
        {
            base.InitializeAfterBinding();

            this.HidePrompt();
        }
    }
}