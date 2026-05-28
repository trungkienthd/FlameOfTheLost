using UnityEngine;

namespace FlameOfTheLost
{
    public interface IDamageable
    {
        public CharacterFaction CharacterFaction
        {
            get;
        }

        public void TakeDamage(int damageAmount);
        public void Heal(int healAmount);
        public void Kill();
    }
}
