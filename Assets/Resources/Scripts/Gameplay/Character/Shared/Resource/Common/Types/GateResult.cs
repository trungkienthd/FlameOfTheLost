namespace FlameOfTheLost
{
    public struct GateResult
    {
        public bool IsAccepted
        {
            get; 
            set;
        }
        public GateRejectType RejectType
        {
            get;
            set;
        }
        public RuntimeResourceCost RuntimeCost
        {
            get;
            set;
        }
    }
}
