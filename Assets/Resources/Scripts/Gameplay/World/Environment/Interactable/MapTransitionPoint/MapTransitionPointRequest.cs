using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class MapTransitionPointRequest
    {
        // --- Data ---
        public string SourceSceneName { get; }
        public string DestinationSceneName { get; }
        public string DestinationArrivalKey { get; }

        // --- Runtime ---
        private static MapTransitionPointRequest _pending;
        public static MapTransitionPointRequest Pending => _pending;

        public static bool HasPendingRequest => _pending != null;

        // --- Constructor ---
        private MapTransitionPointRequest( string sourceSceneName, string destinationSceneName, string destinationArrivalKey)
        {
            this.SourceSceneName = sourceSceneName;
            this.DestinationSceneName = destinationSceneName;
            this.DestinationArrivalKey = destinationArrivalKey;
        }

        // --- Public APIs ---
        //  1. Request Creation
        public static bool TryCreateRequest(string sourceSceneName, string destinationSceneName, string destinationArrivalKey)
        {
            if (MapTransitionPointRequest.HasPendingRequest) return false;

            if (string.IsNullOrWhiteSpace(sourceSceneName))         return false;
            if (string.IsNullOrWhiteSpace(destinationSceneName))    return false;
            if (string.IsNullOrWhiteSpace(destinationArrivalKey))   return false;

            _pending = new MapTransitionPointRequest(sourceSceneName, destinationSceneName, destinationArrivalKey);

            return true;
        }

        //  2. Request Cleanup
        public static void ClearRequest()
        {
            _pending = null;
        }
    }
}
