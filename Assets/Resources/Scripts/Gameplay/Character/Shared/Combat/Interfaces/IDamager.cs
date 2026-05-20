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

        public CharacterFactionType SourceCharacterFaction
        {
            get;
        }

        public Transform OriginTransform
        {
            get;
        }
    }
}
