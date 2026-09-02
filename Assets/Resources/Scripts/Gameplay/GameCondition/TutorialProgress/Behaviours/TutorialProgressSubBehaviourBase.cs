using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class TutorialProgressSubBehaviourBase : FOTLSubBehaviour<TutorialProgressController>
    {
        // --- References ---
        protected TutorialProgressController TutorialProgressController => this.BrainBehaviour;

        protected CharacterRootController Player => this.TutorialProgressController?.Player;

        // --- Runtime ---
        private bool _gameplayReferencesBound;
        protected bool GameplayReferencesBound => _gameplayReferencesBound;

        // --- Internal APIs ---
        //  1. Gameplay Reference Lifecycle
        internal void BindGameplayReferences()
        {
            if (!this.EnsureBound()) return;
            if (this.GameplayReferencesBound) return;

            this.PrepareGameplayReferences();

            if (!this.ValidateGameplayReferences()) return;

            this.BindGameplayReferencesInternal();

            _gameplayReferencesBound = true;

            this.OnGameplayReferencesBound();
        }
        internal void UnbindGameplayReferences()
        {
            if (!this.GameplayReferencesBound) return;

            this.OnGameplayReferencesUnbinding();

            this.UnbindGameplayReferencesInternal();

            _gameplayReferencesBound = false;

            this.OnGameplayReferencesUnbound();
        }

        // --- Helpers ---
        //  1. Gameplay Reference Lifecycle
        protected virtual void PrepareGameplayReferences()
        {
        }
        protected virtual bool ValidateGameplayReferences()
        {
            return true;
        }
        protected virtual void BindGameplayReferencesInternal()
        {
        }
        protected virtual void UnbindGameplayReferencesInternal()
        {
        }
        protected virtual void OnGameplayReferencesBound()
        {
        }
        protected virtual void OnGameplayReferencesUnbinding()
        {
        }
        protected virtual void OnGameplayReferencesUnbound()
        {
        }

        //  2. Shared Validation
        protected bool ValidatePlayerReference()
        {
            if (this.Player != null) return true;

            Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(this.Player)} reference is not assigned.", this);
            return false;
        }

        // --- FOTLSubBehaviour Overrides ---
        protected override void OnUnbinding(TutorialProgressController brainBehaviour)
        {
            this.UnbindGameplayReferences();

            base.OnUnbinding(brainBehaviour);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _gameplayReferencesBound = false;
        }
    }
}