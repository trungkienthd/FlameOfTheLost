using UnityEngine.EventSystems;

namespace FlameOfTheLost
{
    public class CraftingPanelBackgroundClickClearAreaHandler : CraftingPanelSubHandlerBase, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!this.EnsureBound()) return;

            this.CraftingPanelController.Close();
        }
    }
}
