using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public partial class CandidateLoadoutSubPanelController : MonoBehaviour
    {
        // --- States ---
        //  2. Titles
        [Header("Titles")]
        [SerializeField] private GameObject _titleReplaceWeapon;
        [SerializeField] private GameObject _titleReplaceConsumableItem;
        [SerializeField] private GameObject _titleReplaceInteractableItem;
        [SerializeField] private GameObject _titleReplaceSpecialSkill;
        private GameObject TitleReplaceWeapon => _titleReplaceWeapon;
        private GameObject TitleReplaceConsumableItem => _titleReplaceConsumableItem;
        private GameObject TitleReplaceInteractableItem => _titleReplaceInteractableItem;
        private GameObject TitleReplaceSpecialSkill => _titleReplaceSpecialSkill;

        //  6. Runtime States
        //      6.2. Spawned Slots (Controllers)
        private readonly List<CandidateLoadoutSlotUIController> _spawnedSlots = new();
        private IReadOnlyList<CandidateLoadoutSlotUIController> SpawnedSlots => _spawnedSlots;

        //      6.3. Candidates (Current)
        private readonly List<object> _currentCandidates = new();
        private IReadOnlyList<object> CurrentCandidates => _currentCandidates;

        //      6.4. Selection Mode
        private CandidateLoadoutSelectionModeType _currentMode = CandidateLoadoutSelectionModeType.None;
        private CandidateLoadoutSelectionModeType CurrentMode
        {
            get => _currentMode;
            set => _currentMode = value;
        }

        // --- Public APIs ---
        public void HidePanel()
        {
            this.TargetSlot = null;
            this.CurrentMode = CandidateLoadoutSelectionModeType.None;
            this.CurrentPageIndex = 0;

            _currentCandidates.Clear();
            this.ClearSpawnedSlots();

            this.gameObject.SetActive(false);
        }

        public void ShowWeaponCandidates(EquippedLoadoutSlotUIController targetSlot)
        {
            IReadOnlyList<InventoryItemEntry> weaponEntries = this.PlayerInventoryController.AvailableWeaponEntries;

            this.TargetSlot = targetSlot;
            this.CurrentMode = CandidateLoadoutSelectionModeType.ReplaceWeapon;
            this.CurrentPageIndex = 0;

            _currentCandidates.Clear();
            if (weaponEntries != null) _currentCandidates.AddRange(weaponEntries);

            this.gameObject.SetActive(true);
            this.RefreshCurrentPage();
        }

        public void ShowConsumableCandidates(EquippedLoadoutSlotUIController targetSlot)
        {
            IReadOnlyList<InventoryItemEntry> consumableEntries = this.PlayerInventoryController.AvailableConsumableItemEntries;

            this.TargetSlot = targetSlot;
            this.CurrentMode = CandidateLoadoutSelectionModeType.ReplaceConsumableItem;
            this.CurrentPageIndex = 0;

            _currentCandidates.Clear();
            if (consumableEntries != null) _currentCandidates.AddRange(consumableEntries);

            this.gameObject.SetActive(true);
            this.RefreshCurrentPage();
        }

        public void ShowInteractableCandidates(EquippedLoadoutSlotUIController targetSlot)
        {
            IReadOnlyList<InventoryItemEntry> interactableEntries = this.PlayerInventoryController.AvailableInteractableItemEntries;

            this.TargetSlot = targetSlot;
            this.CurrentMode = CandidateLoadoutSelectionModeType.ReplaceInteractableItem;
            this.CurrentPageIndex = 0;

            _currentCandidates.Clear();
            if (interactableEntries != null) _currentCandidates.AddRange(interactableEntries);

            this.gameObject.SetActive(true);
            this.RefreshCurrentPage();
        }

        public void ShowSpecialSkillCandidates(EquippedLoadoutSlotUIController targetSlot)
        {
            IReadOnlyList<SkillProfile> skillCandidates = this.PlayerInventoryController.GetPossibleActiveSpecialSkillCandidates(targetSlot.BoundWeaponEntry);

            this.TargetSlot = targetSlot;
            this.CurrentMode = CandidateLoadoutSelectionModeType.ChangeWeaponSpecialSkill;
            this.CurrentPageIndex = 0;

            _currentCandidates.Clear();
            if (skillCandidates != null) _currentCandidates.AddRange(skillCandidates);

            this.gameObject.SetActive(true);
            this.RefreshCurrentPage();
        }

        // --- Helpers ---
        private void RefreshCurrentPage()
        {
            this.RefreshTitles();
            this.ClearSpawnedSlots();

            int startIndex = this.CurrentPageIndex * this.SlotsPerPage;
            int endIndexExclusive = Mathf.Min(startIndex + this.SlotsPerPage, this.CurrentCandidates.Count);

            for (int i = startIndex; i < endIndexExclusive; i++)
            {
                int localIndex = i - startIndex;
                CandidateLoadoutSlotUIController slot = this.CreateCandidateSlot(localIndex);
                if (slot == null) continue;

                object candidate = this.CurrentCandidates[i];

                switch (this.CurrentMode)
                {
                    case CandidateLoadoutSelectionModeType.ReplaceWeapon:
                        slot.BindWeapon(candidate as InventoryItemEntry);
                        break;

                    case CandidateLoadoutSelectionModeType.ReplaceConsumableItem:
                        slot.BindConsumableItem(candidate as InventoryItemEntry);
                        break;

                    case CandidateLoadoutSelectionModeType.ReplaceInteractableItem:
                        slot.BindInteractableItem(candidate as InventoryItemEntry);
                        break;

                    case CandidateLoadoutSelectionModeType.ChangeWeaponSpecialSkill:
                        slot.BindSpecialSkill(candidate as SkillProfile);
                        break;
                }

                slot.OnClicked -= this.HandleCandidateSlotClicked;
                slot.OnClicked += this.HandleCandidateSlotClicked;

                _spawnedSlots.Add(slot);
            }

            this.RefreshPagination();
        }

        private void RefreshTitles()
        {
            if (this.TitleReplaceWeapon != null) this.TitleReplaceWeapon.SetActive(this.CurrentMode == CandidateLoadoutSelectionModeType.ReplaceWeapon);
            if (this.TitleReplaceConsumableItem != null) this.TitleReplaceConsumableItem.SetActive(this.CurrentMode == CandidateLoadoutSelectionModeType.ReplaceConsumableItem);
            if (this.TitleReplaceInteractableItem != null) this.TitleReplaceInteractableItem.SetActive(this.CurrentMode == CandidateLoadoutSelectionModeType.ReplaceInteractableItem);
            if (this.TitleReplaceSpecialSkill != null) this.TitleReplaceSpecialSkill.SetActive(this.CurrentMode == CandidateLoadoutSelectionModeType.ChangeWeaponSpecialSkill);
        }

        private void ClearSpawnedSlots()
        {
            for (int i = 0; i < this.SpawnedSlots.Count; i++)
            {
                if (this.SpawnedSlots[i] != null)
                {
                    UnityEngine.Object.Destroy(this.SpawnedSlots[i].gameObject);
                }
            }

            _spawnedSlots.Clear();
        }
    }
}
