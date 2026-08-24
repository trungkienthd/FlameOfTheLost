using UnityEngine;

namespace FlameOfTheLost
{
    public readonly struct CraftingEvaluationRecord
    {
        public bool CanCraft { get; }
        public CraftingFailureReason FailureReason { get; }

        public CraftingEvaluationRecord(bool canCraft, CraftingFailureReason failureReason)
        {
            this.CanCraft = canCraft;
            this.FailureReason = failureReason;
        }

        // --- Statics ---
        public static CraftingEvaluationRecord Success()
        {
            return new CraftingEvaluationRecord
            (
                canCraft: true,
                failureReason: CraftingFailureReason.None
            );
        }
        public static CraftingEvaluationRecord Failure(CraftingFailureReason reason)
        {
            return new CraftingEvaluationRecord
            (
                canCraft: false,
                failureReason: reason
            );
        }
    }
}
