using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class SceneTransitionPointRequest
    {
        // --- Data ---
        public string SourceSceneName { get; }
        public string DestinationSceneName { get; }
        public string DestinationArrivalKey { get; }

        // --- Runtime ---
        private static SceneTransitionPointRequest _pending;
        public static SceneTransitionPointRequest Pending => _pending;

        public static bool HasPendingRequest => _pending != null;

        // --- Constructor ---
        private SceneTransitionPointRequest( string sourceSceneName, string destinationSceneName, string destinationArrivalKey)
        {
            this.SourceSceneName = sourceSceneName;
            this.DestinationSceneName = destinationSceneName;
            this.DestinationArrivalKey = destinationArrivalKey;
        }

        // --- Public APIs ---
        //  1. Request Creation
        public static bool TryCreateRequest(string sourceSceneName, string destinationSceneName, string destinationArrivalKey)
        {
            if (SceneTransitionPointRequest.HasPendingRequest) return false;

            if (string.IsNullOrWhiteSpace(sourceSceneName))         return false;
            if (string.IsNullOrWhiteSpace(destinationSceneName))    return false;
            if (string.IsNullOrWhiteSpace(destinationArrivalKey))   return false;

            _pending = new SceneTransitionPointRequest(sourceSceneName, destinationSceneName, destinationArrivalKey);

            return true;
        }

        //  2. Request Cleanup
        public static void ClearRequest()
        {
            _pending = null;
        }
    }
}
