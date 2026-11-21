namespace FlameOfTheLost
{
    public interface IStatusEffectFlagConsumer
    {
        public void SetStatusEffectFlag(StatusEffectFlagTarget flagType, bool value);
    }
}
