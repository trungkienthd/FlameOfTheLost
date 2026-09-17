namespace FlameOfTheLost
{
    public readonly struct RuntimeHarvestDamage
    {
        public bool IsEnabled { get; }
        public HarvestImpactType ImpactType { get; }

        public float CombatDamageMultiplier { get; }
        public int FlatHarvestDamage { get; }
        public int MinimumHarvestDamage { get; }

        public bool TriggerSelfOnHitStatusEffects { get; }

        public RuntimeHarvestDamage(HarvestDamageEntry entry)
        {
            bool isEnabled = entry != null && entry.IsEnabled;

            this.IsEnabled = isEnabled;

            this.ImpactType = isEnabled
                ? entry.ImpactType
                : HarvestImpactType.None;

            this.CombatDamageMultiplier = isEnabled
                ? entry.CombatDamageMultiplier
                : 0f;

            this.FlatHarvestDamage = isEnabled
                ? entry.FlatHarvestDamage
                : 0;

            this.MinimumHarvestDamage = isEnabled
                ? entry.MinimumHarvestDamage
                : 0;

            this.TriggerSelfOnHitStatusEffects = isEnabled && entry.TriggerSelfOnHitStatusEffects;
        }

        public static RuntimeHarvestDamage Disabled => new RuntimeHarvestDamage();
    }
}
