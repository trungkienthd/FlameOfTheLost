using UnityEngine;

namespace FlameOfTheLost
{
    public class AnimalRoamingHandler : AnimalBrainSubBehaviourBase
    {
        // --- Runtime ---
        [Header("Wandering (Runtime)")]
        [SerializeField] private Vector2 _moveDirection;
        public Vector2 MoveDirection
        {
            get => _moveDirection;
            private set => _moveDirection = value;
        }

        // --- Public APIs ---
        public bool BeginWandering(AnimalHomeAreaEntry homeArea)
        {
            if (!homeArea.HasValidArea())
            {
                this.StopWandering();
                return false;
            }

            if (this.AnimalRootController == null)
            {
                this.StopWandering();
                return false;
            }

            Vector2 currentPosition = this.AnimalRootController.transform.position;

            float availableLeft = homeArea.GetAvailableHorizontalSpaceLeft(currentPosition);
            float availableRight = homeArea.GetAvailableHorizontalSpaceRight(currentPosition);

            bool canMoveLeft = availableLeft > 0.05f;
            bool canMoveRight = availableRight > 0.05f;

            if (!canMoveLeft && !canMoveRight)
            {
                this.StopWandering();
                return false;
            }

            if (!canMoveLeft)
            {
                this.MoveDirection = Vector2.right;
            }
            else if (!canMoveRight)
            {
                this.MoveDirection = Vector2.left;
            }
            else
            {
                this.MoveDirection = UnityEngine.Random.value < 0.5f ? Vector2.left : Vector2.right;
            }

            return true;
        }

        public bool TickWandering(AnimalHomeAreaEntry homeArea)
        {
            if (!homeArea.HasValidArea())
            {
                this.StopWandering();
                return false;
            }

            if (this.AnimalRootController == null)
            {
                this.StopWandering();
                return false;
            }

            if (Mathf.Abs(this.MoveDirection.x) <= 0.001f)
            {
                this.StopWandering();
                return false;
            }

            Vector2 currentPosition = this.AnimalRootController.transform.position;

            const float boundaryProbeDistance = 0.25f;

            if (!homeArea.CanMoveHorizontally(currentPosition, this.MoveDirection.x, boundaryProbeDistance))
            {
                this.StopWandering();
                return false;
            }

            this.NeutralCharacterInputHandler?.SetMoveInput(this.MoveDirection);

            return true;
        }

        public void StopWandering()
        {
            this.MoveDirection = Vector2.zero;
            this.NeutralCharacterInputHandler?.ClearMoveInput();
        }

        public void ResetRuntime()
        {
            this.StopWandering();
        }
    }
}