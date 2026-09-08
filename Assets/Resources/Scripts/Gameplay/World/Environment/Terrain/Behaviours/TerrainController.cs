using UnityEngine;
using UnityEngine.Tilemaps;

namespace FlameOfTheLost
{
    public class TerrainController : FOTLWorldObjectProfileBehaviour<TerrainProfile>
    {
        // --- Data / Configs ---
        public int Priority => this.Profile != null ? this.Profile.Priority : 0;

        // --- References ---
        [Header("References")]
        [SerializeField] private TilemapRenderer _tilemapRenderer;
        private TilemapRenderer TilemapRenderer => _tilemapRenderer;

        protected override void Awake()
        {
            base.Awake();

            this.ConfigureCollider();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CharacterTerrainContactHandler contactHandler = other.GetComponentInParent<CharacterTerrainContactHandler>();

            if (contactHandler == null) return;

            contactHandler.RegisterTerrain(this);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            CharacterTerrainContactHandler contactHandler = other.GetComponentInParent<CharacterTerrainContactHandler>();

            if (contactHandler == null) return;

            contactHandler.UnregisterTerrain(this);
        }

        // --- Helpers ---
        //  1. Setup Collider's isTrigger
        private void ConfigureCollider()
        {
            Collider2D collider = this.GetComponent<Collider2D>();

            if (collider != null)
            {
                collider.isTrigger = true;
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.TilemapRenderer == null) _tilemapRenderer = this.GetComponent<TilemapRenderer>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.TilemapRenderer == null)
            {
                Debug.LogError($"[{this.gameObject.name}{nameof(TerrainController)}] {nameof(UnityEngine.Tilemaps.TilemapRenderer)} reference is missing.", this);
            }
        }
    }
}
