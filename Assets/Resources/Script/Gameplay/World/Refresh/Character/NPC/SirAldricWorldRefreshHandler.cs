using UnityEngine;

namespace FlameOfTheLost
{
    public class SirAldricWorldRefreshHandler : FOTLMonoBehaviour, IWorldRefreshable
    {
        [Header("References - Sir Aldric")]
        [SerializeField] private SirAldricBrainController _sirAldricBrain;
        public SirAldricBrainController SirAldricBrain => _sirAldricBrain;

        public int WorldRefreshPriority => 40;
        public void RefreshWorldContent(WorldRefreshContext context)
        {
            if (!context.IsSaveReload && !context.IsPlayerRespawn) return;

            if (this.SirAldricBrain == null) return;

            this.SirAldricBrain.SynchronizeAfterWorldRestore();
        }

        // --- FOTLMonoBehaviour ---
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.SirAldricBrain == null)
            {
                _sirAldricBrain = UnityEngine.Object.FindFirstObjectByType<SirAldricBrainController>(FindObjectsInactive.Include);
            }
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.SirAldricBrain == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(SirAldricWorldRefreshHandler)}] Sir Aldric Brain reference is missing.", this);
            }
        }
    }
}