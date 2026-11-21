using System;

namespace FlameOfTheLost
{
    [Serializable]
    public class GameConditionSaveData
    {
        public GameConditionStateMap States = new();
    }
}
