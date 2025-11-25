using System;
using UnityEditor;

namespace FlameOfTheLost
{
    [CustomEditor(typeof(StatusEffectProfile))]
    public class StatusEffectProfileEditor : Editor
    {
        public override void OnInspectorGUI()
        { 
            this.serializedObject.Update();
            var profile = this.target as StatusEffectProfile;

            // Draw "Identity & UI" header
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_id"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_displayName"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_description"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_icon"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_iconTint"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_dispelClass"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_tags"), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Lifetime & Stacking", EditorStyles.boldLabel);

            var durationProperty            = this.serializedObject.FindProperty("_duration");
            var combinationTypeProperty     = this.serializedObject.FindProperty("_combinationType");       
            var durationRefreshTypeProperty = this.serializedObject.FindProperty("_durationRefreshType");   
            var maxStacksProperty           = this.serializedObject.FindProperty("_maxStacks");

            EditorGUILayout.PropertyField(durationProperty);
            EditorGUILayout.PropertyField(combinationTypeProperty);
            EditorGUILayout.PropertyField(durationRefreshTypeProperty);

            // MaxStacks only meaningful when we actually stack (StackAll)
            bool isStackAll = (StatusEffectCombinationType)combinationTypeProperty.enumValueIndex == StatusEffectCombinationType.StackAll;
            using (new EditorGUI.DisabledScope(!isStackAll))
            {
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_maxStacks"));
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_modifiers"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_periodic"), true);

            this.serializedObject.ApplyModifiedProperties();
        }    
    }
}
