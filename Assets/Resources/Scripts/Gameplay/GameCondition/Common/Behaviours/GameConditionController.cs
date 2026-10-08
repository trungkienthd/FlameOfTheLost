using System;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEditor.U2D.Tooling.Analyzer;
using UnityEngine;

namespace FlameOfTheLost
{
    public class GameConditionController : FOTLMonoBehaviour, ISaveDataHandler<GameConditionSaveData>
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
        //  1. Authoritative Runtime State
        private GameConditionStateMap _conditionStates = new();

        public IReadOnlyDictionary<GameConditionKey, bool> ConditionStates => _conditionStates;

        //  2. Inspector / Debug Mirror
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
            if (!_conditionStates.TryGetValue(conditionKey, out bool value)) return false;

            return value;
        }

        //  2. State Updates
        public void SetConditionState(GameConditionKey conditionKey, bool value)
        {
            if (conditionKey == GameConditionKey.None) return;

            bool hasExistingValue = _conditionStates.TryGetValue(conditionKey, out bool currentValue);
            if (hasExistingValue && currentValue == value) return;

            _conditionStates[conditionKey] = value;

            this.RefreshRuntimeDebugEntries();

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

            this.PublishAllConditionStates();
        }

        //  4. ISaveDataHandler: Save / Load
        public GameConditionSaveData CaptureSaveData()
        {
            GameConditionSaveData saveData = new();

            foreach (KeyValuePair<GameConditionKey, bool> pair in _conditionStates)
            {
                if (pair.Key == GameConditionKey.None) continue;

                saveData.States[pair.Key] = pair.Value;
            }

            return saveData;
        }
        public void RestoreSaveData(GameConditionSaveData saveData)
        {
            // a. Rebuild from the CURRENT game configuration.
            this.InitializeConditionStates();

            // b. Overlay the saved state.
            if (saveData?.States != null)
            {
                foreach (KeyValuePair<GameConditionKey, bool> pair in saveData.States)
                {
                    if (pair.Key == GameConditionKey.None) continue;

                    _conditionStates[pair.Key] = pair.Value;
                }
            }

            // c. Synchronize the Inspector representation.
            this.RefreshRuntimeDebugEntries();

            // d. Publish only the FINAL restored values.
            this.PublishAllConditionStates();
        }

        // --- Helpers ---
        //  1. Runtime State Initialization
        private void InitializeConditionStates()
        {
            if (_conditionStates == null)
            {
                _conditionStates = new GameConditionStateMap();
            }
            else
            {
                _conditionStates.Clear();
            }

            if (this.InitialStateEntries != null)
            {
                for (int i = 0; i < this.InitialStateEntries.Count; i++)
                {
                    GameConditionInitialStateEntry initialStateEntry = this.InitialStateEntries[i];

                    if (initialStateEntry == null) continue;
                    if (initialStateEntry.ConditionKey == GameConditionKey.None) continue;

                    _conditionStates[initialStateEntry.ConditionKey] = initialStateEntry.InitialValue;
                }
            }

            this.RefreshRuntimeDebugEntries();
        }

        //  2. Inspector / Debug State
        private void RefreshRuntimeDebugEntries()
        {
            if (_conditionStateEntries == null)
            {
                _conditionStateEntries = new List<GameConditionStateEntry>();
            }
            else
            {
                _conditionStateEntries.Clear();
            }

            if (_conditionStates == null) return;

            foreach (KeyValuePair<GameConditionKey, bool> pair in _conditionStates)
            {
                if (pair.Key == GameConditionKey.None) continue;

                var conditionState = new GameConditionStateEntry(pair.Key, pair.Value);
                _conditionStateEntries.Add(conditionState);
            }
        }

        //  3. State Publishing
        private void PublishAllConditionStates()
        {
            if (_conditionStates == null) return;

            foreach (KeyValuePair<GameConditionKey, bool> pair in _conditionStates)
            {
                if (pair.Key == GameConditionKey.None) continue;

                this.ConditionStateChanged?.Invoke(pair.Key, pair.Value);
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.TutorialProgressController     == null) _tutorialProgressController    = this.GetComponentInChildren<TutorialProgressController>(true);
            if (this.MilestoneProgressController    == null) _milestoneProgressController   = this.GetComponentInChildren<MilestoneProgressController>(true);
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
