using UnityEngine;

namespace FlameOfTheLost
{
    public class HarvestDamageSourceProvider : FOTLMonoBehaviour, IHarvestDamageSource
    {
        // --- Configs ---
        private RuntimeHarvestDamage _activeHarvestDamage;
        public RuntimeHarvestDamage ActiveHarvestDamage => _activeHarvestDamage;

        public bool IsHarvestDamageEnabled => this.ActiveHarvestDamage.IsEnabled;

        public HarvestImpactType ImpactType => this.IsHarvestDamageEnabled
            ? this.ActiveHarvestDamage.ImpactType
            : HarvestImpactType.None;

        public bool TriggerSelfOnHitStatusEffects => this.IsHarvestDamageEnabled && this.ActiveHarvestDamage.TriggerSelfOnHitStatusEffects;

        // --- Public APIs ---
        public void ConfigureHarvestDamage(RuntimeHarvestDamage runtimeHarvestDamage)
        {
            _activeHarvestDamage = runtimeHarvestDamage;
        }

        public void ClearHarvestDamage()
        {
            _activeHarvestDamage = RuntimeHarvestDamage.Disabled;
        }

        public int ComputeBaseHarvestDamage(int combatDamageAmount)
        {
            if (!this.IsHarvestDamageEnabled) return 0;

            combatDamageAmount = Mathf.Max(0, combatDamageAmount);

            float rawDamage = combatDamageAmount * this.ActiveHarvestDamage.CombatDamageMultiplier + this.ActiveHarvestDamage.FlatHarvestDamage;
            if (rawDamage <= 0f) return 0;

            int result = Mathf.RoundToInt(rawDamage);

            return Mathf.Max(this.ActiveHarvestDamage.MinimumHarvestDamage, result);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ClearHarvestDamage();
        }
    }
}