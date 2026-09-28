using UnityEngine;

namespace FlameOfTheLost
{
    public class AnimalDamageReactionHandler : AnimalBrainSubBehaviourBase
    {
        // --- References ---
        private DamageReceivingHandler DamageReceivingHandler => this.AnimalRootController != null
            ? this.AnimalRootController.DamageReceivingHandler
            : null;

        // --- Unity Callbacks ---
        private void OnEnable()
        {
            this.BindDamageEvents();
        }
        private void OnDisable()
        {
            this.UnbindDamageEvents();
        }

        // --- Helpers ---
        //  1. Damage Event
        private void HandleDamageApplied(DamageAppliedContext context)
        {
            if (this.AnimalBrainController == null) return;

            this.AnimalBrainController.NotifyDamageReceived();
        }

        //  2. Event Binding
        private void BindDamageEvents()
        {
            if (this.DamageReceivingHandler == null) return;

            this.DamageReceivingHandler.OnDamageApplied -= this.HandleDamageApplied;
            this.DamageReceivingHandler.OnDamageApplied += this.HandleDamageApplied;
        }
        private void UnbindDamageEvents()
        {
            if (this.DamageReceivingHandler == null) return;

            this.DamageReceivingHandler.OnDamageApplied -= this.HandleDamageApplied;
        }

        // --- FOTLSubBehaviour Overrides ---
        protected override void OnBound(AnimalBrainController animalBrainController)
        {
            base.OnBound(animalBrainController);

            this.BindDamageEvents();
        }
        protected override void OnUnbinding(AnimalBrainController animalBrainController)
        {
            this.UnbindDamageEvents();

            base.OnUnbinding(animalBrainController);
        }
    }
}