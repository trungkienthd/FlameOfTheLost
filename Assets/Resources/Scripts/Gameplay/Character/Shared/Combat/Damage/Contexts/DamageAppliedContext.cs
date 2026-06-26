using UnityEngine;

namespace FlameOfTheLost
{
    public readonly struct DamageAppliedContext
    {
        public readonly CharacterRootController Attacker;
        public readonly CharacterRootController Victim;
        public readonly int DamageAmount;

        public DamageAppliedContext(CharacterRootController attacker, CharacterRootController victim, int damageAmount)
        {
            this.Attacker = attacker;
            this.Victim = victim;
            this.DamageAmount = damageAmount;
        }
    }
}