using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class ResourceBarUIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Slider _slider;

        private Slider Slider
        {
            get => _slider;
            set => _slider = value;
        }

        private void Awake()
        {
            if (!this.Slider) this.Slider = this.GetComponent<Slider>();
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
    }
}
