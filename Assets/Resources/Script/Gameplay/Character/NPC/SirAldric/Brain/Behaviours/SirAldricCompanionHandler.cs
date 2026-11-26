
using UnityEngine;

namespace FlameOfTheLost
{
    public class SirAldricCompanionHandler : NPCBrainSubBehaviourBase
    {
        // --- Data / Configs ---
        //  1. Following
        [Header("Configs - Following")]
        [SerializeField, Min(0f)] private float _stopFollowingDistance = 1.4f;
        [SerializeField, Min(0f)] private float _resumeFollowingDistance = 2f;
        [SerializeField, Min(0f)] private float _followSpeedMultiplier = 1f;
        public float StopFollowingDistance => _stopFollowingDistance;
        public float ResumeFollowingDistance => _resumeFollowingDistance;
        public float FollowSpeedMultiplier => _followSpeedMultiplier;

        //  2. Regrouping
        [Header("Configs - Regrouping")]
        [SerializeField, Min(0f)] private float _regroupTriggerDistance = 5.5f;
        [SerializeField, Min(0f)] private float _regroupCompletionDistance = 2.5f;
        [SerializeField, Min(0f)] private float _regroupSpeedMultiplier = 1.3f;
        public float RegroupTriggerDistance => _regroupTriggerDistance;
        public float RegroupCompletionDistance => _regroupCompletionDistance;
        public float RegroupSpeedMultiplier => _regroupSpeedMultiplier;

        // --- References ---
        private SirAldricBrainController SirAldricBrain => this.NPCBrainController as SirAldricBrainController;

        private PlayerRootController Player => this.SirAldricBrain != null
            ? this.SirAldricBrain.Player
            : null;

        private NPCInputHandler NPCInput => this.NPCRootController?.NPCInputHandler;

        private MovementHandler Movement => this.NPCRootController?.MovementHandler;

        private NPCDialogueHandler Dialogue => this.NPCRootController?.DialogueHandler;

        // --- Runtime ---
        [Header("Runtime - Following")]
        [SerializeField] private bool _isMoving;
        public bool IsMoving => _isMoving;

        // --- Unity Callbacks ---
        private void Update()
        {
            if (!this.EnsureBound()) return;

            this.TickCompanion();
        }

        // --- Helpers ---
        //  1. Companion Update
        private void TickCompanion()
        {
            if (this.SirAldricBrain == null)
            {
                this.StopMovement();
                return;
            }

            if (this.SirAldricBrain.IsDeathDisabled)
            {
                this.StopMovement();
                return;
            }

            if (!this.SirAldricBrain.IsAllied)
            {
                this.StopMovement();
                return;
            }

            if (!this.IsPlayerAvailable())
            {
                this.StopMovement();
                return;
            }

            // Type A Interaction Dialogue pauses companion movement.
            if (this.Dialogue != null && this.Dialogue.IsConversationActive)
            {
                this.StopMovement();
                return;
            }

            switch (this.SirAldricBrain.CurrentAIState)
            {
                case SirAldricAIState.Following:
                    this.HandleFollowing();
                    break;

                case SirAldricAIState.Regrouping:
                    this.HandleRegrouping();
                    break;

                default:
                    this.StopMovement();
                    break;
            }
        }

        //  2. Player Validation
        private bool IsPlayerAvailable()
        {
            if (this.Player == null) return false;

            if (!this.Player.gameObject.activeInHierarchy) return false;

            if (this.Player.HPController != null && this.Player.HPController.IsDead)
            {
                return false;
            }

            return true;
        }

        //  3. Following
        private void HandleFollowing()
        {
            Vector2 toPlayer = this.GetDirectionToPlayer();

            float distance = toPlayer.magnitude;

            // Player is too far away: enter Regrouping.
            if (distance >= this.RegroupTriggerDistance)
            {
                this.SirAldricBrain.SetAIState(SirAldricAIState.Regrouping);
                return;
            }

            // Stop when sufficiently close.
            if (distance <= this.StopFollowingDistance)
            {
                this.StopMovement();
                return;
            }

            // Do not immediately resume walking after stopping near the Player.
            if (!this.IsMoving && distance < this.ResumeFollowingDistance)
            {
                this.StopMovement();
                return;
            }

            this.Move(toPlayer, this.FollowSpeedMultiplier);
        }

        //  4. Regrouping
        private void HandleRegrouping()
        {
            Vector2 toPlayer = this.GetDirectionToPlayer();

            float distance = toPlayer.magnitude;

            if (distance <= this.RegroupCompletionDistance)
            {
                this.StopMovement();

                this.SirAldricBrain.SetAIState(SirAldricAIState.Following);

                return;
            }

            this.Move(toPlayer, this.RegroupSpeedMultiplier);
        }

        //  5. Movement
        private void Move(Vector2 direction, float speedMultiplier)
        {
            if (direction.sqrMagnitude <= 0.0001f)
            {
                this.StopMovement();
                return;
            }

            if (this.NPCInput == null || this.Movement == null)
            {
                this.StopMovement();
                return;
            }

            if (!this.NPCInput.AllowAIInput)
            {
                this.StopMovement();
                return;
            }

            this.Movement.SetAIMovementSpeedMultiplier(speedMultiplier);

            this.NPCInput.SetMoveInput(direction.normalized);

            _isMoving = true;
        }

        private void StopMovement()
        {
            this.NPCInput?.ClearMoveInput();

            this.Movement?.SetAIMovementSpeedMultiplier(1f);

            _isMoving = false;
        }

        //  6. Position
        private Vector2 GetDirectionToPlayer()
        {
            if (this.Player == null || this.NPCRootController == null)
            {
                return Vector2.zero;
            }

            Vector2 playerPosition = this.Player.transform.position;

            Vector2 currentPosition = this.NPCRootController.transform.position;

            return playerPosition - currentPosition;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _isMoving = false;
        }
        protected override void CleanupStates()
        {
            this.StopMovement();

            base.CleanupStates();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _resumeFollowingDistance = Mathf.Max(this.StopFollowingDistance, this.ResumeFollowingDistance);

            _regroupCompletionDistance = Mathf.Max(this.ResumeFollowingDistance, this.RegroupCompletionDistance);

            _regroupTriggerDistance = Mathf.Max(this.RegroupCompletionDistance, this.RegroupTriggerDistance);
        }
#endif
    }
}
