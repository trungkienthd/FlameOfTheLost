using UnityEngine;

namespace FlameOfTheLost
{
    public class AnimalFleeHandler : AnimalBrainSubBehaviourBase
    {
        // --- Public APIs ---
        public bool TickFlee(CharacterRootController threat)
        {
            if (threat == null)
            {
                this.StopFleeing();
                return false;
            }

            Vector2 currentPosition = this.AnimalRootController.transform.position;
            Vector2 threatPosition = threat.transform.position;

            Vector2 awayDirection = currentPosition - threatPosition;

            if (awayDirection.sqrMagnitude <= 0.001f)
            {
                awayDirection = Random.insideUnitCircle;
            }

            if (awayDirection.sqrMagnitude <= 0.001f)
            {
                awayDirection = Vector2.right;
            }

            awayDirection.Normalize();

            this.NeutralCharacterInputHandler?.SetMoveInput(awayDirection);

            return true;
        }

        public void StopFleeing()
        {
            this.NeutralCharacterInputHandler?.ClearMoveInput();
        }

        public float GetDistanceToThreat(CharacterRootController threat)
        {
            if (threat == null) return float.PositiveInfinity;

            Vector2 currentPosition = this.AnimalRootController.transform.position;
            Vector2 threatPosition = threat.transform.position;

            return Vector2.Distance(currentPosition, threatPosition);
        }

        public void ResetRuntime()
        {
            this.StopFleeing();
        }
    }
}