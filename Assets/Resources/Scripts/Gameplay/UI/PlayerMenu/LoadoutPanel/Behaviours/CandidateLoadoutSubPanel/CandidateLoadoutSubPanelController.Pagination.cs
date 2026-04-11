using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public partial class CandidateLoadoutSubPanelController : MonoBehaviour
    {
        // --- States ---
        //  4. Pagination
        [Header("Pagination")]
        [SerializeField] private Button _buttonPrevious;
        [SerializeField] private Button _buttonNext;
        [SerializeField] private Image _pageCurrent;
        [SerializeField] private Image _pageMax;
        private Button ButtonPrevious => _buttonPrevious;
        private Button ButtonNext => _buttonNext;
        private Image PageCurrent => _pageCurrent;
        private Image PageMax => _pageMax;

        //  6. Runtime States
        //      6.1. Pagination State
        private int _currentPageIndex;
        private int CurrentPageIndex
        {
            get => _currentPageIndex;
            set => _currentPageIndex = value;
        }

        // --- Helpers ---
        private void GoToPreviousPage()
        {
            if (this.CurrentPageIndex <= 0) return;

            this.CurrentPageIndex--;
            this.RefreshCurrentPage();
        }

        private void GoToNextPage()
        {
            if (this.CurrentPageIndex >= this.GetMaxPageIndex()) return;

            this.CurrentPageIndex++;
            this.RefreshCurrentPage();
        }

        private void RefreshPagination()
        {
            int maxPageNumber = Mathf.Clamp(this.GetMaxPageIndex() + 1, 1, this.MaxSupportedPages);
            int currentPageNumber = Mathf.Clamp(this.CurrentPageIndex + 1, 1, this.MaxSupportedPages);

            if (this.PageCurrent != null)
            {
                this.PageCurrent.sprite = SharedSpritesUIUtility.GetDigitSprite(currentPageNumber);
                this.PageCurrent.enabled = this.PageCurrent.sprite != null;
            }

            if (this.PageMax != null)
            {
                this.PageMax.sprite = SharedSpritesUIUtility.GetDigitSprite(maxPageNumber);
                this.PageMax.enabled = this.PageMax.sprite != null;
            }

            if (this.ButtonPrevious != null)
            {
                this.ButtonPrevious.interactable = this.CurrentPageIndex > 0;
            }

            if (this.ButtonNext != null)
            {
                this.ButtonNext.interactable = this.CurrentPageIndex < this.GetMaxPageIndex();
            }
        }

        private int GetMaxPageIndex()
        {
            int pageCount = Mathf.CeilToInt(this.CurrentCandidates.Count / (float)this.SlotsPerPage);
            pageCount = Mathf.Clamp(pageCount, 1, this.MaxSupportedPages);
            return pageCount - 1;
        }
    }
}
