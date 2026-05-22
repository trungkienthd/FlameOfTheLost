using TMPro;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerInteractionPromptPanelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text _promptText;
        private TMP_Text PromptText => _promptText;

        private void Awake()
        {
            this.CacheReferences();
            // this.Hide();
        }

        public void Show(string text)
        {
            if (this.PromptText != null)
            {
                this.PromptText.text = text;
            }

            this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (this.PromptText != null)
            {
                this.PromptText.text = string.Empty;
            }

            this.gameObject.SetActive(false);
        }

        private void CacheReferences()
        {
            if (this.PromptText == null) _promptText = this.GetComponentInChildren<TMP_Text>(true);
        }
    }
}
