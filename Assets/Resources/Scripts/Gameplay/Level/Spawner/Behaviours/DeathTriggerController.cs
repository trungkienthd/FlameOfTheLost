using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Collider2D))]
    public class DeathTriggerController : FOTLMonoBehaviour, IIdentityProvider
    {
        // --- States ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _id = 1;
        [SerializeField] private string _key = "DEATH_TRIGGER_EXAMPLE";
        [SerializeField] private string _displayName = "Death Trigger Example";
        [SerializeField, TextArea(5, 5)] private string _description = "An example trigger that causes characters to die when entered.";
        public int Id  => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        //  2. Trigger Settings
        [Header("Trigger Settings")]
        [SerializeField] private bool _triggerOnceUntilExit = true;
        private bool TriggerOnceUntilExit => _triggerOnceUntilExit;

        [Header("Affected Factions")]
        [SerializeField] private bool _useFactionFilter = true;
        private bool UseFactionFilter => _useFactionFilter;
        [SerializeField] private List<CharacterFaction> _affectedFactions = new()
        {
            CharacterFaction.Player
        };
        private IReadOnlyList<CharacterFaction> AffectedFactions => _affectedFactions;

        [Header("Affected Traversal Types")]
        [SerializeField] private bool _useTraversalTypeFilter = false;
        [SerializeField] private List<CharacterTraversalType> _affectedTraversalTypes = new()
        {
            CharacterTraversalType.Humanoid,
            CharacterTraversalType.LandCreature
        };
        private bool UseTraversalTypeFilter => _useTraversalTypeFilter;
        private IReadOnlyList<CharacterTraversalType> AffectedTraversalTypes => _affectedTraversalTypes;

        [Header("Affected Enemy Types")]
        [SerializeField] private bool _useEnemyTypeFilter = false;
        // [SerializeField] private List<EnemyType> _affectedEnemyTypes = new();
        private bool UseEnemyTypeFilter => _useEnemyTypeFilter;

        private readonly HashSet<HPController> _triggeredHPControllers = new();
        private IReadOnlyCollection<HPController> TriggeredHPControllers => _triggeredHPControllers;

        // --- Unity Callbacks ---
        private void Reset()
        {
            this.ConfigureColliderAsTrigger();
        }
        private void OnValidate()
        {
            this.ConfigureColliderAsTrigger();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CharacterRootController characterRoot = other.GetComponentInParent<CharacterRootController>();
            if (characterRoot == null) return;

            HPController hpCtrl = characterRoot.HPController;
            if (hpCtrl == null) return;
            if (hpCtrl.IsDead) return;

            if (this.TriggerOnceUntilExit && this.TriggeredHPControllers.Contains(hpCtrl)) return;

            CharacterIdentityController identityCtrl = characterRoot.IdentityController;
            if (!this.CanAffectCharacter(identityCtrl)) return;

            if (this.TriggerOnceUntilExit && this.TriggeredHPControllers.Contains(hpCtrl)) return;

            _triggeredHPControllers.Add(hpCtrl);

            CharacterDeathCauseEntry deathCause = CharacterDeathCauseGenerationUtility.Environment(sourceObject: this.gameObject);
            hpCtrl.Kill(deathCause);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            CharacterRootController characterRoot = other.GetComponentInParent<CharacterRootController>();
            if (characterRoot == null) return;

            HPController hpCtrl = characterRoot.HPController;
            if (hpCtrl == null) return;

            _triggeredHPControllers.Remove(hpCtrl);
        }

        // --- Helpers ---
        private bool CanAffectCharacter(CharacterIdentityController identityController)
        {
            if (identityController == null) return false;

            if (this.UseFactionFilter)
            {
                if (this.AffectedFactions == null || this.AffectedFactions.Count == 0) return false;
                if (!this.AffectedFactions.Contains(identityController.Faction)) return false;
            }

            if (this.UseTraversalTypeFilter)
            {
                if (this.AffectedTraversalTypes == null || this.AffectedTraversalTypes.Count == 0) return false;
                if (!this.AffectedTraversalTypes.Contains(identityController.TraversalType)) return false;
            }

            if (this.UseEnemyTypeFilter)
            {
                // TODO: Implement enemy type filter logic when EnemyType is fully defined
            }

            return true;
        }

        private void ConfigureColliderAsTrigger()
        {
            Collider2D triggerCollider = this.GetComponent<Collider2D>();

            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true;
            }
        }
    }
}
