using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "TutPTrg_New", menuName = "Data/FlameOfTheLost/Level/Tutorial/TutorialPromptTriggerProfile")]
    public class TutorialPromptTriggerProfile : ScriptableObject,
        IIdentityProvider,
        IIconProvider
    {
        // --- Data / Configs ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _id = 41000;
        [SerializeField] private string _key;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(5, 5)] private string _description;
        public int Id => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        //  2. Display
        [Header("Display")]
        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;

        //  3. Prompt Settings
        [Header("Prompt Settings")]
        [SerializeField] private int _interactionPromptPriority = 0;
        [SerializeField] private bool _startEnabled = true;
        [SerializeField] private bool _hideAfterCompleted = true;
        public int InteractionPromptPriority => _interactionPromptPriority;
        public bool StartEnabled => _startEnabled;
        public bool HideAfterCompleted => _hideAfterCompleted;

        //  4. Trigger Conditions
        [Header("Trigger Conditions")]
        [SerializeField] private TutorialPromptTriggerConditionCombinationType _conditionCombinationType = TutorialPromptTriggerConditionCombinationType.All;
        [SerializeField] private List<TutorialPromptTriggerConditionEntry> _conditionEntries = new();
        public TutorialPromptTriggerConditionCombinationType ConditionCombinationType => _conditionCombinationType;
        public IReadOnlyList<TutorialPromptTriggerConditionEntry> ConditionEntries => _conditionEntries;
    }
}
