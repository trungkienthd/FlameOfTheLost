using UnityEngine;

namespace FlameOfTheLost
{
    public interface IIconProvider
    {
        public Sprite Icon
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
