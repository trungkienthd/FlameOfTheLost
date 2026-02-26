using UnityEngine;

namespace FlameOfTheLost
{
    public interface IDamageable
    {
        public CharacterFactionType CharacterFaction
        {
            get;
        }

        public void TakeDamage(int damageAmount);
        public void Heal(int healAmount);
        public void Kill();
    }
}
