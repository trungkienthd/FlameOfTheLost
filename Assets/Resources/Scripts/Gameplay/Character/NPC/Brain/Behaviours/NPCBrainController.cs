using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(NPCDialogueHandler))]
    [RequireComponent(typeof(NPCQuestHandler))]
    [RequireComponent(typeof(NPCRewardHandler))]
    public class NPCBrainController : CharacterBehaviourControllerBase
    {
        // --- References ---
        //  1. NPC Root
        public NPCRootController NPCRootController => this.CharacterRootController as NPCRootController;

        //  2. Sub-Behaviours - Common
        [Header("References - Sub-Behaviours - Common")]
        [SerializeField] private NPCDialogueHandler _dialogueHandler;
        [SerializeField] private NPCQuestHandler _questHandler;
        [SerializeField] private NPCRewardHandler _rewardHandler;
        public NPCDialogueHandler DialogueHandler => _dialogueHandler;
        public NPCQuestHandler QuestHandler => _questHandler;
        public NPCRewardHandler RewardHandler => _rewardHandler;

        //  3. Sub-Behaviours - Optional
        [Header("References - Sub-Behaviours - Optional")]
        [SerializeField] private NPCShopHandler _shopHandler;
        [SerializeField] private NPCSkillTrainingHandler _skillTrainingHandler;
        [SerializeField] private NPCAttackReactionHandler _attackReactionHandler;
        public NPCShopHandler ShopHandler => _shopHandler;
        public NPCSkillTrainingHandler SkillTrainingHandler => _skillTrainingHandler;
        public NPCAttackReactionHandler AttackReactionHandler => _attackReactionHandler;

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.DialogueHandler    == null) _dialogueHandler   = this.GetComponent<NPCDialogueHandler>();
            if (this.QuestHandler       == null) _questHandler      = this.GetComponent<NPCQuestHandler>();
            if (this.RewardHandler      == null) _rewardHandler     = this.GetComponent<NPCRewardHandler>();

            if (this.ShopHandler            == null) _shopHandler           = this.GetComponent<NPCShopHandler>();
            if (this.SkillTrainingHandler   == null) _skillTrainingHandler  = this.GetComponent<NPCSkillTrainingHandler>();
            if (this.AttackReactionHandler  == null) _attackReactionHandler = this.GetComponent<NPCAttackReactionHandler>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.NPCRootController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCBrainController)}] {nameof(this.NPCRootController)} reference is missing.", this);
            }

            if (this.DialogueHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCBrainController)}] {nameof(this.DialogueHandler)} reference is missing.", this);
            }
            if (this.QuestHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCBrainController)}] {nameof(this.QuestHandler)} reference is missing.", this);
            }

            if (this.RewardHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCBrainController)}] {nameof(this.RewardHandler)} reference is missing.", this);
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.DialogueHandler?.Bind(this);
            this.QuestHandler?.Bind(this);
            this.RewardHandler?.Bind(this);

            this.ShopHandler?.Bind(this);
            this.SkillTrainingHandler?.Bind(this);
            this.AttackReactionHandler?.Bind(this);
        }
        protected override void UnbindInternalReferences()
        {
            this.AttackReactionHandler?.Unbind(this);
            this.SkillTrainingHandler?.Unbind(this);
            this.ShopHandler?.Unbind(this);

            this.RewardHandler?.Unbind(this);
            this.QuestHandler?.Unbind(this);
            this.DialogueHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}