using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "KeyItmItr_New", menuName = "Data/FlameOfTheLost/World/Environment/Interactable/KeyItemInteractable")]
    public class KeyItemInteractableProfile : FOTLWorldObjectProfile
    {
        // --- Data / Configs ---
        //  1. KeyItem Requirements
        [Header("KeyItem Requirements")]
        [SerializeField] private KeyItemProfile _requiredKeyItem;
        [SerializeField] private bool _consumeRequiredKeyItem = true;
        public KeyItemProfile RequiredKeyItem => _requiredKeyItem;
        public bool ConsumeRequiredKeyItem => _consumeRequiredKeyItem;

        //  2. Interaction Requirements
        [Header("Interaction Requirements")]
        [SerializeField] private GameConditionCombinationType _interactionRequirementCombinationType = GameConditionCombinationType.All;
        [SerializeField] private List<GameConditionRequirementEntry> _interactionRequirements = new();
        public GameConditionCombinationType InteractionRequirementCombinationType => _interactionRequirementCombinationType;
        public IReadOnlyList<GameConditionRequirementEntry> InteractionRequirements => _interactionRequirements;

        //  3. Interaction Configuration
        [Header("Interaction Configuration")]
        [SerializeField] private PlayerInteractionType _interactionType = PlayerInteractionType.Hold;
        [SerializeField, Min(0.1f)] private float _holdDuration = 3f;
        public PlayerInteractionType InteractionType => _interactionType;
        public float HoldDuration => _holdDuration;

        //  4. Interaction Outcomes
        [Header("Interaction Outcomes")]
        [SerializeField] private List<GameConditionOperationEntry> _conditionOperations = new();
        public IReadOnlyList<GameConditionOperationEntry> ConditionOperations => _conditionOperations;

        //  5. Completion State
        [Header("Completion State")]
        [SerializeField] private GameConditionCombinationType _completionConditionCombinationType = GameConditionCombinationType.All;
        [SerializeField] private List<GameConditionRequirementEntry> _completionConditions = new();
        [SerializeField] private bool _disableInteractionWhenCompleted = true;
        public GameConditionCombinationType CompletionConditionCombinationType => _completionConditionCombinationType;
        public IReadOnlyList<GameConditionRequirementEntry> CompletionConditions => _completionConditions;
        public bool DisableInteractionWhenCompleted => _disableInteractionWhenCompleted;

        //  6. Completion Visual
        [Header("Completion Visual")]
        [SerializeField] private KeyItemInteractableCompletionVisualType _completionVisualType = KeyItemInteractableCompletionVisualType.Preserve;
        [SerializeField] private Sprite _completedSprite;
        public KeyItemInteractableCompletionVisualType CompletionVisualType => _completionVisualType;
        public Sprite CompletedSprite => _completedSprite;

        //  7. Interaction Prompt
        [Header("Interaction Prompt")]
        [SerializeField] private int _interactionPromptPriority = 100;
        [SerializeField] private string _interactionPromptText = "Interact";
        [SerializeField] private string _requirementsNotSatisfiedPromptText = "Interaction unavailable";
        [SerializeField] private string _completedPromptText = string.Empty;
        public int InteractionPromptPriority => _interactionPromptPriority;
        public string InteractionPromptText => string.IsNullOrWhiteSpace(_interactionPromptText)
            ? "Interact"
            : _interactionPromptText;
        public string RequirementsNotSatisfiedPromptText => _requirementsNotSatisfiedPromptText;
        public string CompletedPromptText => _completedPromptText;
    }
}