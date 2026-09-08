using UnityEngine;

namespace FlameOfTheLost
{
    public class HarvestableBrokenStateHandler : HarvestableObjectSubHandlerBase
    {
        // --- References ---
        //  1. State References
        [Header("References")]
        [SerializeField] private SpriteRenderer _visualSpriteRenderer;
        [SerializeField] private Collider2D _activeCollider;
        [SerializeField] private Collider2D _brokenColliderPreset;
        private SpriteRenderer VisualSpriteRenderer => _visualSpriteRenderer;
        private Collider2D ActiveCollider => _activeCollider;
        private Collider2D BrokenColliderPreset => _brokenColliderPreset;

        // --- Data / Configs ---
        private HarvestableBreakBehaviour BreakBehaviour    => this.Profile != null ? this.Profile.BreakBehaviour : HarvestableBreakBehaviour.ReplaceVisual;
        private Sprite HarvestedSprite                      => this.Profile != null ? this.Profile.HarvestedSprite : null;

        // --- Runtime States ---
        private Sprite _defaultSprite;
        private ColliderSnapshotRecord _defaultColliderSnapshot;
        private Sprite DefaultSprite
        {
            get => _defaultSprite;
            set => _defaultSprite = value;
        }
        private ColliderSnapshotRecord DefaultColliderSnapshot
        {
            get => _defaultColliderSnapshot;
            set => _defaultColliderSnapshot = value;
        }

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
                    Debug.LogError
                    (
                        $"[{this.gameObject.name}." +
                        $"{nameof(HarvestableBrokenStateHandler)}] " +
                        $"Unsupported " +
                        $"{nameof(HarvestableBreakBehaviour)}: " +
                        $"{this.BreakBehaviour}.",
                        this
                    );
                    break;
            }
        }

        public void RestoreDefaultState()
        {
            if (!this.EnsureBound()) return;

            this.RestoreDefaultVisual();
            this.RestoreDefaultCollider();
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
            if (this.ActiveCollider == null) return;
            if (this.BrokenColliderPreset == null) return;

            if (!ColliderSnapshotRecord.TryCapture(this.BrokenColliderPreset, out ColliderSnapshotRecord brokenState)) return;

            brokenState.ApplyTo(this.ActiveCollider, this);
        }
        private void RestoreDefaultCollider()
        {
            if (this.ActiveCollider == null) return;
            if (this.DefaultColliderSnapshot == null) return;

            this.DefaultColliderSnapshot.ApplyTo(this.ActiveCollider, this);
        }
        private bool AreColliderTypesCompatible()
        {
            if (this.ActiveCollider == null) return false;
            if (this.BrokenColliderPreset == null) return false;

            return this.ActiveCollider.GetType() == this.BrokenColliderPreset.GetType();
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
                Debug.LogError
                (
                    $"[{this.gameObject.name}." +
                    $"{nameof(HarvestableBrokenStateHandler)}] " +
                    $"{nameof(SpriteRenderer)} reference " +
                    $"is not assigned.",
                    this
                );
            }

            if (this.ActiveCollider == null)
            {
                Debug.LogError
                (
                    $"[{this.gameObject.name}." +
                    $"{nameof(HarvestableBrokenStateHandler)}] " +
                    $"Active {nameof(Collider2D)} reference " +
                    $"is not assigned.",
                    this
                );
            }

            if (this.BrokenColliderPreset == null)
            {
                Debug.LogError
                (
                    $"[{this.gameObject.name}." +
                    $"{nameof(HarvestableBrokenStateHandler)}] " +
                    $"Broken collider preset is not assigned.",
                    this
                );

                return;
            }

            if (this.ActiveCollider != null && !this.AreColliderTypesCompatible())
            {
                Debug.LogError
                (
                    $"[{this.gameObject.name}." +
                    $"{nameof(HarvestableBrokenStateHandler)}] " +
                    $"Active collider type " +
                    $"'{this.ActiveCollider.GetType().Name}' " +
                    $"does not match broken collider preset " +
                    $"type " +
                    $"'{this.BrokenColliderPreset.GetType().Name}'.",
                    this
                );
            }

            if (this.BrokenColliderPreset.enabled)
            {
                Debug.LogWarning
                (
                    $"[{this.gameObject.name}." +
                    $"{nameof(HarvestableBrokenStateHandler)}] " +
                    $"Broken collider preset should be disabled.",
                    this
                );
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            if (this.VisualSpriteRenderer != null)
            {
                this.DefaultSprite = this.VisualSpriteRenderer.sprite;
            }

            if (this.ActiveCollider != null && ColliderSnapshotRecord.TryCapture(this.ActiveCollider, out ColliderSnapshotRecord defaultState))
            {
                this.DefaultColliderSnapshot = defaultState;
            }
        }
    }
}