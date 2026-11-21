using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    public partial class EquippedLoadoutSubPanelController : LoadoutPanelSubBehaviourBase
    {
        // --- References ---
        //  1. Refs - Player
        [Header("References (Player)")]
        [SerializeField] private CharacterCombatLoadoutController _playerCombatLoadoutController;
        [SerializeField] private PlayerCarryLoadoutController _playerCarryLoadoutController;
        internal CharacterCombatLoadoutController PlayerCombatLoadoutController => _playerCombatLoadoutController;
        internal PlayerCarryLoadoutController PlayerCarryLoadoutController => _playerCarryLoadoutController;

        //  2. Refs - Sub-Behaviours
        [Header("References (Sub-Behaviours)")]
        [SerializeField] private EquippedLoadoutSubPanelWeaponListSectionController _weaponListSection;
        [SerializeField] private EquippedLoadoutSubPanelProjectileListSectionController _projectileListSection;
        [SerializeField] private EquippedLoadoutSubPanelConsumableItemListSectionController _consumableItemListSection;
        [SerializeField] private EquippedLoadoutSubPanelCarriableItemSectionController _carriableItemSection;
        [SerializeField] private EquippedLoadoutSlotActionTooltipController _slotActionTooltip;
        private EquippedLoadoutSubPanelWeaponListSectionController WeaponListSection => _weaponListSection;
        private EquippedLoadoutSubPanelProjectileListSectionController ProjectileListSection => _projectileListSection;
        private EquippedLoadoutSubPanelConsumableItemListSectionController ConsumableItemListSection => _consumableItemListSection;
        private EquippedLoadoutSubPanelCarriableItemSectionController CarriableItemSection => _carriableItemSection;
        private EquippedLoadoutSlotActionTooltipController SlotActionTooltip => _slotActionTooltip;

        // --- Configs ---
        //  1. Tooltip
        [Header("Configs (Tooltip)")]
        [SerializeField] private RectTransform _tooltipParent;
        [SerializeField] private Vector2 _tooltipOffset = new Vector2(140f, 0f);
        private RectTransform TooltipParent => _tooltipParent;
        private Vector2 TooltipOffset => _tooltipOffset;

        // --- Runtime ---
        //  1. Selected Slot
        private EquippedLoadoutSlotController _selectedSlot;
        public EquippedLoadoutSlotController SelectedSlot
        {
            get => _selectedSlot;
            private set => _selectedSlot = value;
        }

        //  2. Cached Pointer Position (for tooltip positioning)
        private Vector2 _lastPointerScreenPosition;
        private Vector2 LastPointerScreenPosition
        {
            get => _lastPointerScreenPosition;
            set => _lastPointerScreenPosition = value;
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
            this.HideCandidatePanel();
            this.RefreshAll();
        }

        // --- Helpers ---
        //  0. Others 
        private void HideCandidatePanel()
        {
            this.LoadoutPanelController?.CandidateLoadoutSubPanelController?.HidePanel();
        }
        private Vector2 GetCurrentPointerScreenPosition()
        {
            if (Mouse.current != null) return Mouse.current.position.ReadValue();

            return Input.mousePosition;
        }

        //  1. Selection
        private void HandleSlotClicked(EquippedLoadoutSlotController slot)
        {
            if (slot == null) return;

            this.LastPointerScreenPosition = this.GetCurrentPointerScreenPosition();

            if (this.SelectedSlot == slot)
            {
                this.ClearSelection();

                return;
            }

            this.SelectedSlot = slot;

            this.HideCandidatePanel();            
            this.RefreshAll();                      // Refresh first so selection visuals are up to date before the tooltip is positioned/shown.
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

            this.LoadoutPanelController?.CandidateLoadoutSubPanelController?.ShowSpecialSkillCandidates(slot);
        }

        //  2. Tooltip
        //      2.1. Tooltip Display
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

            bool canChangeSkill =   this.SelectedSlot.SlotType == LoadoutPanelSlotType.Weapon   &&
                                    this.SelectedSlot.BoundWeaponEntry != null                  &&
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

        //      2.2. Tooltip Actions
        //          2.2.1. Unequip
        private void HandleTooltipUnequipClicked(EquippedLoadoutSlotController slot)
        {
            if (slot == null) return;

            bool success = slot.SlotType switch
            {
                LoadoutPanelSlotType.Weapon         => this.TryUnequipWeapon(slot),
                LoadoutPanelSlotType.Projectile     => this.TryUnequipProjectile(slot),
                LoadoutPanelSlotType.Consumable     => this.TryUnequipConsumable(slot),
                LoadoutPanelSlotType.CarriableItem  => this.TryUnequipCarriable(slot),
                _ => false
            };

            if (!success) return;

            this.ClearSelection();
            //this.HideCandidatePanel();
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

            return this.PlayerCombatLoadoutController.TryUnequipEquippedConsumableItemEntry(slot.BoundConsumableEntry);
        }
        private bool TryUnequipCarriable(EquippedLoadoutSlotController slot)
        {
            if (slot == null || this.PlayerCarryLoadoutController == null) return false;

            return this.PlayerCarryLoadoutController.TryUnequipCarriableEntry();
        }

        //          2.2.2. Replace
        private void HandleTooltipReplaceClicked(EquippedLoadoutSlotController slot)
        {
            if (slot == null) return;

            CandidateLoadoutSubPanelController candidatePanel =this.LoadoutPanelController?.CandidateLoadoutSubPanelController;

            this.SelectedSlot = slot;

            this.HideTooltip();
            this.RefreshAll();

            switch (slot.SlotType)
            {
                case LoadoutPanelSlotType.Weapon:
                    candidatePanel.ShowWeaponCandidates(slot);
                    break;

                case LoadoutPanelSlotType.Projectile:
                    candidatePanel.ShowProjectileCandidates(slot);
                    break;

                case LoadoutPanelSlotType.Consumable:
                    candidatePanel.ShowConsumableCandidates(slot);
                    break;

                case LoadoutPanelSlotType.CarriableItem:
                    candidatePanel.ShowCarriableCandidates(slot);
                    break;
            }
        }
        private void HandleTooltipChangeSkillClicked(EquippedLoadoutSlotController slot)
        {
            if (slot == null || slot.BoundWeaponEntry == null) return;

            this.SelectedSlot = slot;

            this.HideTooltip();
            this.RefreshAll();

            this.LoadoutPanelController?.CandidateLoadoutSubPanelController?.ShowSpecialSkillCandidates(slot);
        }

        //  4. Event Binding
        //      4.1. Sections
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

        //      4.2. Tooltip
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


        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.WeaponListSection          == null) _weaponListSection         = this.GetComponentInChildren<EquippedLoadoutSubPanelWeaponListSectionController>(true);
            if (this.ProjectileListSection      == null) _projectileListSection     = this.GetComponentInChildren<EquippedLoadoutSubPanelProjectileListSectionController>(true);
            if (this.ConsumableItemListSection  == null) _consumableItemListSection = this.GetComponentInChildren<EquippedLoadoutSubPanelConsumableItemListSectionController>(true);
            if (this.CarriableItemSection       == null) _carriableItemSection      = this.GetComponentInChildren<EquippedLoadoutSubPanelCarriableItemSectionController>(true);
            if (this.SlotActionTooltip          == null) _slotActionTooltip         = this.GetComponentInChildren<EquippedLoadoutSlotActionTooltipController>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.WeaponListSection          == null) Debug.LogError($"{nameof(EquippedLoadoutSubPanelController)} requires a " + $"{nameof(EquippedLoadoutSubPanelWeaponListSectionController)}.", this);
            if (this.ProjectileListSection      == null) Debug.LogError($"{nameof(EquippedLoadoutSubPanelController)} requires a " + $"{nameof(EquippedLoadoutSubPanelProjectileListSectionController)}.", this);
            if (this.ConsumableItemListSection  == null) Debug.LogError($"{nameof(EquippedLoadoutSubPanelController)} requires a " + $"{nameof(EquippedLoadoutSubPanelConsumableItemListSectionController)}.", this);
            if (this.CarriableItemSection       == null) Debug.LogError($"{nameof(EquippedLoadoutSubPanelController)} requires a " + $"{nameof(EquippedLoadoutSubPanelCarriableItemSectionController)}.", this);
            if (this.SlotActionTooltip          == null) Debug.LogError($"{nameof(EquippedLoadoutSubPanelController)} requires an " + $"{nameof(EquippedLoadoutSlotActionTooltipController)}.", this);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.WeaponListSection?.Bind(this);
            this.ProjectileListSection?.Bind(this);
            this.ConsumableItemListSection?.Bind(this);
            this.CarriableItemSection?.Bind(this);
            this.SlotActionTooltip?.Bind(this);

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
                if (this.PlayerCombatLoadoutController  == null) _playerCombatLoadoutController = player.GetComponent<CharacterCombatLoadoutController>();
                if (this.PlayerCarryLoadoutController   == null) _playerCarryLoadoutController  = player.GetComponent<PlayerCarryLoadoutController>();
            }
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.PlayerCombatLoadoutController  == null) Debug.LogError($"{nameof(EquippedLoadoutSubPanelController)} requires a " + $"{nameof(CharacterCombatLoadoutController)}.", this);
            if (this.PlayerCarryLoadoutController   == null) Debug.LogError($"{nameof(EquippedLoadoutSubPanelController)} requires a " + $"{nameof(PlayerCarryLoadoutController)}.", this);
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            this.ClearSelection();
        }
        protected override void UnbindInternalReferences()
        {
            this.UnbindSectionEvents();
            this.UnbindTooltipEvents();

            this.WeaponListSection?.Unbind(this);
            this.ProjectileListSection?.Unbind(this);
            this.ConsumableItemListSection?.Unbind(this);
            this.CarriableItemSection?.Unbind(this);
            this.SlotActionTooltip?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
