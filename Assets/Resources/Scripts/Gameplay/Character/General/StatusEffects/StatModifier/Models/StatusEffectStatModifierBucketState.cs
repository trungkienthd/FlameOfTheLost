namespace FlameOfTheLost
{
    public struct StatusEffectStatModifierBucketState
    {
        public float StackedValue
        {
            get;
            set;
        }
        public bool HasAbsoluteOverridden
        {
            get;
            set;
        }
        public float AbsoluteOverriddenValue
        {
            get;
            set;
        }
    }
}