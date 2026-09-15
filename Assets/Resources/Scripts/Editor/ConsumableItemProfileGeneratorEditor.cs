#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace FlameOfTheLost
{
    public class ConsumableItemProfileGeneratorEditor : EditorWindow
    {
        // --- Constants ---
        private const string DefaultSpriteSheetPath = "Assets/Resources/Art/Sprites/Inventory/Items/Consumables/CFRPG16_Food_Icons_NO_Outline.png";
        private const string DefaultOutputFolderPath = "Assets/Resources/Data/Inventory/Items/Consumables";

        private const int ConsumableIdBase = 72001;

        private static readonly Regex ConsumableSpriteNameRegex = new(@"_(\d+)_Csm_(.+)$", RegexOptions.Compiled);

        // --- Configs ---
        [SerializeField] private string _spriteSheetPath = ConsumableItemProfileGeneratorEditor.DefaultSpriteSheetPath;
        [SerializeField] private string _outputFolderPath = ConsumableItemProfileGeneratorEditor.DefaultOutputFolderPath;

        [SerializeField] private bool _isStackable = true;
        [SerializeField, Range(1, 99)] private int _maxStackSize = 99;

        // --- Runtime ---
        private Vector2 _scrollPosition;

        private readonly List<ConsumableGenerationEntry> _entries = new();

        // --- Menu ---
        [MenuItem("FlameOfTheLost/Tools/Data/Inventory/Item/Consumable Item Profiles")]
        public static void OpenWindow()
        {
            ConsumableItemProfileGeneratorEditor window = ConsumableItemProfileGeneratorEditor.GetWindow<ConsumableItemProfileGeneratorEditor>("Consumable Profile Generator");

            window.minSize = new Vector2(720f, 500f);
        }

        // --- GUI ---
        private void OnGUI()
        {
            this.DrawConfiguration();

            EditorGUILayout.Space(10f);

            this.DrawActions();

            EditorGUILayout.Space(10f);

            DrawPreview();
        }

        // --- Helpers ---
        private void DrawConfiguration()
        {
            EditorGUILayout.LabelField("Consumable Item Profile Generator", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox("Generates ConsumableItemProfile assets from valid CFRPG16_Food_Icons_NO_Outline sub-sprites.\n\nOnly sprites ending in '_<index>_Csm_<Name>' are accepted. Existing assets are never overwritten.", MessageType.Info);

            EditorGUILayout.Space(5f);

            _spriteSheetPath = EditorGUILayout.TextField("Sprite Sheet Path", _spriteSheetPath);
            _outputFolderPath = EditorGUILayout.TextField("Output Folder", _outputFolderPath);

            EditorGUILayout.Space(5f);

            EditorGUILayout.LabelField("Generated Inventory Defaults", EditorStyles.boldLabel);

            _isStackable = EditorGUILayout.Toggle("Is Stackable", _isStackable);

            using (new EditorGUI.DisabledScope(!_isStackable))
            {
                _maxStackSize = EditorGUILayout.IntSlider("Max Stack Size", _maxStackSize, 1,99);
            }

            if (!_isStackable)
            {
                _maxStackSize = 1;
            }
        }
        private void DrawActions()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Refresh Preview", GUILayout.Height(30f)))
            {
                this.RefreshPreview();
            }

            using (new EditorGUI.DisabledScope(_entries.Count == 0))
            {
                if (GUILayout.Button("Generate Profiles", GUILayout.Height(30f)))
                {
                    this.GenerateProfiles();
                }
            }

            EditorGUILayout.EndHorizontal();
        }
        private void DrawPreview()
        {
            EditorGUILayout.LabelField($"Preview ({_entries.Count})", EditorStyles.boldLabel);

            if (_entries.Count == 0)
            {
                EditorGUILayout.HelpBox("No generation entries loaded. Click 'Refresh Preview'.", MessageType.None);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (ConsumableGenerationEntry entry in _entries)
            {
                this.DrawPreviewEntry(entry);
            }

            EditorGUILayout.EndScrollView();
        }
        private void DrawPreviewEntry(ConsumableGenerationEntry entry)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(entry.Sprite.texture, GUILayout.Width(32f), GUILayout.Height(32f));

            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField($"{entry.Id} - {entry.DisplayName}", EditorStyles.boldLabel);

            EditorGUILayout.LabelField($"Key: {entry.Key}");

            EditorGUILayout.LabelField($"Sprite: {entry.Sprite.name}");

            EditorGUILayout.LabelField($"Asset: {entry.AssetFileName}");

            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            ConsumableItemProfileGeneratorEditor.DrawStatusLabel(entry.Status);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        private static void DrawStatusLabel(ConsumableGenerationStatus status)
        {
            string text = status switch
            {
                ConsumableGenerationStatus.Ready => "Ready",
                ConsumableGenerationStatus.Exists => "Exists",
                ConsumableGenerationStatus.Invalid => "Invalid",
                _ => "Unknown"
            };

            GUILayout.Label(text, EditorStyles.miniBoldLabel, GUILayout.Width(60f));
        }
        private void RefreshPreview()
        {
            _entries.Clear();

            if (!this.ValidateConfiguration()) return;

            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(_spriteSheetPath);
            foreach (UnityEngine.Object asset in assets)
            {
                if (asset is not Sprite sprite) continue;
                if (!this.TryParseConsumableSprite(sprite, out ConsumableGenerationEntry entry)) continue;

                string assetPath = this.GetAssetPath(entry.AssetFileName);

                entry.Status = AssetDatabase.LoadAssetAtPath<ConsumableItemProfile>(assetPath) != null
                    ? ConsumableGenerationStatus.Exists
                    : ConsumableGenerationStatus.Ready;

                _entries.Add(entry);
            }

            _entries.Sort((left, right) => left.Id.CompareTo(right.Id));

            Debug.Log($"[{nameof(ConsumableItemProfileGeneratorEditor)}] Preview refreshed. Valid consumable sprites: {_entries.Count}.");
        }
        private bool TryParseConsumableSprite(Sprite sprite, out ConsumableGenerationEntry entry)
        {
            entry = null;

            Match match = ConsumableItemProfileGeneratorEditor.ConsumableSpriteNameRegex.Match(sprite.name);

            if (!match.Success) return false;
            if (!int.TryParse(match.Groups[1].Value, out int spriteIndex)) return false;

            string semanticName = match.Groups[2].Value;
            if (string.IsNullOrWhiteSpace(semanticName)) return false;

            int id = ConsumableIdBase + spriteIndex;

            string profileName = $"Csm_{semanticName}";
            string key = ConsumableItemProfileGeneratorEditor.ConvertToUpperSnakeCase(semanticName);
            string displayName = ObjectNames.NicifyVariableName(semanticName);
            string assetFileName = $"{id}_{profileName}.asset";

            entry = new ConsumableGenerationEntry
            {
                Sprite = sprite,
                SpriteIndex = spriteIndex,
                Id = id,
                Key = key,
                DisplayName = displayName,
                AssetFileName = assetFileName,
                Status = ConsumableGenerationStatus.Ready
            };

            return true;
        }
        private void GenerateProfiles()
        {
            if (!this.ValidateConfiguration()) return;

            this.RefreshPreview();

            if (_entries.Count == 0)
            {
                Debug.LogWarning($"[{nameof(ConsumableItemProfileGeneratorEditor)}] No valid consumable sprites were found.");
                return;
            }

            int createdCount = 0;
            int existingCount = 0;
            int failedCount = 0;

            AssetDatabase.StartAssetEditing();

            try
            {
                foreach (ConsumableGenerationEntry entry in _entries)
                {
                    string assetPath = this.GetAssetPath(entry.AssetFileName);

                    ConsumableItemProfile existingProfile = AssetDatabase.LoadAssetAtPath<ConsumableItemProfile>(assetPath);
                    if (existingProfile != null)
                    {
                        existingCount++;
                        entry.Status = ConsumableGenerationStatus.Exists;
                        continue;
                    }

                    try
                    {
                        this.CreateConsumableProfile(entry, assetPath);

                        createdCount++;
                        entry.Status = ConsumableGenerationStatus.Exists;
                    }
                    catch (Exception exception)
                    {
                        failedCount++;

                        Debug.LogError($"[{nameof(ConsumableItemProfileGeneratorEditor)}] Failed to create '{entry.AssetFileName}'.\n" + exception);
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log($"[{nameof(ConsumableItemProfileGeneratorEditor)}] Generation finished. Created: {createdCount}, Already existed: {existingCount}, Failed: {failedCount}.");

            EditorUtility.DisplayDialog("Consumable Profile Generation",$"Generation completed.\n\nCreated: {createdCount}\nAlready existed: {existingCount}\nFailed: {failedCount}", "OK");
        }
        private void CreateConsumableProfile(ConsumableGenerationEntry entry, string assetPath)
        {
            ConsumableItemProfile profile = CreateInstance<ConsumableItemProfile>();

            SerializedObject serializedProfile = new(profile);

            ConsumableItemProfileGeneratorEditor.SetIntProperty(serializedProfile, "_id", entry.Id);
            ConsumableItemProfileGeneratorEditor.SetStringProperty(serializedProfile, "_key", entry.Key);
            ConsumableItemProfileGeneratorEditor.SetStringProperty(serializedProfile, "_displayName", entry.DisplayName);
            ConsumableItemProfileGeneratorEditor.SetStringProperty(serializedProfile, "_description", string.Empty);
            ConsumableItemProfileGeneratorEditor.SetObjectReferenceProperty(serializedProfile, "_icon", entry.Sprite);
            ConsumableItemProfileGeneratorEditor.SetBoolProperty(serializedProfile, "_isStackable", _isStackable);
            int maxStackSize = _isStackable ? Mathf.Clamp(_maxStackSize, 1, 99) : 1;
            ConsumableItemProfileGeneratorEditor.SetIntProperty(serializedProfile, "_maxStackSize", maxStackSize);

            serializedProfile.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(profile, assetPath);

            EditorUtility.SetDirty(profile);

            Debug.Log($"[{nameof(ConsumableItemProfileGeneratorEditor)}] Created '{assetPath}'.");
        }
        private bool ValidateConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_spriteSheetPath))
            {
                Debug.LogError($"[{nameof(ConsumableItemProfileGeneratorEditor)}] Sprite Sheet Path is empty.");
                return false;
            }

            Texture2D spriteSheet = AssetDatabase.LoadAssetAtPath<Texture2D>(_spriteSheetPath);
            if (spriteSheet == null)
            {
                Debug.LogError($"[{nameof(ConsumableItemProfileGeneratorEditor)}] Could not load sprite sheet at '{_spriteSheetPath}'.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_outputFolderPath))
            {
                Debug.LogError($"[{nameof(ConsumableItemProfileGeneratorEditor)}] Output Folder is empty.");
                return false;
            }
            if (!AssetDatabase.IsValidFolder(_outputFolderPath))
            {
                Debug.LogError($"[{nameof(ConsumableItemProfileGeneratorEditor)}] Output folder does not exist: '{_outputFolderPath}'.");
                return false;
            }

            return true;
        }
        private string GetAssetPath(string assetFileName)
        {
            return $"{_outputFolderPath.TrimEnd('/')}/{assetFileName}";
        }

        private static void SetIntProperty(SerializedObject serializedObject, string propertyName, int value)
        {
            SerializedProperty property = ConsumableItemProfileGeneratorEditor.RequireProperty(serializedObject, propertyName);
            property.intValue = value;
        }
        private static void SetStringProperty(SerializedObject serializedObject, string propertyName, string value)
        {
            SerializedProperty property = ConsumableItemProfileGeneratorEditor.RequireProperty(serializedObject, propertyName);
            property.stringValue = value;
        }
        private static void SetBoolProperty(SerializedObject serializedObject, string propertyName, bool value)
        {
            SerializedProperty property = ConsumableItemProfileGeneratorEditor.RequireProperty(serializedObject, propertyName);
            property.boolValue = value;
        }
        private static void SetObjectReferenceProperty(SerializedObject serializedObject, string propertyName, UnityEngine.Object value)
        {
            SerializedProperty property = ConsumableItemProfileGeneratorEditor.RequireProperty(serializedObject, propertyName);
            property.objectReferenceValue = value;
        }
        private static SerializedProperty RequireProperty(SerializedObject serializedObject, string propertyName)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null) throw new InvalidOperationException($"Serialized property '{propertyName}' was not found on '{serializedObject.targetObject.GetType().Name}'.");

            return property;
        }

        private static string ConvertToUpperSnakeCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            string result = Regex.Replace(value, @"(?<=[a-z0-9])([A-Z])", "_$1");

            return result.ToUpperInvariant();
        }

        // --- Entry ---
        private sealed class ConsumableGenerationEntry
        {
            public Sprite Sprite;
            public int SpriteIndex;
            public int Id;
            public string Key;
            public string DisplayName;
            public string AssetFileName;
            public ConsumableGenerationStatus Status;
        }

        private enum ConsumableGenerationStatus
        {
            Ready,
            Exists,
            Invalid
        }
    }
}

#endif
