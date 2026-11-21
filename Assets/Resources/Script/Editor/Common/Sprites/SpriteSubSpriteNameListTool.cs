#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlameOfTheLost.Editor
{
    public sealed class SpriteSubSpriteNameListTool : EditorWindow
    {
        // --- References ---
        private Sprite _selectedSprite;

        // --- Runtime ---
        private readonly List<string> _subSpriteNames = new();
        private Vector2 _scrollPosition;

        // --- Editor Window ---
        [MenuItem("FlameOfTheLost/Tools/Assets/Art/Sprites/Common/Sprite Sub-Sprite Name List")]
        private static void OpenWindow()
        {
            SpriteSubSpriteNameListTool window = SpriteSubSpriteNameListTool.GetWindow<SpriteSubSpriteNameListTool>();

            window.titleContent = new GUIContent("Sprite Names");
            window.minSize = new Vector2(420.0f, 300.0f);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Sprite Sub-Sprite Name Tool", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            _selectedSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", _selectedSprite, typeof(Sprite), false);

            if (EditorGUI.EndChangeCheck())
            {
                this.RefreshSubSpriteNames();
            }

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(_selectedSprite == null))
            {
                if (GUILayout.Button("Refresh"))
                {
                    this.RefreshSubSpriteNames();
                }
            }

            using (new EditorGUI.DisabledScope(_selectedSprite == null))
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Refresh"))
                {
                    this.RefreshSubSpriteNames();
                }

                using (new EditorGUI.DisabledScope(_subSpriteNames.Count == 0))
                {
                    if (GUILayout.Button("Log List"))
                    {
                        this.LogSubSpriteNames();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            this.DrawSubSpriteNames();
        }

        // --- Helpers ---
        private void RefreshSubSpriteNames()
        {
            _subSpriteNames.Clear();

            if (_selectedSprite == null) return;

            string assetPath = AssetDatabase.GetAssetPath(_selectedSprite);

            if (string.IsNullOrEmpty(assetPath)) return;

            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            foreach (Object asset in assets)
            {
                if (asset is Sprite sprite)
                {
                    _subSpriteNames.Add(sprite.name);
                }
            }

            this.Repaint();
        }

        private void DrawSubSpriteNames()
        {
            EditorGUILayout.LabelField($"Sub-Sprites ({_subSpriteNames.Count})", EditorStyles.boldLabel);

            if (_selectedSprite == null)
            {
                EditorGUILayout.HelpBox("Select a Sprite from a texture imported using Sprite Mode: Multiple.", MessageType.Info);
                return;
            }

            if (_subSpriteNames.Count == 0)
            {
                EditorGUILayout.HelpBox("No sub-sprites were found in the selected Sprite asset.", MessageType.Warning);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            for (int i = 0; i < _subSpriteNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"{i + 1}.", GUILayout.Width(30.0f));

                EditorGUILayout.SelectableLabel(_subSpriteNames[i], EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void LogSubSpriteNames()
        {
            if (_subSpriteNames.Count == 0) return;

            string log = $"--- Sub-Sprites ({_subSpriteNames.Count}) ---\n"
                + string.Join("\n", _subSpriteNames);

            Debug.Log(log);
        }
    }
}

#endif