using UnityEngine;

namespace FlameOfTheLost
{
    public interface IIconProvider
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

        public Sprite Icon
        {
            get;
        }
    }
}
