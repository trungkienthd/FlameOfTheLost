using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public interface IQuickItemProvider
    {
        public Sprite CurrentQuickItemIcon 
        {
            get; 
        }

        public event Action OnQuickItemChanged;
    }
}
