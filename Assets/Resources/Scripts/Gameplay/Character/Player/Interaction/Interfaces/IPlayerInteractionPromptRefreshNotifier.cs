using System;

namespace FlameOfTheLost
{
    public interface IPlayerInteractionPromptRefreshNotifier
    {
        public event Action PromptRefreshRequested;
    }
}
