namespace FlameOfTheLost
{
    internal class StatusEffectModifierAccumulator
    {
        private float _totalMultiplier = 1f;
        private float _totalAdditiveFlat = 0f;

        public float TotalMultiplier
        {
            get => _totalMultiplier;
            set => _totalMultiplier = value;
        }
        public float TotalAdditiveFlat
        {
            get => _totalAdditiveFlat;
            set => _totalAdditiveFlat = value;
        }
    }
}