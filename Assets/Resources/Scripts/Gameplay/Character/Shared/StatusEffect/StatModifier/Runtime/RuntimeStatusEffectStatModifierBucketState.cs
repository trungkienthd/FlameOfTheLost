namespace FlameOfTheLost
{
    public struct RuntimeStatusEffectStatModifierBucketState
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