using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(KeyItemInteractableVisualHandler))]
    public class KeyItemInteractableController : FOTLWorldObjectProfileBehaviour<KeyItemInteractableProfile>,
        IPlayerHoldInteractable,
        IPlayerInteractionPromptSource,
        IPlayerInteractionPromptRefreshNotifier
    {
        // --- References ---
        //  1. Sub-Behaviours
        [Header("References - Sub-Behaviours")]
        [SerializeField]
        private KeyItemInteractableVisualHandler _visualHandler;
        private KeyItemInteractableVisualHandler VisualHandler => _visualHandler;

        //  2. External
        private GameConditionController _conditionController;
        private GameConditionController ConditionController => _conditionController;

        // --- Runtime ---
        //  1. Initialization
        private bool _isReady;
        public bool IsReady => _isReady;

        //  2. Interaction Execution
        private bool _isExecutingInteraction;
        public bool IsExecutingInteraction => _isExecutingInteraction;

        //  3. Observed Conditions
        private readonly HashSet<GameConditionKey> _observedConditionKeys = new();

        // --- Public Properties ---
        //  1. Completion State
        public bool IsCompleted => this.EvaluateCompletionState();

        //  2. IPlayerHoldInteractable
        public PlayerInteractionType InteractionType => this.Profile != null
            ? this.Profile.InteractionType
            : PlayerInteractionType.Press;

        public float HoldDuration => this.Profile != null
            ? this.Profile.HoldDuration
            : 0f;

        //  3. IPlayerInteractionPromptSource
        public int InteractionPromptPriority => this.Profile != null
            ? this.Profile.InteractionPromptPriority
            : 0;

        // --- Events ---
        public event Action PromptRefreshRequested;
        public event Action InteractionSucceeded;

        // --- Public APIs ---
        //  1. IPlayerInteractable Implementation
        public bool CanInteract(GameObject interactor)
        {
            if (!this.IsOperational()) return false;
            if (this.IsExecutingInteraction) return false;

            if (this.IsInteractionDisabledByCompletion()) return false;

            if (!this.AreInteractionRequirementsSatisfied()) return false;

            if (!this.TryGetPlayer(interactor, out PlayerRootController player)) return false;

            PlayerCarryLoadoutController carryLoadout = player.PlayerCarryLoadoutController;
            if (carryLoadout == null) return false;

            return carryLoadout.IsActivelyCarryingItem(this.Profile.RequiredKeyItem);
        }

        public bool TryInteract(GameObject interactor)
        {
            // a. Validate current interaction eligibility.
            if (!this.CanInteract(interactor)) return false;

            if (!this.TryGetPlayer(interactor, out PlayerRootController player)) return false;

            PlayerCarryLoadoutController carryLoadout = player.PlayerCarryLoadoutController;
            if (carryLoadout == null) return false;

            // b. Validate operations before consuming the item.
            if (!this.ConditionController.CanApplyConditionOperations(this.Profile.ConditionOperations)) return false;

            bool didSucceed = false;

            _isExecutingInteraction = true;

            try
            {
                // c. Consume the required KeyItem if configured.
                if (this.Profile.ConsumeRequiredKeyItem)
                {
                    bool didConsume = carryLoadout.TryConsumeActivelyCarriedItem(this.Profile.RequiredKeyItem);
                    if (!didConsume) return false;
                }

                // d. Apply configured GameCondition operations.
                didSucceed = this.ConditionController != null && this.ConditionController.TryApplyConditionOperations(this.Profile.ConditionOperations);

                if (!didSucceed)
                {
                    Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Failed to apply configured GameCondition operations.", this);
                }

                return didSucceed;
            }
            finally
            {
                // e. Synchronize the visual state while execution remains protected against reentrant interaction.
                try
                {
                    this.RefreshWorldObjectState(playSuccessEffect: didSucceed);

                    if (didSucceed)
                    {
                        this.InteractionSucceeded?.Invoke();
                    }
                }
                finally
                {
                    _isExecutingInteraction = false;

                    this.PromptRefreshRequested?.Invoke();
                }
            }
        }

        //  2. IPlayerHoldInteractable Implementation
        public bool CanContinueHoldInteraction(GameObject interactor)
        {
            if (this.InteractionType != PlayerInteractionType.Hold) return false;

            return this.CanInteract(interactor);
        }

        //  3. IPlayerInteractionPromptSource Implementation
        public bool CanShowInteractionPrompt(GameObject interactor)
        {
            return !string.IsNullOrWhiteSpace(this.GetInteractionPromptText(interactor));
        }

        public string GetInteractionPromptText(GameObject interactor)
        {
            if (!this.IsOperational()) return string.Empty;
            if (this.IsExecutingInteraction) return string.Empty;

            if (!this.TryGetPlayer(interactor, out PlayerRootController player)) return string.Empty;

            // a. Completed state.
            if (this.IsInteractionDisabledByCompletion()) return this.Profile.CompletedPromptText;

            // b. Additional GameCondition requirements.
            if (!this.AreInteractionRequirementsSatisfied()) return this.Profile.RequirementsNotSatisfiedPromptText;

            InventoryController inventory = player.InventoryController;
            PlayerCarryLoadoutController carryLoadout = player.PlayerCarryLoadoutController;
            if (inventory == null || carryLoadout == null) return string.Empty;

            KeyItemProfile requiredKeyItem = this.Profile.RequiredKeyItem;

            string itemDisplayName = requiredKeyItem.DisplayName;

            // c. Required KeyItem is not possessed.
            if (!inventory.HasItem(requiredKeyItem)) return $"Requires {itemDisplayName}";

            // d. Required KeyItem is not equipped.
            InventoryItemEntry equippedEntry = carryLoadout.EquippedCarriableItemEntry;
            if (equippedEntry == null || equippedEntry.Item != requiredKeyItem || equippedEntry.Quantity != 1 || !equippedEntry.Equipped || !inventory.ContainsEntry(equippedEntry))
            {
                return $"Equip {itemDisplayName}";
            }

            // e. Required KeyItem is not actively carried.
            if (!carryLoadout.IsActivelyCarryingItem(requiredKeyItem))
            {
                return $"Carry {itemDisplayName}";
            }

            // f. Interaction is available.
            return this.Profile.InteractionPromptText;
        }

        // --- Helpers ---
        //  1. Operational Validation
        private bool IsOperational()
        {
            return this.IsReady &&
                   this.isActiveAndEnabled &&
                   this.Profile != null &&
                   this.Profile.RequiredKeyItem != null &&
                   this.ConditionController != null;
        }
        private bool TryGetPlayer(GameObject interactor, out PlayerRootController player)
        {
            player = null;

            if (interactor == null) return false;

            return interactor.TryGetComponent(out player);
        }

        //  2. Interaction Requirements
        private bool AreInteractionRequirementsSatisfied()
        {
            if (this.Profile == null) return false;
            if (this.ConditionController == null) return false;

            return GameConditionEvaluator.AreConditionsSatisfied
            (
                requirements        : this.Profile.InteractionRequirements,
                combinationType     : this.Profile.InteractionRequirementCombinationType,
                conditionController : this.ConditionController
            );
        }

        //  3. Completion State
        private bool EvaluateCompletionState()
        {
            if (this.Profile == null) return false;
            if (this.ConditionController == null) return false;

            IReadOnlyList<GameConditionRequirementEntry> completionConditions = this.Profile.CompletionConditions;

            // An empty list represents no configured persistent completion state.
            if (completionConditions == null || completionConditions.Count == 0) return false;

            return GameConditionEvaluator.AreConditionsSatisfied
            (
                requirements        : completionConditions,
                combinationType     : this.Profile.CompletionConditionCombinationType,
                conditionController : this.ConditionController
            );
        }
        private bool IsInteractionDisabledByCompletion()
        {
            if (this.Profile == null) return false;

            return this.Profile.DisableInteractionWhenCompleted && this.IsCompleted;
        }

        //  4. Observed Condition Keys
        private void CacheObservedConditionKeys()
        {
            _observedConditionKeys.Clear();

            if (this.Profile == null) return;

            this.AddObservedRequirementKeys(this.Profile.InteractionRequirements);
            this.AddObservedRequirementKeys(this.Profile.CompletionConditions);

            IReadOnlyList<GameConditionOperationEntry> operations = this.Profile.ConditionOperations;

            if (operations == null) return;

            foreach (GameConditionOperationEntry operation in operations)
            {
                if (operation == null) continue;
                if (operation.ConditionKey == GameConditionKey.None) continue;

                _observedConditionKeys.Add(operation.ConditionKey);
            }
        }
        private void AddObservedRequirementKeys(IReadOnlyList<GameConditionRequirementEntry> requirements)
        {
            if (requirements == null) return;

            foreach (GameConditionRequirementEntry requirement in requirements)
            {
                if (requirement == null) continue;
                if (requirement.ConditionKey == GameConditionKey.None) continue;

                _observedConditionKeys.Add(requirement.ConditionKey);
            }
        }

        //  5. Condition Event Handling
        private void HandleConditionStateChanged(GameConditionKey conditionKey, bool value)
        {
            if (!this.IsReady) return;

            if (!_observedConditionKeys.Contains(conditionKey)) return;

            // Defer our own interaction notifications until the complete execution sequence has finished.
            if (this.IsExecutingInteraction) return;

            this.RefreshWorldObjectState(playSuccessEffect: false);

            this.PromptRefreshRequested?.Invoke();
        }

        //  6. World Object State
        private void RefreshWorldObjectState(bool playSuccessEffect)
        {
            if (!this.IsReady) return;

            this.VisualHandler?.ApplyVisualState
            (
                isCompleted: this.IsCompleted,
                playSuccessEffect: playSuccessEffect
            );
        }

        //  7. Configuration Validation
        private bool IsProfileConfigurationValid(bool logErrors)
        {
            if (this.Profile == null) return false;

            if (this.Profile.RequiredKeyItem == null)
            {
                return this.ReportConfigurationError("Required KeyItem is not assigned.", logErrors);
            }

            if (this.Profile.RequiredKeyItem.IsStackable)
            {
                return this.ReportConfigurationError("Required KeyItem must be non-stackable.", logErrors);
            }

            if (this.InteractionType != PlayerInteractionType.Press && this.InteractionType != PlayerInteractionType.Hold)
            {
                return this.ReportConfigurationError("Invalid Interaction Type.", logErrors);
            }

            if (this.InteractionType == PlayerInteractionType.Hold)
            {
                float duration = this.HoldDuration;

                if (duration <= 0f || float.IsNaN(duration) || float.IsInfinity(duration))
                {
                    return this.ReportConfigurationError("Invalid Hold Duration.", logErrors);
                }
            }

            if (!this.AreRequirementEntriesValid(this.Profile.InteractionRequirements))
            {
                return this.ReportConfigurationError("Interaction Requirements contain an invalid entry.", logErrors);
            }

            if (!this.AreRequirementEntriesValid( this.Profile.CompletionConditions))
            {
                return this.ReportConfigurationError("Completion Conditions contain an invalid entry.", logErrors);
            }

            if (!this.IsConditionCombinationTypeValid(this.Profile.InteractionRequirementCombinationType) || !this.IsConditionCombinationTypeValid(this.Profile.CompletionConditionCombinationType))
            {
                return this.ReportConfigurationError("Invalid GameCondition Combination Type.", logErrors);
            }

            if (this.Profile.ConditionOperations == null || this.Profile.ConditionOperations.Count == 0)
            {
                return this.ReportConfigurationError("At least one GameCondition operation is required.", logErrors);
            }

            return true;
        }
        private bool AreRequirementEntriesValid(IReadOnlyList<GameConditionRequirementEntry> requirements)
        {
            if (requirements == null) return true;

            foreach (GameConditionRequirementEntry requirement in requirements)
            {
                if (requirement == null || requirement.ConditionKey == GameConditionKey.None) return false;
            }

            return true;
        }
        private bool IsConditionCombinationTypeValid(GameConditionCombinationType combinationType)
        {
            return combinationType == GameConditionCombinationType.All ||
                   combinationType == GameConditionCombinationType.Any;
        }
        private bool ReportConfigurationError(string message, bool logError)
        {
            if (logError) Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {message}", this);
            return false;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.VisualHandler == null) _visualHandler = this.GetComponent<KeyItemInteractableVisualHandler>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.VisualHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(VisualHandler)} is not assigned.", this);
            }

            this.IsProfileConfigurationValid(logErrors: true);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.VisualHandler?.Bind(this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _isReady = false;
            _isExecutingInteraction = false;

            this.CacheObservedConditionKeys();
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            _conditionController = GameConditionController.Instance;
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.ConditionController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(GameConditionController)} is unavailable.", this);
                return;
            }

            if (!this.ConditionController.CanApplyConditionOperations( this.Profile?.ConditionOperations))
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] GameCondition operations are invalid or unavailable.", this);
            }
        }
        protected override void BindExternalReferences()
        {
            base.BindExternalReferences();

            if (this.ConditionController != null) this.ConditionController.ConditionStateChanged += this.HandleConditionStateChanged;
        }

        protected override void InitializePregameplayState()
        {
            // The base Behaviour applies the initial transforms and visual configuration first.
            base.InitializePregameplayState();

            if (!this.IsProfileConfigurationValid(logErrors: false)) return;

            if (this.ConditionController == null) return;

            if (!this.ConditionController.CanApplyConditionOperations(this.Profile.ConditionOperations)) return;

            if (this.VisualHandler == null) return;

            // Initialize the Visual Handler explicitly after the base World Object visual setup has completed.
            if (!this.VisualHandler.TryInitializeVisualState(this.VisualSpriteRenderer)) return;

            _isReady = true;

            this.RefreshWorldObjectState(playSuccessEffect: false);

            this.PromptRefreshRequested?.Invoke();
        }
        protected override void CleanupStates()
        {
            _isReady = false;
            _isExecutingInteraction = false;

            _observedConditionKeys.Clear();

            base.CleanupStates();
        }
        protected override void UnbindInternalReferences()
        {
            this.VisualHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
        protected override void UnbindExternalReferences()
        {
            if (this.ConditionController != null) this.ConditionController.ConditionStateChanged -= this.HandleConditionStateChanged;

            base.UnbindExternalReferences();
        }
    }
}