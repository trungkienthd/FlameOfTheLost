using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class StatusEffectVFXHandler : MonoBehaviour
    {
        // --- States ---
        [Header("References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        private SpriteRenderer SpriteRenderer
        {
            get => _spriteRenderer;
            set => _spriteRenderer = value;
        }
        private Animator Animator
        {
            get => _animator;
            set => _animator = value;
        }

        [Header("Runtime")]
        [SerializeField] private AnimationClip _placeholderClip;
        [SerializeField] private uint _runtimeStatusEffectID;
        [SerializeField] private StatusEffectProfile _statusEffectProfile;
        [SerializeField] private GameObject _ownerRoot;
        private AnimationClip PlaceholderClip => _placeholderClip;
        public uint RuntimeStatusEffectID => _runtimeStatusEffectID;
        public StatusEffectProfile StatusEffectProfile => _statusEffectProfile;
        public GameObject OwnerRoot => _ownerRoot;

        private AnimatorOverrideController _runtimeOverrideController;
        public AnimatorOverrideController RuntimeOverrideController
        {
            get => _runtimeOverrideController;
            private set => _runtimeOverrideController = value;
        }

        // --- Unity Methods ---
        private void Awake()
        {
            if (!this.SpriteRenderer) this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
            if (!this.Animator) this.Animator = this.GetComponent<Animator>();
        }

        // --- Public APIs ---
        public void Initialize(uint runtimeStatusEffectID, StatusEffectProfile profile, GameObject ownerRoot, int ownerVisualSortingOrder)
        {
            _runtimeStatusEffectID = runtimeStatusEffectID;
            _statusEffectProfile = profile;
            _ownerRoot = ownerRoot;

            this.ApplyOwnerLayer();
            this.ApplySortingOrder(ownerVisualSortingOrder);
            this.ApplyAnimationClip();
        }

        // --- Helpers ---
        private void ApplyOwnerLayer()
        {
            if (this.OwnerRoot == null) return;
            this.gameObject.layer = this.OwnerRoot.layer;
        }

        private void ApplySortingOrder(int ownerVisualSortingOrder)
        {
            if (this.SpriteRenderer == null) return;

            int offset = 5;
            if (this.StatusEffectProfile != null)
            {
                offset = this.StatusEffectProfile.VFXSortingOrderOffset;
            }

            this.SpriteRenderer.sortingOrder = ownerVisualSortingOrder + offset;
        }

        private void ApplyAnimationClip()
        {
            if (this.Animator == null || this.StatusEffectProfile == null || this.StatusEffectProfile.VFXClip == null)
                return;

            RuntimeAnimatorController baseController = this.Animator.runtimeAnimatorController;
            if (baseController == null) return;

            this.RuntimeOverrideController = new AnimatorOverrideController(baseController);
            this.RuntimeOverrideController[this.PlaceholderClip] = this.StatusEffectProfile.VFXClip;
            this.Animator.runtimeAnimatorController = this.RuntimeOverrideController;
        }
    }
}
