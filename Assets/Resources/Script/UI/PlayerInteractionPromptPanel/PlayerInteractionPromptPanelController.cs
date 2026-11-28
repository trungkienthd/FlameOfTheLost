using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class PlayerInteractionPromptPanelController : FOTLMonoBehaviour
    {
        // --- References ---
        [Header("References - Prompt Text")]
        [SerializeField] private TMP_Text _promptText;
        private TMP_Text PromptText => _promptText;

        [Header("References - Hold Progress")]
        [SerializeField] private GameObject _holdProgressRoot;
        [SerializeField] private Image _holdProgressFill;
        private GameObject HoldProgressRoot => _holdProgressRoot;
        private Image HoldProgressFill => _holdProgressFill;

        // --- Configs ---
        [Header("Configs - Normal Prompt")]
        [SerializeField] private float _normalPromptPositionY = 75f;
        [SerializeField] private float _normalPromptFontSize = 35f;
        private float NormalPromptPositionY => _normalPromptPositionY;
        private float NormalPromptFontSize => _normalPromptFontSize;

        [Header("Configs - Hold Prompt")]
        [SerializeField] private float _holdPromptPositionY = 100f;
        [SerializeField] private float _holdPromptFontSize = 30f;
        private float HoldPromptPositionY => _holdPromptPositionY;
        private float HoldPromptFontSize => _holdPromptFontSize;

        // --- Public APIs ---
        //  1. Text-only Prompt
        public void Show(string text)
        {
            this.gameObject.SetActive(true);

            this.ConfigurePromptTextLayout(isHoldProgressActive: false);

            this.SetPromptText(text);

            this.HideHoldProgress();
        }

        //  2. Active Hold Prompt
        public void ShowHoldProgress(string text, float normalizedProgress)
        {
            this.gameObject.SetActive(true);

            this.ConfigurePromptTextLayout(isHoldProgressActive: true);

            this.SetPromptText(text);

            if (this.HoldProgressFill != null)
            {
                this.HoldProgressFill.fillAmount = Mathf.Clamp01(normalizedProgress);
            }

            if (this.HoldProgressRoot != null)
            {
                this.HoldProgressRoot.SetActive(true);
            }
        }

        //  3. Hide Prompt
        public void Hide()
        {
            this.SetPromptText(string.Empty);

            this.HideHoldProgress();

            this.ConfigurePromptTextLayout(isHoldProgressActive: false);

            this.gameObject.SetActive(false);
        }

        // --- Helpers ---
        //  1. Prompt Text Management
        private void SetPromptText(string text)
        {
            if (this.PromptText == null) return;
            if (this.PromptText.text == text) return;

            this.PromptText.text = text;
        }

        //  2. Prompt Text Layout
        private void ConfigurePromptTextLayout(bool isHoldProgressActive)
        {
            if (this.PromptText == null) return;

            float positionY = isHoldProgressActive
                ? this.HoldPromptPositionY
                : this.NormalPromptPositionY;

            float fontSize = isHoldProgressActive
                ? this.HoldPromptFontSize
                : this.NormalPromptFontSize;

            RectTransform promptRectTransform = this.PromptText.rectTransform;

            Vector2 anchoredPosition = promptRectTransform.anchoredPosition;

            // a. Configure vertical position.
            if (!Mathf.Approximately(anchoredPosition.y, positionY))
            {
                anchoredPosition.y = positionY;

                promptRectTransform.anchoredPosition = anchoredPosition;
            }

            // b. Configure font size.
            if (!Mathf.Approximately(this.PromptText.fontSize, fontSize))
            {
                this.PromptText.fontSize = fontSize;
            }
        }

        //  3. Hold Progress Management
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
