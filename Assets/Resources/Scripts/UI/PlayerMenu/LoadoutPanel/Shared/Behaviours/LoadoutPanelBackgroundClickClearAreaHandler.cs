using UnityEngine;
using UnityEngine.EventSystems;

namespace FlameOfTheLost
{
    public class LoadoutPanelBackgroundClickClearAreaHandler : FOTLMonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private EquippedLoadoutSubPanelController _equippedLoadoutSubPanelController;
        private EquippedLoadoutSubPanelController EquippedLoadoutSubPanelController => _equippedLoadoutSubPanelController;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (this.EquippedLoadoutSubPanelController == null) return;
            this.EquippedLoadoutSubPanelController.ClearSelection();
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.EquippedLoadoutSubPanelController != null) return;
            if (this.transform.parent == null) return;

            Transform equippedLoadoutSubPanelTransform =
                this.transform.parent.Find("EquippedLoadoutSubPanel");

            if (equippedLoadoutSubPanelTransform == null) return;

            _equippedLoadoutSubPanelController = equippedLoadoutSubPanelTransform.GetComponent<EquippedLoadoutSubPanelController>();
        }
    }
}
