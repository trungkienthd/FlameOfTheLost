namespace FlameOfTheLost
{
    public static class MainMenuSessionRequest
    {
        // --- Runtime ---
        private static MainMenuJourneyType _pendingJourneyType = MainMenuJourneyType.None;

        public static MainMenuJourneyType PendingJourneyType => _pendingJourneyType;

        public static bool HasPendingRequest => _pendingJourneyType != MainMenuJourneyType.None;

        // --- Public APIs ---
        public static void SetRequest(MainMenuJourneyType journeyType)
        {
            _pendingJourneyType = journeyType;
        }

        public static void ClearRequest()
        {
            _pendingJourneyType = MainMenuJourneyType.None;
        }
    }
}
