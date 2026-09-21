using UnityEngine;

namespace FlameOfTheLost
{
    public class HarvestableBrokenStateHandler : HarvestableObjectSubHandlerBase
    {
        // --- References ---
        //  1. State References
        [Header("References")]
        [SerializeField] private SpriteRenderer _visualSpriteRenderer;
        private SpriteRenderer VisualSpriteRenderer => _visualSpriteRenderer;

        // --- Data / Configs ---
        private HarvestableBreakBehaviour BreakBehaviour    => this.Profile != null ? this.Profile.BreakBehaviour : HarvestableBreakBehaviour.ReplaceVisual;
        private Sprite HarvestedSprite                      => this.Profile != null ? this.Profile.HarvestedSprite : null;

        // --- Runtime ---
        private Sprite _defaultSprite;
        private Sprite DefaultSprite
        {
            get => _defaultSprite;
            set => _defaultSprite = value;
        }

        private Collider2D _activeCollider;
        private Collider2D _brokenColliderPreset;
        private Collider2D ActiveCollider => _activeCollider;
        private Collider2D BrokenColliderPreset => _brokenColliderPreset;

        // --- Public APIs ---
        //  1. State Application
        public void ApplyBrokenState()
        {
            if (!this.EnsureBound()) return;

            switch (this.BreakBehaviour)
            {
                case HarvestableBreakBehaviour.ReplaceVisual:
                    this.ApplyHarvestedVisual();
                    this.ApplyBrokenCollider();
                    break;

                case HarvestableBreakBehaviour.DisableGameObject:
                    this.gameObject.SetActive(false);
                    break;

                case HarvestableBreakBehaviour.DestroyGameObject:
                    UnityEngine.Object.Destroy(this.gameObject);
                    break;

                default:
                    Debug.LogError($"[{this.gameObject.name}.{nameof(HarvestableBrokenStateHandler)}] Unsupported {nameof(HarvestableBreakBehaviour)}: {this.BreakBehaviour}.", this);
                    break;
            }
        }

        public void RestoreDefaultState()
        {
            if (!this.EnsureBound()) return;

            this.RestoreDefaultVisual();
            this.RestoreDefaultCollider();
        }

        //  2. Collider Configuration
        public void ConfigureColliders(Collider2D activeCollider, Collider2D brokenColliderPreset)
        {
            _activeCollider = activeCollider;
            _brokenColliderPreset = brokenColliderPreset;
        }

        // --- Helpers ---
        //  1. Visual
        private void ApplyHarvestedVisual()
        {
            if (this.VisualSpriteRenderer == null) return;
            if (this.HarvestedSprite == null) return;

            this.VisualSpriteRenderer.sprite = this.HarvestedSprite;
        }

        private void RestoreDefaultVisual()
        {
            if (this.VisualSpriteRenderer == null) return;

            this.VisualSpriteRenderer.sprite = this.DefaultSprite;
        }

        //  2. Collider
        private void ApplyBrokenCollider()
        {
            if (this.ActiveCollider         != null) this.ActiveCollider.enabled        = false;
            if (this.BrokenColliderPreset   != null) this.BrokenColliderPreset.enabled  = true;
        }
        private void RestoreDefaultCollider()
        {
            if (this.BrokenColliderPreset   != null) this.BrokenColliderPreset.enabled  = false;
            if (this.ActiveCollider         != null) this.ActiveCollider.enabled        = true;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.VisualSpriteRenderer == null) _visualSpriteRenderer = this.GetComponentInChildren<SpriteRenderer>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.VisualSpriteRenderer == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(HarvestableBrokenStateHandler)}] {nameof(SpriteRenderer)} reference is not assigned.", this);
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            if (this.VisualSpriteRenderer != null)
            {
                this.DefaultSprite = this.VisualSpriteRenderer.sprite;
            }
        }
    }
}