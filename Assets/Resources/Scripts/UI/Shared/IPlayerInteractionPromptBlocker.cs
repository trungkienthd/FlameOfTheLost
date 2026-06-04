using System;

namespace FlameOfTheLost
{
    public interface IPlayerInteractionPromptBlocker
    {
        public bool IsBlockingPlayerInteractionPrompt { get; }

        public event Action<IPlayerInteractionPromptBlocker> PromptBlockStarted;
        public event Action<IPlayerInteractionPromptBlocker> PromptBlockEnded;
    }
}   