using System.Collections.Generic;

namespace FlameOfTheLost
{
    public sealed class GameConditionSaveHandler : GameConditionSubBehaviourBase
    {
        // --- Public APIs ---
        //  1. Save Data Capture
        public GameConditionSaveData CaptureSaveData()
        {
            if (!this.EnsureBound()) return null;

            GameConditionSaveData saveData = new();

            foreach (KeyValuePair<GameConditionKey, bool> pair in this.GameConditionController.ConditionStates)
            {
                if (pair.Key == GameConditionKey.None) continue;

                saveData.States[pair.Key] = pair.Value;
            }

            return saveData;
        }

        //  2. Save Data Restoration
        public void RestoreSaveData(GameConditionSaveData saveData)
        {
            if (!this.EnsureBound()) return;

            this.GameConditionController.RestoreConditionStates(saveData?.States);
        }
    }
}