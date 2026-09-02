using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(TutorialProgressEquipmentHandler))]
    [RequireComponent(typeof(TutorialProgressFirstEnemyHandler))]
    public class TutorialProgressController : GameConditionSubBehaviourBase
    {
        // --- References ---
        //  1. Characters
        [Header("References (Characters)")]
        [SerializeField] private CharacterRootController _player;
        internal CharacterRootController Player => _player;

        //  2. Sub-Behaviours
        [Header("References (Sub-Behaviours)")]
        [SerializeField] private TutorialProgressEquipmentHandler _equipmentHandler;
        [SerializeField] private TutorialProgressFirstEnemyHandler _firstEnemyHandler;
        private TutorialProgressEquipmentHandler EquipmentHandler => _equipmentHandler;
        private TutorialProgressFirstEnemyHandler FirstEnemyHandler => _firstEnemyHandler;

        // --- Public APIs ---
        public void ResetTutorialProgress()
        {
            if (!this.EnsureBound()) return;

            this.GameConditionController.ResetConditions();

            this.EquipmentHandler?.RefreshConditions();
        }

        // --- Internal APIs ---
        internal void SetPlayerHasMultipleWeapons(bool value)
        {
            if (!this.EnsureBound()) return;

            this.GameConditionController.SetConditionState(GameConditionKey.PlayerHasMultipleWeapons, value);
        }
        internal void SetPlayerReadyToCycleWeapons(bool value)
        {
            if (!this.EnsureBound()) return;

            this.GameConditionController.SetConditionState(GameConditionKey.PlayerReadyToCycleWeapons, value);
        }

        internal void MarkPlayerHitByFirstEnemy()
        {
            if (!this.EnsureBound()) return;

            this.GameConditionController.SatisfyCondition(GameConditionKey.PlayerHitByFirstEnemy);
        }
        internal void MarkFirstEnemyDefeated()
        {
            if (!this.EnsureBound()) return;

            this.GameConditionController.SatisfyCondition(GameConditionKey.FirstEnemyDefeated);
        }

        // --- FOTLMonoBehaviour Overrides  ----
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.EquipmentHandler   == null) _equipmentHandler  = this.GetComponent<TutorialProgressEquipmentHandler>();
            if (this.FirstEnemyHandler  == null) _firstEnemyHandler = this.GetComponent<TutorialProgressFirstEnemyHandler>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.EquipmentHandler   == null) Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressController)}] {nameof(this.EquipmentHandler)} reference is not assigned.", this);
            if (this.FirstEnemyHandler  == null) Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressController)}] {nameof(this.FirstEnemyHandler)} reference is not assigned.", this);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.EquipmentHandler?.Bind(this);
            this.FirstEnemyHandler?.Bind(this);
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.Player == null)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null) _player = player.GetComponent<CharacterRootController>();
            }
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.Player == null) Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressController)}] {nameof(this.Player)} reference is not assigned.", this);
        }
        protected override void BindExternalReferences()
        {
            base.BindExternalReferences();

            // The Brain has completed external-reference caching before allowing its Sub-Behaviours to bind gameplay dependencies.
            this.EquipmentHandler?.BindGameplayReferences();
            this.FirstEnemyHandler?.BindGameplayReferences();
        }
        protected override void UnbindInternalReferences()
        {
            this.EquipmentHandler?.Unbind(this);
            this.FirstEnemyHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
