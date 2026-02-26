namespace FlameOfTheLost
{
    public interface IFacingProvider
    {
        public FacingStatusType FacingStatus
        {
            get; 
        }

        // For Side facing only, true if facing right, false if facing left
        public bool IsFacingRight
        { 
            get;
        }
    }
}
