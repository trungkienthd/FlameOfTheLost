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

        //  2.2. Condition Operation Validation
        public bool CanApplyConditionOperations(IReadOnlyList<GameConditionOperationEntry> operations)
        {
            if (!this.EnsureBound()) return false;

            if (operations == null || operations.Count == 0) return false;

            HashSet<GameConditionKey> operationKeys = new();

            for (int i = 0; i < operations.Count; i++)
            {
                GameConditionOperationEntry operation = operations[i];

                if (operation == null) return false;

                if (operation.ConditionKey == GameConditionKey.None) return false;

                if (!operationKeys.Add(operation.ConditionKey)) return false;
            }

            return true;
        }

        //  2.3. Condition Operation Execution
        public bool TryApplyConditionOperations(IReadOnlyList<GameConditionOperationEntry> operations)
        {
            if (!this.CanApplyConditionOperations(operations)) return false;

            GameConditionStateMap states = this.GameConditionController.MutableConditionStates;

            List<GameConditionOperationEntry> changedOperations = new();

            // a. Apply all configured condition states.
            for (int i = 0; i < operations.Count; i++)
            {
                GameConditionOperationEntry operation = operations[i];

                GameConditionKey conditionKey = operation.ConditionKey;
                bool value = operation.Value;

                bool hasExistingValue = states.TryGetValue(conditionKey, out bool currentValue);

                if (hasExistingValue && currentValue == value) continue;

                states[conditionKey] = value;

                changedOperations.Add(operation);
            }

            // b. No state changes are necessary.
            if (changedOperations.Count == 0) return true;

            // c. Synchronize the Inspector representation once.
            this.RefreshRuntimeDebugEntries();

            // d. Publish events after all operations have been applied.
            for (int i = 0; i < changedOperations.Count; i++)
            {
                GameConditionOperationEntry operation = changedOperations[i];
             
                bool currentValue = this.GetConditionState(operation.ConditionKey);
                if (currentValue != operation.Value) continue;  // The condition may have been modified by an earlier synchronous event subscriber.

                this.GameConditionController.PublishConditionStateChanged(operation.ConditionKey, operation.Value);
            }

            return true;
        }

        //  3. Reset
        public void ResetConditions()
        {
            if (!this.EnsureBound()) return;

            List<GameConditionKey> previousConditionKeys = new(this.GameConditionController.ConditionStates.Keys);

            this.InitializeConditionStates();

            this.PublishAllConditionStates(previousConditionKeys);
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

            // a. Capture previously registered condition keys.
            List<GameConditionKey> previousConditionKeys = new(this.GameConditionController.ConditionStates.Keys);

            // b. Rebuild from the current configuration.
            this.InitializeConditionStates();

            // c. Overlay saved values.
            if (savedStates != null)
            {
                GameConditionStateMap states = this.GameConditionController.MutableConditionStates;

                foreach (KeyValuePair<GameConditionKey, bool> pair in savedStates)
                {
                    if (pair.Key == GameConditionKey.None) continue;

                    states[pair.Key] = pair.Value;
                }
            }

            // d. Synchronize the Inspector representation.
            this.RefreshRuntimeDebugEntries();

            // e. Publish the final restored condition states, including previously registered conditions.
            this.PublishAllConditionStates(previousConditionKeys);
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
        private void PublishAllConditionStates(IReadOnlyList<GameConditionKey> previousConditionKeys = null)
        {
            if (!this.EnsureBound()) return;

            // a. Capture the current condition keys.
            List<GameConditionKey> conditionKeys = new(this.GameConditionController.ConditionStates.Keys);

            HashSet<GameConditionKey> registeredKeys = new(conditionKeys);

            // b. Include previously registered keys that no longer exist in the current condition map.
            if (previousConditionKeys != null)
            {
                foreach (GameConditionKey conditionKey in previousConditionKeys)
                {
                    if (conditionKey == GameConditionKey.None) continue;

                    if (!registeredKeys.Add(conditionKey)) continue;

                    conditionKeys.Add(conditionKey);
                }
            }

            // c. Publish the authoritative current values.
            foreach (GameConditionKey conditionKey in conditionKeys)
            {
                if (conditionKey == GameConditionKey.None) continue;

                bool value = this.GetConditionState(conditionKey);

                this.GameConditionController.PublishConditionStateChanged(conditionKey, value);
            }
        }
    }
}