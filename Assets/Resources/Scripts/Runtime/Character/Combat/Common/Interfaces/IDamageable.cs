using UnityEngine;

namespace FlameOfTheLost
{
    public interface IDamageable
    {
        public void TakeDamage(int damageAmount, Vector2 hitDir, float knockbackForce);

        public CharacterFactionType CharacterFaction 
        { 
            get; 
        }
    }
}
