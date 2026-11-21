using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CraftingStationController : FOTLWorldObjectProfileBehaviour<CraftingStationProfile>,
        IPlayerInteractable,
        IPlayerInteractionPromptSource,
        ICraftingRecipeProvider
    {
        // --- References ---
        //  1. UI
        [Header("References (UI)")]
        [SerializeField] private CraftingPanelController _craftingPanelController;
        private CraftingPanelController CraftingPanelController => _craftingPanelController;

        // --- Data / Configs ---
        //  1. Prompt
        public int InteractionPromptPriority => this.Profile != null ? this.Profile.InteractionPromptPriority : 100;

        // --- Public APIs ---
        //  1. IPlayerInteractable implementation
        public bool CanInteract(GameObject interactor)
        {
            if (!this.TryGetPlayer(interactor, out PlayerRootController player))
            {
                return false;
            }

            return player.PlayerCraftingController != null;
        }
        public bool TryInteract(GameObject interactor)
        {
            if (!this.TryGetPlayer(interactor, out PlayerRootController player))
            {
                return false;
            }

            PlayerCraftingController craftingController = player.PlayerCraftingController;

            if (craftingController == null || this.CraftingPanelController == null)
            {
                return false;
            }

            if (!craftingController.TryBeginCrafting(this))
            {
                return false;
            }

            this.CraftingPanelController.Open(craftingController);

            return true;
        }

        //  2. IPlayerInteractionPromptSource implementation
        public bool CanShowInteractionPrompt(GameObject interactor)
        {
            return this.CanInteract(interactor);
        }
        public string GetInteractionPromptText(GameObject interactor)
        {
            if (string.IsNullOrWhiteSpace(this.DisplayName))
            {
                return "Craft";
            }

            return $"Use {this.DisplayName}";
        }

        //  3. ICraftingRecipeProvider implementation
        public IReadOnlyList<CraftingRecipeProfile> GetAvailableCraftingRecipes(CharacterRootController crafter)
        {
            if (this.Profile == null) return System.Array.Empty<CraftingRecipeProfile>();

            return this.Profile.CraftingRecipes;
        }

        // --- Helpers ---
        private bool TryGetPlayer(GameObject interactor, out PlayerRootController player)
        {
            player = interactor != null ? interactor.GetComponentInParent<PlayerRootController>() : null;

            return player != null;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.CraftingPanelController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] " + $"{nameof(CraftingPanelController)} reference is missing.", this);
            }
        }
    }
}
