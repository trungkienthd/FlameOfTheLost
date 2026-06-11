namespace FlameOfTheLost
{
    public struct RuntimeResourceCost
    {
        public int HPCost
        {
            get;
            set;
        }
        public int FPCost
        {
            get;
            set;
        }
        public int StaminaCost
        {
            get;
            set;
        }

        public ResourceCostTriggerType CostTriggerType
        {
            get;
            set;
        }

        public bool IsSilencedBlocked 
        {
            get; 
            set; 
        }

        public bool AllowSelfKillWithHPCost
        {
            get;
            set;
        }

        public bool HasAnyCost
        {
            get => (this.HPCost > 0 || this.FPCost > 0 || this.StaminaCost > 0);
        }
    }
}
