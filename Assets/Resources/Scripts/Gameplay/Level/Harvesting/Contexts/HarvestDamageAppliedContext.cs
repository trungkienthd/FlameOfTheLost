using UnityEngine;

namespace FlameOfTheLost
{
    public readonly struct HarvestDamageAppliedContext
    {
        public readonly CharacterRootController SourceCharacter;
        public readonly GameObject SourceObject;
        public readonly HarvestImpactType ImpactType;
        public readonly int DamageAmount;

        public HarvestDamageAppliedContext(CharacterRootController sourceCharacter, GameObject sourceObject, HarvestImpactType impactType, int damageAmount)
        {
            this.SourceCharacter = sourceCharacter;
            this.SourceObject = sourceObject;
            this.ImpactType = impactType;
            this.DamageAmount = damageAmount;
        }
    }
}
