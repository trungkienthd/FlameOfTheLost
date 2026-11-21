using UnityEngine.EventSystems;

namespace FlameOfTheLost
{
    public class MainMenuBackgroundClickClearAreaHandler : MainMenuSubBehaviourBase, IPointerClickHandler
    {
        // --- Unity Callbacks ---
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!this.EnsureBound()) return;

            if (eventData.button != PointerEventData.InputButton.Left) return;

            this.MainMenuController.HandleCancel();
        }
    }
}