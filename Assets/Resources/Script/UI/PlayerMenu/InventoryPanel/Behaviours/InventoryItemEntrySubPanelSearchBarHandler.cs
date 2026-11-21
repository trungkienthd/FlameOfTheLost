using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class InventoryItemEntrySubPanelSearchBarHandler : InventoryPanelItemEntryListSubBehaviourBase
    {
        // --- References ---
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _clearButton;
        private TMP_InputField InputField => _inputField;
        private Button ClearButton => _clearButton;

        // --- Runtime ---
        public string CurrentKeyword { get; private set; } = string.Empty;

        // --- Events ---
        public event Action<string> OnSearchValueChanged;

        // --- Public APIs ---
        public void SetKeywordWithoutNotify(string keyword)
        {
            this.CurrentKeyword = keyword ?? string.Empty;

            if (this.InputField != null)
            {
                this.InputField.SetTextWithoutNotify(this.CurrentKeyword);
            }
        }

        // --- Helpers ---
        //  1. Event Handling
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

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            if (this.InputField == null) _inputField = this.GetComponentInChildren<TMP_InputField>();

            if (this.ClearButton == null)
            {
                Transform clearButtonTransform = this.transform.Find("ClearButton");
                if (clearButtonTransform != null) _clearButton = clearButtonTransform.GetComponent<Button>();
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.InputField == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryItemEntrySubPanelSearchBarHandler)}] {nameof(this.InputField)} is missing.", this);
            }
            if (this.ClearButton == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryItemEntrySubPanelSearchBarHandler)}] {nameof(this.ClearButton)} is missing.", this);
            }
        }
        protected override void BindInternalReferences()
        {
            if (this.InputField != null) this.InputField.onValueChanged.AddListener(this.HandleInputChanged);
            if (this.ClearButton != null) this.ClearButton.onClick.AddListener(this.HandleClearClicked);
        }
        protected override void UnbindInternalReferences()
        {
            if (this.InputField != null) this.InputField.onValueChanged.RemoveListener(this.HandleInputChanged);
            if (this.ClearButton != null) this.ClearButton.onClick.RemoveListener(this.HandleClearClicked);
        }
    }
}