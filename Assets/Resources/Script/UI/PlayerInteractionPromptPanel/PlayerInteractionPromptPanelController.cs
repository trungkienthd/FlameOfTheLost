using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class PlayerInteractionPromptPanelController : FOTLMonoBehaviour
    {
        [Header("References - Prompt Text")]
        [SerializeField] private TMP_Text _promptText;
        private TMP_Text PromptText => _promptText;


        [Header("References - Hold Progress")]
        [SerializeField] private GameObject _holdProgressRoot;
        [SerializeField] private Image _holdProgressFill;
        private GameObject HoldProgressRoot => _holdProgressRoot;
        private Image HoldProgressFill => _holdProgressFill;

        // --- Public APIs ---
        //  1. Text-only Prompt
        public void Show(string text)
        {
            if (this.PromptText != null)
            {
                this.PromptText.text = text;
            }

            this.HideHoldProgress();

            this.gameObject.SetActive(true);
        }

        //  2. Active Hold Prompt
        public void ShowHoldProgress(string text, float normalizedProgress)
        {
            if (this.PromptText != null)
            {
                this.PromptText.text = text;
            }

            if (this.HoldProgressFill != null)
            {
                this.HoldProgressFill.fillAmount = Mathf.Clamp01(normalizedProgress);
            }

            if (this.HoldProgressRoot != null)
            {
                this.HoldProgressRoot.SetActive(true);
            }

            this.gameObject.SetActive(true);
        }

        //  3. Hide Prompt
        public void Hide()
        {
            if (this.PromptText != null)
            {
                this.PromptText.text = string.Empty;
            }

            this.HideHoldProgress();

            this.gameObject.SetActive(false);
        }

        // --- Helpers ---
        private void HideHoldProgress()
        {
            if (this.HoldProgressFill != null)
            {
                this.HoldProgressFill.fillAmount = 0f;
            }

            if (this.HoldProgressRoot != null)
            {
                this.HoldProgressRoot.SetActive(false);
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PromptText == null) _promptText = this.GetComponentInChildren<TMP_Text>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.PromptText == null)
            {
                Debug.LogError($"[{this.name}.{this.GetType().Name}] {nameof(PromptText)} reference is not assigned.", this);
            }

            if (this.HoldProgressRoot == null)
            {
                Debug.LogWarning($"[{this.name}.{this.GetType().Name}] {nameof(HoldProgressRoot)} reference is not assigned.", this);
            }
            if (this.HoldProgressFill == null)
            {
                Debug.LogWarning($"[{this.name}.{this.GetType().Name}] {nameof(HoldProgressFill)} reference is not assigned.", this);
            }
        }
    }
}
