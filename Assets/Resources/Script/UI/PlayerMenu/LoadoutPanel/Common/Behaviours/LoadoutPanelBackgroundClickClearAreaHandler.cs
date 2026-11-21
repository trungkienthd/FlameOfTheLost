using UnityEngine.EventSystems;

namespace FlameOfTheLost
{
    public class LoadoutPanelBackgroundClickClearAreaHandler : LoadoutPanelSubBehaviourBase, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!this.EnsureBound()) return;

            this.LoadoutPanelController.EquippedLoadoutSubPanelController?.ClearSelection();
        }
    }
}
