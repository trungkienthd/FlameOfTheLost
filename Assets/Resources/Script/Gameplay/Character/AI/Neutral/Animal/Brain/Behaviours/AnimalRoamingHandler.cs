using UnityEngine;

namespace FlameOfTheLost
{
    public class AnimalRoamingHandler : AnimalBrainSubBehaviourBase
    {
        // --- Runtime ---
        [Header("Wandering (Runtime)")]
        [SerializeField] private Vector2 _moveDirection;
        [SerializeField] private bool _isReturningHome;
        public Vector2 MoveDirection
        {
            get => _moveDirection;
            private set => _moveDirection = value;
        }
        public bool IsReturningHome
        {
            get => _isReturningHome;
            private set => _isReturningHome = value;
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

            // If already outside, immediately return.
            if (!homeArea.ContainsHorizontalPosition(currentPosition.x))
            {
                return this.TickReturnHome(homeArea);
            }

            float availableLeft     = homeArea.GetAvailableHorizontalSpaceLeft(currentPosition);
            float availableRight    = homeArea.GetAvailableHorizontalSpaceRight(currentPosition);

            bool canMoveLeft    = availableLeft     > 0.05f;
            bool canMoveRight   = availableRight    > 0.05f;

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

            this.IsReturningHome = false;

            this.NeutralCharacterInputHandler?.SetMoveInput(this.MoveDirection);

            return true;
        }

        public bool TickWandering()
        {
            if (this.AnimalRootController == null)
            {
                this.StopWandering();
                return false;
            }

            if (Mathf.Abs(this.MoveDirection.x) <= 0.001f)
            {
                return false;
            }

            this.IsReturningHome = false;

            this.NeutralCharacterInputHandler?.SetMoveInput(this.MoveDirection);

            return true;
        }

        public bool TickReturnHome(AnimalHomeAreaEntry homeArea)
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

            Vector2 position = this.AnimalRootController.transform.position;

            Vector2 min = homeArea.GetMin();
            Vector2 max = homeArea.GetMax();

            float directionX;

            if (position.x < min.x)
            {
                directionX = +1f;
            }
            else if (position.x > max.x)
            {
                directionX = -1f;
            }
            else
            {
                // Re-entered the Home Area.
                this.IsReturningHome = false;
                return this.TickWandering();
            }

            this.IsReturningHome = true;

            this.MoveDirection = directionX < 0f ? Vector2.left : Vector2.right;

            this.NeutralCharacterInputHandler?.SetMoveInput(this.MoveDirection);

            return true;
        }

        public void StopWandering()
        {
            this.MoveDirection = Vector2.zero;
            this.IsReturningHome = false;

            this.NeutralCharacterInputHandler?.ClearMoveInput();
        }

        public void ResetRuntime()
        {
            this.StopWandering();
        }
    }
}