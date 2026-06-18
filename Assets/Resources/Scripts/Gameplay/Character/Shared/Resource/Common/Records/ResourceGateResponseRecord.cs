namespace FlameOfTheLost
{
    public struct ResourceGateResponseRecord
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
