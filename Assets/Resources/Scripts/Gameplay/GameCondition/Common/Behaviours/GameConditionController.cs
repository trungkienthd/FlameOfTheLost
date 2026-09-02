using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class GameConditionController : FOTLMonoBehaviour
    {
        // --- References ---
        //  1. Sub-Behaviours
        [Header("References (Sub-Behaviours)")]
        [SerializeField] private TutorialProgressController _tutorialProgressController;
        [SerializeField] private MilestoneProgressController _milestoneProgressController;
        private TutorialProgressController TutorialProgressController => _tutorialProgressController;
        private MilestoneProgressController MilestoneProgressController => _milestoneProgressController;

        // --- Data / Configs ---
        [Header("Initial Condition States")]
        [SerializeField] private List<GameConditionInitialStateEntry> _initialStateEntries = new();
        private IReadOnlyList<GameConditionInitialStateEntry> InitialStateEntries => _initialStateEntries;

        // --- Runtime State ---
        [Header("Condition States (Runtime)")]
        [SerializeField] private List<GameConditionStateEntry> _conditionStateEntries = new();
        private IReadOnlyList<GameConditionStateEntry> ConditionStateEntries => _conditionStateEntries;

        // --- Events ---
        public event Action<GameConditionKey, bool> ConditionStateChanged;

        // --- Public APIs ---
        //  1. State Queries
        public bool GetConditionState(GameConditionKey conditionKey)
        {
            if (conditionKey == GameConditionKey.None) return false;

            GameConditionStateEntry stateEntry = this.FindConditionStateEntry(conditionKey);
            return stateEntry != null && stateEntry.Value;
        }

        //  2. State Updates
        public void SetConditionState(GameConditionKey conditionKey, bool value)
        {
            if (conditionKey == GameConditionKey.None) return;

            GameConditionStateEntry stateEntry = this.FindConditionStateEntry(conditionKey);

            if (stateEntry == null)
            {
                stateEntry = new GameConditionStateEntry(conditionKey, value);
                _conditionStateEntries.Add(stateEntry);

                this.ConditionStateChanged?.Invoke(conditionKey, value);
                return;
            }

            if (stateEntry.Value == value) return;

            stateEntry.Value = value;

            this.ConditionStateChanged?.Invoke(conditionKey, value);
        }
        public void SatisfyCondition(GameConditionKey conditionKey)
        {
            this.SetConditionState(conditionKey, true);
        }
        public void UnsatisfyCondition(GameConditionKey conditionKey)
        {
            this.SetConditionState(conditionKey, false);
        }

        //  3. Reset
        public void ResetConditions()
        {
            this.InitializeConditionStates();

            for (int i = 0; i < this.ConditionStateEntries.Count; i++)
            {
                GameConditionStateEntry stateEntry = this.ConditionStateEntries[i];
                if (stateEntry == null) continue;

                this.ConditionStateChanged?.Invoke(stateEntry.ConditionKey, stateEntry.Value);
            }
        }

        // --- Helpers ---
        private GameConditionStateEntry FindConditionStateEntry(GameConditionKey conditionKey)
        {
            for (int i = 0; i < this.ConditionStateEntries.Count; i++)
            {
                GameConditionStateEntry stateEntry = this.ConditionStateEntries[i];
                if (stateEntry == null) continue;

                if (stateEntry.ConditionKey == conditionKey) return stateEntry;
            }

            return null;
        }

        private void InitializeConditionStates()
        {
            _conditionStateEntries.Clear();

            for (int i = 0; i < this.InitialStateEntries.Count; i++)
            {
                GameConditionInitialStateEntry initialStateEntry = this.InitialStateEntries[i];

                if (initialStateEntry == null) continue;
                if (initialStateEntry.ConditionKey == GameConditionKey.None) continue;

                GameConditionStateEntry entry = new GameConditionStateEntry(initialStateEntry.ConditionKey, initialStateEntry.InitialValue);
                _conditionStateEntries.Add(entry);
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.TutorialProgressController == null) _tutorialProgressController = this.GetComponentInChildren<TutorialProgressController>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.TutorialProgressController == null) Debug.LogError($"[{this.gameObject.name}.{nameof(GameConditionController)}] {nameof(this.TutorialProgressController)} reference is not assigned.", this);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.TutorialProgressController?.Bind(this);
            this.MilestoneProgressController?.Bind(this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.InitializeConditionStates();
        }
        protected override void UnbindInternalReferences()
        {
            this.TutorialProgressController?.Unbind(this);
            this.MilestoneProgressController?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
