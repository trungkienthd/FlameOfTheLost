using System.Collections.Generic;

namespace FlameOfTheLost
{
    public interface IStatusEffectStatPeriodicConsumer
    {
        public StatusEffectStatPeriodicTarget Target
        {
            get; 
        }

        public void SetActivePeriodicStreams(IReadOnlyList<RuntimeStatusEffectStatPeriodicStream> streams);
    }
}
