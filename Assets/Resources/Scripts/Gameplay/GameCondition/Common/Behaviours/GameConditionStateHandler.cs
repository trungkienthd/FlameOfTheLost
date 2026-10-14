using System.Collections.Generic;

namespace FlameOfTheLost
{
    public sealed class GameConditionStateHandler : GameConditionSubBehaviourBase
    {
        // --- Public APIs ---
        //  1. State Queries
        public bool GetConditionState(GameConditionKey conditionKey)
        {
            if (!this.EnsureBound()) return false;
            if (conditionKey == GameConditionKey.None) return false;

            if (!this.GameConditionController.ConditionStates.TryGetValue(conditionKey, out bool value)) return false;

            return value;
        }

        //  2. State Updates
        public void SetConditionState(GameConditionKey conditionKey, bool value)
        {
            if (!this.EnsureBound()) return;
            if (conditionKey == GameConditionKey.None) return;

            GameConditionStateMap states = this.GameConditionController.MutableConditionStates;

            bool hasExistingValue = states.TryGetValue(conditionKey, out bool currentValue);
            if (hasExistingValue && currentValue == value) return;

            states[conditionKey] = value;

            this.RefreshRuntimeDebugEntries();

            this.GameConditionController.PublishConditionStateChanged(conditionKey, value);
        }

        //  3. Reset
        public void ResetConditions()
        {
            if (!this.EnsureBound()) return;

            this.InitializeConditionStates();

            this.PublishAllConditionStates();
        }

        // --- Internal APIs ---
        //  1. Runtime State Initialization
        internal void InitializeConditionStates()
        {
            if (!this.EnsureBound()) return;

            GameConditionStateMap states = this.GameConditionController.MutableConditionStates;

            states.Clear();

            IReadOnlyList<GameConditionInitialStateEntry> initialStates = this.GameConditionController.InitialStateEntries;

            if (initialStates != null)
            {
                for (int i = 0; i < initialStates.Count; i++)
                {
                    GameConditionInitialStateEntry entry = initialStates[i];

                    if (entry == null) continue;
                    if (entry.ConditionKey == GameConditionKey.None) continue;

                    states[entry.ConditionKey] = entry.InitialValue;
                }
            }

            this.RefreshRuntimeDebugEntries();
        }

        //  2. Save Restoration Support
        internal void RestoreConditionStates(IReadOnlyDictionary<GameConditionKey, bool> savedStates)
        {
            if (!this.EnsureBound()) return;

            // a. Rebuild from the current configuration.
            this.InitializeConditionStates();

            // b. Overlay saved values.
            if (savedStates != null)
            {
                GameConditionStateMap states = this.GameConditionController.MutableConditionStates;

                foreach (KeyValuePair<GameConditionKey, bool> pair in savedStates)
                {
                    if (pair.Key == GameConditionKey.None) continue;

                    states[pair.Key] = pair.Value;
                }
            }

            // c. Synchronize the Inspector representation.
            this.RefreshRuntimeDebugEntries();

            // d. Publish only the final restored state.
            this.PublishAllConditionStates();
        }

        // --- Helpers ---
        //  1. Inspector / Debug State
        private void RefreshRuntimeDebugEntries()
        {
            if (!this.EnsureBound()) return;

            List<GameConditionStateEntry> debugEntries = this.GameConditionController.MutableConditionStateEntries;

            debugEntries.Clear();

            foreach (KeyValuePair<GameConditionKey, bool> pair in this.GameConditionController.ConditionStates)
            {
                if (pair.Key == GameConditionKey.None) continue;

                GameConditionStateEntry entry = new(pair.Key, pair.Value);

                debugEntries.Add(entry);
            }
        }

        //  2. State Publishing
        private void PublishAllConditionStates()
        {
            if (!this.EnsureBound()) return;

            foreach (KeyValuePair<GameConditionKey, bool> pair in this.GameConditionController.ConditionStates)
            {
                if (pair.Key == GameConditionKey.None) continue;

                this.GameConditionController.PublishConditionStateChanged(pair.Key, pair.Value);
            }
        }
    }
}