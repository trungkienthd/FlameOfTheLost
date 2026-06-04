using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerInteractionPanelListManager : MonoBehaviour, IPlayerInteractionPromptBlocker
    {
        private PlayerInteractionBasePanelController CurrentOpenPanel
        {
            get;
            set;
        }

        public bool HasOpenPanel => this.CurrentOpenPanel != null && this.CurrentOpenPanel.IsOpen;

        public bool IsBlockingPlayerInteractionPrompt => this.HasOpenPanel;

        public event Action<IPlayerInteractionPromptBlocker> PromptBlockStarted;
        public event Action<IPlayerInteractionPromptBlocker> PromptBlockEnded;

        public void RegisterOpenedPanel(PlayerInteractionBasePanelController panel)
        {
            if (panel == null) return;

            bool wasBlocking = this.IsBlockingPlayerInteractionPrompt;

            if (this.CurrentOpenPanel != null && this.CurrentOpenPanel != panel)
            {
                PlayerInteractionBasePanelController oldPanel = this.CurrentOpenPanel;

                this.CurrentOpenPanel = panel;

                oldPanel.Close();
            }
            else
            {
                this.CurrentOpenPanel = panel;
            }

            if (!wasBlocking && this.IsBlockingPlayerInteractionPrompt)
            {
                this.PromptBlockStarted?.Invoke(this);
            }
        }

        public void RegisterClosedPanel(PlayerInteractionBasePanelController panel)
        {
            if (this.CurrentOpenPanel != panel) return;

            bool wasBlocking = this.IsBlockingPlayerInteractionPrompt;

            this.CurrentOpenPanel = null;

            if (wasBlocking && !this.IsBlockingPlayerInteractionPrompt)
            {
                this.PromptBlockEnded?.Invoke(this);
            }
        }

        public void CloseCurrentPanel()
        {
            if (this.CurrentOpenPanel == null) return;

            this.CurrentOpenPanel.Close();
        }
    }
}
