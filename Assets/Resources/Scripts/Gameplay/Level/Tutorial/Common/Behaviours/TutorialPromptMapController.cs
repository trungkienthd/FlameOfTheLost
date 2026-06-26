using UnityEngine;

namespace FlameOfTheLost
{
    public class TutorialPromptMapController : FOTLMonoBehaviour
    {
        // --- States ---
        [Header("References - Characters")]
        [SerializeField] private CharacterRootController _player;
        [SerializeField] private CharacterRootController _firstBiteshroom;
        private CharacterRootController Player => _player;
        private CharacterRootController FirstBiteshroom => _firstBiteshroom;

        [Header("References - Prompt Triggers")]
        [SerializeField] private PromptTriggerController _movementPromptTrigger;
        [SerializeField] private PromptTriggerController _equipPromptTrigger;
        [SerializeField] private PromptTriggerController _normalAttackPromptTrigger;
        [SerializeField] private PromptTriggerController _dodgePromptTrigger;
        private PromptTriggerController MovementPromptTrigger => _movementPromptTrigger;
        private PromptTriggerController EquipPromptTrigger => _equipPromptTrigger;
        private PromptTriggerController NormalAttackPromptTrigger => _normalAttackPromptTrigger;
        private PromptTriggerController DodgePromptTrigger => _dodgePromptTrigger;

        // --- Unity Callbacks ---
        private void OnEnable()
        {
            this.SubscribeEvents();
        }

        private void OnDisable()
        {
            this.UnsubscribeEvents();
        }

        // --- Public APIs ---
        public void MarkPlayerHitByFirstBiteshroom()
        {
            if (this.DodgePromptTrigger == null) return;

            this.DodgePromptTrigger.SatisfyCondition(PromptTriggerConditionKey.PlayerHitByFirstBiteshroom);
        }
        public void MarkFirstBiteshroomDefeated()
        {
            if (this.DodgePromptTrigger == null) return;

            this.DodgePromptTrigger.SatisfyCondition(PromptTriggerConditionKey.FirstBiteshroomDefeated);
        }

        public void ResetTutorialPrompts()
        {
            if (this.MovementPromptTrigger      != null) this.MovementPromptTrigger.ResetPrompt();
            if (this.EquipPromptTrigger         != null) this.EquipPromptTrigger.ResetPrompt();
            if (this.NormalAttackPromptTrigger  != null) this.NormalAttackPromptTrigger.ResetPrompt();
            if (this.DodgePromptTrigger         != null) this.DodgePromptTrigger.ResetPrompt();
        }

        // --- Helpers ---
        //  1. Event bindings
        private void SubscribeEvents()
        {
            if (this.Player != null)
            {
                // Example only.
                // Subscribe to player's DamageReceivingHandler / HPController / CombatReceiver event.
                this.Player.DamageReceivingHandler.OnDamageApplied += this.HandleDamageApplied;
            }

            if (this.FirstBiteshroom != null)
            {
                // Example only.
                // Subscribe to the Biteshroom's death event.
                this.FirstBiteshroom.DeathHandler.OnTechnicalDeath += this.HandleFirstBiteshroomDeathStarted;
            }
        }
        private void UnsubscribeEvents()
        {
            if (this.Player != null)
            {
                this.Player.DamageReceivingHandler.OnDamageApplied -= this.HandleDamageApplied;
            }

            if (this.FirstBiteshroom != null)
            {
                this.FirstBiteshroom.DeathHandler.OnTechnicalDeath -= this.HandleFirstBiteshroomDeathStarted;
            }
        }

        //  2. Event handlers
        private void HandleDamageApplied(DamageAppliedContext context)
        {
            if (context.Attacker != this.FirstBiteshroom) return;
            if (context.Victim != this.Player) return;
            if (context.DamageAmount <= 0) return;

            this.MarkPlayerHitByFirstBiteshroom();
        }
        private void HandleFirstBiteshroomDeathStarted(CharacterDeathContext context)
        {
            if (context.Character != this.FirstBiteshroom) return;

            this.MarkFirstBiteshroomDefeated();
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.Player == null)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null) _player = player.GetComponent<CharacterRootController>();
            }

            if (this.FirstBiteshroom == null)
            {
                GameObject enemyGroup = GameObject.Find("EnemyGroup");
                if (enemyGroup != null)
                {
                    GameObject firstBiteshroom = enemyGroup.transform.GetChild(0)?.gameObject;
                    if (firstBiteshroom != null) _firstBiteshroom = firstBiteshroom.GetComponent<CharacterRootController>();
                }
            }
        }

        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Player == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialPromptMapController)}] Player reference is not assigned.", this);
            }

            if (this.FirstBiteshroom == null || this.FirstBiteshroom.IdentityController.CharacterType != CharacterType.Biteshroom)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialPromptMapController)}] {nameof(this.FirstBiteshroom)} reference is not assigned or incorrect.", this);
            }
        }
    }
}
