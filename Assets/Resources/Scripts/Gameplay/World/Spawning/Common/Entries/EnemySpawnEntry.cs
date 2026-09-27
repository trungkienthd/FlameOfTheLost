using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public abstract class EnemySpawnEntry : CharacterAISpawnEntry
    {
        // --- Data / Configs ---
        //  1. Enemy Variant
        [Header("Configs - Enemy Variant")]
        [SerializeField] private EnemyVariantProfile _enemyVariant;
        public EnemyVariantProfile EnemyVariant => _enemyVariant;

        public sealed override CharacterAIVariantProfile CharacterAIVariant => this.EnemyVariant;
    }
}