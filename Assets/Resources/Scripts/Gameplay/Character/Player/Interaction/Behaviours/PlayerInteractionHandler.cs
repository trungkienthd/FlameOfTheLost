using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(PlayerInteractionTargetHandler))]
    [RequireComponent(typeof(PlayerInteractionPromptHandler))]
    [RequireComponent(typeof(PlayerInteractionPromptBlockerHandler))]
    public class PlayerInteractionHandler : CharacterBehaviourControllerBase, 
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private PlayerInteractionTargetHandler _targetHandler;
        [SerializeField] private PlayerInteractionPromptHandler _promptHandler;
        [SerializeField] private PlayerInteractionPromptBlockerHandler _promptBlockerHandler;
        public PlayerInteractionTargetHandler TargetHandler => _targetHandler;
        private PlayerInteractionPromptHandler PromptHandler => _promptHandler;
        private PlayerInteractionPromptBlockerHandler PromptBlockerHandler => _promptBlockerHandler;

        private GameObject PlayerRoot => this.CharacterRootController.CharacterRoot;

        // --- States ---
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
        public bool CanProcessInteraction => !this.IsDeathDisabled && !this.IsPromptBlocked;

        // --- Unity Callbacks ---
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (this.IsDeathDisabled) return;
            if (other == null || !other.isTrigger) return;
            if (this.TargetHandler == null) return;

            this.TargetHandler.RegisterNearbyCollider(other);
            this.RefreshPrompt();
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == null || !other.isTrigger) return;
            if (this.TargetHandler == null) return;

            this.TargetHandler.UnregisterNearbyCollider(other);

            if (!this.IsDeathDisabled)
            {
                this.RefreshPrompt();
            }
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

        //  1. For Sub-handlers
        public GameObject GetInteractor() 
        {
            return this.PlayerRoot; 
        }
        public PlayerRootController GetPlayer()
        {
            return this.CharacterRootController is PlayerRootController player ? player : null;
        }

        //  2. (Nearest) Interaction Handling
        public bool TryInteractNearest()
        {
            if (!this.CanProcessInteraction)
            {
                this.PromptHandler?.HidePrompt();
                return false;
            }

            if (this.TargetHandler == null)
            {
                this.PromptHandler?.HidePrompt();
                return false;
            }

            GameObject interactor = this.GetInteractor();

            IPlayerInteractable nearestInteractable = this.TargetHandler.GetNearestInteractable(interactor);
            if (nearestInteractable == null)
            {
                this.RefreshPrompt();
                return false;
            }

            bool didInteract = nearestInteractable.TryInteract(interactor);

            this.TargetHandler.RemoveInvalidTargets(); 
            this.RefreshPrompt();

            return didInteract;
        }

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

        public void ClearInteractionState() 
        {
            this.TargetHandler?.ClearNearbyTargets(); 
            this.PromptHandler?.HidePrompt(); 
        }

        // --- Helpers ---
        //  1. Event Handling
        private void HandlePromptRefreshRequested()
        {
            this.RefreshPrompt();
        }
        private void HandlePromptBlockingStateChanged(bool isBlocked)
        {
            if (isBlocked)
            {
                this.PromptHandler?.HidePrompt();
                return;
            }

            this.RefreshPrompt();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.TargetHandler          == null) _targetHandler         = this.GetComponent<PlayerInteractionTargetHandler>(); 
            if (this.PromptHandler          == null) _promptHandler         = this.GetComponent<PlayerInteractionPromptHandler>(); 
            if (this.PromptBlockerHandler   == null) _promptBlockerHandler  = this.GetComponent<PlayerInteractionPromptBlockerHandler>(); 
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.TargetHandler == null)
            {
                Debug.LogError
                (
                    $"[{this.GetType().Name}] " + 
                    $"{nameof(PlayerInteractionTargetHandler)} " + 
                    $"reference is not assigned.", 
                    this
                );
            }
            if (this.PromptHandler == null)
            {
                Debug.LogError
                (
                    $"[{this.GetType().Name}] " + 
                    $"{nameof(PlayerInteractionPromptHandler)} " + 
                    $"reference is not assigned.", 
                    this
                );
            }
            if (this.PromptBlockerHandler   == null)
            {
                Debug.LogError
                (
                    $"[{this.GetType().Name}] " + 
                    $"{nameof(PlayerInteractionPromptBlockerHandler)} " + 
                    $"reference is not assigned.", 
                    this
                );
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.TargetHandler?.Initialize(this);
            this.PromptHandler?.Initialize(this);
            this.PromptBlockerHandler?.Initialize(this);

            if (this.TargetHandler != null) this.TargetHandler.PromptRefreshRequested += this.HandlePromptRefreshRequested; 

            if (this.PromptBlockerHandler != null) this.PromptBlockerHandler.PromptBlockingStateChanged += this.HandlePromptBlockingStateChanged; 
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState(); 
            
            this.IsDeathDisabled = false; 
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();
            
            this.PromptHandler?.HidePrompt(); 
        }
        protected override void UnbindInternalReferences()
        {
            if (this.TargetHandler != null) this.TargetHandler.PromptRefreshRequested -= this.HandlePromptRefreshRequested;

            if (this.PromptBlockerHandler != null) this.PromptBlockerHandler.PromptBlockingStateChanged -= this.HandlePromptBlockingStateChanged;

            base.UnbindInternalReferences();
        }
    }
}