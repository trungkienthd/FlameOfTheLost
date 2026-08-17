using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "DTrg_New", menuName = "Data/FlameOfTheLost/Level/Spawning/DeathTriggerProfile")]
    public class DeathTriggerProfile : ScriptableObject,
        IIdentityProvider,
        IIconProvider
    {
        // --- Data / Configs ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _id = 21000;
        [SerializeField] private string _key;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(5, 5)] private string _description;
        public int Id => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        //  2. Display
        [Header("Display")]
        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;

        //  3. Trigger Settings
        [Header("Trigger Settings")]
        [SerializeField] private bool _triggerOnceUntilExit = true;
        public bool TriggerOnceUntilExit => _triggerOnceUntilExit;

        //  4. Affected Factions
        [Header("Affected Factions")]
        [SerializeField] private bool _useFactionFilter = true;
        [SerializeField] private List<CharacterFaction> _affectedFactions = new()
        {
            CharacterFaction.Player
        };
        public bool UseFactionFilter => _useFactionFilter;
        public IReadOnlyList<CharacterFaction> AffectedFactions => _affectedFactions;

        //  5. Affected Traversal Types
        [Header("Affected Traversal Types")]
        [SerializeField] private bool _useTraversalTypeFilter = false;
        [SerializeField] private List<CharacterTraversalType> _affectedTraversalTypes = new()
        {
            CharacterTraversalType.Humanoid,
            CharacterTraversalType.LandCreature
        };
        public bool UseTraversalTypeFilter => _useTraversalTypeFilter;
        public IReadOnlyList<CharacterTraversalType> AffectedTraversalTypes => _affectedTraversalTypes;

        //  6. Affected Enemy Types
        [Header("Affected Enemy Types")]
        [SerializeField] private bool _useEnemyTypeFilter = false;
        public bool UseEnemyTypeFilter => _useEnemyTypeFilter;

    }
}
