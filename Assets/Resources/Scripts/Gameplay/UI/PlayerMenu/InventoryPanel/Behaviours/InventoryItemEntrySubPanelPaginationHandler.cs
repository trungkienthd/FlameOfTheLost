using System;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class InventoryItemEntrySubPanelPaginationHandler : MonoBehaviour
    {
        [SerializeField] private Button _buttonPrevious;
        [SerializeField] private Button _buttonNext;
        [SerializeField] private Image _pageCurrentImage;
        [SerializeField] private Image _pageMaxImage;
        [SerializeField] private int _slotsPerPage = 16;
        [SerializeField] private int _maxSupportedPages = 9;
        [SerializeField] private int _columnCount = 4;
        private Button ButtonPrevious => _buttonPrevious;
        private Button ButtonNext => _buttonNext;
        private Image PageCurrentImage => _pageCurrentImage;
        private Image PageMaxImage => _pageMaxImage;
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
                _buttonPrevious = buttonPrevious.GetComponent<Button>();
            }
            if (this.ButtonNext == null)
            {
                Transform buttonNext = this.transform.Find("ButtonNext");
                _buttonNext = buttonNext.GetComponent<Button>();
            }
            if (this.ButtonPrevious != null)
            {
                this.ButtonPrevious.onClick.AddListener(this.HandlePreviousClicked);
            }

            if (this.ButtonNext != null)
            {
                this.ButtonNext.onClick.AddListener(this.HandleNextClicked);
            }

            if (this.PageCurrentImage == null)
            {
                Transform pageCurrent = this.transform.Find("PageCurrent");
                _pageCurrentImage = pageCurrent.GetComponent<Image>();
            }
            if (this.PageMaxImage == null)
            {
                Transform pageMax = this.transform.Find("PageMax");
                _pageMaxImage = pageMax.GetComponent<Image>();
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
                Debug.LogWarning($"[{this.gameObject.name}.InventoryItemEntrySubPanelPaginationHandler] Inventory pages exceeded supported limit ({this.MaxSupportedPages}). Some entries will not be visible.", this);
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
            if (this.PageCurrentImage != null)
            {
                this.PageCurrentImage.sprite = SharedSpritesUIUtility.GetDigitSprite(this.CurrentPageNumber);
                this.PageCurrentImage.enabled = this.PageCurrentImage.sprite != null;
            }

            if (this.PageMaxImage != null)
            {
                this.PageMaxImage.sprite = SharedSpritesUIUtility.GetDigitSprite(this.MaxPageNumber);
                this.PageMaxImage.enabled = this.PageMaxImage.sprite != null;
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
