using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(PlayerCraftingValidationHandler))]
    [RequireComponent(typeof(PlayerCraftingExecutionHandler))]
    public class PlayerCraftingController : CharacterBehaviourControllerBase,
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- References ---
        //  1. Sub-handlers
        [Header("References")]
        [SerializeField] private PlayerCraftingValidationHandler _validationHandler;
        [SerializeField] private PlayerCraftingExecutionHandler _executionHandler;
        internal PlayerCraftingValidationHandler ValidationHandler => _validationHandler;
        internal PlayerCraftingExecutionHandler ExecutionHandler => _executionHandler;

        //  2. Player-related
        public PlayerRootController Player => this.GetCharacter() as PlayerRootController;
        public InventoryController InventoryController => this.Player != null ? this.Player.InventoryController : null;
        public CharacterRewardWalletController RewardWalletController => this.Player != null ? this.Player.RewardWalletController : null;

        // --- Runtime ---
        //  0. Death Disable
        [Header("Crafting State (Runtime)")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled
        {
            get => _isDeathDisabled;
            private set => _isDeathDisabled = value;
        }

        //  1. Crafting Provider
        private ICraftingRecipeProvider _currentRecipeProvider;
        public ICraftingRecipeProvider CurrentRecipeProvider
        {
            get => _currentRecipeProvider;
            private set => _currentRecipeProvider = value;
        }

        public bool IsCrafting => this.IsValidRecipeProvider(this.CurrentRecipeProvider);

        public bool CanProcessCrafting => !this.IsDeathDisabled &&
            this.InventoryController != null &&
            this.RewardWalletController != null;

        // --- Events ---
        public event Action<ICraftingRecipeProvider> CraftingStarted;
        public event Action CraftingEnded;
        public event Action<CraftingRecipeProfile, int> CraftingCompleted;
        public event Action<CraftingRecipeProfile, CraftingFailureReason> CraftingFailed;

        // --- Public APIs ---
        //  0.1. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 50;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.IsDeathDisabled = false;
            this.EndCrafting();
        }

        //  0.2. ICharacterDeathDisableable
        public int DeathDisablePriority => 15;
        public void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;
            this.EndCrafting();
        }

        //  1. Crafting Session
        public bool TryBeginCrafting(ICraftingRecipeProvider provider)
        {
            if (!this.CanProcessCrafting) return false;
            if (!this.IsValidRecipeProvider(provider)) return false;

            if (UnityEngine.Object.ReferenceEquals(this.CurrentRecipeProvider, provider)) return true;

            if (this.IsCrafting)
            {
                this.EndCrafting();
            }

            this.CurrentRecipeProvider = provider;
            this.CraftingStarted?.Invoke(provider);

            return true;
        }
        public void EndCrafting()
        {
            if (this.CurrentRecipeProvider == null) return;

            this.CurrentRecipeProvider = null;

            this.CraftingEnded?.Invoke();
        }

        //  2. Available Recipes
        public IReadOnlyList<CraftingRecipeProfile> GetAvailableCraftingRecipes()
        {
            if (!this.CanProcessCrafting) return Array.Empty<CraftingRecipeProfile>();
            if (!this.IsValidRecipeProvider(this.CurrentRecipeProvider)) return Array.Empty<CraftingRecipeProfile>();
            
            IReadOnlyList<CraftingRecipeProfile> recipes = this.CurrentRecipeProvider.GetAvailableCraftingRecipes(this.Player);

            return recipes ?? Array.Empty<CraftingRecipeProfile>();
        }
        public bool IsRecipeAvailable(CraftingRecipeProfile recipe)
        {
            if (recipe == null) return false;

            IReadOnlyList<CraftingRecipeProfile> availableRecipes = this.GetAvailableCraftingRecipes();

            for (int i = 0; i < availableRecipes.Count; i++)
            {
                if (UnityEngine.Object.ReferenceEquals(availableRecipes[i], recipe)) return true;
            }

            return false;
        }

        //  3. Crafting Validation
        public CraftingEvaluationRecord EvaluateCrafting(CraftingRecipeProfile recipe, int craftCount = 1)
        {
            if (this.ValidationHandler == null)
            {
                return CraftingEvaluationRecord.Failure(CraftingFailureReason.CraftingUnavailable);
            }

            return this.ValidationHandler.EvaluateCrafting(recipe, craftCount);
        }
        public bool IsRecipeValid(CraftingRecipeProfile recipe)
        {
            return this.ValidationHandler != null &&
                   this.ValidationHandler.IsRecipeValid(recipe);
        }

        //  4. Crafting Execution
        public bool TryCraft(CraftingRecipeProfile recipe, int craftCount = 1)
        {
            if (this.ExecutionHandler == null)
            {
                this.NotifyCraftingFailed(recipe, CraftingFailureReason.CraftingUnavailable);

                return false;
            }

            return this.ExecutionHandler.TryCraft(recipe, craftCount);
        }

        // --- Helpers ---
        //  1. Provider Validation
        internal bool IsValidRecipeProvider(ICraftingRecipeProvider provider)
        {
            if (provider == null) return false;

            if (provider is UnityEngine.Object unityObject)
            {
                return unityObject != null;
            }

            return true;
        }
        internal bool IsValidCurrentRecipeProvider() => this.IsValidRecipeProvider(this.CurrentRecipeProvider);

        //  2. Crafting Event Notifications
        internal void NotifyCraftingCompleted(CraftingRecipeProfile recipe, int craftCount)
        {
            this.CraftingCompleted?.Invoke(recipe, craftCount);
        }
        internal void NotifyCraftingFailed(CraftingRecipeProfile recipe, CraftingFailureReason failureReason)
        {
            this.CraftingFailed?.Invoke(recipe, failureReason);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.ValidationHandler == null) _validationHandler = this.GetComponent<PlayerCraftingValidationHandler>();
            if (this.ExecutionHandler == null) _executionHandler = this.GetComponent<PlayerCraftingExecutionHandler>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Player == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(PlayerCraftingController)}] " + $"requires a {nameof(PlayerRootController)}.", this);
                return;
            }

            if (this.ExecutionHandler == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(PlayerCraftingController)}] " + $"{nameof(PlayerCraftingExecutionHandler)} reference is not assigned.", this);
            }
            if (this.ValidationHandler == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(PlayerCraftingController)}] " + $"{nameof(PlayerCraftingValidationHandler)} reference is not assigned.", this);
            }

            if (this.InventoryController == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(PlayerCraftingController)}] " + $"{nameof(InventoryController)} reference is unavailable.", this);
            }
            if (this.RewardWalletController == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(PlayerCraftingController)}] " + $"{nameof(CharacterRewardWalletController)} reference is unavailable.", this);
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.ValidationHandler?.Bind(this);
            this.ExecutionHandler?.Bind(this);
        }

        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.IsDeathDisabled = false;
            this.CurrentRecipeProvider = null;
        }
        protected override void CleanupStates()
        {
            this.EndCrafting();

            base.CleanupStates();
        }
        protected override void UnbindInternalReferences()
        {
            this.ValidationHandler?.Unbind(this);
            this.ExecutionHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
