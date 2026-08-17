using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PromptTriggerController : FOTLMonoBehaviour,
        IPlayerInteractionPromptSource,
        IPlayerInteractionPromptRefreshNotifier
    {
        // --- Data / Configs ---
        [Header("Data / Configs")]

        //  1. Profile
        [SerializeField] private PromptTriggerProfile _profile;
        private PromptTriggerProfile Profile => _profile;

        //  2. Identity
        public string Description => this.Profile != null ? this.Profile.Description : "";

        //  3. Prompt Settings
        public int InteractionPromptPriority => this.Profile != null ? this.Profile.InteractionPromptPriority : 0;
        private bool StartEnabled => this.Profile != null ? this.Profile.StartEnabled : true;
        private bool HideAfterCompleted => this.Profile != null ? this.Profile.HideAfterCompleted : true;

        //  4. Trigger Conditions
        private PromptTriggerConditionCombinationType ConditionCombinationType => this.Profile != null ? this.Profile.ConditionCombinationType : PromptTriggerConditionCombinationType.All;
        private IReadOnlyList<PromptTriggerConditionEntry> ConditionEntries => this.Profile != null ? this.Profile.ConditionEntries : Array.Empty<PromptTriggerConditionEntry>();

        // --- Runtime State ---
        //  1. Condition States
        [Header("Condition State (Runtime)")]
        [SerializeField] private PromptTriggerConditionStateMap _conditionStateMap = new();
        private IReadOnlyDictionary<PromptTriggerConditionKey, bool> ConditionStateMap => _conditionStateMap;

        //  2. Runtime State
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
        public void SetConditionState(PromptTriggerConditionKey conditionKey, bool value)
        {
            if (conditionKey == PromptTriggerConditionKey.None) return;

            bool oldValue = this.GetConditionState(conditionKey);
            if (oldValue == value) return;

            _conditionStateMap[conditionKey] = value;
            this.RequestPromptRefresh();
        }

        public void SatisfyCondition(PromptTriggerConditionKey conditionKey)
        {
            this.SetConditionState(conditionKey, true);
        }

        public void UnsatisfyCondition(PromptTriggerConditionKey conditionKey)
        {
            this.SetConditionState(conditionKey, false);
        }

        public bool GetConditionState(PromptTriggerConditionKey conditionKey)
        {
            if (conditionKey == PromptTriggerConditionKey.None) return false;

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
                PromptTriggerConditionEntry entry = this.ConditionEntries[i];
                if (entry == null) continue;
                if (entry.ConditionKey == PromptTriggerConditionKey.None) continue;

                _conditionStateMap[entry.ConditionKey] = entry.StartValue;
            }
        }

        private bool AreTriggerConditionsSatisfied()
        {
            if (this.ConditionEntries == null || this.ConditionEntries.Count == 0)
            {
                return true;
            }

            if (this.ConditionCombinationType == PromptTriggerConditionCombinationType.Any)
            {
                return this.IsAnyTriggerConditionSatisfied();
            }

            return this.AreAllTriggerConditionsSatisfied();
        }

        private bool AreAllTriggerConditionsSatisfied()
        {
            for (int i = 0; i < this.ConditionEntries.Count; i++)
            {
                PromptTriggerConditionEntry entry = this.ConditionEntries[i];
                if (entry == null) continue;
                if (entry.ConditionKey == PromptTriggerConditionKey.None) continue;

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
                PromptTriggerConditionEntry entry = this.ConditionEntries[i];
                if (entry == null) continue;
                if (entry.ConditionKey == PromptTriggerConditionKey.None) continue;

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