namespace FlameOfTheLost
{
    public interface IIdentityProvider
    {
        public int Id
        {
            get;
        }

        public string Key
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
