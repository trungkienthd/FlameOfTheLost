#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace FlameOfTheLost.Editor
{
    public sealed class SpriteSubSpriteManualRenameTool : EditorWindow
    {
        // --- Constants ---
        private const float PreviewSize = 64f;
        private const float IndexWidth = 35f;

        // --- Input ---
        [SerializeField] private Texture2D _spriteSheet;

        // --- Runtime State ---
        private readonly List<SpriteRenameEntry> _entries = new();

        private Vector2 _scrollPosition;

        // --- Menu ---
        [MenuItem("FlameOfTheLost/Tools/Assets/Art/Sprites/Common/Sub-Sprite Manual Rename Tool")]
        private static void OpenWindow()
        {
            SpriteSubSpriteManualRenameTool window = SpriteSubSpriteManualRenameTool.GetWindow<SpriteSubSpriteManualRenameTool>();

            window.titleContent = new GUIContent("Sub-Sprite Renamer");

            window.minSize = new Vector2(550f, 400f);

            window.Show();
        }

        // --- GUI ---
        private void OnGUI()
        {
            SpriteSubSpriteManualRenameTool.DrawHeader();

            EditorGUILayout.Space(8f);

            this.DrawSpriteSheetField();

            EditorGUILayout.Space(8f);

            if (_spriteSheet == null)
            {
                EditorGUILayout.HelpBox("Select a Multiple-mode sprite sheet.", MessageType.Info);
                return;
            }

            if (!this.TryValidateSpriteSheet(out string validationMessage))
            {
                EditorGUILayout.HelpBox(validationMessage, MessageType.Warning);
                return;
            }

            this.DrawToolbar();

            EditorGUILayout.Space(5f);

            this.DrawEntries();

            EditorGUILayout.Space(8f);

            this.DrawApplySection();
        }

        // --- Helpers ---
        //  1. Header
        private static void DrawHeader()
        {
            EditorGUILayout.LabelField("Sub-Sprite Rename Tool", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Rename sub-sprites belonging to a Multiple-mode sprite sheet.", EditorStyles.wordWrappedMiniLabel);
        }

        //  2. Sprite Sheet
        private void DrawSpriteSheetField()
        {
            EditorGUI.BeginChangeCheck();

            Texture2D newSpriteSheet = (Texture2D)EditorGUILayout.ObjectField("Sprite Sheet", _spriteSheet, typeof(Texture2D), false);

            if (!EditorGUI.EndChangeCheck()) return;

            _spriteSheet = newSpriteSheet;

            this.RefreshEntries();
        }
        private bool TryValidateSpriteSheet(out string message)
        {
            message = string.Empty;

            if (_spriteSheet == null)
            {
                message = "No sprite sheet selected.";
                return false;
            }

            string assetPath = AssetDatabase.GetAssetPath(_spriteSheet);
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                message = "The selected texture is not a valid project asset.";
                return false;
            }

            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                message = "The selected asset does not use a TextureImporter.";
                return false;
            }
            if (importer.textureType != TextureImporterType.Sprite)
            {
                message = "The selected texture is not imported as Sprite (2D and UI).";
                return false;
            }
            if (importer.spriteImportMode != SpriteImportMode.Multiple)
            {
                message = "The selected sprite must use Sprite Mode: Multiple.";
                return false;
            }

            return true;
        }

        //  3. Toolbar
        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Refresh", GUILayout.Width(90f)))
                {
                    this.RefreshEntries();
                }

                GUILayout.FlexibleSpace();

                EditorGUILayout.LabelField($"Sub-Sprites: {_entries.Count}", EditorStyles.miniLabel, GUILayout.Width(110f));
            }
        }

        //  4. Entries
        private void DrawEntries()
        {
            if (_entries.Count == 0)
            {
                EditorGUILayout.HelpBox("No sub-sprites were found.", MessageType.Info);
                return;
            }

            SpriteSubSpriteManualRenameTool.DrawColumnHeaders();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            for (int i = 0; i < _entries.Count; i++)
            {
                this.DrawEntry(i, _entries[i]);
            }

            EditorGUILayout.EndScrollView();
        }
        private static void DrawColumnHeaders()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("#", EditorStyles.boldLabel, GUILayout.Width(IndexWidth));
                GUILayout.Label("Preview", EditorStyles.boldLabel, GUILayout.Width(PreviewSize));
                GUILayout.Space(8f);
                GUILayout.Label("Current / New Name", EditorStyles.boldLabel);
            }

            EditorGUILayout.Space(3f);
        }
        private void DrawEntry(int index, SpriteRenameEntry entry)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    SpriteSubSpriteManualRenameTool.DrawIndex(index);

                    SpriteSubSpriteManualRenameTool.DrawSpritePreview(entry);

                    GUILayout.Space(8f);

                    SpriteSubSpriteManualRenameTool.DrawNameField(entry);
                }
            }
        }
        private static void DrawIndex(int index)
        {
            GUILayout.Label((index + 1).ToString(), GUILayout.Width(IndexWidth));
        }
        private static void DrawSpritePreview(SpriteRenameEntry entry)
        {
            Rect previewRect = GUILayoutUtility.GetRect(PreviewSize, PreviewSize, GUILayout.Width(PreviewSize), GUILayout.Height(PreviewSize));

            if (entry.Sprite == null) return;

            Texture2D preview = AssetPreview.GetAssetPreview(entry.Sprite);
            if (preview == null)
            {
                preview = AssetPreview.GetMiniThumbnail(entry.Sprite);
            }

            if (preview != null)
            {
                GUI.DrawTexture(previewRect, preview, ScaleMode.ScaleToFit, true);
            }
        }
        private static void DrawNameField(SpriteRenameEntry entry)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.LabelField(entry.OriginalName, EditorStyles.miniLabel);

                Color originalBackgroundColor = GUI.backgroundColor;

                if (entry.HasChanged)
                {
                    GUI.backgroundColor = new Color(1f, 0.85f, 0.4f);
                }

                entry.NewName = EditorGUILayout.TextField(entry.NewName);

                GUI.backgroundColor = originalBackgroundColor;

                if (entry.HasChanged)
                {
                    EditorGUILayout.LabelField("Modified", EditorStyles.miniLabel);
                }
            }
        }

        //  4. Apply Section
        private void DrawApplySection()
        {
            int changedCount = _entries.Count(entry => entry.HasChanged);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label($"Pending Changes: {changedCount}", EditorStyles.boldLabel);

                GUILayout.FlexibleSpace();

                using (new EditorGUI.DisabledScope(changedCount == 0))
                {
                    if (GUILayout.Button("Reset Changes", GUILayout.Width(120f)))
                    {
                        this.ResetChanges();
                    }

                    if (GUILayout.Button("Apply Rename", GUILayout.Width(130f), GUILayout.Height(28f)))
                    {
                        this.ApplyRenames();
                    }
                }
            }
        }

        //  5. Refresh
        private void RefreshEntries()
        {
            _entries.Clear();

            if (_spriteSheet == null)
            {
                this.Repaint();
                return;
            }

            if (!TryValidateSpriteSheet(out _))
            {
                this.Repaint();
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(_spriteSheet);

            Sprite[] sprites = AssetDatabase
                .LoadAllAssetRepresentationsAtPath(assetPath)
                .OfType<Sprite>()
                .OrderBy(sprite => sprite.name)
                .ToArray();

            foreach (Sprite sprite in sprites)
            {
                _entries.Add(new SpriteRenameEntry(sprite, sprite.name));
            }

            this.Repaint();
        }
        private void ResetChanges()
        {
            foreach (SpriteRenameEntry entry in _entries)
            {
                entry.NewName = entry.OriginalName;
            }

            this.Repaint();
        }

        //  6. Apply
        private void ApplyRenames()
        {
            if (_spriteSheet == null) return;

            if (!this.ValidateNames(out string errorMessage))
            {
                EditorUtility.DisplayDialog("Invalid Sprite Names", errorMessage, "OK");
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(_spriteSheet);

            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer == null)
            {
                Debug.LogError($"[{nameof(SpriteSubSpriteManualRenameTool)}] Unable to find TextureImporter for '{assetPath}'.");
                return;
            }

            SpriteDataProviderFactories factories = new SpriteDataProviderFactories();

            factories.Init();

            ISpriteEditorDataProvider dataProvider = factories.GetSpriteEditorDataProviderFromObject(importer);

            if (dataProvider == null)
            {
                Debug.LogError($"[{nameof(SpriteSubSpriteManualRenameTool)}] Unable to create sprite data provider for '{assetPath}'.");
                return;
            }

            dataProvider.InitSpriteEditorDataProvider();

            SpriteRect[] spriteRects = dataProvider.GetSpriteRects();

            Dictionary<string, string> renameLookup = _entries
                .Where(entry => entry.HasChanged)
                .ToDictionary(entry => entry.OriginalName, entry => entry.NewName);

            int renameCount = 0;

            foreach (SpriteRect spriteRect in spriteRects)
            {
                if (!renameLookup.TryGetValue(spriteRect.name, out string newName)) continue;

                spriteRect.name = newName;
                renameCount++;
            }

            if (renameCount == 0) return;

            dataProvider.SetSpriteRects(spriteRects);

            dataProvider.Apply();

            importer.SaveAndReimport();

            AssetDatabase.Refresh();

            Debug.Log($"[{nameof(SpriteSubSpriteManualRenameTool)}] Renamed {renameCount} sub-sprite(s) in '{_spriteSheet.name}'.");

            this.RefreshEntries();
        }

        //  7. Validation
        private bool ValidateNames(out string errorMessage)
        {
            errorMessage = string.Empty;

            foreach (SpriteRenameEntry entry in _entries)
            {
                if (string.IsNullOrWhiteSpace(entry.NewName))
                {
                    errorMessage = $"Sprite '{entry.OriginalName}' has an empty name.";
                    return false;
                }
            }

            string[] duplicates = _entries
                .GroupBy(entry => entry.NewName.Trim(), StringComparer.Ordinal)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToArray();

            if (duplicates.Length > 0)
            {
                errorMessage = "Duplicate sprite names detected:\n\n" + string.Join("\n", duplicates);
                return false;
            }

            return true;
        }

        // --- Classes ---
        //  1. Entry
        private sealed class SpriteRenameEntry
        {
            public Sprite Sprite { get; }
            public string OriginalName { get; }
            public string NewName { get; set; }

            public bool HasChanged => !string.Equals(this.OriginalName, this.NewName, StringComparison.Ordinal);

            public SpriteRenameEntry(Sprite sprite, string originalName)
            {
                this.Sprite = sprite;
                this.OriginalName = originalName;
                this.NewName = originalName;
            }
        }
    }
}

#endif
