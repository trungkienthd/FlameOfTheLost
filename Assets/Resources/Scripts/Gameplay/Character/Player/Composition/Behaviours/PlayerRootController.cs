using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerRootController : CharacterRootController
    {
        [Header("References (Player-only)")]
        [SerializeField] private PlayerCarryLoadoutController _playerCarryLoadoutController;
        [SerializeField] private PlayerInteractionHandler _playerInteractionHandler;
        [SerializeField] private PlayerRespawnHandler _playerRespawnHandler;
        public PlayerCarryLoadoutController PlayerCarryLoadoutController => _playerCarryLoadoutController;
        public PlayerInteractionHandler PlayerInteractionHandler => _playerInteractionHandler;
        public PlayerRespawnHandler PlayerRespawnHandler => _playerRespawnHandler;

        public PlayerInputHandler PlayerInputHandler => base.InputHandler as PlayerInputHandler;

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.PlayerCarryLoadoutController == null) _playerCarryLoadoutController = this.GetComponentInChildren<PlayerCarryLoadoutController>(true);
            if (this.PlayerInteractionHandler == null) _playerInteractionHandler = this.GetComponentInChildren<PlayerInteractionHandler>(true);
            if (this.PlayerRespawnHandler == null) _playerRespawnHandler = this.GetComponentInChildren<PlayerRespawnHandler>();
        }

        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.PlayerInputHandler == null)
            {
                Debug.LogError($"{this.gameObject.name}.{this.GetType().Name} PlayerInputHandler could not be casted successfully");
            }
        }
    }
}
