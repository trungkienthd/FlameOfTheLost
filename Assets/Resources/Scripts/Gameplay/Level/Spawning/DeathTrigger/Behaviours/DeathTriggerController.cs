using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Collider2D))]
    public class DeathTriggerController : FOTLWorldObjectProfileBehaviour<DeathTriggerProfile>
    {
        // --- References ---
        //  1. Collider
        [SerializeField] private PolygonCollider2D _polygonCollider;
        private PolygonCollider2D PolygonCollider => _polygonCollider;

        // --- Runtime ---
        private readonly HashSet<HPController> _triggeredHPControllers = new();
        private IReadOnlyCollection<HPController> TriggeredHPControllers => _triggeredHPControllers;

        private void OnTriggerEnter2D(Collider2D other)
        {
            CharacterRootController characterRoot = other.GetComponentInParent<CharacterRootController>();
            if (characterRoot == null) return;

            HPController hpCtrl = characterRoot.HPController;
            if (hpCtrl == null) return;
            if (hpCtrl.IsDead) return;

            if (this.Profile == null) return;

            if (this.Profile.TriggerOnceUntilExit && this.TriggeredHPControllers.Contains(hpCtrl)) return;

            if (!this.CanAffectCharacter(characterRoot)) return;

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
        private bool CanAffectCharacter(CharacterRootController character)
        {
            if (character == null) return false;
            if (this.Profile == null) return false;

            if (this.Profile.UseFactionFilter)
            {
                if (this.Profile.AffectedFactions == null || this.Profile.AffectedFactions.Count == 0) return false;
                if (!this.Profile.AffectedFactions.Contains(character.Faction)) return false;
            }

            if (this.Profile.UseTraversalTypeFilter)
            {
                if (this.Profile.AffectedTraversalTypes == null || this.Profile.AffectedTraversalTypes.Count == 0) return false;
                if (!this.Profile.AffectedTraversalTypes.Contains(character.TraversalType)) return false;
            }

            if (this.Profile.UseEnemyTypeFilter)
            {
                // TODO: Implement enemy type filter logic when EnemyType is fully defined
            }

            return true;
        }

        private void ConfigureColliderPoints()
        {
            if (this.PolygonCollider == null) return;
            if (this.Profile == null) return;
            
            this.PolygonCollider.isTrigger = this.Profile.IsTrigger;

            if (this.Profile.ColliderPoints == null) return;
            if (this.Profile.ColliderPoints.Count < 3) return;

            this.PolygonCollider.pathCount = 1;
            this.PolygonCollider.SetPath
            (
                index: 0,
                points: this.Profile.ColliderPoints.ToArray()
            );
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PolygonCollider == null) _polygonCollider = this.GetComponent<PolygonCollider2D>();
        }

        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.PolygonCollider == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(DeathTriggerController)}] " + $"{nameof(PolygonCollider2D)} reference is missing.", this);
            }
        }

        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            this.ConfigureColliderPoints();
        }
    }
}
