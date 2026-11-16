
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "DoorSwc_New", menuName = "Data/FlameOfTheLost/World/Environment/Interactable/Door/EnvironmentDoorSwitchProfile")]
    public class EnvironmentDoorSwitchProfile : FOTLWorldObjectProfile
    {
        // --- Data / Configs ---
        //  1. Interaction
        [Header("Interaction")]
        [SerializeField] private bool _initiallyInteractionEnabled = true;
        [SerializeField] private List<EnvironmentDoorGatePositionState> _allowedInteractionStates = new()
        {
            EnvironmentDoorGatePositionState.Outside,
            EnvironmentDoorGatePositionState.OppositeSide
        };

        public bool InitiallyInteractionEnabled => _initiallyInteractionEnabled;
        public IReadOnlyList<EnvironmentDoorGatePositionState> AllowedInteractionStates => _allowedInteractionStates;

        //  2. Interaction Prompt
        [Header("Interaction Prompt")]
        [SerializeField] private int _interactionPromptPriority = 0;
        [SerializeField] private string _openPromptText = "Open Door";
        [SerializeField] private string _closePromptText = "Close Door";

        public int InteractionPromptPriority => _interactionPromptPriority;

        public string OpenPromptText => _openPromptText;
        public string ClosePromptText => _closePromptText;

        //  3. Visual
        [Header("Door Switch Visual")]
        [SerializeField] private Sprite _openSprite;

        public Sprite ClosedSprite => this.Icon;
        public Sprite OpenSprite => _openSprite;

        // --- Public APIs ---
        //  1. Interaction
        public bool IsInteractionStateAllowed(EnvironmentDoorGatePositionState state)
        {
            return _allowedInteractionStates != null && _allowedInteractionStates.Contains(state);
        }

        //  2. Visual
        public Sprite GetSprite(bool isOpen)
        {
            return isOpen ? this.OpenSprite : this.ClosedSprite;
        }
    }
}
