using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerComponentController : CharacterComponentController
    {
        [Header("References (Player-only)")]
        [SerializeField] private PlayerCarryLoadoutController _playerCarryLoadoutController;
        [SerializeField] private PlayerItemPickupHandler _playerItemPickupHandler;
        [SerializeField] private PlayerRespawnHandler _playerRespawnHandler;
        public PlayerCarryLoadoutController PlayerCarryLoadoutController => _playerCarryLoadoutController;
        public PlayerItemPickupHandler PlayerItemPickupHandler => _playerItemPickupHandler;
        public PlayerRespawnHandler PlayerRespawnHandler => _playerRespawnHandler;

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.PlayerCarryLoadoutController == null) _playerCarryLoadoutController = this.GetComponentInChildren<PlayerCarryLoadoutController>(true);
            if (this.PlayerItemPickupHandler == null) _playerItemPickupHandler = this.GetComponentInChildren<PlayerItemPickupHandler>(true);
            if (this.PlayerRespawnHandler == null) _playerRespawnHandler = this.GetComponentInChildren<PlayerRespawnHandler>();
        }
    }
}
