using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Image))]
    public class UIRoundedImageHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image _targetImage;
        private Image TargetImage => _targetImage;

        [Header("Rounded Border Settings")]
        [SerializeField] private bool _useRoundedBorder = true;
        [SerializeField, Range(0f, 100f)] private float _borderRadius = 16f;
        [SerializeField] private Material _roundedImageMaterial;
        private bool UseRoundedBorder => _useRoundedBorder;
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
