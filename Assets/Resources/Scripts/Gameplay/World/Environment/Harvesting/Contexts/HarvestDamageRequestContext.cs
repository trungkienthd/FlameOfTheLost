using UnityEngine;

namespace FlameOfTheLost
{
    public readonly struct HarvestDamageRequestContext
    {
        public readonly CharacterRootController SourceCharacter;
        public readonly GameObject SourceObject;

        public readonly HarvestImpactType ImpactType;
        public readonly int BaseDamageAmount;

        public HarvestDamageRequestContext(CharacterRootController sourceCharacter, GameObject sourceObject, HarvestImpactType impactType, int baseDamageAmount)
        {
            this.SourceCharacter = sourceCharacter;
            this.SourceObject = sourceObject;

            this.ImpactType = impactType;
            this.BaseDamageAmount = Mathf.Max(0, baseDamageAmount);
        }
    }
}