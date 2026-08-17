using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Collider2D))]
    public class DeathTriggerController : FOTLMonoBehaviour
    {
        // --- Data / Configs ---
        [Header("Data / Configs")]

        //  1. Profile
        [SerializeField] private DeathTriggerProfile _profile;
        public DeathTriggerProfile Profile => _profile;

        // --- Runtime State ---
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

            if (this.Profile == null) return;

            if (this.Profile.TriggerOnceUntilExit && this.TriggeredHPControllers.Contains(hpCtrl)) return;

            CharacterIdentityController identityCtrl = characterRoot.IdentityController;
            if (!this.CanAffectCharacter(identityCtrl)) return;

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
            if (this.Profile == null) return false;

            if (this.Profile.UseFactionFilter)
            {
                if (this.Profile.AffectedFactions == null || this.Profile.AffectedFactions.Count == 0) return false;
                if (!this.Profile.AffectedFactions.Contains(identityController.Faction)) return false;
            }

            if (this.Profile.UseTraversalTypeFilter)
            {
                if (this.Profile.AffectedTraversalTypes == null || this.Profile.AffectedTraversalTypes.Count == 0) return false;
                if (!this.Profile.AffectedTraversalTypes.Contains(identityController.TraversalType)) return false;
            }

            if (this.Profile.UseEnemyTypeFilter)
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

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Profile == null)
            {
                Debug.LogError($"[{this.name}.{nameof(DeathTriggerController)}] " + $"requires a {nameof(DeathTriggerProfile)}.", this);
            }
        }
    }
}
