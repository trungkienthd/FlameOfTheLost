namespace FlameOfTheLost
{
    public struct GateResult
    {
        private bool _allowed;
        private GateRejectStatus _reason;

        public bool Allowed
        {
            get => _allowed; 
            private set => _allowed = value;
        }
        public GateRejectStatus Reason
        {
            get => _reason;
            private set => _reason = value;
        }

        public static GateResult GetAllowedGateResult()
        {
            return new GateResult
            {
                Allowed = true,
                Reason = GateRejectStatus.None,
            };
        }

        public static GateResult GetRejectedGateResult(GateRejectStatus r)
        {
            return new GateResult
            {
                Allowed = false,
                Reason = r
            };
        }
    }
}
