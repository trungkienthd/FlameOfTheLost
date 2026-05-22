using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FlameOfTheLost
{
    public class ItemContainerPanelBackgroundClickClearAreaHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ItemContainerPanelController _itemContainerPanelController;
        private ItemContainerPanelController ItemContainerPanelController => _itemContainerPanelController;

        private void Awake()
        {
            if (this.ItemContainerPanelController == null)
            {
                Transform itemContainerPanelTransform = this.transform.parent.Find("ItemContainerPanel");
                if (itemContainerPanelTransform != null)
                {
                    _itemContainerPanelController = itemContainerPanelTransform.GetComponent<ItemContainerPanelController>();
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (this.ItemContainerPanelController == null) return;
            this.ItemContainerPanelController.Close();
        }
    }
}
