using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    public class PlayerInteractionHoldHandler : PlayerInteractionSubHandlerBase
    {
        // --- Runtime ---
        //  1. Active Target
        private IPlayerHoldInteractable _activeTarget;
        public IPlayerHoldInteractable ActiveTarget => _activeTarget;

        private MonoBehaviour _activeTargetBehaviour;

        public bool IsHolding => _activeTarget != null;

        //  2. Timing
        private float _elapsedTime;
        private float _requiredDuration;
        public float ElapsedTime => _elapsedTime;
        public float RequiredDuration => _requiredDuration;

        public float NormalizedProgress => this.IsHolding && this.RequiredDuration > 0f
            ? Mathf.Clamp01(this.ElapsedTime / this.RequiredDuration)
            : 0f;

        // --- Unity Callbacks ---
        private void Update()
        {
            if (!this.IsHolding) return;

            if (!this.CanContinueActiveInteraction())
            {
                this.CancelHoldInteraction();
                return;
            }

            _elapsedTime += Time.deltaTime;

            if (this.ElapsedTime >= this.RequiredDuration)
            {
                this.CompleteHoldInteraction();
                return;
            }

            this.InteractionHandler?.RefreshPrompt();
        }
        private void OnDisable()
        {
            this.CancelHoldInteraction(refreshPrompt: false);
        }

        // --- Public APIs ---
        //  1. Begin Hold
        public bool TryBeginHoldInteraction(IPlayerHoldInteractable target)
        {
            if (!this.isActiveAndEnabled) return false;
            if (this.IsHolding) return false;
            if (this.InteractionHandler == null) return false;

            if (!this.InteractionHandler.CanProcessInteraction) return false;

            if (target is not MonoBehaviour targetBehaviour) return false;
            if (targetBehaviour == null || !targetBehaviour.isActiveAndEnabled) return false;
            if (target.InteractionType != PlayerInteractionType.Hold) return false;

            var targetHandler = this.InteractionHandler.InteractableTargetHandler;
            if (targetHandler == null || !targetHandler.IsInteractableNearby(target)) return false;

            GameObject interactor = this.InteractionHandler.GetInteractor();
            if (interactor == null) return false;
            if (!target.CanInteract(interactor)) return false;
            if (!target.CanContinueHoldInteraction(interactor)) return false;

            float duration = target.HoldDuration;
            if (duration <= 0f || float.IsNaN(duration) || float.IsInfinity(duration)) return false;

            if (!this.IsInteractInputPressed()) return false;

            _activeTarget = target;
            _activeTargetBehaviour = targetBehaviour;

            _elapsedTime = 0f;
            _requiredDuration = duration;

            this.InteractionHandler.RefreshPrompt();

            return true;
        }

        //  2. Cancel Hold
        public void CancelHoldInteraction(bool refreshPrompt = true)
        {
            if (!this.IsHolding) return;

            this.ClearHoldState();

            if (refreshPrompt)
            {
                this.InteractionHandler?.RefreshPrompt();
            }
        }

        //  3. Revalidate Active Hold
        public void ValidateActiveHoldInteraction(bool refreshPrompt = true)
        {
            if (!this.IsHolding) return;

            if (!this.CanContinueActiveInteraction())
            {
                this.CancelHoldInteraction(refreshPrompt);
            }
        }

        // --- Helpers ---
        //  1. Input Validation
        private bool IsInteractInputPressed()
        {
            PlayerInput playerInput = this.InteractionHandler?.GetPlayer()?.PlayerInput;

            InputAction interactAction =playerInput?.actions?.FindAction("Player/Interact", false);

            return interactAction != null && interactAction.enabled && interactAction.IsPressed();
        }

        //  2. Continuous Validation
        private bool CanContinueActiveInteraction()
        {
            if (!this.IsHolding) return false;

            if (this.InteractionHandler == null || !this.InteractionHandler.isActiveAndEnabled) return false;
            if (!this.InteractionHandler.CanProcessInteraction) return false;

            if (_activeTargetBehaviour == null || !_activeTargetBehaviour.isActiveAndEnabled) return false;
            if (_activeTarget.InteractionType != PlayerInteractionType.Hold) return false;

            if (!this.IsInteractInputPressed()) return false;

            var targetHandler = this.InteractionHandler.InteractableTargetHandler;
            if (targetHandler == null || !targetHandler.IsInteractableNearby(this.ActiveTarget)) return false;

            GameObject interactor = this.InteractionHandler.GetInteractor();
            if (interactor == null) return false;

            return this.ActiveTarget.CanContinueHoldInteraction(interactor);
        }

        //  3. Complete Hold
        private void CompleteHoldInteraction()
        {
            if (!this.CanContinueActiveInteraction())
            {
                this.CancelHoldInteraction();
                return;
            }

            IPlayerHoldInteractable completedTarget = this.ActiveTarget;

            // Clear transient state before executing gameplay operations that may publish synchronous events.
            this.ClearHoldState();

            this.InteractionHandler?.TryExecuteInteraction(completedTarget);
        }

        //  4. Cleanup
        private void ClearHoldState()
        {
            _activeTarget = null;
            _activeTargetBehaviour = null;

            _elapsedTime = 0f;
            _requiredDuration = 0f;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ClearHoldState();
        }
        protected override void CleanupStates()
        {
            this.ClearHoldState();

            base.CleanupStates();
        }
    }
}