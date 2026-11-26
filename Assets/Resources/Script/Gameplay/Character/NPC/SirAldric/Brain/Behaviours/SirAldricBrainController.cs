using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class SirAldricBrainController : NPCBrainController, 
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- Configs ---
        [Header("Configs - Companion Activation")]
        [SerializeField, Min(0.1f)] private float _playerActivationDistance = 3.5f;
        public float PlayerActivationDistance => _playerActivationDistance;

        // --- References ---
        private GameConditionController _conditions;
        private PlayerRootController _player;
        private GameConditionController GameConditions => _conditions;
        public PlayerRootController Player => _player;
        private NPCInputHandler NPCInput => this.NPCRootController?.NPCInputHandler;

        // --- Runtime ---
        [Header("Runtime - Story")]
        [SerializeField] private SirAldricStoryPhase _storyPhase = SirAldricStoryPhase.Imprisoned;
        public SirAldricStoryPhase StoryPhase => _storyPhase;

        [Header("Runtime - AI")]
        [SerializeField] private SirAldricAIState _currentAIState = SirAldricAIState.Imprisoned;
        public SirAldricAIState CurrentAIState => _currentAIState;

        [Header("Runtime - Death")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled => _isDeathDisabled;

        public bool IsReleased =>
            this.StoryPhase == SirAldricStoryPhase.Released ||
            this.StoryPhase == SirAldricStoryPhase.PostBoss;

        public bool IsFollowing =>
            this.CurrentAIState == SirAldricAIState.Following ||
            this.CurrentAIState == SirAldricAIState.Regrouping;

        public bool IsAllied =>
            this.NPCRootController != null &&
            this.NPCRootController.Faction == CharacterFaction.Ally;

        public bool IsHostile =>
            this.NPCRootController != null &&
            this.NPCRootController.Faction == CharacterFaction.Enemy;

        public NPCHostilityResponseType EffectiveHostilityResponseType
        {
            get
            {
                if (this.IsDeathDisabled) return NPCHostilityResponseType.None;

                if (this.IsHostile) return NPCHostilityResponseType.None;

                if (!this.IsReleased) return NPCHostilityResponseType.DialogueOnly;

                if (this.StoryPhase == SirAldricStoryPhase.PostBoss) return NPCHostilityResponseType.BecomeHostile;

                return NPCHostilityResponseType.DialogueOnly;
            }
        }

        public bool IsAvailableForCombat =>
            !this.IsDeathDisabled &&
            this.IsReleased &&
            (this.IsFollowing || this.CurrentAIState == SirAldricAIState.Alert || this.CurrentAIState == SirAldricAIState.Chasing || this.CurrentAIState == SirAldricAIState.Positioning || this.CurrentAIState == SirAldricAIState.NormalAttacking || this.CurrentAIState == SirAldricAIState.SpecialAttacking || this.CurrentAIState == SirAldricAIState.Recovering || this.CurrentAIState == SirAldricAIState.Retreating);

        // --- Events ---
        public event Action<SirAldricStoryPhase> OnStoryPhaseChanged;
        public event Action<SirAldricAIState> OnAIStateChanged;

        // --- Unity Callbacks ---
        private void Update()
        {
            if (this.IsDeathDisabled) return;

            if (this.CurrentAIState == SirAldricAIState.WaitingForPlayer)
            {
                this.TryActivateCompanion();
            }

            // Following and Combat behavior are implemented by dedicated Sir Aldric sub-behaviours later.
        }

        // --- Public APIs ---
        //      0.1. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 90;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            _isDeathDisabled = false;

            _player = UnityEngine.Object.FindFirstObjectByType<PlayerRootController>();

            if (this.GameConditions == null)
            {
                _conditions = GameConditionController.Instance;
            }

            this.StopMovement();

            this.NPCInput?.SetAIInputEnabled(false);

            this.ResolveStoryFromConditions(true);

            this.SynchronizeAIInput();
        }

        //      0.2. ICharacterDeathDisableable implementation
        public int DeathDisablePriority => 25;
        public void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            if (this.IsDeathDisabled) return;

            _isDeathDisabled = true;

            this.StopMovement();

            this.NPCInput?.SetAIInputEnabled(false);

            _currentAIState = SirAldricAIState.Dead;

            this.OnAIStateChanged?.Invoke(this.CurrentAIState);
        }

        //  1. AI Controlling
        public void SetAIState(SirAldricAIState nextState)
        {
            if (this.IsDeathDisabled && nextState != SirAldricAIState.Dead) return;

            if (this.CurrentAIState == nextState) return;

            this.StopMovement();

            _currentAIState = nextState;

            this.SynchronizeAIInput();

            this.OnAIStateChanged?.Invoke(nextState);
        }

        public bool TryActivateCompanion()
        {
            if (this.IsDeathDisabled) return false;
            if (!this.IsAllied) return false;

            if (this.CurrentAIState != SirAldricAIState.WaitingForPlayer) return false;

            if (this.Player == null) _player = UnityEngine.Object.FindFirstObjectByType<PlayerRootController>();
            if (this.Player == null) return false;
            if (this.Player.HPController != null && this.Player.HPController.IsDead) return false;

            float distance = Vector2.Distance(this.NPCRootController.transform.position, this.Player.transform.position);
            if (distance > this.PlayerActivationDistance) return false;

            this.SetAIState(SirAldricAIState.Following);

            return true;
        }

        //  2. World Refresh
        public void SynchronizeAfterWorldRestore()
        {
            if (this.IsDeathDisabled) return;

            _conditions = GameConditionController.Instance;

            _player = UnityEngine.Object .FindFirstObjectByType<PlayerRootController>();

            if (this.GameConditions == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(SirAldricBrainController)}] Cannot synchronize after World Refresh: GameConditionController is missing.", this);
                return;
            }

            this.StopMovement();

            this.NPCInput?.SetAIInputEnabled(false);

            this.ResolveStoryFromConditions(resetActivity: true);

            this.SynchronizeAIInput();
        }

        // --- Helpers ---
        //  1. Condition Resolution
        private void ResolveStoryFromConditions(bool resetActivity)
        {
            if (this.IsDeathDisabled) return;
            if (this.GameConditions == null) return;
            if (this.NPCRootController == null) return;

            bool accepted       = this.GameConditions.GetConditionState(GameConditionKey.QuestAnOathUnbrokenAccepted);
            bool freed          = this.GameConditions.GetConditionState(GameConditionKey.QuestAnOathUnbrokenAldricFreed);
            bool completed      = this.GameConditions.GetConditionState(GameConditionKey.QuestAnOathUnbrokenCompleted);
            bool bossDefeated   = this.GameConditions.GetConditionState(GameConditionKey.ForgottenSanctumGorVakhDefeated);
            bool betrayed       = this.GameConditions.GetConditionState(GameConditionKey.SirAldricBetrayed);

            SirAldricStoryPhase nextPhase;

            if (!accepted)
                nextPhase = SirAldricStoryPhase.Imprisoned;
            else if (!freed)
                nextPhase = SirAldricStoryPhase.AwaitingRelease;
            else if (completed || bossDefeated)
                nextPhase = SirAldricStoryPhase.PostBoss;
            else
                nextPhase = SirAldricStoryPhase.Released;

            bool phaseChanged = this.StoryPhase != nextPhase;

            _storyPhase = nextPhase;

            if (phaseChanged) this.OnStoryPhaseChanged?.Invoke(nextPhase);

            CharacterFaction nextFaction;

            if (betrayed)
                nextFaction = CharacterFaction.Enemy;
            else if (freed && accepted)
                nextFaction = CharacterFaction.Ally;
            else
                nextFaction = CharacterFaction.None;

            bool factionChanged = this.NPCRootController.Faction != nextFaction;

            this.NPCRootController.SetRuntimeFaction(nextFaction);

            if (!phaseChanged && !factionChanged && !resetActivity) return;

            this.StopMovement();

            if (betrayed)
            {
                this.SetAIState(SirAldricAIState.HostileAlert);
            }
            else
            {
                switch (nextPhase)
                {
                    case SirAldricStoryPhase.Imprisoned:
                    case SirAldricStoryPhase.AwaitingRelease:
                        this.SetAIState(SirAldricAIState.Imprisoned);
                        break;

                    case SirAldricStoryPhase.Released:
                        this.SetAIState(SirAldricAIState.WaitingForPlayer);
                        break;

                    case SirAldricStoryPhase.PostBoss:
                        this.SetAIState(SirAldricAIState.Resting);
                        break;
                }
            }
        }

        private void HandleConditionChanged(GameConditionKey key, bool value)
        {
            switch (key)
            {
                case GameConditionKey.QuestAnOathUnbrokenAccepted:
                case GameConditionKey.QuestAnOathUnbrokenAldricFreed:
                case GameConditionKey.QuestAnOathUnbrokenCompleted:
                case GameConditionKey.ForgottenSanctumGorVakhDefeated:
                case GameConditionKey.SirAldricBetrayed:
                    this.ResolveStoryFromConditions(false);
                    break;
            }
        }

        private bool ShouldEnableAIMovement(SirAldricAIState state)
        {
            return state switch
            {
                SirAldricAIState.Following => true,
                SirAldricAIState.Regrouping => true,
                SirAldricAIState.Chasing => true,
                SirAldricAIState.Positioning => true,
                SirAldricAIState.Retreating => true,

                _ => false
            };
        }

        private void SynchronizeAIInput()
        {
            if (this.NPCInput == null) return;

            bool enableMovement = !this.IsDeathDisabled && this.ShouldEnableAIMovement(this.CurrentAIState);

            this.NPCInput.SetAIInputEnabled(enableMovement);
        }

        private void StopMovement()
        {
            this.NPCInput?.ClearMoveInput();

            this.NPCRootController?.MovementHandler ?.SetAIMovementSpeedMultiplier(1f);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _isDeathDisabled = false;

            _storyPhase = SirAldricStoryPhase.Imprisoned;
            _currentAIState = SirAldricAIState.Imprisoned;

            this.NPCInput?.SetAIInputEnabled(false);
        }
        protected override void InitializeGameplay()
        {
            base.InitializeGameplay();

            _player = UnityEngine.Object.FindFirstObjectByType<PlayerRootController>();

            _conditions = GameConditionController.Instance;

            if (this.GameConditions != null)
            {
                _conditions.ConditionStateChanged += this.HandleConditionChanged;

                this.ResolveStoryFromConditions(true);

                this.SynchronizeAIInput();
            }
            else
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(SirAldricBrainController)}] {nameof(GameConditionController)} missing.", this);
            }
        }
        protected override void CleanupStates()
        {
            if (this.GameConditions != null)
            {
                _conditions.ConditionStateChanged -= this.HandleConditionChanged;
            }

            this.StopMovement();

            base.CleanupStates();
        }
    }
}