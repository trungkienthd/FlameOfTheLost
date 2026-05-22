using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public enum UIRoundedImageType
    {
        SpriteSliced,
        Material
    }

    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    public class UIRoundedImageHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image _targetImage;
        private Image TargetImage => _targetImage;

        [Header("Rounded Border Settings")]
        [SerializeField] private bool _useRoundedBorder = true;
        [SerializeField] private UIRoundedImageType _roundedImageType = UIRoundedImageType.SpriteSliced;
        private bool UseRoundedBorder => _useRoundedBorder;
        private UIRoundedImageType RoundedImageType => _roundedImageType;

        [Header("Sprite Sliced Settings")]
        [SerializeField] private Sprite _roundedSprite;
        private Sprite RoundedSprite => _roundedSprite;

        [Header("Material Settings")]
        [SerializeField, Range(0f, 100f)] private float _borderRadius = 16f;
        [SerializeField] private Material _roundedImageMaterial;
        private float BorderRadius => _borderRadius;
        private Material RoundedImageMaterial => _roundedImageMaterial;

        private Material _runtimeMaterial;
        private Material RuntimeMaterial
        {
            get => _runtimeMaterial;
            set => _runtimeMaterial = value;
        }

        private void Awake()
        {
            this.CacheReferences();
            this.ApplyRoundedBorderSettings();
        }
        private void OnEnable()
        {
            this.CacheReferences();
            this.ApplyRoundedBorderSettings();
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            this.CacheReferences();
            this.ApplyRoundedBorderSettings();
        }
#endif

        private void CacheReferences()
        {
            if (this.TargetImage == null) _targetImage = GetComponent<Image>();
        }

        private void ApplyRoundedBorderSettings()
        {
            if (this.TargetImage == null) return;

            if (!this.UseRoundedBorder)
            {
                this.TargetImage.material = null;
                return;
            }

            switch (this.RoundedImageType)
            {
                case UIRoundedImageType.SpriteSliced:
                    this.ApplySpriteSlicedRoundedBorder();
                    break;

                case UIRoundedImageType.Material:
                    this.ApplyMaterialRoundedBorder();
                    break;
            }
        }

        private void ApplySpriteSlicedRoundedBorder()
        {
            this.TargetImage.material = null;

            if (this.RoundedSprite == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.UIRoundedImageHandler] No rounded sprite assigned.");
                return;
            }

            this.TargetImage.sprite = this.RoundedSprite;
            this.TargetImage.type = Image.Type.Sliced;
        }

        private void ApplyMaterialRoundedBorder()
        {
            if (this.RoundedImageMaterial == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.UIRoundedImageHandler] No rounded image material assigned.");
                return;
            }

            if (this.RuntimeMaterial == null)
            {
                this.RuntimeMaterial = new Material(this.RoundedImageMaterial);
            }

            this.RuntimeMaterial.SetFloat("_Radius", this.BorderRadius);
            this.TargetImage.material = this.RuntimeMaterial;
        }
    }
}
