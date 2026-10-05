using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    public class PlayerRootController : CharacterRootController<PlayerCharacterProfile>
    {
        // --- Data / Configs ---
        public PlayerCharacterProfile PlayerProfile => this.TypedProfile;

        // --- References ---
        [Header("References (Player-only)")]
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private PlayerCarryLoadoutController _playerCarryLoadoutController;
        [SerializeField] private PlayerInteractionHandler _playerInteractionHandler;
        [SerializeField] private PlayerCraftingController _playerCraftingController;
        public PlayerInput PlayerInput => _playerInput;
        public PlayerCarryLoadoutController PlayerCarryLoadoutController => _playerCarryLoadoutController;
        public PlayerInteractionHandler PlayerInteractionHandler => _playerInteractionHandler;
        public PlayerCraftingController PlayerCraftingController => _playerCraftingController;

        public PlayerInputHandler PlayerInputHandler => base.InputHandler as PlayerInputHandler;

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PlayerInput                    == null) _playerInput                   = this.GetComponentInChildren<PlayerInput>(true);
            if (this.PlayerCarryLoadoutController   == null) _playerCarryLoadoutController  = this.GetComponentInChildren<PlayerCarryLoadoutController>(true);
            if (this.PlayerInteractionHandler       == null) _playerInteractionHandler      = this.GetComponentInChildren<PlayerInteractionHandler>(true);
            if (this.PlayerCraftingController       == null) _playerCraftingController      = this.GetComponentInChildren<PlayerCraftingController>(true);
        }

        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.PlayerInput == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{this.GetType().Name}] No {nameof(UnityEngine.InputSystem.PlayerInput)} assigned.", this);
            }   
            
            if (this.PlayerInputHandler == null)
            {
                Debug.LogError($"{this.gameObject.name}.{this.GetType().Name} {nameof(CharacterInputHandler)} must be a {nameof(PlayerInputHandler)}.", this);
            }

            if (this.PlayerCarryLoadoutController == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{this.GetType().Name}] No {nameof(PlayerCarryLoadoutController)} assigned.", this);
            }

            if (this.PlayerInteractionHandler == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{this.GetType().Name}] No {nameof(PlayerInteractionHandler)} assigned.", this);
            }

            if (this.PlayerCraftingController == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{this.GetType().Name}] No {nameof(PlayerCraftingController)} assigned.", this);
            }
        }
    }
}
