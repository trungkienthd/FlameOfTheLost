using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(GameConditionStateHandler))]
    [RequireComponent(typeof(GameConditionSaveHandler))]
    [RequireComponent(typeof(GameConditionSceneHandler))]
    public class GameConditionController : FOTLSingletonMonoBehaviour<GameConditionController>, ISaveDataHandler<GameConditionSaveData>
    {
        // --- Singleton ---
        protected override bool PersistAcrossScenes => true;

        // --- References ---
        //  1. Sub-Behaviours - Game Condition Common 
        [Header("References - Sub-Behaviours - Game Condition Common)")]
        [SerializeField] private GameConditionStateHandler _stateHandler;
        [SerializeField] private GameConditionSaveHandler _saveHandler;
        [SerializeField] private GameConditionSceneHandler _sceneHandler;
        private GameConditionStateHandler StateHandler  => _stateHandler;
        private GameConditionSaveHandler SaveHandler    => _saveHandler;
        private GameConditionSceneHandler SceneHandler  => _sceneHandler;

        //  2. Sub-behaviours - Progress Controllers
        [Header("References - Sub-behaviours - Progress Controllers)")]
        [SerializeField] private TutorialProgressController     _tutorialProgressController;
        [SerializeField] private MilestoneProgressController    _milestoneProgressController;
        [SerializeField] private QuestProgressController        _questProgressController;
        private TutorialProgressController TutorialProgressController   => _tutorialProgressController;
        private MilestoneProgressController MilestoneProgressController => _milestoneProgressController;
        private QuestProgressController QuestProgressController         => _questProgressController;

        // --- Data / Configs ---
        [Header("Configs - Initial Condition States")]
        [SerializeField] private List<GameConditionInitialStateEntry> _initialStateEntries = new();
        internal IReadOnlyList<GameConditionInitialStateEntry> InitialStateEntries => _initialStateEntries;

        // --- Runtime ---
        //  1. Authoritative Runtime State
        private GameConditionStateMap _conditionStates = new();
        public IReadOnlyDictionary<GameConditionKey, bool> ConditionStates => _conditionStates;

        internal GameConditionStateMap MutableConditionStates
        {
            get
            {
                _conditionStates ??= new GameConditionStateMap();

                return _conditionStates;
            }
        }

        //  2. Inspector / Debug Mirror
        [Header("Runtime - Condition States")]
        [SerializeField] private List<GameConditionStateEntry> _conditionStateEntries = new();
        private IReadOnlyList<GameConditionStateEntry> ConditionStateEntries => _conditionStateEntries;

        internal List<GameConditionStateEntry> MutableConditionStateEntries
        {
            get
            {
                _conditionStateEntries ??= new List<GameConditionStateEntry>();

                return _conditionStateEntries;
            }
        }

        //  3. Runtime Ref
        private bool _gameplayReferencesBound;

        // --- Events ---
        public event Action<GameConditionKey, bool> ConditionStateChanged;

        // --- Public APIs ---
        //  1. Gameplay Reference Lifecycle
        public bool BindGameplayReferences()
        {
            if (_gameplayReferencesBound) return true;

            if (this.TutorialProgressController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(GameConditionController)}] Cannot bind gameplay references because {nameof(this.TutorialProgressController)} is missing.", this);
                return false;
            }
            if (this.MilestoneProgressController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(GameConditionController)}] Cannot bind gameplay references because {nameof(this.MilestoneProgressController)} is missing.", this);
                return false;
            }
            if (this.QuestProgressController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(GameConditionController)}] Cannot bind gameplay references because {nameof(this.QuestProgressController)} is missing.", this);
                return false;
            }

            // a. Bind tutorial progression.
            if (!this.TutorialProgressController.BindGameplayReferences())
            {
                this.UnbindProgressControllerGameplayReferences();
                return false;
            }

            // b. Bind milestone progression.
            if (!this.MilestoneProgressController.BindGameplayReferences())
            {
                this.UnbindProgressControllerGameplayReferences();
                return false;
            }

            // c. Bind quest progression.
            if (!this.QuestProgressController.BindGameplayReferences())
            {
                this.UnbindProgressControllerGameplayReferences();
                return false;
            }

            _gameplayReferencesBound = true;

            return true;
        }
        public void UnbindGameplayReferences()
        {
            this.UnbindProgressControllerGameplayReferences();

            _gameplayReferencesBound = false;
        }

        //  2. State Queries
        public bool GetConditionState(GameConditionKey conditionKey)
        {
            if (this.StateHandler == null) return false;

            return this.StateHandler.GetConditionState(conditionKey);
        }

        //  3. State Updates
        //  3.1. Individual State Updates
        public void SetConditionState(GameConditionKey conditionKey, bool value)
        {
            this.StateHandler?.SetConditionState(conditionKey, value);
        }
        public void SatisfyCondition(GameConditionKey conditionKey)
        {
            this.SetConditionState(conditionKey, true);
        }
        public void UnsatisfyCondition(GameConditionKey conditionKey)
        {
            this.SetConditionState(conditionKey, false);
        }

        //  3.2. Condition Operations
        public bool CanApplyConditionOperations(IReadOnlyList<GameConditionOperationEntry> operations)
        {
            if (this.StateHandler == null) return false;

            return this.StateHandler.CanApplyConditionOperations(operations);
        }
        public bool TryApplyConditionOperations(IReadOnlyList<GameConditionOperationEntry> operations)
        {
            if (this.StateHandler == null) return false;

            return this.StateHandler.TryApplyConditionOperations(operations);
        }

        //  4. Reset
        public void ResetConditions()
        {
            this.StateHandler?.ResetConditions();
        }

        //  5. ISaveDataHandler implementation
        public GameConditionSaveData CaptureSaveData()
        {
            if (this.SaveHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(GameConditionController)}] Cannot capture save data because {nameof(this.SaveHandler)} is missing.", this);
                return null;
            }

            return this.SaveHandler.CaptureSaveData();
        }
        public void RestoreSaveData(GameConditionSaveData saveData)
        {
            this.SaveHandler?.RestoreSaveData(saveData);
        }

        // --- Internal APIs ---
        //  1. Condition State Event Publishing
        internal void PublishConditionStateChanged(GameConditionKey conditionKey, bool value)
        {
            this.ConditionStateChanged?.Invoke(conditionKey, value);
        }

        //  2. Gameplay Reference Lifecycle
        internal void HandleGameplaySceneChanged()
        {
            this.UnbindGameplayReferences();
        }

        //  3. Save Restoration Support
        internal void RestoreConditionStates(IReadOnlyDictionary<GameConditionKey, bool> savedStates)
        {
            this.StateHandler?.RestoreConditionStates(savedStates);
        }

        // --- Helpers ---
        //  1. Progress Controller Cleanup
        private void UnbindProgressControllerGameplayReferences()
        {
            this.QuestProgressController?.UnbindGameplayReferences();
            this.MilestoneProgressController?.UnbindGameplayReferences();
            this.TutorialProgressController?.UnbindGameplayReferences();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.StateHandler   == null) _stateHandler  = this.GetComponent<GameConditionStateHandler>();
            if (this.SaveHandler    == null) _saveHandler   = this.GetComponent<GameConditionSaveHandler>();
            if (this.SceneHandler   == null) _sceneHandler  = this.GetComponent<GameConditionSceneHandler>();

            if (this.TutorialProgressController     == null) _tutorialProgressController    = this.GetComponentInChildren<TutorialProgressController>(true);
            if (this.MilestoneProgressController    == null) _milestoneProgressController   = this.GetComponentInChildren<MilestoneProgressController>(true);
            if (this.QuestProgressController        == null) _questProgressController       = this.GetComponentInChildren<QuestProgressController>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.StateHandler   == null) Debug.LogError($"[{this.gameObject.name}.{nameof(GameConditionController)}] {nameof(this.StateHandler)} reference is not assigned.", this);
            if (this.SaveHandler    == null) Debug.LogError($"[{this.gameObject.name}.{nameof(GameConditionController)}] {nameof(this.SaveHandler)} reference is not assigned.", this);
            if (this.SceneHandler   == null) Debug.LogError($"[{this.gameObject.name}.{nameof(GameConditionController)}] {nameof(this.SceneHandler)} reference is not assigned.", this);

            if (this.TutorialProgressController     == null) Debug.LogError($"[{this.gameObject.name}.{nameof(GameConditionController)}] {nameof(this.TutorialProgressController)} reference is not assigned.", this);
            if (this.MilestoneProgressController    == null) Debug.LogError($"[{this.gameObject.name}.{nameof(GameConditionController)}] {nameof(this.MilestoneProgressController)} reference is not assigned.", this);
            if (this.QuestProgressController        == null) Debug.LogError($"[{this.gameObject.name}.{nameof(GameConditionController)}] {nameof(this.QuestProgressController)} reference is not assigned.", this);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.StateHandler?.Bind(this);
            this.SaveHandler?.Bind(this);
            this.SceneHandler?.Bind(this);

            this.TutorialProgressController?.Bind(this);
            this.MilestoneProgressController?.Bind(this);
            this.QuestProgressController?.Bind(this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _gameplayReferencesBound = false;

            this.StateHandler?.InitializeConditionStates();
        }
        protected override void CleanupStates()
        {
            this.UnbindGameplayReferences();

            base.CleanupStates();
        }
        protected override void UnbindInternalReferences()
        {
            this.QuestProgressController?.Unbind(this);
            this.MilestoneProgressController?.Unbind(this);
            this.TutorialProgressController?.Unbind(this);

            this.SceneHandler?.Unbind(this);
            this.SaveHandler?.Unbind(this);
            this.StateHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
