using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class InventoryItemEntrySubPanelSearchBarHandler : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _clearButton;

        private TMP_InputField InputField => _inputField;
        private Button ClearButton => _clearButton;

        public event Action<string> OnSearchValueChanged;

        public string CurrentKeyword { get; private set; } = string.Empty;

        private void Awake()
        {
            if (this.InputField != null)
            {
                this.InputField.onValueChanged.AddListener(this.HandleInputChanged);
            }

            if (this.ClearButton != null)
            {
                this.ClearButton.onClick.AddListener(this.HandleClearClicked);
            }
        }
        private void OnDestroy()
        {
            if (this.InputField != null)
            {
                this.InputField.onValueChanged.RemoveListener(this.HandleInputChanged);
            }

            if (this.ClearButton != null)
            {
                this.ClearButton.onClick.RemoveListener(this.HandleClearClicked);
            }
        }

        public void SetKeywordWithoutNotify(string keyword)
        {
            this.CurrentKeyword = keyword ?? string.Empty;

            if (this.InputField != null)
            {
                this.InputField.SetTextWithoutNotify(this.CurrentKeyword);
            }
        }

        private void HandleInputChanged(string value)
        {
            this.CurrentKeyword = value ?? string.Empty;
            this.OnSearchValueChanged?.Invoke(this.CurrentKeyword);
        }

        private void HandleClearClicked()
        {
            this.CurrentKeyword = string.Empty;

            if (this.InputField != null)
            {
                this.InputField.SetTextWithoutNotify(string.Empty);
            }

            this.OnSearchValueChanged?.Invoke(this.CurrentKeyword);
        }
    }
}