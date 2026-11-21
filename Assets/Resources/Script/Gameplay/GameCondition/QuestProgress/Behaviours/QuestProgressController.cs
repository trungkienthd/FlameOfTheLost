using System;

namespace FlameOfTheLost
{
    public class QuestProgressController : GameConditionProgressControllerBase
    {
        // --- Events ---
        public event Action<NPCQuestProfile> QuestAccepted;
        public event Action<NPCQuestProfile> QuestCompleted;



        // --- Public APIs ---
        //  1. Gameplay Reference Lifecycle
        public override bool BindGameplayReferences()
        {
            if (!this.EnsureBound()) return false;

            return this.GameConditionController != null;
        }
        public override void UnbindGameplayReferences()
        {
        }

        //  2. Quest State
        public NPCQuestStatus GetQuestStatus(NPCQuestProfile quest)
        {
            if (quest == null) return NPCQuestStatus.Locked;

            GameConditionController conditions = this.GameConditionController;
            if (conditions == null) return NPCQuestStatus.Locked;
            if (conditions.GetConditionState(quest.CompletedCondition)) return NPCQuestStatus.Completed;

            bool accepted = conditions.GetConditionState(quest.AcceptedCondition);

            if (!accepted)
            {
                bool available = NPCConditionRequirementEntry.AreAllSatisfied(quest.AvailabilityRequirements, conditions);

                return available
                    ? NPCQuestStatus.Available
                    : NPCQuestStatus.Locked;
            }

            bool objectivesCompleted = NPCConditionRequirementEntry.AreAllSatisfied(quest.CompletionRequirements, conditions);

            return objectivesCompleted
                ? NPCQuestStatus.ReadyToComplete
                : NPCQuestStatus.Active;
        }

        //  3. Acceptance
        public bool CanAcceptQuest(NPCQuestProfile quest)
        {
            if (!this.EnsureBound()) return false;

            return this.GetQuestStatus(quest) == NPCQuestStatus.Available;
        }
        public bool TryAcceptQuest(NPCQuestProfile quest)
        {
            if (!this.CanAcceptQuest(quest)) return false;

            this.GameConditionController.SatisfyCondition(quest.AcceptedCondition);

            this.QuestAccepted?.Invoke(quest);

            return true;
        }

        //  4. Completion
        public bool CanCompleteQuest(NPCQuestProfile quest)
        {
            if (!this.EnsureBound()) return false;

            return this.GetQuestStatus(quest) == NPCQuestStatus.ReadyToComplete;
        }
        public bool TryCompleteQuest(NPCQuestProfile quest)
        {
            if (!this.CanCompleteQuest(quest)) return false;

            this.GameConditionController.SatisfyCondition(quest.CompletedCondition);

            this.QuestCompleted?.Invoke(quest);

            return true;
        }
    }
}
