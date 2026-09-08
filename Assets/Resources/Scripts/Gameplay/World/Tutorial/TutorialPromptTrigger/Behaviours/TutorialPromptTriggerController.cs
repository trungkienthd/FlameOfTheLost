using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class TutorialPromptTriggerController : FOTLWorldObjectProfileBehaviour<TutorialPromptTriggerProfile>,
        IPlayerInteractionPromptSource,
        IPlayerInteractionPromptRefreshNotifier
    {
        // --- References ---
        [Header("References (Internal - Trigger Area)")]
        [SerializeField] private GameObject _interactionTriggerArea;
        private GameObject InteractionTriggerArea => _interactionTriggerArea;

        [Header("References (External - Game Condition")]
        [SerializeField] private GameConditionController _gameConditionController;
        private GameConditionController GameConditionController => _gameConditionController;

        // --- Data / Configs ---
        //  1. Prompt Settings
        public int InteractionPromptPriority => this.Profile != null ? this.Profile.InteractionPromptPriority : 0;
        private bool StartEnabled => this.Profile != null ? this.Profile.StartEnabled : true;
        private bool HideAfterCompleted => this.Profile != null ? this.Profile.HideAfterCompleted : true;

        //  2. Interaction Trigger Area
        private ColliderConfigurationEntry InteractionTriggerColliderConfiguration => this.Profile != null ? this.Profile.InteractionTriggerColliderConfiguration : null;

        //  3. Trigger Conditions
        private GameConditionCombinationType ConditionCombinationType => this.Profile != null ? this.Profile.ConditionCombinationType : GameConditionCombinationType.All;
        private IReadOnlyList<GameConditionRequirementEntry> ConditionEntries => this.Profile != null ? this.Profile.ConditionEntries : Array.Empty<GameConditionRequirementEntry>();

        // --- Runtime ---
        //  1. Runtime State
        [Header("Prompt State (Runtime)")]
        [SerializeField] private bool _isPromptEnabled;
        [SerializeField] private bool _isCompleted;
        public bool IsPromptEnabled
        {
            get => _isPromptEnabled;
            private set => _isPromptEnabled = value;
        }
        public bool IsCompleted
        {
            get => _isCompleted;
            private set => _isCompleted = value;
        }

        // --- Events ---
        public event Action PromptRefreshRequested;

        // --- Public APIs ---
        //  1. IPlayerInteractionPromptSource implementation
        public bool CanShowInteractionPrompt(GameObject interactor)
        {
            if (!this.gameObject.activeInHierarchy) return false;
            if (!this.IsPromptEnabled) return false;
            if (this.HideAfterCompleted && this.IsCompleted) return false;
            if (!this.AreTriggerConditionsSatisfied()) return false;

            return true;
        }
        public string GetInteractionPromptText(GameObject interactor)
        {
            return this.Description;
        }

        //  2. Enable/Disable Prompt
        public void EnablePrompt()
        {
            if (this.IsPromptEnabled) return;

            this.IsPromptEnabled = true;
            this.RequestPromptRefresh();
        }
        public void DisablePrompt()
        {
            if (!this.IsPromptEnabled) return;

            this.IsPromptEnabled = false;
            this.RequestPromptRefresh();
        }

        //  3. Tutorial Prompt is completed
        public void MarkCompleted()
        {
            if (this.IsCompleted) return;

            this.IsCompleted = true;

            if (this.HideAfterCompleted)
            {
                this.DisablePrompt();
            }
            else
            {
                this.RequestPromptRefresh();
            }
        }

        //  4. Reset
        public void ResetPrompt()
        {
            this.IsCompleted = false;
            this.IsPromptEnabled = this.StartEnabled;

            this.RequestPromptRefresh();
        }

        // --- Helpers ---
        private void InitializeInteractionTriggerArea()
        {
            if (this.InteractionTriggerArea == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialPromptTriggerController)}] " + $"Interaction Trigger Area is not assigned.", this);
                return;
            }

            if (this.InteractionTriggerColliderConfiguration == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialPromptTriggerController)}] " + $"Interaction Trigger Collider Configuration is not assigned.", this);
                return;
            }

            this.InteractionTriggerColliderConfiguration.ConfigureCollider(this.InteractionTriggerArea);
        }

        private bool AreTriggerConditionsSatisfied()
        {
            return GameConditionEvaluator.AreConditionsSatisfied(this.ConditionEntries, this.ConditionCombinationType, this.GameConditionController);
        }

        private bool UsesCondition(GameConditionKey conditionKey)
        {
            for (int i = 0; i < this.ConditionEntries.Count; i++)
            {
                GameConditionRequirementEntry requirement = this.ConditionEntries[i];

                if (requirement == null) continue;

                if (requirement.ConditionKey == conditionKey) return true;
            }

            return false;
        }

        private void RequestPromptRefresh()
        {
            this.PromptRefreshRequested?.Invoke();
        }

        //  2. Event Handlers
        private void HandleConditionStateChanged(GameConditionKey conditionKey, bool value)
        {
            if (!this.UsesCondition(conditionKey)) return;

            this.RequestPromptRefresh();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.InteractionTriggerArea == null)
            {
                Transform interactionTriggerAreaTransform = this.transform.Find("InteractionTriggerArea");
                if (interactionTriggerAreaTransform != null) _interactionTriggerArea = interactionTriggerAreaTransform.gameObject;
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.InitializeInteractionTriggerArea();

            this.IsPromptEnabled = this.StartEnabled;
            this.IsCompleted = false;
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.GameConditionController == null) _gameConditionController = UnityEngine.Object.FindFirstObjectByType<GameConditionController>();
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.GameConditionController == null) Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialPromptTriggerController)}] {nameof(this.GameConditionController)} reference is not assigned.", this);
        }
        protected override void BindExternalReferences()
        {
            base.BindExternalReferences();

            if (this.GameConditionController != null) this.GameConditionController.ConditionStateChanged += this.HandleConditionStateChanged;
        }
        protected override void UnbindExternalReferences()
        {
            if (this.GameConditionController != null) this.GameConditionController.ConditionStateChanged -= this.HandleConditionStateChanged;

            base.UnbindExternalReferences();
        }
    }
}