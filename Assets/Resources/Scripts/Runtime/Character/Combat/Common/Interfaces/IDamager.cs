using UnityEngine;

namespace FlameOfTheLost
{
    public interface IDamager
    {
        public int DamageAmount
        {
            get;
        }

        public float KnockbackForce
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
    }
}
