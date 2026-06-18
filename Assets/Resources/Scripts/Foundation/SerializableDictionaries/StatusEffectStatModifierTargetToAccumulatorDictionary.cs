using System;

namespace FlameOfTheLost
{
    [Serializable]
    public class StatusEffectStatModifierTargetToAccumulatorDictionary : SerializableDictionary<StatusEffectStatModifierTarget, RuntimeStatusEffectStatModifierAccumulator>
    {
    }
}
