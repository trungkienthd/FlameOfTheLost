using UnityEngine;

namespace FlameOfTheLost
{
    public readonly struct HarvestDamageAppliedContext
    {
        public readonly CharacterRootController SourceCharacter;
        public readonly GameObject SourceObject;

        public readonly HarvestImpactType ImpactType;

        public readonly int BaseDamageAmount;
        public readonly float ImpactDamageMultiplier;
        public readonly int AppliedDamageAmount;

        // Compatibility / convenience alias.
        public int DamageAmount => this.AppliedDamageAmount;

        public HarvestDamageAppliedContext(CharacterRootController sourceCharacter, GameObject sourceObject, HarvestImpactType impactType, int baseDamageAmount, float impactDamageMultiplier, int appliedDamageAmount)
        {
            this.SourceCharacter = sourceCharacter;
            this.SourceObject = sourceObject;

            this.ImpactType = impactType;

            this.BaseDamageAmount = Mathf.Max(0, baseDamageAmount);
            this.ImpactDamageMultiplier = Mathf.Max(0f, impactDamageMultiplier);
            this.AppliedDamageAmount = Mathf.Max(0, appliedDamageAmount);
        }
    }
}
