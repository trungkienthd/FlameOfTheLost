using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class CharacterProgressionAttributeDisplayEntry
    {
        [SerializeField] private CharacterProgressionAttributeType _attributeType;
        public CharacterProgressionAttributeType AttributeType => _attributeType;

        [Header("Display")]
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea] private string _description;
        public Sprite Icon => _icon;
        public string DisplayName => _displayName;
        public string Description => _description;

        [Header("Upgrade")]
        [SerializeField, TextArea] private string _costScalingExplanation;
        public string CostScalingExplanation => _costScalingExplanation;
    }
}
