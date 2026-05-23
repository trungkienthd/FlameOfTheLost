using UnityEngine;

namespace FlameOfTheLost
{
    public class InputPromptTriggerController : MonoBehaviour, IPlayerInteractionPromptSource
    {
        // --- States ---
        //  1. Prompt Settings
        [Header("Prompt Settings")]
        [SerializeField, TextArea(5, 5)] private string _promptText = "Tutorial prompt.";
        [SerializeField] private bool _startEnabled = true;
        [SerializeField] private bool _hideAfterCompleted = true;
        private string PromptText => _promptText;
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
        private void Awake()
        {
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
            return this.PromptText;
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