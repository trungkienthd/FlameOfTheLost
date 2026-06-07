using UnityEngine;
using UnityEngine.EventSystems;

namespace FlameOfTheLost
{
    public class LoadoutPanelBackgroundClickClearAreaHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private EquippedLoadoutSubPanelController _equippedLoadoutSubPanelController;
        private EquippedLoadoutSubPanelController EquippedLoadoutSubPanelController => _equippedLoadoutSubPanelController;

        private void Awake()
        {
            if (this.EquippedLoadoutSubPanelController == null)
            {
                Transform equippedLoadoutSubPanelTransform = this.transform.parent.Find("EquippedLoadoutSubPanel");
                if (equippedLoadoutSubPanelTransform != null)
                {
                    _equippedLoadoutSubPanelController = equippedLoadoutSubPanelTransform.GetComponent<EquippedLoadoutSubPanelController>();
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (this.EquippedLoadoutSubPanelController == null) return;
            this.EquippedLoadoutSubPanelController.ClearSelection();
        }
    }
}
