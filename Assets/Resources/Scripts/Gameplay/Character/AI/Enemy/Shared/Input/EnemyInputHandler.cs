using UnityEngine;

namespace FlameOfTheLost
{
    public class EnemyInputHandler : CharacterInputHandler
    {
        [Header("AI Input")]
        [SerializeField] private bool _isAIInputBlocked;
        public bool IsAIInputBlocked
        {
            get => _isAIInputBlocked;
            private set => _isAIInputBlocked = value;
        }

        public void SetAIMoveInput(Vector2 moveInput)
        {
            if (this.IsAIInputBlocked)
            {
                this.ClearMoveInput();
                return;
            }

            this.SetMoveInput(moveInput);
        }

        public void ClearAIMoveInput()
        {
            this.ClearMoveInput();
        }

        public void RequestAINormalAttack(AttackSide attackSide)
        {
            if (this.IsAIInputBlocked) return;

            this.RequestNormalAttack(attackSide);
        }

        public void RequestAISpecialAttack(AttackSide attackSide)
        {
            if (this.IsAIInputBlocked) return;

            this.RequestSpecialAttack(attackSide);
        }

        public void BlockAIInput()
        {
            this.IsAIInputBlocked = true;
            this.ClearMoveInput();
        }

        public void UnblockAIInput()
        {
            this.IsAIInputBlocked = false;
        }

        public override void RefreshAfterCharacterPlacement(CharacterPlacementContext context)
        {
            base.RefreshAfterCharacterPlacement(context);

            this.UnblockAIInput();
        }
    }
}
