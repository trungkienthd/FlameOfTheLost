using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    public partial class EquippedLoadoutSubPanelController : FOTLMonoBehaviour
    {
        // --- States ---
        //  1. References
        //      1.1. Refs - Player
        [Header("References (Player)")]
        [SerializeField] private CharacterCombatLoadoutController _playerCombatLoadoutController;
        [SerializeField] private PlayerCarryLoadoutController _playerInteractionLoadoutController;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController => _playerCombatLoadoutController;
        private PlayerCarryLoadoutController PlayerInteractionLoadoutController => _playerInteractionLoadoutController;

        //      1.2. Refs - Other Sub-Panels
        [Header("References (Candidate Loadout Sub-Panel)")]
        [SerializeField] private CandidateLoadoutSubPanelController _candidateLoadoutSubPanelController;
        private CandidateLoadoutSubPanelController CandidateLoadoutSubPanelController => _candidateLoadoutSubPanelController;

        //      1.3. Refs - Child Section Controllers
        [Header("References (Child Sections)")]
        [SerializeField] private EquippedLoadoutSubPanelWeaponListSectionController _weaponListSection;
        [SerializeField] private EquippedLoadoutSubPanelProjectileListSectionController _projectileListSection;
        [SerializeField] private EquippedLoadoutSubPanelConsumableItemListSectionController _consumableItemListSection;
        [SerializeField] private EquippedLoadoutSubPanelCarriableItemSectionController _carriableItemSection;
        private EquippedLoadoutSubPanelWeaponListSectionController WeaponListSection => _weaponListSection;
        private EquippedLoadoutSubPanelProjectileListSectionController ProjectileListSection => _projectileListSection;
        private EquippedLoadoutSubPanelConsumableItemListSectionController ConsumableItemListSection => _consumableItemListSection;
        private EquippedLoadoutSubPanelCarriableItemSectionController CarriableItemSection => _carriableItemSection;

        //      1.4. Refs - Tooltip
        [Header("References (Tooltip)")]
        [SerializeField] private EquippedLoadoutSlotActionTooltipController _slotActionTooltip;
        [SerializeField] private RectTransform _tooltipParent;
        [SerializeField] private Vector2 _tooltipOffset = new Vector2(140f, 0f);
        private EquippedLoadoutSlotActionTooltipController SlotActionTooltip => _slotActionTooltip;
        private RectTransform TooltipParent => _tooltipParent;
        private Vector2 TooltipOffset => _tooltipOffset;

        //  2. Runtime State
        //      2.1. Selected Slot
        private EquippedLoadoutSlotController _selectedSlot;
        public EquippedLoadoutSlotController SelectedSlot
        {
            get => _selectedSlot;
            private set => _selectedSlot = value;
        }

        //      2.2. Cached Pointer Position (for tooltip positioning)
        private Vector2 _lastPointerScreenPosition;
        private Vector2 LastPointerScreenPosition
        {
            get => _lastPointerScreenPosition;
            set => _lastPointerScreenPosition = value;
        }

        //  3. 
        private bool _hasInitializedPregameplayState;
        private bool HasInitializedPregameplayState
        {
            get => _hasInitializedPregameplayState;
            set => _hasInitializedPregameplayState = value;
        }

        // --- Unity Callbacks ---
        private void OnEnable()
        {
            if (!this.HasInitializedPregameplayState) return;

            this.ClearSelection();
        }

        // --- Public APIs ---
        public void RefreshAll()
        {
            this.WeaponListSection?.Refresh(this.SelectedSlot);
            this.ProjectileListSection?.Refresh(this.SelectedSlot);
            this.ConsumableItemListSection?.Refresh(this.SelectedSlot);
            this.CarriableItemSection?.Refresh(this.SelectedSlot);
        }

        public void ClearSelection()
        {
            this.SelectedSlot = null;
            this.HideTooltip();
            this.CandidateLoadoutSubPanelController?.HidePanel();
            this.RefreshAll();
        }

        // --- Helpers ---
        private void BindTooltipEvents()
        {
            if (this.SlotActionTooltip == null) return;

            this.SlotActionTooltip.OnUnequipClicked -= this.HandleTooltipUnequipClicked;
            this.SlotActionTooltip.OnUnequipClicked += this.HandleTooltipUnequipClicked;

            this.SlotActionTooltip.OnReplaceClicked -= this.HandleTooltipReplaceClicked;
            this.SlotActionTooltip.OnReplaceClicked += this.HandleTooltipReplaceClicked;

            this.SlotActionTooltip.OnChangeSkillClicked -= this.HandleTooltipChangeSkillClicked;
            this.SlotActionTooltip.OnChangeSkillClicked += this.HandleTooltipChangeSkillClicked;
        }
        private void UnbindTooltipEvents()
        {
            if (this.SlotActionTooltip == null) return;

            this.SlotActionTooltip.OnUnequipClicked -= this.HandleTooltipUnequipClicked;
            this.SlotActionTooltip.OnReplaceClicked -= this.HandleTooltipReplaceClicked;
            this.SlotActionTooltip.OnChangeSkillClicked -= this.HandleTooltipChangeSkillClicked;
        }

        private void HandleSlotClicked(EquippedLoadoutSlotController slot)
        {
            if (slot == null) return;

            this.LastPointerScreenPosition = this.GetCurrentPointerScreenPosition();

            if (this.SelectedSlot == slot)
            {
                this.SelectedSlot = null;
                this.HideTooltip();
                this.HideCandidatePanel();
                this.RefreshAll();
                return;
            }

            this.SelectedSlot = slot;
            this.HideCandidatePanel();

            // Refresh first so selection visuals are up to date before the tooltip is positioned/shown.
            this.RefreshAll();

            this.ShowTooltipForSelectedSlot();
        }

        private void HandleWeaponSubSlotClicked(EquippedLoadoutSlotController slot)
        {
            if (slot == null || slot.BoundWeaponEntry == null)
            {
                this.HideCandidatePanel();
                return;
            }

            this.LastPointerScreenPosition = this.GetCurrentPointerScreenPosition();

            this.SelectedSlot = slot;
            this.HideTooltip();
            this.RefreshAll();

            this.CandidateLoadoutSubPanelController?.ShowSpecialSkillCandidates(slot);
        }


        private void ShowTooltipForSelectedSlot()
        {
            if (this.SlotActionTooltip == null) return;

            if (this.SelectedSlot == null)
            {
                this.HideTooltip();
                return;
            }

            RectTransform parentRect = this.TooltipParent != null
                ? this.TooltipParent
                : this.transform.parent as RectTransform;

            if (parentRect == null)
            {
                this.HideTooltip();
                return;
            }

            bool canChangeSkill =
                this.SelectedSlot.SlotType == LoadoutPanelSlotType.Weapon &&
                this.SelectedSlot.BoundWeaponEntry != null &&
                this.SelectedSlot.BoundWeaponEntry.Item != null;
            this.SlotActionTooltip.ShowForSlotAtScreenPosition
            (
                slot: this.SelectedSlot,
                tooltipParent: parentRect,
                screenPosition: this.LastPointerScreenPosition,
                offset: this.TooltipOffset,
                changeSkillBtnIsActive: canChangeSkill
            );
        }

        private void HideTooltip()
        {
            if (this.SlotActionTooltip == null) return;
            this.SlotActionTooltip.Hide();
        }

        private void HideCandidatePanel()
        {
            if (this.CandidateLoadoutSubPanelController == null) return;
            this.CandidateLoadoutSubPanelController.HidePanel();
        }

        private Vector2 GetCurrentPointerScreenPosition()
        {
            if (Mouse.current != null)
            {
                return Mouse.current.position.ReadValue();
            }

            return Input.mousePosition;
        }

        private void HandleTooltipUnequipClicked(EquippedLoadoutSlotController slot)
        {
            if (slot == null) return;

            bool success = slot.SlotType switch
            {
                LoadoutPanelSlotType.Weapon => this.TryUnequipWeapon(slot),
                LoadoutPanelSlotType.Projectile => this.TryUnequipProjectile(slot),
                LoadoutPanelSlotType.Consumable => this.TryUnequipConsumable(slot),
                LoadoutPanelSlotType.CarriableItem => this.TryUnequipCarriable(slot),
                _ => false
            };

            if (!success) return;

            this.ClearSelection();
            this.HideCandidatePanel();
        }

        private bool TryUnequipWeapon(EquippedLoadoutSlotController slot)
        {
            if (slot == null || this.PlayerCombatLoadoutController == null) return false;

            // Replace this with your real API name if needed.
            return this.PlayerCombatLoadoutController.TryUnequipWeaponEntry(slot.BoundWeaponEntry);
        }

        private bool TryUnequipProjectile(EquippedLoadoutSlotController slot)
        {
            if (slot == null || this.PlayerCombatLoadoutController == null) return false;

            return this.PlayerCombatLoadoutController.TryUnequipEquippedProjectileEntry(slot.BoundProjectileEntry);
        }

        private bool TryUnequipConsumable(EquippedLoadoutSlotController slot)
        {
            if (slot == null || this.PlayerCombatLoadoutController == null) return false;

            // Replace this with your real API name if needed.
            return this.PlayerCombatLoadoutController.TryUnequipEquippedConsumableItemEntry(slot.BoundConsumableEntry);
        }

        private bool TryUnequipCarriable(EquippedLoadoutSlotController slot)
        {
            if (slot == null || this.PlayerInteractionLoadoutController == null) return false;

            // Replace this with your real API name if needed.
            return this.PlayerInteractionLoadoutController.TryUnequipCarriableEntry();
        }

        private void HandleTooltipReplaceClicked(EquippedLoadoutSlotController slot)
        {
            if (slot == null || this.CandidateLoadoutSubPanelController == null) return;

            this.SelectedSlot = slot;
            this.HideTooltip();
            this.RefreshAll();

            switch (slot.SlotType)
            {
                case LoadoutPanelSlotType.Weapon:
                    this.CandidateLoadoutSubPanelController.ShowWeaponCandidates(slot);
                    break;

                case LoadoutPanelSlotType.Projectile:
                    this.CandidateLoadoutSubPanelController.ShowProjectileCandidates(slot);
                    break;

                case LoadoutPanelSlotType.Consumable:
                    this.CandidateLoadoutSubPanelController.ShowConsumableCandidates(slot);
                    break;

                case LoadoutPanelSlotType.CarriableItem:
                    this.CandidateLoadoutSubPanelController.ShowCarriableCandidates(slot);
                    break;
            }
        }

        private void HandleTooltipChangeSkillClicked(EquippedLoadoutSlotController slot)
        {
            if (slot == null || slot.BoundWeaponEntry == null) return;

            this.SelectedSlot = slot;
            this.HideTooltip();
            this.RefreshAll();

            this.CandidateLoadoutSubPanelController?.ShowSpecialSkillCandidates(slot);
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.WeaponListSection == null) _weaponListSection = this.GetComponentInChildren<EquippedLoadoutSubPanelWeaponListSectionController>(true);
            if (this.ProjectileListSection == null) _projectileListSection = this.GetComponentInChildren<EquippedLoadoutSubPanelProjectileListSectionController>(true);
            if (this.ConsumableItemListSection == null) _consumableItemListSection = this.GetComponentInChildren<EquippedLoadoutSubPanelConsumableItemListSectionController>(true);
            if (this.CarriableItemSection == null) _carriableItemSection = this.GetComponentInChildren<EquippedLoadoutSubPanelCarriableItemSectionController>(true);

            if (this.SlotActionTooltip == null)
            {
                _slotActionTooltip = this.GetComponentInChildren<EquippedLoadoutSlotActionTooltipController>(true);
            }

            if (this.CandidateLoadoutSubPanelController == null && this.transform.parent != null)
            {
                _candidateLoadoutSubPanelController = this.transform.parent.GetComponentInChildren<CandidateLoadoutSubPanelController>(true);
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.BindSectionEvents();
            this.BindTooltipEvents();
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.SelectedSlot = null;
            this.LastPointerScreenPosition = Vector2.zero;

            this.HideTooltip();
            this.HideCandidatePanel();
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                if (this.PlayerCombatLoadoutController == null) _playerCombatLoadoutController = player.GetComponent<CharacterCombatLoadoutController>();
                if (this.PlayerInteractionLoadoutController == null) _playerInteractionLoadoutController = player.GetComponent<PlayerCarryLoadoutController>();
            }
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            this.InitializeSections();

            this.HasInitializedPregameplayState = true;

            this.ClearSelection();
        }
        protected override void UnbindInternalReferences()
        {
            base.UnbindInternalReferences();

            this.UnbindSectionEvents();
            this.UnbindTooltipEvents();
        }

        private void InitializeSections()
        {
            this.WeaponListSection?.Initialize(this.PlayerCombatLoadoutController);
            this.ProjectileListSection?.Initialize(this.PlayerCombatLoadoutController);
            this.ConsumableItemListSection?.Initialize(this.PlayerCombatLoadoutController);
            this.CarriableItemSection?.Initialize(this.PlayerInteractionLoadoutController);
        }

        private void BindSectionEvents()
        {
            if (this.WeaponListSection != null)
            {
                this.WeaponListSection.OnSlotClicked -= this.HandleSlotClicked;
                this.WeaponListSection.OnSlotClicked += this.HandleSlotClicked;

                this.WeaponListSection.OnSubSlotClicked -= this.HandleWeaponSubSlotClicked;
                this.WeaponListSection.OnSubSlotClicked += this.HandleWeaponSubSlotClicked;
            }

            if (this.ProjectileListSection != null)
            {
                this.ProjectileListSection.OnSlotClicked -= this.HandleSlotClicked;
                this.ProjectileListSection.OnSlotClicked += this.HandleSlotClicked;
            }

            if (this.ConsumableItemListSection != null)
            {
                this.ConsumableItemListSection.OnSlotClicked -= this.HandleSlotClicked;
                this.ConsumableItemListSection.OnSlotClicked += this.HandleSlotClicked;
            }

            if (this.CarriableItemSection != null)
            {
                this.CarriableItemSection.OnSlotClicked -= this.HandleSlotClicked;
                this.CarriableItemSection.OnSlotClicked += this.HandleSlotClicked;
            }
        }
        private void UnbindSectionEvents()
        {
            if (this.WeaponListSection != null)
            {
                this.WeaponListSection.OnSlotClicked -= this.HandleSlotClicked;
                this.WeaponListSection.OnSubSlotClicked -= this.HandleWeaponSubSlotClicked;
            }

            if (this.ProjectileListSection != null)
            {
                this.ProjectileListSection.OnSlotClicked -= this.HandleSlotClicked;
            }

            if (this.ConsumableItemListSection != null)
            {
                this.ConsumableItemListSection.OnSlotClicked -= this.HandleSlotClicked;
            }

            if (this.CarriableItemSection != null)
            {
                this.CarriableItemSection.OnSlotClicked -= this.HandleSlotClicked;
            }
        }
    }
}
