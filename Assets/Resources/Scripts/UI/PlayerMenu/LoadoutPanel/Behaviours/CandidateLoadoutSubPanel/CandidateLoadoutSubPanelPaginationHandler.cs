using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class CandidateLoadoutSubPanelPaginationHandler : MonoBehaviour
    {
        // --- States ---
        //  1. References
        [SerializeField] private Button _buttonPrevious;
        [SerializeField] private Button _buttonNext;
        [SerializeField] private TMP_Text _textPageCurrent;
        [SerializeField] private TMP_Text _textPageMax;
        private Button ButtonPrevious => _buttonPrevious;
        private Button ButtonNext => _buttonNext;
        private TMP_Text TextPageCurrent => _textPageCurrent;
        private TMP_Text TextPageMax => _textPageMax;

        //  2. Configurations
        [SerializeField] private int _slotsPerPage = 9;
        [SerializeField] private int _maxSupportedPages = 99;
        [SerializeField] private int _columnCount = 3;
        public int SlotsPerPage => _slotsPerPage;
        public int MaxSupportedPages => _maxSupportedPages;
        public int ColumnCount => _columnCount;

        //  3. Runtime States
        public int CurrentPageNumber { get; private set; } = 1;
        public int CurrentPageIndex => this.CurrentPageNumber - 1;
        public int MaxPageNumber { get; private set; } = 1;

        //  4. Events
        public event Action<int> OnCurrentPageChanged;

        // --- Unity Callbacks ---
        private void Awake()
        {
            this.CacheReferences();
            this.SubscribeEvents();
            this.RefreshVisuals();
        }
        private void OnDestroy()
        {
            this.UnsubscribeEvents();
        }

        // --- Public APIs ---
        public void SetTotalCountAndReset(int totalCount)
        {
            int pageCount = Mathf.CeilToInt(Mathf.Max(0, totalCount) / (float)this.SlotsPerPage);

            if (pageCount <= 0)
            {
                pageCount = 1;
            }

            if (pageCount > this.MaxSupportedPages)
            {
                Debug.LogWarning($"[{this.gameObject.name}] Candidate pages exceeded supported limit ({this.MaxSupportedPages}). Some candidates will not be visible.", this);
            }

            this.MaxPageNumber = Mathf.Clamp(pageCount, 1, this.MaxSupportedPages);
            this.CurrentPageNumber = 1;

            this.RefreshVisuals();
        }

        public void ResetPagination()
        {
            this.CurrentPageNumber = 1;
            this.MaxPageNumber = 1;

            this.RefreshVisuals();
        }

        // --- Helpers ---
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

        private void CacheReferences()
        {
            if (this.ButtonPrevious == null)
            {
                Transform found = this.transform.Find("ButtonPrevious");
                if (found != null) _buttonPrevious = found.GetComponent<Button>();
            }

            if (this.ButtonNext == null)
            {
                Transform found = this.transform.Find("ButtonNext");
                if (found != null) _buttonNext = found.GetComponent<Button>();
            }

            if (this.TextPageCurrent == null)
            {
                Transform found = this.transform.Find("TextPageCurrent");
                if (found != null) _textPageCurrent = found.GetComponent<TMP_Text>();
            }

            if (this.TextPageMax == null)
            {
                Transform found = this.transform.Find("TextPageMax");
                if (found != null) _textPageMax = found.GetComponent<TMP_Text>();
            }
        }
        private void SubscribeEvents()
        {
            if (this.ButtonPrevious != null)
            {
                this.ButtonPrevious.onClick.AddListener(this.HandlePreviousClicked);
            }

            if (this.ButtonNext != null)
            {
                this.ButtonNext.onClick.AddListener(this.HandleNextClicked);
            }
        }
        private void UnsubscribeEvents()
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
    }
}
