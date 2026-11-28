
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CircleSelector
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Selector))]
    public class SelectorEditor : Editor
    {
        public VisualTreeAsset xmlAsset;
        Selector selector;

        private void Awake()
        {
            selector = target as Selector;
        }

        public override VisualElement CreateInspectorGUI()
        {
            //return base.CreateInspectorGUI();

            VisualElement selectorElement = new VisualElement();

            xmlAsset.CloneTree(selectorElement);



            PropertyField propertyField = selectorElement.Q<PropertyField>("ScaleParentOverride");
            propertyField.RegisterValueChangeCallback(OnPropertyValueChange);

            return selectorElement;
        }

        void OnPropertyValueChange(SerializedPropertyChangeEvent changeEvent)
        {
            if (Application.isPlaying)
            {
                selector.UpdateParent();
            }

        }
    }
}
