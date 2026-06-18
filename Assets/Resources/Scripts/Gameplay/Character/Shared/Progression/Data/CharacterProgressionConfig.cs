using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Prg_Character", menuName = "Data/FlameOfTheLost/Character/Shared/Progression/CharacterProgressionConfig")]
    public class CharacterProgressionConfig : ScriptableObject
    {
        [Header("Initial Attributes")]
        [SerializeField] private List<CharacterProgressionBaseAttributeEntry> _baseAttributeEntries = new();
        public IReadOnlyList<CharacterProgressionBaseAttributeEntry> BaseAttributeEntries => _baseAttributeEntries;

        [Header("Derived Stats")]
        [SerializeField] private List<CharacterProgressionDerivedStatEntry> _derivedStatEntries = new();
        public IReadOnlyList<CharacterProgressionDerivedStatEntry> DerivedStatEntries => _derivedStatEntries;
    }
}
