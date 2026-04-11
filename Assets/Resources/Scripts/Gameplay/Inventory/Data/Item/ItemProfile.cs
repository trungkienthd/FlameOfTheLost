using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class ItemProfile : ScriptableObject, IIdentityProvider, IIconProvider
    {
        [Header("Identity")]
        [SerializeField] private int _id;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(2, 5)] private string _description;

        [Header("Visual")]
        [SerializeField] private Sprite _icon;

        [Header("Inventory")]
        [SerializeField] private bool _isStackable = false;
        [SerializeField, Range(1, 99)] private int _maxStackSize = 1;
        public int ID => _id;
        public string DisplayName => _displayName;
        public string Description => _description;
        public Sprite Icon => _icon;

        public virtual bool IsStackable => _isStackable;
        public virtual int MaxStackSize => Mathf.Max(1, _maxStackSize);

        public abstract ItemType ItemType { get; }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (!this.IsStackable && this.MaxStackSize != 1)
            {
                _maxStackSize = 1;
            }
        }
#endif
    }
}