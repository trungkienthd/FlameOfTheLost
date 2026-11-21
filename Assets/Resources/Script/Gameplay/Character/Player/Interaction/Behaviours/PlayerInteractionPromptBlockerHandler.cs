using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerInteractionPromptBlockerHandler : PlayerInteractionSubHandlerBase
    {
        // --- Events ---
        public event Action<bool> PromptBlockingStateChanged;

        // --- References ---
        [Header("References")]
        [SerializeField] private PlayerMenuManager _playerMenuManager;
        [SerializeField] private PlayerInteractionPanelListManager _playerInteractionPanelListManager;
        private PlayerMenuManager PlayerMenuManager => _playerMenuManager;
        private PlayerInteractionPanelListManager PlayerInteractionPanelListManager => _playerInteractionPanelListManager;

        // --- Runtime ---
        private readonly HashSet<IPlayerInteractionPromptBlocker> _registeredPromptBlockers = new();
        private readonly HashSet<IPlayerInteractionPromptBlocker> _activePromptBlockers = new();
        private IReadOnlyCollection<IPlayerInteractionPromptBlocker> RegisteredPromptBlockers => _registeredPromptBlockers;
        public IReadOnlyCollection<IPlayerInteractionPromptBlocker> ActivePromptBlockers => _activePromptBlockers;
        public bool IsPromptBlocked => this.ActivePromptBlockers.Count > 0;

        // --- Public APIs ---
        //  1. Prompt Blocker Registration
        public void RegisterPromptBlocker(IPlayerInteractionPromptBlocker blocker)
        {
            if (blocker == null) return;
            if (!_registeredPromptBlockers.Add(blocker)) return;

            blocker.PromptBlockStarted += this.HandlePromptBlockStarted;
            blocker.PromptBlockEnded += this.HandlePromptBlockEnded;

            if (blocker.IsBlockingPlayerInteractionPrompt)
            {
                this.SetBlockerActive(blocker, true);
            }
        }
        public void UnregisterPromptBlocker(IPlayerInteractionPromptBlocker blocker)
        {
            if (blocker == null) return;
            if (!_registeredPromptBlockers.Remove(blocker)) return;

            blocker.PromptBlockStarted -= this.HandlePromptBlockStarted;
            blocker.PromptBlockEnded -= this.HandlePromptBlockEnded;

            this.SetBlockerActive(blocker, false);
        }

        //  2. Prompt Blocker Cleanup
        public void ClearPromptBlockers(bool notifyStateChange = true)
        {
            foreach (IPlayerInteractionPromptBlocker blocker in this.RegisteredPromptBlockers)
            {
                if (blocker == null) continue;

                blocker.PromptBlockStarted -= this.HandlePromptBlockStarted;
                blocker.PromptBlockEnded -= this.HandlePromptBlockEnded;
            }

            bool wasBlocked = this.IsPromptBlocked;

            _registeredPromptBlockers.Clear();
            _activePromptBlockers.Clear();

            if (notifyStateChange && wasBlocked)
            {
                this.PromptBlockingStateChanged?.Invoke(false);
            }
        }

        // --- Helpers ---
        //  1. State Management
        private void SetBlockerActive(IPlayerInteractionPromptBlocker blocker, bool isActive)
        {
            if (blocker == null) return;

            bool wasBlocked = this.IsPromptBlocked;

            if (isActive)
            {
                _activePromptBlockers.Add(blocker);
            }
            else
            {
                _activePromptBlockers.Remove(blocker);
            }

            bool isBlocked = this.IsPromptBlocked;

            if (wasBlocked == isBlocked) return;

            this.PromptBlockingStateChanged?.Invoke(isBlocked);
        }

        //  2. Event Handling
        private void HandlePromptBlockStarted(IPlayerInteractionPromptBlocker blocker)
        {
            this.SetBlockerActive(blocker, true);
        }
        private void HandlePromptBlockEnded(IPlayerInteractionPromptBlocker blocker)
        {
            this.SetBlockerActive(blocker, false);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _registeredPromptBlockers.Clear();
            _activePromptBlockers.Clear();
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null) return;

            if (this.PlayerMenuManager                  == null) _playerMenuManager                 = canvas.GetComponentInChildren<PlayerMenuManager>(true);
            if (this.PlayerInteractionPanelListManager  == null) _playerInteractionPanelListManager = canvas.GetComponentInChildren<PlayerInteractionPanelListManager>(true);
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.PlayerMenuManager                  == null) Debug.LogError($"[{this.InteractionHandler.CharacterName}.{nameof(PlayerInteractionPromptBlockerHandler)}] " + $"Player Menu Manager reference is not assigned.", this);
            if (this.PlayerInteractionPanelListManager  == null) Debug.LogError($"[{this.InteractionHandler.CharacterName}.{nameof(PlayerInteractionPromptBlockerHandler)}] " + $"Player Interaction Panel List Manager reference is not assigned.", this);
        }
        protected override void BindExternalReferences()
        {
            base.BindExternalReferences();

            this.RegisterPromptBlocker(this.PlayerMenuManager);
            this.RegisterPromptBlocker(this.PlayerInteractionPanelListManager);
        }
        protected override void CleanupStates()
        {
            this.ClearPromptBlockers(false);

            base.CleanupStates();
        }
    }
}