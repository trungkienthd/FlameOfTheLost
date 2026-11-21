using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class WorldMapTransitionRequest
    {
        // --- Runtime ---
        private static WorldMapTransitionRequest _pending;
        public static WorldMapTransitionRequest Pending => _pending;

        public static bool HasPendingRequest => _pending != null;

        // --- Properties ---
        public string SourceSceneName { get; }
        public string DestinationSceneName { get; }
        public WorldMapBoundaryDirection DestinationBoundary { get; }
        public float NormalizedBoundaryPosition { get; }

        // --- Constructor ---
        public WorldMapTransitionRequest(string sourceSceneName, string destinationSceneName, WorldMapBoundaryDirection destinationBoundary, float normalizedBoundaryPosition)
        {
            this.SourceSceneName = sourceSceneName;

            this.DestinationSceneName = destinationSceneName;

            this.DestinationBoundary = destinationBoundary;

            this.NormalizedBoundaryPosition = Mathf.Clamp01(normalizedBoundaryPosition);
        }

        // --- Public APIs ---
        public static void SetRequest(WorldMapTransitionRequest request)
        {
            _pending = request;
        }
        public static void ClearRequest()
        {
            _pending = null;
        }
    }
}