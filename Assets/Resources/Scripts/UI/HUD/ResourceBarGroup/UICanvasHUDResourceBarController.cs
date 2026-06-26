using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class UICanvasHUDResourceBarController : FOTLMonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Slider _slider;

        private Slider Slider
        {
            get => _slider;
            set => _slider = value;
        }

        private void Start()
        {
            if (this.Slider)
            {
                this.Slider.minValue = 0f;
                this.Slider.maxValue = 1f;
                this.Slider.interactable = false;
                this.Slider.transition = Selectable.Transition.None;
            }

            this.SetNormalized(1f);
        }

        public void Set(int currentValue, int maxValue)
        {
            this.SetNormalized(maxValue > 0f ? Mathf.Clamp01((float)currentValue / maxValue) : 0f);
        }

        public void SetNormalized(float normalizedValue)
        {
            if (this.Slider) this.Slider.value = Mathf.Clamp01(normalizedValue);
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (!this.Slider) this.Slider = this.GetComponent<Slider>();
        }

        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Slider ==  null) Debug.LogError($"[{this.gameObject.name}.{nameof(UICanvasHUDResourceBarController)}] Slider reference is missing.");
        }
    }
}
