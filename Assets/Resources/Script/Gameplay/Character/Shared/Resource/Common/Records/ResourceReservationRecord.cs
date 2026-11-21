namespace FlameOfTheLost
{
    public struct ResourceReservationRecord
    {
        public RuntimeResourceCost ReservedCost 
        {
            get;
            set; 
        }

        public int CreatedFrame 
        { 
            get; 
            set;
        }
    }
}
