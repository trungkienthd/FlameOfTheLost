using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class InputPromptTriggerController : FOTLMonoBehaviour, 
        IIdentityProvider, 
        IPlayerInteractionPromptSource,
        IPlayerInteractionPromptRefreshNotifier
    {
        // --- States ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _id = 4100;
        [SerializeField] private string _key = "TUTORIAL_PROMPT";
        [SerializeField] private string _displayName = "Tutorial Prompt";
        [SerializeField, TextArea(5, 5)] private string _description = "Tutorial prompt.";
        public int Id => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        //  2. Prompt Settings
        [Header("Prompt Settings")]
        [SerializeField] private int _interactionPromptPriority = 0;
        [SerializeField] private bool _startEnabled = true;
        [SerializeField] private bool _hideAfterCompleted = true;
        public int InteractionPromptPriority => _interactionPromptPriority;
        private bool StartEnabled => _startEnabled;
        private bool HideAfterCompleted => _hideAfterCompleted;

        //  3. Trigger Conditions
        [Header("Trigger Conditions")]
        [SerializeField] private InputPromptTriggerConditionCombinationType _conditionCombinationType = InputPromptTriggerConditionCombinationType.All;
        [SerializeField] private List<InputPromptTriggerConditionEntry> _conditionEntries = new();
        private InputPromptTriggerConditionCombinationType ConditionCombinationType => _conditionCombinationType;
        private IReadOnlyList<InputPromptTriggerConditionEntry> ConditionEntries => _conditionEntries;

        private readonly Dictionary<InputPromptTriggerConditionKey, bool> _conditionStateMap = new();
        private IReadOnlyDictionary<InputPromptTriggerConditionKey, bool> ConditionStateMap => _conditionStateMap;

        //  4. Runtime State
        [Header("Prompt State (Runtime)")]
        [SerializeField] private bool _isPromptEnabled;
        [SerializeField] private bool _isCompleted;
        public bool IsPromptEnabled
        {
            get => _isPromptEnabled;
            private set => _isPromptEnabled = value;
        }
        public bool IsCompleted
        {
            get => _isCompleted;
            private set => _isCompleted = value;
        }

        // --- Events ---
        public event Action PromptRefreshRequested;

        // --- Unity Callbacks ---
        protected override void Awake()
        {
            base.Awake();

            this.IsPromptEnabled = this.StartEnabled;
            this.IsCompleted = false;

            this.InitializeConditionStates();
        }

        // --- Public APIs ---
        //  1. IPlayerInteractionPromptSource implementation
        public bool CanShowInteractionPrompt(GameObject interactor)
        {
            if (!this.gameObject.activeInHierarchy) return false;
            if (!this.IsPromptEnabled) return false;
            if (this.HideAfterCompleted && this.IsCompleted) return false;
            if (!this.AreTriggerConditionsSatisfied()) return false;

            return true;
        }
        public string GetInteractionPromptText(GameObject interactor)
        {
            return this.Description;
        }

        //  2. Enable/Disable Prompt
        public void EnablePrompt()
        {
            if (this.IsPromptEnabled) return;

            this.IsPromptEnabled = true;
            this.RequestPromptRefresh();
        }
        public void DisablePrompt()
        {
            if (!this.IsPromptEnabled) return;

            this.IsPromptEnabled = false;
            this.RequestPromptRefresh();
        }

        //  3. Tutorial Prompt is completed
        public void MarkCompleted()
        {
            if (this.IsCompleted) return;

            this.IsCompleted = true;

            if (this.HideAfterCompleted)
            {
                this.DisablePrompt();
            }
            else
            {
                this.RequestPromptRefresh();
            }
        }

        //  4. Condition APIs
        public void SetConditionState(InputPromptTriggerConditionKey conditionKey, bool value)
        {
            if (conditionKey == InputPromptTriggerConditionKey.None) return;

            bool oldValue = this.GetConditionState(conditionKey);
            if (oldValue == value) return;

            _conditionStateMap[conditionKey] = value;
            this.RequestPromptRefresh();
        }

        public void SatisfyCondition(InputPromptTriggerConditionKey conditionKey)
        {
            this.SetConditionState(conditionKey, true);
        }

        public void UnsatisfyCondition(InputPromptTriggerConditionKey conditionKey)
        {
            this.SetConditionState(conditionKey, false);
        }

        public bool GetConditionState(InputPromptTriggerConditionKey conditionKey)
        {
            if (conditionKey == InputPromptTriggerConditionKey.None) return false;

            return this.ConditionStateMap.TryGetValue(conditionKey, out bool value) && value;
        }

        //  5. Reset
        public void ResetPrompt()
        {
            this.IsCompleted = false;
            this.IsPromptEnabled = this.StartEnabled;

            this.InitializeConditionStates();
            this.RequestPromptRefresh();
        }

        // --- Helpers ---
        private void InitializeConditionStates()
        {
            _conditionStateMap.Clear();

            for (int i = 0; i < this.ConditionEntries.Count; i++)
            {
                InputPromptTriggerConditionEntry entry = this.ConditionEntries[i];
                if (entry == null) continue;
                if (entry.ConditionKey == InputPromptTriggerConditionKey.None) continue;

                _conditionStateMap[entry.ConditionKey] = entry.StartValue;
            }
        }

        private bool AreTriggerConditionsSatisfied()
        {
            if (this.ConditionEntries == null || this.ConditionEntries.Count == 0)
            {
                return true;
            }

            if (this.ConditionCombinationType == InputPromptTriggerConditionCombinationType.Any)
            {
                return this.IsAnyTriggerConditionSatisfied();
            }

            return this.AreAllTriggerConditionsSatisfied();
        }

        private bool AreAllTriggerConditionsSatisfied()
        {
            for (int i = 0; i < this.ConditionEntries.Count; i++)
            {
                InputPromptTriggerConditionEntry entry = this.ConditionEntries[i];
                if (entry == null) continue;
                if (entry.ConditionKey == InputPromptTriggerConditionKey.None) continue;

                bool currentValue = this.GetConditionState(entry.ConditionKey);
                if (currentValue != entry.RequiredValue)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsAnyTriggerConditionSatisfied()
        {
            bool hasValidCondition = false;

            for (int i = 0; i < this.ConditionEntries.Count; i++)
            {
                InputPromptTriggerConditionEntry entry = this.ConditionEntries[i];
                if (entry == null) continue;
                if (entry.ConditionKey == InputPromptTriggerConditionKey.None) continue;

                hasValidCondition = true;

                bool currentValue = this.GetConditionState(entry.ConditionKey);
                if (currentValue == entry.RequiredValue)
                {
                    return true;
                }
            }

            return !hasValidCondition;
        }

        private void RequestPromptRefresh()
        {
            this.PromptRefreshRequested?.Invoke();
        }
    }
}