namespace FlameOfTheLost
{
    public struct GateResult
    {
        private bool _allowed;
        private GateRejectReason _reason;

        public bool Allowed
        {
            get => _allowed; 
            private set => _allowed = value;
        }
        public GateRejectReason Reason
        {
            get => _reason;
            private set => _reason = value;
        }

        public static GateResult GetAllowedGateResult()
        {
            return new GateResult
            {
                Allowed = true,
                Reason = GateRejectReason.None,
            };
        }

        public static GateResult GetRejectedGateResult(GateRejectReason r)
        {
            return new GateResult
            {
                Allowed = false,
                Reason = r
            };
        }
    }
}
