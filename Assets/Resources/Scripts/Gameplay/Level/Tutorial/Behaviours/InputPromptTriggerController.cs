using UnityEngine;

namespace FlameOfTheLost
{
    public class InputPromptTriggerController : FOTLMonoBehaviour, IIdentityProvider, IPlayerInteractionPromptSource
    {
        // --- States ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _id = 4100;
        [SerializeField] private string _key = "TUTORIAL_PROMPT";
        [SerializeField] private string _displayName = "Tutorial Prompt";
        [SerializeField, TextArea(5, 5)] private string _description = "Tutorial prompt.";
        public int Id => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        //  2. Prompt Settings
        [Header("Prompt Settings")]
        [SerializeField] private bool _startEnabled = true;
        [SerializeField] private bool _hideAfterCompleted = true;
        private bool StartEnabled => _startEnabled;
        private bool HideAfterCompleted => _hideAfterCompleted;

        //  2. Runtime State
        private bool _isPromptEnabled;
        private bool _isCompleted;
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

        // --- Unity Callbacks ---
        protected override void Awake()
        {
            base.Awake();
            this.IsPromptEnabled = this.StartEnabled;
        }

        // --- Public APIs ---
        //  1. IPlayerInteractionPromptSource implementation
        public bool CanShowInteractionPrompt(GameObject interactor)
        {
            if (!this.gameObject.activeInHierarchy) return false;
            if (!this.IsPromptEnabled) return false;
            if (this.HideAfterCompleted && this.IsCompleted) return false;

            return true;
        }
        public string GetInteractionPromptText(GameObject interactor)
        {
            return this.Description;
        }

        //  2. Enable/Disable Prompt
        public void EnablePrompt()
        {
            this.IsPromptEnabled = true;
        }
        public void DisablePrompt()
        {
            this.IsPromptEnabled = false;
        }

        //  3. Tutorial Prompt is completed
        public void MarkCompleted()
        {
            this.IsCompleted = true;

            if (this.HideAfterCompleted)
            {
                this.DisablePrompt();
            }
        }

        //  4. Reset
        public void ResetPrompt()
        {
            this.IsCompleted = false;
            this.IsPromptEnabled = this.StartEnabled;
        }
    }
}