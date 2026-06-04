using UnityEngine;

namespace FlameOfTheLost
{
    public interface IDamageConfigurator
    {
        public void SetDamageAmount(int value);
        public void SetKnockbackForce(float force);
        public void SetOrigin(Transform origin);
        public void SetSourceCharacterFaction(CharacterFaction faction);
    }
}