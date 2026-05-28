using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerComponentController : CharacterComponentController
    {
        [Header("References (Player-only)")]
        [SerializeField] private PlayerCarryLoadoutController _playerCarryLoadoutController;
        [SerializeField] private PlayerInteractionHandler _playerInteractionHandler;
        [SerializeField] private PlayerRespawnHandler _playerRespawnHandler;
        public PlayerCarryLoadoutController PlayerCarryLoadoutController => _playerCarryLoadoutController;
        public PlayerInteractionHandler PlayerInteractionHandler => _playerInteractionHandler;
        public PlayerRespawnHandler PlayerRespawnHandler => _playerRespawnHandler;

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.PlayerCarryLoadoutController == null) _playerCarryLoadoutController = this.GetComponentInChildren<PlayerCarryLoadoutController>(true);
            if (this.PlayerInteractionHandler == null) _playerInteractionHandler = this.GetComponentInChildren<PlayerInteractionHandler>(true);
            if (this.PlayerRespawnHandler == null) _playerRespawnHandler = this.GetComponentInChildren<PlayerRespawnHandler>();
        }
    }
}
