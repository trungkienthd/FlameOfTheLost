using System;
using System.Collections.Generic;

namespace FlameOfTheLost
{
    [Serializable]
    public class StatusEffectStatPeriodicTargetToConsumerDictionary : SerializableDictionary<StatusEffectStatPeriodicTarget, List<IStatusEffectStatPeriodicConsumer>>
    {
    }
}
