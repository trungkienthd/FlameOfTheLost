using UnityEngine;

namespace FlameOfTheLost
{
    public class TutorialPromptMapController : FOTLMonoBehaviour
    {
        // --- States ---
        [Header("References - Characters")]
        [SerializeField] private CharacterRootController _player;
        [SerializeField] private CharacterRootController _firstEnemy;
        private CharacterRootController Player => _player;
        private CharacterRootController FirstEnemy => _firstEnemy;

        [Header("References - Prompt Triggers")]
        [SerializeField] private TutorialPromptTriggerController _movementPromptTrigger;
        [SerializeField] private TutorialPromptTriggerController _equipPromptTrigger;
        [SerializeField] private TutorialPromptTriggerController _normalAttackPromptTrigger;
        [SerializeField] private TutorialPromptTriggerController _dodgePromptTrigger;
        private TutorialPromptTriggerController MovementPromptTrigger => _movementPromptTrigger;
        private TutorialPromptTriggerController EquipPromptTrigger => _equipPromptTrigger;
        private TutorialPromptTriggerController NormalAttackPromptTrigger => _normalAttackPromptTrigger;
        private TutorialPromptTriggerController DodgePromptTrigger => _dodgePromptTrigger;

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
        public void MarkPlayerHitByFirstEnemy()
        {
            if (this.DodgePromptTrigger == null) return;

            this.DodgePromptTrigger.SatisfyCondition(TutorialPromptTriggerConditionKey.PlayerHitByFirstEnemy);
        }
        public void MarkFirstEnemyDefeated()
        {
            if (this.DodgePromptTrigger == null) return;

            this.DodgePromptTrigger.SatisfyCondition(TutorialPromptTriggerConditionKey.FirstEnemyDefeated);
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

            if (this.FirstEnemy != null)
            {
                // Example only.
                // Subscribe to the First enemy's death event.
                this.FirstEnemy.DeathHandler.OnTechnicalDeath += this.HandleFirstEnemyDeathStarted;
            }
        }
        private void UnsubscribeEvents()
        {
            if (this.Player != null)
            {
                this.Player.DamageReceivingHandler.OnDamageApplied -= this.HandleDamageApplied;
            }

            if (this.FirstEnemy != null)
            {
                this.FirstEnemy.DeathHandler.OnTechnicalDeath -= this.HandleFirstEnemyDeathStarted;
            }
        }

        //  2. Event handlers
        private void HandleDamageApplied(DamageAppliedContext context)
        {
            if (context.Attacker != this.FirstEnemy) return;
            if (context.Victim != this.Player) return;
            if (context.DamageAmount <= 0) return;

            this.MarkPlayerHitByFirstEnemy();
        }
        private void HandleFirstEnemyDeathStarted(CharacterDeathContext context)
        {
            if (context.Character != this.FirstEnemy) return;

            this.MarkFirstEnemyDefeated();
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.Player == null)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null) _player = player.GetComponent<CharacterRootController>();
            }

            if (this.FirstEnemy == null)
            {
                GameObject enemyGroup = GameObject.Find("EnemyGroup");
                if (enemyGroup != null)
                {
                    GameObject firstEnemy = enemyGroup.transform.GetChild(0)?.gameObject;
                    if (firstEnemy != null) _firstEnemy = firstEnemy.GetComponent<CharacterRootController>();
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

            if (this.FirstEnemy == null || this.FirstEnemy.IdentityController.CharacterType != CharacterType.Feralshroom)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialPromptMapController)}] {nameof(this.FirstEnemy)} reference is not assigned or incorrect.", this);
            }
        }
    }
}
