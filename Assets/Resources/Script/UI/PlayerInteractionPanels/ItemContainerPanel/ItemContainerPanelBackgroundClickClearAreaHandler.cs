using UnityEngine.EventSystems;

namespace FlameOfTheLost
{
    public class ItemContainerPanelBackgroundClickClearAreaHandler : ItemContainerPanelSubHandlerBase, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!this.EnsureBound()) return;

            this.ItemContainerPanelController.Close();
        }
    }
}
