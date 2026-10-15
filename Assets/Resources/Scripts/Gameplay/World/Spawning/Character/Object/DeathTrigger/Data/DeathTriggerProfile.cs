using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "DTrg_New", menuName = "Data/FlameOfTheLost/World/Spawning/DeathTriggerProfile")]
    public class DeathTriggerProfile : FOTLWorldObjectProfile,
        IIdentityProvider,
        IIconProvider
    {
        // --- Data / Configs ---
        //  1. Trigger Settings
        [Header("Trigger Settings")]
        [SerializeField] private bool _triggerOnceUntilExit = true;
        public bool TriggerOnceUntilExit => _triggerOnceUntilExit;

        //  2. Collider
        [Header("Collider")]
        [SerializeField] private bool _isTrigger = true;
        [SerializeField] private List<Vector2> _colliderPoints = new();
        public bool IsTrigger => _isTrigger;
        public IReadOnlyList<Vector2> ColliderPoints => _colliderPoints;

        //  3. Affected Factions
        [Header("Affected Factions")]
        [SerializeField] private bool _useFactionFilter = true;
        [SerializeField] private List<CharacterFaction> _affectedFactions = new()
        {
            CharacterFaction.Player
        };
        public bool UseFactionFilter => _useFactionFilter;
        public IReadOnlyList<CharacterFaction> AffectedFactions => _affectedFactions;

        //  4. Affected Traversal Types
        [Header("Affected Traversal Types")]
        [SerializeField] private bool _useTraversalTypeFilter = false;
        [SerializeField] private List<CharacterTraversalType> _affectedTraversalTypes = new()
        {
            CharacterTraversalType.Humanoid,
            CharacterTraversalType.LandCreature
        };
        public bool UseTraversalTypeFilter => _useTraversalTypeFilter;
        public IReadOnlyList<CharacterTraversalType> AffectedTraversalTypes => _affectedTraversalTypes;

        //  5. Affected Enemy Types
        [Header("Affected Enemy Types")]
        [SerializeField] private bool _useEnemyTypeFilter = false;
        public bool UseEnemyTypeFilter => _useEnemyTypeFilter;

    }
}
