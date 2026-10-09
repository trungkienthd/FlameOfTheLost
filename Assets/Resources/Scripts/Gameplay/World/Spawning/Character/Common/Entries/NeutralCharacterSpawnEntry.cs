using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public abstract class NeutralCharacterSpawnEntry : CharacterAISpawnEntry
    {
        // --- Data / Configs ---
        //  1. Neutral Character Variant
        [Header("Configs - Neutral Character Variant")]
        [SerializeField] private NeutralCharacterVariantProfile _neutralCharacterVariant;
        public NeutralCharacterVariantProfile NeutralCharacterVariant => _neutralCharacterVariant;

        public sealed override CharacterAIVariantProfile CharacterAIVariant => this.NeutralCharacterVariant;
    }
}