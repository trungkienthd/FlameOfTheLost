using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public partial class EquippedLoadoutSubPanelController : MonoBehaviour
    {
        // --- States ---
        //  1. References
        //      1.1. Refs - Weapons Section
        [Header("References (Weapons Section)")]
        [SerializeField] private List<EquippedLoadoutSlotUIController> _weaponSlots = new List<EquippedLoadoutSlotUIController>(4);
        private IReadOnlyList<EquippedLoadoutSlotUIController> WeaponSlots => _weaponSlots;

        //      1.2. Refs - Consumables Section
        [Header("References (Consumables Section)")]
        [SerializeField] private List<EquippedLoadoutSlotUIController> _consumableSlots = new List<EquippedLoadoutSlotUIController>(8);
        private IReadOnlyList<EquippedLoadoutSlotUIController> ConsumableItemSlots => _consumableSlots;

        //      1.3. Refs - Interactable Section
        [Header("References (Interactable Section)")]
        [SerializeField] private EquippedLoadoutSlotUIController _interactableSlot;
        private EquippedLoadoutSlotUIController InteractableItemSlot => _interactableSlot;

        //      1.4. Refs - Player
        [Header("References (Player)")]
        [SerializeField] private CharacterCombatLoadoutController _playerCombatLoadoutController;
        [SerializeField] private PlayerInteractionLoadoutController _playerInteractionLoadoutController;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController => _playerCombatLoadoutController;
        private PlayerInteractionLoadoutController PlayerInteractionLoadoutController => _playerInteractionLoadoutController;

        //      1.5. Refs - Candidate Loadout Sub-Panel
        [Header("References (Candidate Loadout Sub-Panel)")]
        [SerializeField] private CandidateLoadoutSubPanelController _candidateLoadoutSubPanelController;
        private CandidateLoadoutSubPanelController CandidateLoadoutSubPanelController => _candidateLoadoutSubPanelController;

        //      1.6. Refs - Tooltip
        [Header("References (Tooltip)")]
        [SerializeField] private EquippedLoadoutSlotActionTooltipController _slotActionTooltip;
        [SerializeField] private RectTransform _tooltipParent;
        [SerializeField] private Vector2 _tooltipOffset = new Vector2(140f, 0f);
        private EquippedLoadoutSlotActionTooltipController SlotActionTooltip => _slotActionTooltip;
        private RectTransform TooltipParent => _tooltipParent;
        private Vector2 TooltipOffset => _tooltipOffset;

        //  2. Selected Slot State
        private EquippedLoadoutSlotUIController _selectedSlot;
        public EquippedLoadoutSlotUIController SelectedSlot
        {
            get => _selectedSlot;
            private set => _selectedSlot = value;
        }

        private void Awake()
        {
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                if (this.PlayerCombatLoadoutController == null) _playerCombatLoadoutController = player.GetComponent<CharacterCombatLoadoutController>();
                if (this.PlayerInteractionLoadoutController == null) _playerInteractionLoadoutController = player.GetComponent<PlayerInteractionLoadoutController>();
            }

            if (this.CandidateLoadoutSubPanelController == null) _candidateLoadoutSubPanelController = this.transform.parent.GetComponentInChildren<CandidateLoadoutSubPanelController>();

            this.BindSlotEvents();
            this.BindTooltipEvents();

            this.RefreshAll();
            this.HideTooltip();
        }
        private void OnEnable()
        {
            this.RefreshAll();
            this.HideTooltip();
        }
        private void OnDestroy()
        {
            this.UnbindSlotEvents();
            this.UnbindTooltipEvents();
        }

        public void ClearSelection()
        {
            this.SelectedSlot = null;
            this.HideTooltip();
            this.CandidateLoadoutSubPanelController?.HidePanel();
            this.RefreshAll();
        }

        private void BindSlotEvents()
        {
            foreach (EquippedLoadoutSlotUIController slot in this.WeaponSlots)
            {
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
                slot.OnClicked += this.HandleSlotClicked;

                slot.OnSubSlotClicked -= this.HandleWeaponSubSlotClicked;
                slot.OnSubSlotClicked += this.HandleWeaponSubSlotClicked;
            }

            foreach (EquippedLoadoutSlotUIController slot in this.ConsumableItemSlots)
            {
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
                slot.OnClicked += this.HandleSlotClicked;
            }

            if (this.InteractableItemSlot != null)
            {
                this.InteractableItemSlot.OnClicked -= this.HandleSlotClicked;
                this.InteractableItemSlot.OnClicked += this.HandleSlotClicked;
            }
        }

        private void UnbindSlotEvents()
        {
            foreach (EquippedLoadoutSlotUIController slot in this.WeaponSlots)
            {
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
                slot.OnSubSlotClicked -= this.HandleWeaponSubSlotClicked;
            }

            foreach (EquippedLoadoutSlotUIController slot in this.ConsumableItemSlots)
            {
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
            }

            if (this.InteractableItemSlot != null)
            {
                this.InteractableItemSlot.OnClicked -= this.HandleSlotClicked;
            }
        }

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
    }
}
