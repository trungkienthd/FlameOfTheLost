using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class ItemEntryListPaginationHandler : MonoBehaviour
    {
        [SerializeField] private Button _buttonPrevious;
        [SerializeField] private Button _buttonNext;

        [SerializeField] private TMP_Text _textPageCurrent;
        [SerializeField] private TMP_Text _textPageMax;

        [SerializeField] private int _slotsPerPage = 12;
        [SerializeField] private int _maxSupportedPages = 99;
        [SerializeField] private int _columnCount = 4;

        private Button ButtonPrevious => _buttonPrevious;
        private Button ButtonNext => _buttonNext;
        private TMP_Text TextPageCurrent => _textPageCurrent;
        private TMP_Text TextPageMax => _textPageMax;
        public int SlotsPerPage => _slotsPerPage;   
        public int MaxSupportedPages => _maxSupportedPages;
        public int ColumnCount => _columnCount;

        public event Action<int> OnCurrentPageChanged;

        public int CurrentPageNumber { get; private set; } = 1;
        public int MaxPageNumber { get; private set; } = 1;

        private void Awake()
        {
            if (this.ButtonPrevious == null)
            {
                Transform buttonPrevious = this.transform.Find("ButtonPrevious");
                if (buttonPrevious != null) _buttonPrevious = buttonPrevious.GetComponent<Button>();
            }
            if (this.ButtonNext == null)
            {
                Transform buttonNext = this.transform.Find("ButtonNext");
                if (buttonNext != null) _buttonNext = buttonNext.GetComponent<Button>();
            }
            if (this.ButtonPrevious != null)
            {
                this.ButtonPrevious.onClick.AddListener(this.HandlePreviousClicked);
            }

            if (this.ButtonNext != null)
            {
                this.ButtonNext.onClick.AddListener(this.HandleNextClicked);
            }

            if (this.TextPageCurrent == null)
            {
                Transform textPageCurrent = this.transform.Find("TextPageCurrent");
                if (textPageCurrent != null) _textPageCurrent = textPageCurrent.GetComponent<TMP_Text>();
            }
            if (this.TextPageMax == null)
            {
                Transform textPageMax = this.transform.Find("TextPageMax");
                if (textPageMax != null) _textPageMax = textPageMax.GetComponent<TMP_Text>();
            }

            this.RefreshVisuals();
        }
        private void OnDestroy()
        {
            if (this.ButtonPrevious != null)
            {
                this.ButtonPrevious.onClick.RemoveListener(this.HandlePreviousClicked);
            }

            if (this.ButtonNext != null)
            {
                this.ButtonNext.onClick.RemoveListener(this.HandleNextClicked);
            }
        }

        public bool TrySetTotalEntryCount(int totalEntryCount)
        {
            int oldPage = this.CurrentPageNumber;

            int pageCount = this.SlotsPerPage > 0
                ? Mathf.CeilToInt(totalEntryCount / (float)this.SlotsPerPage)
                : 1;

            if (pageCount > this.MaxSupportedPages)
            {
                Debug.LogWarning($"[{this.gameObject.name}.ItemEntryListPaginationHandler] Inventory pages exceeded supported limit ({this.MaxSupportedPages}). Some entries will not be visible.", this);
            }

            this.MaxPageNumber = Mathf.Clamp(pageCount, 1, this.MaxSupportedPages);
            this.CurrentPageNumber = Mathf.Clamp(this.CurrentPageNumber, 1, this.MaxPageNumber);

            this.RefreshVisuals();

            bool pageChanged = oldPage != this.CurrentPageNumber;
            if (pageChanged)
            {
                this.OnCurrentPageChanged?.Invoke(this.CurrentPageNumber);
            }

            return pageChanged;
        }

        public void SetCurrentPageWithoutNotify(int pageNumber)
        {
            this.CurrentPageNumber = Mathf.Clamp(pageNumber, 1, this.MaxPageNumber);
            this.RefreshVisuals();
        }

        private void HandlePreviousClicked()
        {
            if (this.CurrentPageNumber <= 1) return;

            this.CurrentPageNumber--;
            this.RefreshVisuals();
            this.OnCurrentPageChanged?.Invoke(this.CurrentPageNumber);
        }

        private void HandleNextClicked()
        {
            if (this.CurrentPageNumber >= this.MaxPageNumber) return;

            this.CurrentPageNumber++;
            this.RefreshVisuals();
            this.OnCurrentPageChanged?.Invoke(this.CurrentPageNumber);
        }

        private void RefreshVisuals()
        {
            if (this.TextPageCurrent != null)
            {
                this.TextPageCurrent.text = this.CurrentPageNumber.ToString();
            }

            if (this.TextPageMax != null)
            {
                this.TextPageMax.text = this.MaxPageNumber.ToString();
            }

            if (this.ButtonPrevious != null)
            {
                this.ButtonPrevious.interactable = this.CurrentPageNumber > 1;
            }

            if (this.ButtonNext != null)
            {
                this.ButtonNext.interactable = this.CurrentPageNumber < this.MaxPageNumber;
            }
        }
    }
}
