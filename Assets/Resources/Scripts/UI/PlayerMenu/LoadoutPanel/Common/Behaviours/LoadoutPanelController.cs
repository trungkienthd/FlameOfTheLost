using UnityEngine;

namespace FlameOfTheLost
{
    public class LoadoutPanelController : PlayerMenuPanelController
    {
        // --- References ---
        [Header("References (Sub-Behaviours)")]
        [SerializeField] private LoadoutPanelBackgroundClickClearAreaHandler _backgroundClickClearAreaHandler;
        [SerializeField] private EquippedLoadoutSubPanelController _equippedLoadoutSubPanelController;
        [SerializeField] private CandidateLoadoutSubPanelController _candidateLoadoutSubPanelController;
        private LoadoutPanelBackgroundClickClearAreaHandler BackgroundClickClearAreaHandler => _backgroundClickClearAreaHandler;
        internal EquippedLoadoutSubPanelController EquippedLoadoutSubPanelController => _equippedLoadoutSubPanelController;
        internal CandidateLoadoutSubPanelController CandidateLoadoutSubPanelController => _candidateLoadoutSubPanelController;

        // --- Public APIs ---
        public override void ShowPanel()
        {
            this.gameObject.SetActive(true);

            if (this.EquippedLoadoutSubPanelController != null)
            {
                this.EquippedLoadoutSubPanelController.gameObject.SetActive(true);
                this.EquippedLoadoutSubPanelController.ClearSelection();
                this.EquippedLoadoutSubPanelController.RefreshAll();
            }

            this.CandidateLoadoutSubPanelController?.HidePanel();
        }
        public override void HidePanel()
        {
            this.EquippedLoadoutSubPanelController?.ClearSelection();
            this.CandidateLoadoutSubPanelController?.HidePanel();

            this.gameObject.SetActive(false);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.BackgroundClickClearAreaHandler    == null) _backgroundClickClearAreaHandler       = this.GetComponentInChildren<LoadoutPanelBackgroundClickClearAreaHandler>(true);
            if (this.EquippedLoadoutSubPanelController  == null) _equippedLoadoutSubPanelController     = this.GetComponentInChildren<EquippedLoadoutSubPanelController>(true);
            if (this.CandidateLoadoutSubPanelController == null) _candidateLoadoutSubPanelController    = this.GetComponentInChildren<CandidateLoadoutSubPanelController>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.BackgroundClickClearAreaHandler    == null) Debug.LogError($"{nameof(LoadoutPanelController)} requires a {nameof(LoadoutPanelBackgroundClickClearAreaHandler)}.", this);
            if (this.EquippedLoadoutSubPanelController  == null) Debug.LogError($"{nameof(LoadoutPanelController)} requires an {nameof(FlameOfTheLost.EquippedLoadoutSubPanelController)}.", this);
            if (this.CandidateLoadoutSubPanelController == null) Debug.LogError($"{nameof(LoadoutPanelController)} requires a {nameof(FlameOfTheLost.CandidateLoadoutSubPanelController)}.", this);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.BackgroundClickClearAreaHandler?.Bind(this);
            this.EquippedLoadoutSubPanelController?.Bind(this);
        }
        protected override void UnbindInternalReferences()
        {
            this.BackgroundClickClearAreaHandler?.Unbind(this);
            this.EquippedLoadoutSubPanelController?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}