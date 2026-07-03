using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public partial class CandidateLoadoutSubPanelController : MonoBehaviour
    {
        // --- States ---
        //  1. References: Player-related
        [Header("References (Player)")]
        [SerializeField] private CharacterCombatLoadoutController _playerCombatLoadoutController;
        [SerializeField] private PlayerCarryLoadoutController _playerInteractionLoadoutController;
        [SerializeField] private InventoryController _playerInventoryController;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController => _playerCombatLoadoutController;
        private PlayerCarryLoadoutController PlayerInteractionLoadoutController => _playerInteractionLoadoutController;
        private InventoryController PlayerInventoryController => _playerInventoryController;

        //  2. References: Other Sub-Panels
        [Header("References (Equipped Loadout Sub-Panel)")]
        [SerializeField] private EquippedLoadoutSubPanelController _equippedLoadoutSubPanelController;
        private EquippedLoadoutSubPanelController EquippedLoadoutSubPanelController => _equippedLoadoutSubPanelController;

        //  3. References: Child Handlers
        [Header("References (Child Handlers)")]
        [SerializeField] private CandidateLoadoutSubPanelTitleHandler _titleHandler;
        [SerializeField] private CandidateLoadoutSubPanelCandidateListHandler _candidateListHandler;
        [SerializeField] private CandidateLoadoutSubPanelPaginationHandler _paginationHandler;
        private CandidateLoadoutSubPanelTitleHandler TitleHandler => _titleHandler;
        private CandidateLoadoutSubPanelCandidateListHandler CandidateListHandler => _candidateListHandler;
        private CandidateLoadoutSubPanelPaginationHandler PaginationHandler => _paginationHandler;

        //  4. Runtime States
        //      4.1. Candidates (Current)
        private readonly List<object> _currentCandidates = new();
        private IReadOnlyList<object> CurrentCandidates => _currentCandidates;

        //      4.2. Target Slot (Controller)
        private EquippedLoadoutSlotUIController _targetSlot;
        private EquippedLoadoutSlotUIController TargetSlot
        {
            get => _targetSlot;
            set => _targetSlot = value;
        }

        //      4.3. Selection Mode
        private CandidateLoadoutSelectionModeType _currentMode = CandidateLoadoutSelectionModeType.None;
        private CandidateLoadoutSelectionModeType CurrentMode
        {
            get => _currentMode;
            set => _currentMode = value;
        }

        // --- Unity Callbacks ---
        private void Awake()
        {
            this.CacheReferences();
            this.SubscribeEvents();
        }

        private void OnDestroy()
        {
            this.UnsubscribeEvents();
        }

        // --- Public APIs ---
        public void HidePanel()
        {
            this.TargetSlot = null;
            this.CurrentMode = CandidateLoadoutSelectionModeType.None;

            _currentCandidates.Clear();

            this.TitleHandler?.HideAll();
            this.CandidateListHandler?.ClearSpawnedSlots();
            this.PaginationHandler?.ResetPagination();

            this.gameObject.SetActive(false);
        }

        public void ShowWeaponCandidates(EquippedLoadoutSlotUIController targetSlot)
        {
            if (targetSlot == null || this.PlayerInventoryController == null)
            {
                this.HidePanel();
                return;
            }

            IReadOnlyList<InventoryItemEntry> weaponEntries = this.PlayerInventoryController.AvailableWeaponEntries;
            this.ShowCandidates(targetSlot, CandidateLoadoutSelectionModeType.ReplaceWeapon, weaponEntries);
        }

        public void ShowProjectileCandidates(EquippedLoadoutSlotUIController targetSlot)
        {
            if (targetSlot == null || this.PlayerInventoryController == null)
            {
                this.HidePanel();
                return;
            }

            IReadOnlyList<InventoryItemEntry> projectileEntries = this.PlayerInventoryController.AvailableProjectileEntries;
            this.ShowCandidates(targetSlot, CandidateLoadoutSelectionModeType.ReplaceProjectile, projectileEntries);
        }

        public void ShowConsumableCandidates(EquippedLoadoutSlotUIController targetSlot)
        {
            if (targetSlot == null || this.PlayerInventoryController == null)
            {
                this.HidePanel();
                return;
            }

            IReadOnlyList<InventoryItemEntry> consumableEntries = this.PlayerInventoryController.AvailableConsumableItemEntries;
            this.ShowCandidates(targetSlot, CandidateLoadoutSelectionModeType.ReplaceConsumableItem, consumableEntries);
        }

        public void ShowCarriableCandidates(EquippedLoadoutSlotUIController targetSlot)
        {
            if (targetSlot == null || this.PlayerInventoryController == null)
            {
                this.HidePanel();
                return;
            }

            IReadOnlyList<InventoryItemEntry> carriableEntries = this.PlayerInventoryController.AvailableCarriableItemEntries;
            this.ShowCandidates(targetSlot, CandidateLoadoutSelectionModeType.ReplaceCarriableItem, carriableEntries);
        }

        public void ShowSpecialSkillCandidates(EquippedLoadoutSlotUIController targetSlot)
        {
            if (targetSlot == null || targetSlot.BoundWeaponEntry == null || this.PlayerInventoryController == null)
            {
                this.HidePanel();
                return;
            }

            IReadOnlyList<SkillProfile> skillCandidates = this.PlayerInventoryController.GetPossibleActiveSpecialSkillCandidates(targetSlot.BoundWeaponEntry);
            this.ShowCandidates(targetSlot, CandidateLoadoutSelectionModeType.ChangeWeaponSpecialSkill, skillCandidates);
        }

        // --- Helpers ---
        //  1. Cache
        private void CacheReferences()
        {
            this.CachePlayerReferences();
            this.CacheSubPanelReferences();
            this.CacheChildHandlers();
        }
        private void CachePlayerReferences()
        {
            GameObject player = GameObject.Find("Player");
            if (player == null) return;

            if (this.PlayerCombatLoadoutController == null) _playerCombatLoadoutController = player.GetComponentInChildren<CharacterCombatLoadoutController>();
            if (this.PlayerInteractionLoadoutController == null) _playerInteractionLoadoutController = player.GetComponentInChildren<PlayerCarryLoadoutController>();
            if (this.PlayerInventoryController == null) _playerInventoryController = player.GetComponentInChildren<InventoryController>();
        }
        private void CacheChildHandlers()
        {
            if (this.TitleHandler == null)          _titleHandler           = this.GetComponentInChildren<CandidateLoadoutSubPanelTitleHandler>(true);
            if (this.CandidateListHandler == null)  _candidateListHandler   = this.GetComponentInChildren<CandidateLoadoutSubPanelCandidateListHandler>(true);
            if (this.PaginationHandler == null)     _paginationHandler      = this.GetComponentInChildren<CandidateLoadoutSubPanelPaginationHandler>(true);
        }
        private void CacheSubPanelReferences()
        {
            if (this.EquippedLoadoutSubPanelController == null && this.transform.parent != null)
            {
                _equippedLoadoutSubPanelController = this.transform.parent.GetComponentInChildren<EquippedLoadoutSubPanelController>(true);
            }
        }
        private void SubscribeEvents()
        {
            if (this.PaginationHandler != null)
            {
                this.PaginationHandler.OnCurrentPageChanged += this.HandleCurrentPageChanged;
            }

            if (this.CandidateListHandler != null)
            {
                this.CandidateListHandler.OnCandidateSlotClicked += this.HandleCandidateSlotClicked;
            }
        }
        private void UnsubscribeEvents()
        {
            if (this.PaginationHandler != null)
            {
                this.PaginationHandler.OnCurrentPageChanged -= this.HandleCurrentPageChanged;
            }

            if (this.CandidateListHandler != null)
            {
                this.CandidateListHandler.OnCandidateSlotClicked -= this.HandleCandidateSlotClicked;
            }
        }

        //  2. Display
        private void ShowCandidates<TCandidate>(EquippedLoadoutSlotUIController targetSlot, CandidateLoadoutSelectionModeType mode, IReadOnlyList<TCandidate> candidates)
        {
            this.TargetSlot = targetSlot;
            this.CurrentMode = mode;

            _currentCandidates.Clear();

            if (candidates != null)
            {
                for (int i = 0; i < candidates.Count; i++)
                {
                    _currentCandidates.Add(candidates[i]);
                }
            }

            this.gameObject.SetActive(true);

            this.TitleHandler?.Refresh(this.CurrentMode);

            if (this.PaginationHandler != null)
            {
                this.PaginationHandler.SetTotalCountAndReset(this.CurrentCandidates.Count);
            }

            this.RefreshCurrentPage();
        }

        private void RefreshCurrentPage()
        {
            if (this.CandidateListHandler == null || this.PaginationHandler == null) return;

            this.CandidateListHandler.RefreshCurrentPage(this.CurrentMode, this.CurrentCandidates, this.PaginationHandler.CurrentPageIndex, this.PaginationHandler.SlotsPerPage);
        }

        private void HandleCurrentPageChanged(int pageNumber)
        {
            this.RefreshCurrentPage();
        }

        //  3. Actions
        private void HandleCandidateSlotClicked(CandidateLoadoutSlotUIController slot)
        {
            if (slot == null) return;

            bool success = this.TryApplyCandidate(slot);
            if (!success) return;

            this.EquippedLoadoutSubPanelController?.RefreshAll();

            if (this.CurrentMode == CandidateLoadoutSelectionModeType.ChangeWeaponSpecialSkill)
            {
                this.RefreshSpecialSkillCandidatesForCurrentTarget();
                return;
            }

            this.HidePanel();
        }

        private bool TryApplyCandidate(CandidateLoadoutSlotUIController slot)
        {
            switch (this.CurrentMode)
            {
                case CandidateLoadoutSelectionModeType.ReplaceWeapon:
                    return this.TryApplyWeaponCandidate(slot.BoundWeaponEntry);

                case CandidateLoadoutSelectionModeType.ReplaceProjectile:
                    return this.TryApplyProjectileCandidate(slot.BoundProjectileEntry);

                case CandidateLoadoutSelectionModeType.ReplaceConsumableItem:
                    return this.TryApplyConsumableCandidate(slot.BoundConsumableEntry);

                case CandidateLoadoutSelectionModeType.ReplaceCarriableItem:
                    return this.TryApplyCarriableCandidate(slot.BoundCarriableItem);

                case CandidateLoadoutSelectionModeType.ChangeWeaponSpecialSkill:
                    return this.TryApplySpecialSkillCandidate(slot.BoundSpecialSkill);
            }

            return false;
        }

        private bool TryApplyWeaponCandidate(InventoryItemEntry weaponEntry)
        {
            if (weaponEntry == null || this.TargetSlot == null || this.PlayerCombatLoadoutController == null) return false;
            if (this.TargetSlot.SlotType != LoadoutPanelSlotType.Weapon) return false;

            return this.PlayerCombatLoadoutController.TryReplaceEquippedWeaponEntry(this.TargetSlot.SlotIndex, weaponEntry, out _);
        }

        private bool TryApplyProjectileCandidate(InventoryItemEntry projectileEntry)
        {
            if (projectileEntry == null || this.TargetSlot == null || this.PlayerCombatLoadoutController == null) return false;
            if (this.TargetSlot.SlotType != LoadoutPanelSlotType.Projectile) return false;

            return this.PlayerCombatLoadoutController.TryReplaceEquippedProjectileEntry(this.TargetSlot.SlotIndex, projectileEntry, out _);
        }

        private bool TryApplyConsumableCandidate(InventoryItemEntry consumableEntry)
        {
            if (consumableEntry == null || this.TargetSlot == null || this.PlayerCombatLoadoutController == null) return false;
            if (this.TargetSlot.SlotType != LoadoutPanelSlotType.Consumable) return false;

            return this.PlayerCombatLoadoutController.TryReplaceEquippedConsumableItemEntry(this.TargetSlot.SlotIndex, consumableEntry, out _);
        }

        private bool TryApplyCarriableCandidate(InventoryItemEntry carriableEntry)
        {
            if (carriableEntry == null || this.TargetSlot == null || this.PlayerInteractionLoadoutController == null) return false;
            if (this.TargetSlot.SlotType != LoadoutPanelSlotType.CarriableItem) return false;

            return this.PlayerInteractionLoadoutController.TryReplaceEquippedCarriableItemEntry(carriableEntry, out _);
        }

        private bool TryApplySpecialSkillCandidate(SkillProfile skill)
        {
            if (skill == null || this.TargetSlot == null || this.PlayerCombatLoadoutController == null) return false;
            if (this.TargetSlot.BoundWeaponEntry == null) return false;

            return this.PlayerCombatLoadoutController.TrySetEquippedWeaponActiveSpecialSkill(this.TargetSlot.BoundWeaponEntry, skill);
        }

        private void RefreshSpecialSkillCandidatesForCurrentTarget()
        {
            if (this.TargetSlot == null || this.TargetSlot.BoundWeaponEntry == null || this.PlayerInventoryController == null)
            {
                this.HidePanel();
                return;
            }

            this.ShowSpecialSkillCandidates(this.TargetSlot);
        }
    }
}