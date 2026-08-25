using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "TutPTrg_New", menuName = "Data/FlameOfTheLost/Level/Tutorial/TutorialPromptTriggerProfile")]
    public class TutorialPromptTriggerProfile : FOTLWorldObjectProfile
    {
        // --- Data / Configs ---
        //  1. Prompt Settings
        [Header("Prompt Settings")]
        [SerializeField] private int _interactionPromptPriority = 0;
        [SerializeField] private bool _startEnabled = true;
        [SerializeField] private bool _hideAfterCompleted = true;
        public int InteractionPromptPriority => _interactionPromptPriority;
        public bool StartEnabled => _startEnabled;
        public bool HideAfterCompleted => _hideAfterCompleted;

        //  2. Interaction Trigger Area
        [Header("Interaction Trigger Area")]
        [SerializeField] private ColliderConfigurationEntry _interactionTriggerColliderConfiguration = new();
        public ColliderConfigurationEntry InteractionTriggerColliderConfiguration => _interactionTriggerColliderConfiguration;

        //  2. Trigger Conditions
        [Header("Trigger Conditions")]
        [SerializeField] private TutorialPromptTriggerConditionCombinationType _conditionCombinationType = TutorialPromptTriggerConditionCombinationType.All;
        [SerializeField] private List<TutorialPromptTriggerConditionEntry> _conditionEntries = new();
        public TutorialPromptTriggerConditionCombinationType ConditionCombinationType => _conditionCombinationType;
        public IReadOnlyList<TutorialPromptTriggerConditionEntry> ConditionEntries => _conditionEntries;
    }
}
