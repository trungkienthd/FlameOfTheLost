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

        private void Start()
        {
            //if (this.TilemapRenderer != null)
            //{
            //    this.TilemapRenderer.sortingOrder = this.Priority;
            //}
        }
        private void Reset()
        {
            Collider2D collider = this.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }
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
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.TilemapRenderer == null) _tilemapRenderer = this.GetComponent<TilemapRenderer>();
        }
    }
}
