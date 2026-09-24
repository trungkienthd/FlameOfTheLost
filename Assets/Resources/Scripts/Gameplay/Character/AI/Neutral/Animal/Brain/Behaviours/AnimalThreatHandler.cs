using UnityEngine;

namespace FlameOfTheLost
{
    public class AnimalThreatHandler : AnimalBrainSubBehaviourBase
    {
        // --- Runtime ---
        [Header("Threat (Runtime)")]
        [SerializeField] private CharacterRootController _currentThreat;
        [SerializeField] private float _lastThreatTime;
        public CharacterRootController CurrentThreat
        {
            get => _currentThreat;
            private set => _currentThreat = value;
        }
        public float LastThreatTime
        {
            get => _lastThreatTime;
            private set => _lastThreatTime = value;
        }

        public bool HasThreat => this.CurrentThreat != null;

        // --- References ---
        private DamageReceivingHandler DamageReceivingHandler => this.AnimalRootController != null
                ? this.AnimalRootController.DamageReceivingHandler
                : null;

        // --- Public APIs ---
        public void RegisterThreat(CharacterRootController threat)
        {
            if (threat == null) return;
            if (threat == this.AnimalRootController) return;

            this.CurrentThreat = threat;
            this.LastThreatTime = Time.time;
        }

        public void ClearThreat()
        {
            this.CurrentThreat = null;
            this.LastThreatTime = 0f;
        }

        public bool HasThreatExpired(float forgetDuration)
        {
            if (!this.HasThreat) return true;

            return Time.time >= this.LastThreatTime + forgetDuration;
        }

        public void ResetRuntime()
        {
            this.ClearThreat();
        }

        // --- Damage Event ---
        private void HandleDamageApplied(DamageAppliedContext context)
        {
            /*
             * DamageReceivingHandler constructs DamageAppliedContext with:
             * attacker,
             * victim,
             * damageAmount.
             *
             * Use your actual DamageAppliedContext property name here.
             */
            CharacterRootController attacker = context.Attacker;

            if (attacker == null) return;

            this.RegisterThreat(attacker);

            this.AnimalBrainController?.NotifyThreatRegistered(attacker);
        }

        // --- Unity Callbacks ---
        private void OnEnable()
        {
            this.BindDamageEvents();
        }
        private void OnDisable()
        {
            this.UnbindDamageEvents();
        }

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

        protected override void OnBound(AnimalBrainController animalBrainController)
        {
            base.OnBound(animalBrainController);

            this.BindDamageEvents();
        }
        protected override void OnUnbinding(AnimalBrainController animalBrainController)
        {
            this.UnbindDamageEvents();
            this.ResetRuntime();

            base.OnUnbinding(animalBrainController);
        }
    }
}