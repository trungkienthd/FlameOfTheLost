namespace FlameOfTheLost
{
    public interface IIdentityProvider
    {
        public int ID
        {
            get;
        }

        public string DisplayName
        {
            get;
        }

        public string Description
        {
            get;
        }
    }
}
