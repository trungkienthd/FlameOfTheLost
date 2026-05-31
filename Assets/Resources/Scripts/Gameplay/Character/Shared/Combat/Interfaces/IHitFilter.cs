namespace FlameOfTheLost
{
    public interface IHitFilter
    {
        public bool CanHit(IDamageable target);
    }
}
