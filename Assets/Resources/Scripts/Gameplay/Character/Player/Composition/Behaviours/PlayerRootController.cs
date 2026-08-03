using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    public class PlayerRootController : CharacterRootController
    {
        [Header("References (Player-only)")]
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private PlayerCarryLoadoutController _playerCarryLoadoutController;
        [SerializeField] private PlayerInteractionHandler _playerInteractionHandler;
        public PlayerInput PlayerInput => _playerInput;
        public PlayerCarryLoadoutController PlayerCarryLoadoutController => _playerCarryLoadoutController;
        public PlayerInteractionHandler PlayerInteractionHandler => _playerInteractionHandler;

        public PlayerInputHandler PlayerInputHandler => base.InputHandler as PlayerInputHandler;

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PlayerInput                    == null) _playerInput                   = this.GetComponentInChildren<PlayerInput>(true);
            if (this.PlayerCarryLoadoutController   == null) _playerCarryLoadoutController  = this.GetComponentInChildren<PlayerCarryLoadoutController>(true);
            if (this.PlayerInteractionHandler       == null) _playerInteractionHandler      = this.GetComponentInChildren<PlayerInteractionHandler>(true);
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
                Debug.LogError($"{this.gameObject.name}.{this.GetType().Name} {nameof(CharacterInputHandler)} must be a {nameof(FlameOfTheLost.PlayerInputHandler)}.", this);
            }

            if (this.PlayerCarryLoadoutController == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{this.GetType().Name}] No {nameof(FlameOfTheLost.PlayerCarryLoadoutController)} assigned.", this);
            }

            if (this.PlayerInteractionHandler == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{this.GetType().Name}] No {nameof(FlameOfTheLost.PlayerInteractionHandler)} assigned.", this);
            }
        }
    }
}
