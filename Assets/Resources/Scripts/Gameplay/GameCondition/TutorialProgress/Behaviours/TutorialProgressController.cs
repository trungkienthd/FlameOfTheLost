using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(TutorialProgressEquipmentHandler))]
    [RequireComponent(typeof(TutorialProgressFirstEnemyHandler))]
    public class TutorialProgressController : GameConditionProgressControllerBase
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
        //  1. Gameplay Reference Lifecycle
        public override bool BindGameplayReferences()
        {
            if (!this.EnsureBound()) return false;

            this.UnbindGameplayReferences();

            this.ResolvePlayerReference();

            if (this.Player == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressController)}] Cannot bind gameplay references because Player is missing.", this);
                return false;
            }

            if (this.EquipmentHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressController)}] EquipmentHandler is missing.", this);
                return false;
            }

            // Equipment monitoring is mandatory.
            if (!this.EquipmentHandler.BindGameplayReferences())
            {
                this.UnbindGameplayReferences();
                return false;
            }

            // First-enemy monitoring is specific to the tutorial map.
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GreenvaleBorderPath")
            {
                if (this.FirstEnemyHandler != null)
                {
                    // A missing tutorial enemy should not block the entire gameplay session.
                    if (!this.FirstEnemyHandler.BindGameplayReferences())
                    {
                        Debug.LogWarning($"[{this.gameObject.name}.{nameof(TutorialProgressController)}] First-enemy tutorial monitoring could not be initialized.", this);
                    }
                }
            }

            return true;
        }
        public override void UnbindGameplayReferences()
        {
            this.EquipmentHandler?.UnbindGameplayReferences();
            this.FirstEnemyHandler?.UnbindGameplayReferences();

            _player = null;
        }

        //  2. Reset
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

        // --- Helpers ---
        private void ResolvePlayerReference()
        {
            _player = null;

            GameObject player = GameObject.Find("Player");
            if (player != null) _player = player.GetComponent<CharacterRootController>();
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
        protected override void UnbindInternalReferences()
        {
            this.UnbindGameplayReferences();

            this.EquipmentHandler?.Unbind(this);
            this.FirstEnemyHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
