using UnityEngine;
using UnityEngine.Tilemaps;

namespace FlameOfTheLost
{
    public class TilemapTerrainController : FOTLMonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TilemapRenderer _tilemapRenderer;
        private TilemapRenderer TilemapRenderer => _tilemapRenderer;

        [Header("Data")]
        [SerializeField] private TerrainProfile _profile;
        public TerrainProfile TerrainProfile => _profile;

        public int Priority => this.TerrainProfile != null ? this.TerrainProfile.Priority : 0;

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
        //  1. FOTL Base Behaviour Overrides
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
                Debug.LogError($"[{nameof(TilemapTerrainController)}] TilemapRenderer reference is missing.", this);
            }
            if (this.TerrainProfile == null)
            {
                Debug.LogError($"[{nameof(TilemapTerrainController)}] {nameof(this.TerrainProfile)} reference is missing.", this);
            }
        }

        //  2. Setup Collider's isTrigger
        private void ConfigureCollider()
        {
            Collider2D collider = this.GetComponent<Collider2D>();

            if (collider != null)
            {
                collider.isTrigger = true;
            }
        }
    }
}
