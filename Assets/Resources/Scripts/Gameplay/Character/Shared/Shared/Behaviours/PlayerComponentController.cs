using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerComponentController : CharacterComponentController
    {
        [Header("References (Player-only)")]
        [SerializeField] private PlayerCarryLoadoutController _playerCarryLoadoutController;
        [SerializeField] private PlayerRespawnHandler _playerRespawnHandler;

        public PlayerCarryLoadoutController PlayerCarryLoadoutController => _playerCarryLoadoutController;
        public PlayerRespawnHandler PlayerRespawnHandler => _playerRespawnHandler;

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.PlayerCarryLoadoutController == null) _playerCarryLoadoutController = this.GetComponentInChildren<PlayerCarryLoadoutController>(true);

            if (this.PlayerRespawnHandler == null) _playerRespawnHandler = this.GetComponent<PlayerRespawnHandler>();
        }
    }
}
