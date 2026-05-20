using UnityEngine;

namespace FlameOfTheLost
{
    public class CandidateLoadoutSubPanelTitleHandler : MonoBehaviour
    {
        [Header("Titles")]
        [SerializeField] private GameObject _titleReplaceWeapon;
        [SerializeField] private GameObject _titleReplaceConsumableItem;
        [SerializeField] private GameObject _titleReplaceCarriableItem;
        [SerializeField] private GameObject _titleReplaceSpecialSkill;
        private GameObject TitleReplaceWeapon => _titleReplaceWeapon;
        private GameObject TitleReplaceConsumableItem => _titleReplaceConsumableItem;
        private GameObject TitleReplaceCarriableItem => _titleReplaceCarriableItem;
        private GameObject TitleReplaceSpecialSkill => _titleReplaceSpecialSkill;

        private void Awake()
        {
            this.CacheReferences();
            this.HideAll();
        }

        public void Refresh(CandidateLoadoutSelectionModeType mode)
        {
            if (this.TitleReplaceWeapon != null)
            {
                this.TitleReplaceWeapon.SetActive(mode == CandidateLoadoutSelectionModeType.ReplaceWeapon);
            }

            if (this.TitleReplaceConsumableItem != null)
            {
                this.TitleReplaceConsumableItem.SetActive(mode == CandidateLoadoutSelectionModeType.ReplaceConsumableItem);
            }

            if (this.TitleReplaceCarriableItem != null)
            {
                this.TitleReplaceCarriableItem.SetActive(mode == CandidateLoadoutSelectionModeType.ReplaceCarriableItem);
            }

            if (this.TitleReplaceSpecialSkill != null)
            {
                this.TitleReplaceSpecialSkill.SetActive(mode == CandidateLoadoutSelectionModeType.ChangeWeaponSpecialSkill);
            }
        }

        public void HideAll()
        {
            if (this.TitleReplaceWeapon != null)            this.TitleReplaceWeapon.SetActive(false);
            if (this.TitleReplaceConsumableItem != null)    this.TitleReplaceConsumableItem.SetActive(false);
            if (this.TitleReplaceCarriableItem != null)  this.TitleReplaceCarriableItem.SetActive(false);
            if (this.TitleReplaceSpecialSkill != null)      this.TitleReplaceSpecialSkill.SetActive(false);
        }

        private void CacheReferences()
        {
            if (this.TitleReplaceWeapon == null)
            {
                Transform found = this.transform.Find("Title_ReplaceWeapon");
                if (found != null) _titleReplaceWeapon = found.gameObject;
            }

            if (this.TitleReplaceConsumableItem == null)
            {
                Transform found = this.transform.Find("Title_ReplaceConsumableItem");
                if (found != null) _titleReplaceConsumableItem = found.gameObject;
            }

            if (this.TitleReplaceCarriableItem == null)
            {
                Transform found = this.transform.Find("Title_ReplaceCarriableItem");
                if (found != null) _titleReplaceCarriableItem = found.gameObject;
            }

            if (this.TitleReplaceSpecialSkill == null)
            {
                Transform found = this.transform.Find("Title_ReplaceSpecialSkill");
                if (found != null) _titleReplaceSpecialSkill = found.gameObject;
            }
        }
    }
}
