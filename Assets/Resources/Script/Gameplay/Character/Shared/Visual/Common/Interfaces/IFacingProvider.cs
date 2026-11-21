namespace FlameOfTheLost
{
    public interface IFacingProvider
    {
        public CharacterFacingStatus FacingStatus
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
