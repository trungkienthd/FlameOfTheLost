using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class AppliedStatusEffectSlotUIController : MonoBehaviour
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private Image _icon;
        [SerializeField] private Image _cooldownFill;
        [SerializeField] private TMP_Text _stackText;
        public Image Icon => _icon;
        public Image CooldownFill => _cooldownFill;
        public TMP_Text StackText => _stackText;

        //  2. Runtime
        private RuntimeStatusEffect _boundRuntimeStatusEffect;

        public RuntimeStatusEffect BoundRuntimeStatusEffect => _boundRuntimeStatusEffect;

        // --- Public APIs ---
        public void Bind(RuntimeStatusEffect runtimeStatusEffect)
        {
            _boundRuntimeStatusEffect = runtimeStatusEffect;

            this.Refresh();
        }

        public void Refresh()
        {
            if (this.BoundRuntimeStatusEffect == null || this.BoundRuntimeStatusEffect.Profile == null)
            {
                this.Clear();
                return;
            }

            this.RefreshIcon();
            this.RefreshCooldown();
            this.RefreshStacks();
        }

        // --- Helpers ---
        private void RefreshIcon()
        {
            if (this.Icon == null) return;

            Sprite iconSprite = this.BoundRuntimeStatusEffect.Profile.Icon;

            this.Icon.sprite = iconSprite;
            this.Icon.enabled = iconSprite != null;
        }

        private void RefreshCooldown()
        {
            if (this.CooldownFill == null) return;

            float maxDuration = this.BoundRuntimeStatusEffect.Profile.Duration;
            float remainingDuration = this.BoundRuntimeStatusEffect.RemainingDuration;

            if (maxDuration <= 0f)
            {
                this.CooldownFill.fillAmount = 0f;
                this.CooldownFill.enabled = false;
                return;
            }

            float normalizedRemainingDuration = Mathf.Clamp01(remainingDuration / maxDuration);

            this.CooldownFill.enabled = true;
            this.CooldownFill.fillAmount = normalizedRemainingDuration;
        }

        private void RefreshStacks()
        {
            if (this.StackText == null) return;

            int currentStacks = this.BoundRuntimeStatusEffect.CurrentStacks;

            bool shouldShowStacks = currentStacks > 1;

            this.StackText.gameObject.SetActive(shouldShowStacks);
            this.StackText.text = shouldShowStacks ? currentStacks.ToString() : string.Empty;
        }

        private void Clear()
        {
            if (this.Icon != null)
            {
                this.Icon.sprite = null;
                this.Icon.enabled = false;
            }

            if (this.CooldownFill != null)
            {
                this.CooldownFill.fillAmount = 0f;
                this.CooldownFill.enabled = false;
            }

            if (this.StackText != null)
            {
                this.StackText.text = string.Empty;
                this.StackText.gameObject.SetActive(false);
            }
        }
    }
}
