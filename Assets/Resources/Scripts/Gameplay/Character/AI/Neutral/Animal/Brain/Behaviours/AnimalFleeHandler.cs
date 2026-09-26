using UnityEngine;

namespace FlameOfTheLost
{
    public class AnimalFleeHandler : AnimalBrainSubBehaviourBase
    {
        // --- Public APIs ---
        public bool TickFlee(CharacterRootController threat, AnimalHomeAreaEntry homeArea, AnimalFleeConfig fleeConfig, out bool blockedByHomeBoundary)
        {
            blockedByHomeBoundary = false;

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

            if (!homeArea.HasValidArea())
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

            float probeDistance = fleeConfig != null
                ? fleeConfig.BoundaryProbeDistance
                : 0.25f;

            float boundaryPadding = fleeConfig != null
                ? fleeConfig.BoundaryPadding
                : 0.05f;

            if (!homeArea.CanMoveHorizontally
                (
                    currentPosition,
                    fleeDirectionX,
                    probeDistance,
                    boundaryPadding
                ))
            {
                blockedByHomeBoundary = true;
                this.StopFleeing();
                return false;
            }

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

            float availableLeft = homeArea.GetAvailableHorizontalSpaceLeft(currentPosition);
            float availableRight = homeArea.GetAvailableHorizontalSpaceRight(currentPosition);

            if (!Mathf.Approximately(availableLeft, availableRight))
            {
                return availableLeft > availableRight ? -1f : +1f;
            }

            return Random.value < 0.5f
                ? -1f
                : +1f;
        }
    }
}