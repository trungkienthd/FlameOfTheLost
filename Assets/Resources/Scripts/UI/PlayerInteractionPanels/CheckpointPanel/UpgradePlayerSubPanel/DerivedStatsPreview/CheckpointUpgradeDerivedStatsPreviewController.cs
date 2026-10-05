using UnityEngine;

namespace FlameOfTheLost
{
    public class CheckpointUpgradeDerivedStatsPreviewController : CheckpointUpgradePlayerSubPanelSubBehaviourBase
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private CheckpointUpgradeDerivedStatBarController[] _derivedStatBars;
        public CheckpointUpgradeDerivedStatBarController[] DerivedStatBars => _derivedStatBars;

        // --- Public APIs ---
        //  1. Refresh
        public void RefreshDisplay()
        {
            if (!this.EnsureBound()) return;

            if (this.DerivedStatBars == null) return;

            foreach (CheckpointUpgradeDerivedStatBarController derivedStatBar in this.DerivedStatBars)
            {
                derivedStatBar?.RefreshDisplay();
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.DerivedStatBars == null || this.DerivedStatBars.Length == 0)
            {
                _derivedStatBars = this.GetComponentsInChildren<CheckpointUpgradeDerivedStatBarController>(true);
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.DerivedStatBars == null || this.DerivedStatBars.Length == 0)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] No {nameof(CheckpointUpgradeDerivedStatBarController)} references are assigned.", this);
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            if (this.DerivedStatBars == null) return;

            foreach (CheckpointUpgradeDerivedStatBarController derivedStatBar in this.DerivedStatBars)
            {
                derivedStatBar?.Bind(this);
            }
        }
        protected override void UnbindInternalReferences()
        {
            if (this.DerivedStatBars != null)
            {
                foreach (CheckpointUpgradeDerivedStatBarController derivedStatBar in this.DerivedStatBars)
                {
                    derivedStatBar?.Unbind(this);
                }
            }

            base.UnbindInternalReferences();
        }
    }
}
