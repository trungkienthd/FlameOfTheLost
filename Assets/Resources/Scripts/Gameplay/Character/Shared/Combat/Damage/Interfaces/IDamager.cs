using UnityEngine;

namespace FlameOfTheLost
{
    public interface IDamager
    {
        public int EffectiveDamageAmount
        {
            get;
        }

        public float EffectiveKnockbackForce
        {
            get;
        }

        public CharacterFaction SourceCharacterFaction
        {
            get;
        }

        public Transform OriginTransform
        {
            get;
        }

        public CharacterRootController SourceCharacterRootController 
        { 
            get; 
        }
    }
}
