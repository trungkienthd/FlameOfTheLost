using UnityEngine;

namespace FlameOfTheLost
{
    public struct HitboxDamagePayload
    {
        public int DamageAmount
        {
            get; 
            set;
        }

        public float KnockbackForce
        {
            get; 
            set;
        }

        public Transform AttackOrigin
        {
            get; 
            set;
        }

        public CharacterFaction SourceFaction
        {
            get; 
            set;
        }
    }
}
