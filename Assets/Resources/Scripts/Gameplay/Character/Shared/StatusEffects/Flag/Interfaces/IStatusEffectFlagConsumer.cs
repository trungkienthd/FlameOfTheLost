namespace FlameOfTheLost
{
    public interface IStatusEffectFlagConsumer
    {
        public void SetStatusEffectFlag(StatusEffectFlagType flagType, bool value);
    }
}
