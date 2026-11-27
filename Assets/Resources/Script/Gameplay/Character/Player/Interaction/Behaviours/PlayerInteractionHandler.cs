using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(PlayerInteractionInteractableTargetHandler))]
    [RequireComponent(typeof(PlayerInteractionPromptSourceTargetHandler))]
    [RequireComponent(typeof(PlayerInteractionPromptHandler))]
    [RequireComponent(typeof(PlayerInteractionPromptBlockerHandler))]
    [RequireComponent(typeof(PlayerInteractionHoldHandler))]
    public class PlayerInteractionHandler : CharacterBehaviourControllerBase, 
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- References ---
        [Header("References")]

        //  1. Sub-handlers
        [SerializeField] private PlayerInteractionInteractableTargetHandler _interactableTargetHandler;
        [SerializeField] private PlayerInteractionPromptSourceTargetHandler _promptSourceTargetHandler;
        [SerializeField] private PlayerInteractionPromptHandler _promptHandler;
        [SerializeField] private PlayerInteractionPromptBlockerHandler _promptBlockerHandler;
        [SerializeField] private PlayerInteractionHoldHandler _holdHandler;
        public PlayerInteractionInteractableTargetHandler InteractableTargetHandler => _interactableTargetHandler;
        public PlayerInteractionPromptSourceTargetHandler PromptSourceTargetHandler => _promptSourceTargetHandler;
        private PlayerInteractionPromptHandler PromptHandler => _promptHandler;
        private PlayerInteractionPromptBlockerHandler PromptBlockerHandler => _promptBlockerHandler;
        public PlayerInteractionHoldHandler HoldHandler => _holdHandler;

        //  2. Player-related
        private GameObject PlayerRoot => this.CharacterRootController != null
            ? this.CharacterRootController.CharacterRoot
            : null;
        private PlayerRootController PlayerRootController => this.GetPlayer();
        private PlayerCarryLoadoutController CarryLoadoutController => this.PlayerRootController != null 
            ? this.PlayerRootController.PlayerCarryLoadoutController
            : null;
        private InventoryController InventoryController => this.PlayerRootController != null 
            ? this.PlayerRootController.InventoryController
            : null;

        // --- Runtime ---
        //  1. Death Disable State (Runtime)
        [Header("Death Disable (Runtime)")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled
        {
            get => _isDeathDisabled;
            private set => _isDeathDisabled = value;
        }

        //  2. Interaction State (Runtime)
        public bool IsPromptBlocked => this.PromptBlockerHandler != null && this.PromptBlockerHandler.IsPromptBlocked; 
        public bool CanProcessInteraction => this.isActiveAndEnabled && !this.IsDeathDisabled && !this.IsPromptBlocked;
        public bool IsHoldingInteraction => this.HoldHandler != null && this.HoldHandler.IsHolding;

        // --- Unity Callbacks ---
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!this.isActiveAndEnabled) return;
            if (this.IsDeathDisabled) return;
            if (other == null || !other.isTrigger) return;

            this.InteractableTargetHandler?.RegisterNearbyCollider(other);
            this.PromptSourceTargetHandler?.RegisterNearbyCollider(other);

            this.RefreshPrompt();
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == null || !other.isTrigger) return;

            this.InteractableTargetHandler?.UnregisterNearbyCollider(other);
            this.PromptSourceTargetHandler?.UnregisterNearbyCollider(other);

            this.HoldHandler?.ValidateActiveHoldInteraction(refreshPrompt: false);

            if (!this.IsDeathDisabled)
            {
                this.RefreshPrompt();
            }
        }
        private void OnDisable()
        {
            this.HoldHandler?.CancelHoldInteraction(refreshPrompt: false);

            this.PromptHandler?.HidePrompt();
        }

        // --- Public APIs ---
        //  0.1. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 50;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.IsDeathDisabled = false;

            this.ClearInteractionState();
        }

        //  0.2. ICharacterDeathDisableable implementation
        public int DeathDisablePriority => 15;
        public void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;

            this.ClearInteractionState();
        }

        //  1. Player-related Getters
        public GameObject GetInteractor() 
        {
            return this.PlayerRoot; 
        }
        public PlayerRootController GetPlayer()
        {
            return this.CharacterRootController is PlayerRootController player ? player : null;
        }

        //  2. Resolve Interaction Type
        public PlayerInteractionType GetInteractionType(IPlayerInteractable interactable)
        {
            if (interactable is IPlayerHoldInteractable holdInteractable) return holdInteractable.InteractionType;

            return PlayerInteractionType.Press;
        }

        //  3. (Nearest) Interaction Handling
        public bool TryInteractNearest()
        {
            if (!this.CanProcessInteraction)
            {
                this.PromptHandler?.HidePrompt();
                return false;
            }

            if (this.IsHoldingInteraction) return false;

            if (this.InteractableTargetHandler == null)
            {
                this.PromptHandler?.HidePrompt();
                return false;
            }

            GameObject interactor = this.GetInteractor();
            if (interactor == null) return false;

            IPlayerInteractable nearestInteractable = this.InteractableTargetHandler.GetNearestInteractable(interactor);
            if (nearestInteractable == null)
            {
                this.RefreshPrompt();
                return false;
            }

            PlayerInteractionType interactionType = this.GetInteractionType(nearestInteractable);

            switch (interactionType)
            {
                case PlayerInteractionType.Press:
                    return this.TryExecuteInteraction(nearestInteractable);

                case PlayerInteractionType.Hold:
                    if (this.HoldHandler == null) return false;

                    if (nearestInteractable is not IPlayerHoldInteractable holdInteractable) return false;

                    return this.HoldHandler.TryBeginHoldInteraction(holdInteractable);

                default:
                    return false;
            }
        }

        //  4. Cancel Active Hold
        public void CancelHoldInteraction()
        {
            this.HoldHandler?.CancelHoldInteraction();
        }

        //  5. Prompt Management
        public void RefreshPrompt()
        {
            if (this.PromptHandler == null) return;

            if (!this.CanProcessInteraction)
            {
                this.PromptHandler.HidePrompt();
                return;
            }

            this.PromptHandler.RefreshPrompt();
        }

        //  6. Runtime State Cleanup
        public void ClearInteractionState() 
        {
            this.HoldHandler?.CancelHoldInteraction(refreshPrompt: false);

            this.InteractableTargetHandler?.ClearNearbyTargets();
            this.PromptSourceTargetHandler?.ClearNearbyTargets();

            this.PromptHandler?.HidePrompt(); 
        }

        // --- Internal APIs ---
        //  1. Common Interaction Execution
        internal bool TryExecuteInteraction(IPlayerInteractable interactable)
        {
            if (!this.CanProcessInteraction) return false;
            if (this.InteractableTargetHandler == null) return false;

            if (!this.InteractableTargetHandler.IsInteractableNearby(interactable))
            {
                this.RefreshPrompt();
                return false;
            }

            GameObject interactor = this.GetInteractor();
            if (interactor == null) return false;
            if (!interactable.CanInteract(interactor))
            {
                this.RefreshPrompt();
                return false;
            }

            bool didInteract = interactable.TryInteract(interactor);

            this.InteractableTargetHandler.RemoveInvalidTargets();
            this.PromptSourceTargetHandler?.RemoveInvalidTargets();

            this.RefreshPrompt();

            return didInteract;
        }

        // --- Helpers ---
        //  1. Event Handling
        private void HandlePromptRefreshRequested()
        {
            this.RevalidateHoldAndRefreshPrompt();
        }
        private void HandlePromptBlockingStateChanged(bool isBlocked)
        {
            if (isBlocked)
            {
                this.HoldHandler?.CancelHoldInteraction(refreshPrompt: false);

                this.PromptHandler?.HidePrompt();
                return;
            }

            this.RefreshPrompt();
        }

        //  2. Carry Event Handling
        private void HandleEquippedCarriableItemChanged(InventoryItemEntry entry)
        {
            this.RevalidateHoldAndRefreshPrompt();
        }
        private void HandleCarriableItemCarryingStateChanged(bool isCarrying)
        {
            this.RevalidateHoldAndRefreshPrompt();
        }

        //  3. Inventory Event Handling
        private void HandleInventoryItemChanged(ItemProfile item, int quantity)
        {
            if (item is not CarriableItemProfile) return;

            this.RevalidateHoldAndRefreshPrompt();
        }

        private void HandleInventoryRestored()
        {
            this.HoldHandler?.CancelHoldInteraction(refreshPrompt: false);

            this.RefreshPrompt();
        }

        private void RevalidateHoldAndRefreshPrompt()
        {
            this.HoldHandler?.ValidateActiveHoldInteraction(refreshPrompt: false);

            this.RefreshPrompt();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.InteractableTargetHandler  == null) _interactableTargetHandler = this.GetComponent<PlayerInteractionInteractableTargetHandler>();
            if (this.PromptSourceTargetHandler  == null) _promptSourceTargetHandler = this.GetComponent<PlayerInteractionPromptSourceTargetHandler>();
            if (this.PromptHandler              == null) _promptHandler             = this.GetComponent<PlayerInteractionPromptHandler>(); 
            if (this.PromptBlockerHandler       == null) _promptBlockerHandler      = this.GetComponent<PlayerInteractionPromptBlockerHandler>();
            if (this.HoldHandler                == null) _holdHandler               = this.GetComponent<PlayerInteractionHoldHandler>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.InteractableTargetHandler  == null) Debug.LogError($"[{this.GetType().Name}] {nameof(PlayerInteractionInteractableTargetHandler)} reference is not assigned.", this);
            if (this.PromptSourceTargetHandler  == null) Debug.LogError($"[{this.GetType().Name}] {nameof(PlayerInteractionPromptSourceTargetHandler)} reference is not assigned.", this);
            if (this.PromptHandler              == null) Debug.LogError($"[{this.GetType().Name}] {nameof(PlayerInteractionPromptHandler)} reference is not assigned.",  this);
            if (this.PromptBlockerHandler       == null) Debug.LogError($"[{this.GetType().Name}] {nameof(PlayerInteractionPromptBlockerHandler)} reference is not assigned.",  this);
            if (this.HoldHandler                == null) Debug.LogError($"[{this.GetType().Name}] {nameof(PlayerInteractionHoldHandler)} reference is not assigned.", this);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.InteractableTargetHandler?.Bind(this);
            this.PromptSourceTargetHandler?.Bind(this);
            this.PromptHandler?.Bind(this);
            this.PromptBlockerHandler?.Bind(this);
            this.HoldHandler?.Bind(this);

            if (this.PromptSourceTargetHandler  != null) this.PromptSourceTargetHandler.PromptRefreshRequested  += this.HandlePromptRefreshRequested; 
            if (this.PromptBlockerHandler       != null) this.PromptBlockerHandler.PromptBlockingStateChanged   += this.HandlePromptBlockingStateChanged; 
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState(); 
            
            this.IsDeathDisabled = false; 
        }
        protected override void BindExternalReferences()
        {
            base.BindExternalReferences();

            if (this.CarryLoadoutController != null)
            {
                this.CarryLoadoutController.OnEquippedCarriableItemChanged += this.HandleEquippedCarriableItemChanged;
                this.CarryLoadoutController.OnCarriableItemCarryingStateChanged += this.HandleCarriableItemCarryingStateChanged;
            }

            if (this.InventoryController != null)
            {
                this.InventoryController.OnItemAdded += this.HandleInventoryItemChanged;
                this.InventoryController.OnItemRemoved += this.HandleInventoryItemChanged;
                this.InventoryController.OnInventoryRestored += this.HandleInventoryRestored;
            }
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();
            
            this.PromptHandler?.HidePrompt(); 
        }
        protected override void CleanupStates()
        {
            this.ClearInteractionState();

            base.CleanupStates();
        }
        protected override void UnbindInternalReferences()
        {
            if (this.PromptSourceTargetHandler  != null) this.PromptSourceTargetHandler.PromptRefreshRequested  -= this.HandlePromptRefreshRequested;
            if (this.PromptBlockerHandler       != null) this.PromptBlockerHandler.PromptBlockingStateChanged   -= this.HandlePromptBlockingStateChanged;

            this.HoldHandler?.Unbind(this);
            this.PromptBlockerHandler?.Unbind(this);
            this.PromptHandler?.Unbind(this);
            this.PromptSourceTargetHandler?.Unbind(this);
            this.InteractableTargetHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
        protected override void UnbindExternalReferences()
        {
            if (this.CarryLoadoutController != null)
            {
                this.CarryLoadoutController.OnEquippedCarriableItemChanged -= this.HandleEquippedCarriableItemChanged;
                this.CarryLoadoutController.OnCarriableItemCarryingStateChanged -= this.HandleCarriableItemCarryingStateChanged;
            }

            if (this.InventoryController != null)
            {
                this.InventoryController.OnItemAdded -= this.HandleInventoryItemChanged;
                this.InventoryController.OnItemRemoved -= this.HandleInventoryItemChanged;
                this.InventoryController.OnInventoryRestored -= this.HandleInventoryRestored;
            }

            base.UnbindExternalReferences();
        }
    }
}