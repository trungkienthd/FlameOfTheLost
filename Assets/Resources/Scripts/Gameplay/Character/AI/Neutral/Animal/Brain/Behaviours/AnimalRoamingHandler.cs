using UnityEngine;

namespace FlameOfTheLost
{
    public class AnimalRoamingHandler : AnimalBrainSubBehaviourBase
    {
        // --- Runtime ---
        [Header("Roaming (Runtime)")]
        [SerializeField] private bool _isWalking;
        [SerializeField] private Vector2 _moveDirection;
        [SerializeField] private float _nextPhaseTime;

        [Header("Return (Runtime)")]
        [SerializeField] private bool _hasReturnPoint;
        [SerializeField] private Vector2 _returnPoint;

        public bool IsWalking
        {
            get => _isWalking;
            private set => _isWalking = value;
        }

        private Vector2 MoveDirection
        {
            get => _moveDirection;
            set => _moveDirection = value;
        }

        private float NextPhaseTime
        {
            get => _nextPhaseTime;
            set => _nextPhaseTime = value;
        }

        private bool HasReturnPoint
        {
            get => _hasReturnPoint;
            set => _hasReturnPoint = value;
        }

        private Vector2 ReturnPoint
        {
            get => _returnPoint;
            set => _returnPoint = value;
        }

        // --- Public APIs ---
        public void TickRoaming(AnimalRoamingConfig roamingConfig, AnimalHomeAreaEntry homeArea)
        {
            if (roamingConfig == null)
            {
                this.NeutralCharacterInputHandler?.ClearMoveInput();
                return;
            }

            if (!homeArea.HasValidArea())
            {
                this.NeutralCharacterInputHandler?.ClearMoveInput();
                return;
            }

            Vector2 currentPosition = this.AnimalRootController.transform.position;

            if (!homeArea.Contains(currentPosition))
            {
                this.NeutralCharacterInputHandler?.ClearMoveInput();
                return;
            }

            if (Time.time >= this.NextPhaseTime)
            {
                this.BeginNextPhase
                (
                    roamingConfig,
                    homeArea,
                    currentPosition
                );
            }

            if (!this.IsWalking)
            {
                this.NeutralCharacterInputHandler?.ClearMoveInput();
                return;
            }

            Vector2 projectedPosition = currentPosition + this.MoveDirection * 0.25f;

            if (!homeArea.Contains(projectedPosition))
            {
                Vector2 randomPoint = homeArea.GetRandomPointInside();
                Vector2 direction = randomPoint - currentPosition;

                if (direction.sqrMagnitude > 0.001f)
                {
                    this.MoveDirection = direction.normalized;
                }
            }

            this.NeutralCharacterInputHandler?.SetMoveInput(this.MoveDirection);
        }

        public bool TickReturnToHome(AnimalHomeAreaEntry homeArea)
        {
            if (!homeArea.HasValidArea())
            {
                this.NeutralCharacterInputHandler?.ClearMoveInput();
                return false;
            }

            Vector2 currentPosition = this.AnimalRootController.transform.position;

            if (homeArea.Contains(currentPosition))
            {
                this.ClearReturnRuntime();
                this.NeutralCharacterInputHandler?.ClearMoveInput();
                return true;
            }

            if (!this.HasReturnPoint)
            {
                this.ReturnPoint = homeArea.GetRandomPointInside();
                this.HasReturnPoint = true;
            }

            Vector2 toReturnPoint = this.ReturnPoint - currentPosition;

            if (toReturnPoint.magnitude <= homeArea.ReturnReachDistance)
            {
                this.ClearReturnRuntime();
                this.NeutralCharacterInputHandler?.ClearMoveInput();
                return true;
            }

            this.NeutralCharacterInputHandler?.SetMoveInput
            (
                toReturnPoint.normalized
            );

            return false;
        }

        public void BeginIdlePhase(AnimalRoamingConfig roamingConfig)
        {
            this.IsWalking = false;
            this.MoveDirection = Vector2.zero;

            this.NextPhaseTime = roamingConfig != null
                ? Time.time + roamingConfig.GetRandomIdleDuration()
                : 0f;

            this.NeutralCharacterInputHandler?.ClearMoveInput();
        }

        public void ResetRuntime(AnimalRoamingConfig roamingConfig)
        {
            this.IsWalking = false;
            this.MoveDirection = Vector2.zero;

            this.NextPhaseTime = roamingConfig != null
                ? Time.time + roamingConfig.GetRandomIdleDuration()
                : 0f;

            this.ClearReturnRuntime();

            this.NeutralCharacterInputHandler?.ClearMoveInput();
        }

        // --- Helpers ---
        private void BeginNextPhase(AnimalRoamingConfig roamingConfig, AnimalHomeAreaEntry homeArea, Vector2 currentPosition)
        {
            this.IsWalking = !this.IsWalking;

            if (!this.IsWalking)
            {
                this.MoveDirection = Vector2.zero;

                this.NextPhaseTime = Time.time + roamingConfig.GetRandomIdleDuration();

                return;
            }

            Vector2 randomPoint = homeArea.GetRandomPointInside();
            Vector2 direction = randomPoint - currentPosition;

            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = Random.insideUnitCircle.normalized;
            }

            this.MoveDirection = direction.normalized;

            this.NextPhaseTime = Time.time + roamingConfig.GetRandomWalkDuration();
        }

        private void ClearReturnRuntime()
        {
            this.HasReturnPoint = false;
            this.ReturnPoint = Vector2.zero;
        }
    }
}