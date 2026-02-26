using System.Collections.Generic;

namespace FlameOfTheLost
{
    public interface IStatusEffectStatPeriodicConsumer
    {
        public StatusEffectStatPeriodicTargetType PeriodicTarget
        {
            get; 
        }

        public void SetActivePeriodicStreams(IReadOnlyList<StatusEffectStatPeriodicStream> streams);
    }
}
