namespace FlameOfTheLost
{
    public interface IHarvestDamageSource
    {
        public HarvestImpactType ImpactType 
        {
            get; 
        }
        public bool TriggerSelfOnHitStatusEffects
        {
            get;
        }

        public void ConfigureHarvestDamage(RuntimeHarvestDamage runtimeHarvestDamage);
        public void ClearHarvestDamage();
        public int ComputeHarvestDamage(int combatDamageAmount);
    }
}