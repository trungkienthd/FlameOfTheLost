using UnityEngine;

namespace FlameOfTheLost
{
    public class AnimalFleeHandler : AnimalBrainSubBehaviourBase
    {
        // --- Public APIs ---
        public bool TickFlee(CharacterRootController threat, AnimalHomeAreaEntry homeArea, AnimalFleeConfig fleeConfig)
        {
            if (threat == null)
            {
                this.StopFleeing();
                return false;
            }

            if (this.AnimalRootController == null)
            {
                this.StopFleeing();
                return false;
            }

            Vector2 currentPosition = this.AnimalRootController.transform.position;
            Vector2 threatPosition = threat.transform.position;

            float sameXThreshold = fleeConfig != null
                ? fleeConfig.SameHorizontalPositionThreshold
                : 0.05f;

            float fleeDirectionX = this.ResolveHorizontalFleeDirection(currentPosition, threatPosition, homeArea, sameXThreshold);

            Vector2 fleeDirection = fleeDirectionX < 0f ? Vector2.left : Vector2.right;

            this.NeutralCharacterInputHandler?.SetMoveInput(fleeDirection);

            return true;
        }

        public void StopFleeing()
        {
            this.NeutralCharacterInputHandler?.ClearMoveInput();
        }

        public void ResetRuntime()
        {
            this.StopFleeing();
        }

        // --- Helpers ---
        private float ResolveHorizontalFleeDirection(Vector2 currentPosition, Vector2 threatPosition, AnimalHomeAreaEntry homeArea, float sameXThreshold)
        {
            float deltaX = currentPosition.x - threatPosition.x;

            if (Mathf.Abs(deltaX) > sameXThreshold) return Mathf.Sign(deltaX);

            // Target is almost vertically aligned with Cow.
            // Prefer the direction with more available Home Area.
            if (homeArea.HasValidArea())
            {
                float availableLeft     = homeArea.GetAvailableHorizontalSpaceLeft(currentPosition);
                float availableRight    = homeArea.GetAvailableHorizontalSpaceRight(currentPosition);

                if (!Mathf.Approximately(availableLeft, availableRight))
                {
                    return availableLeft > availableRight
                        ? -1f
                        : +1f;
                }
            }

            return Random.value < 0.5f
                ? -1f
                : +1f;
        }
    }
}