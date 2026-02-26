using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class QuickItemHandler : MonoBehaviour, IQuickItemProvider
    {
        [SerializeField] private Sprite _currentIcon;
        public Sprite CurrentQuickItemIcon
        {
            get => _currentIcon;
            private set => _currentIcon = value;
        }

        public event Action OnQuickItemChanged;

        // Example API you can call from other systems/UI
        public void SetQuickItemIcon(Sprite s)
        {
            if (this.CurrentQuickItemIcon == s) return;
            this.CurrentQuickItemIcon = s;
            this.OnQuickItemChanged?.Invoke();
        }
    }
}