using UnityEngine;

namespace FlameOfTheLost
{
    public class LoadoutPanelController : PlayerMenuPanelController
    {
        [Header("Sub Panels")]
        [SerializeField] private EquippedLoadoutSubPanelController _equippedLoadoutSubPanelController;
        [SerializeField] private CandidateLoadoutSubPanelController _candidateLoadoutSubPanelController;
        private EquippedLoadoutSubPanelController EquippedLoadoutSubPanelController => _equippedLoadoutSubPanelController;
        private CandidateLoadoutSubPanelController CandidateLoadoutSubPanelController => _candidateLoadoutSubPanelController; 

        private void Awake()
        {
            if (this.EquippedLoadoutSubPanelController == null) _equippedLoadoutSubPanelController = this.GetComponentInChildren<EquippedLoadoutSubPanelController>();
            if (this.CandidateLoadoutSubPanelController == null) _candidateLoadoutSubPanelController = this.GetComponentInChildren<CandidateLoadoutSubPanelController>();
        }

        public override void ShowPanel()
        {
            this.gameObject.SetActive(true);

            if (this.EquippedLoadoutSubPanelController != null)
            {
                this.EquippedLoadoutSubPanelController.gameObject.SetActive(true);
                this.EquippedLoadoutSubPanelController.ClearSelection();
                this.EquippedLoadoutSubPanelController.RefreshAll();
            }

            if (this.CandidateLoadoutSubPanelController != null)
            {
                this.CandidateLoadoutSubPanelController.HidePanel();
            }
        }

        public override void HidePanel()
        {
            if (this.EquippedLoadoutSubPanelController != null)
            {
                this.EquippedLoadoutSubPanelController.ClearSelection();
            }

            if (this.CandidateLoadoutSubPanelController != null)
            {
                this.CandidateLoadoutSubPanelController.HidePanel();
            }

            this.gameObject.SetActive(false);
        }
    }
}