using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerRootController : CharacterRootController
    {
        [Header("References (Player-only)")]
        [SerializeField] private PlayerCarryLoadoutController _playerCarryLoadoutController;
        [SerializeField] private PlayerInteractionHandler _playerInteractionHandler;
        public PlayerCarryLoadoutController PlayerCarryLoadoutController => _playerCarryLoadoutController;
        public PlayerInteractionHandler PlayerInteractionHandler => _playerInteractionHandler;

        public PlayerInputHandler PlayerInputHandler => base.InputHandler as PlayerInputHandler;

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.PlayerCarryLoadoutController   == null) _playerCarryLoadoutController  = this.GetComponentInChildren<PlayerCarryLoadoutController>(true);
            if (this.PlayerInteractionHandler       == null) _playerInteractionHandler      = this.GetComponentInChildren<PlayerInteractionHandler>(true);
        }

        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.PlayerInputHandler == null)
            {
                Debug.LogError($"{this.gameObject.name}.{this.GetType().Name} InputHandler must be a PlayerInputHandler.", this);
            }

            if (this.PlayerCarryLoadoutController == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{this.GetType().Name}] No PlayerCarryLoadoutController assigned.", this);
            }

            if (this.PlayerInteractionHandler == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{this.GetType().Name}] No PlayerInteractionHandler assigned.", this);
            }
        }
    }
}
