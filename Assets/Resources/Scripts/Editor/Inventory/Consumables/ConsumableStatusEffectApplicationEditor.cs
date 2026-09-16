#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FlameOfTheLost.Editor
{
    public class ConsumableStatusEffectApplicationEditor : EditorWindow
    {
        // --- Constants ---
        private const string ConsumableProfileFolderPath = "Assets/Resources/Data/Inventory/Items/Consumables";
        private const string ConsumableStatusEffectFolderPath = "Assets/Resources/Data/Character/Shared/StatusEffects/ConsumableItem";

        // --- Runtime ---
        private readonly List<PreviewEntry> _previewEntries = new();

        private Vector2 _scrollPosition;

        private int _correctCount;
        private int _updateCount;
        private int _errorCount;

        // --- Menu ---
        [MenuItem("FlameOfTheLost/Tools/Assets/Data/Inventory/Item/Consumable Status Effect Applications")]
        private static void OpenWindow()
        {
            ConsumableStatusEffectApplicationEditor window = ConsumableStatusEffectApplicationEditor.GetWindow<ConsumableStatusEffectApplicationEditor>();

            window.titleContent = new GUIContent("Consumable Status Effects");
            window.minSize = new Vector2(760f, 500f);
            window.Show();
        }

        // --- Unity Callbacks ---
        private void OnEnable()
        {
            RefreshPreview();
        }

        // --- GUI ---
        private void OnGUI()
        {
            this.DrawHeader();

            EditorGUILayout.Space(8f);

            this.DrawFolderInformation();

            EditorGUILayout.Space(8f);

            this.DrawSummary();

            EditorGUILayout.Space(8f);

            this.DrawToolbar();

            EditorGUILayout.Space(8f);

            this.DrawPreview();
        }

        // --- Helpers ---
        //  1. Draw
        private void DrawHeader()
        {
            EditorGUILayout.LabelField("Consumable Status Effect Applications", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox($"Each {nameof(ConsumableItemProfile)} must contain exactly one {nameof(StatusEffectApplicationEntry)}. The corresponding {nameof(StatusEffectProfile)} is matched by consumable name, and Chance and Potency are both configured to 1.", MessageType.Info);
        }
        private void DrawFolderInformation()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField("Consumables", ConsumableProfileFolderPath);

                EditorGUILayout.TextField("Status Effects", ConsumableStatusEffectFolderPath);
            }
        }
        private void DrawSummary()
        {
            EditorGUILayout.LabelField($"Profiles: {_previewEntries.Count}. Correct: {_correctCount}. Need Update: {_updateCount}. Errors: {_errorCount}", EditorStyles.boldLabel);
        }
        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Refresh Preview", GUILayout.Height(28f)))
                {
                    RefreshPreview();
                }

                using (new EditorGUI.DisabledScope(_updateCount == 0 || _errorCount > 0))
                {
                    if (GUILayout.Button("Apply Missing / Incorrect", GUILayout.Height(28f)))
                    {
                        this.ApplyChanges();
                    }
                }
            }

            if (_errorCount > 0)
            {
                EditorGUILayout.HelpBox("Generation is disabled because one or more Consumables do not have exactly one corresponding Status Effect. Resolve the errors before applying changes.", MessageType.Error);
            }
        }
        private void DrawPreview()
        {
            if (_previewEntries.Count == 0)
            {
                EditorGUILayout.HelpBox("No ConsumableItemProfile assets were found.", MessageType.Warning);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (PreviewEntry entry in _previewEntries)
            {
                ConsumableStatusEffectApplicationEditor.DrawPreviewEntry(entry);
            }

            EditorGUILayout.EndScrollView();
        }
        private static void DrawPreviewEntry(PreviewEntry entry)
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.ObjectField(entry.Consumable, typeof(ConsumableItemProfile), false);

                    GUILayout.Label(entry.Status.ToString(), EditorStyles.boldLabel, GUILayout.Width(100f));
                }

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField("Status Effect", entry.StatusEffect, typeof(StatusEffectProfile), false);
                }

                if (!string.IsNullOrWhiteSpace(entry.Message))
                {
                    MessageType messageType = entry.Status switch
                    {
                        PreviewStatus.Correct => MessageType.None,
                        PreviewStatus.NeedsUpdate => MessageType.Warning,
                        PreviewStatus.Error => MessageType.Error,
                        _ => MessageType.None
                    };

                    EditorGUILayout.HelpBox(entry.Message, messageType);
                }
            }
        }
        private void RefreshPreview()
        {
            _previewEntries.Clear();

            _correctCount = 0;
            _updateCount = 0;
            _errorCount = 0;

            Dictionary<string, List<StatusEffectProfile>> statusEffectsByConsumableName = ConsumableStatusEffectApplicationEditor.BuildStatusEffectLookup();

            string[] consumableGuids = AssetDatabase.FindAssets("t:ConsumableItemProfile", new[] { ConsumableProfileFolderPath });

            List<ConsumableItemProfile> consumables = consumableGuids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ConsumableItemProfile>)
                .Where(profile => profile != null)
                .OrderBy(profile => profile.name, StringComparer.Ordinal)
                .ToList();

            foreach (ConsumableItemProfile consumable in consumables)
            {
                PreviewEntry entry = ConsumableStatusEffectApplicationEditor.BuildPreviewEntry(consumable, statusEffectsByConsumableName);

                _previewEntries.Add(entry);

                switch (entry.Status)
                {
                    case PreviewStatus.Correct:
                        _correctCount++;
                        break;

                    case PreviewStatus.NeedsUpdate:
                        _updateCount++;
                        break;

                    case PreviewStatus.Error:
                        _errorCount++;
                        break;
                }
            }

            this.Repaint();
        }

        //  2. Builders
        private static Dictionary<string, List<StatusEffectProfile>> BuildStatusEffectLookup()
        {
            Dictionary<string, List<StatusEffectProfile>> lookup = new(StringComparer.Ordinal);

            string[] statusEffectGuids = AssetDatabase.FindAssets("t:StatusEffectProfile", new[] { ConsumableStatusEffectFolderPath });

            foreach (string guid in statusEffectGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                StatusEffectProfile profile = AssetDatabase.LoadAssetAtPath<StatusEffectProfile>(path);

                if (profile == null) continue;

                string consumableName = ConsumableStatusEffectApplicationEditor.TryExtractConsumableNameFromStatusEffect(profile.name);

                if (string.IsNullOrWhiteSpace(consumableName)) continue;

                if (!lookup.TryGetValue(consumableName, out List<StatusEffectProfile> profiles))
                {
                    profiles = new List<StatusEffectProfile>();
                    lookup.Add(consumableName, profiles);
                }

                profiles.Add(profile);
            }

            return lookup;
        }

        private static PreviewEntry BuildPreviewEntry(ConsumableItemProfile consumable, Dictionary<string, List<StatusEffectProfile>> statusEffectLookup)
        {
            string consumableName = ConsumableStatusEffectApplicationEditor.TryExtractConsumableNameFromConsumable(consumable.name);

            if (string.IsNullOrWhiteSpace(consumableName))
            {
                return PreviewEntry.CreateError(consumable, $"Could not extract Consumable name from asset '{consumable.name}'. Expected '*_Csm_<Name>'.");
            }
            if (!statusEffectLookup.TryGetValue(consumableName, out List<StatusEffectProfile> matches))
            {
                return PreviewEntry.CreateError(consumable, $"No StatusEffectProfile was found for '{consumableName}'.");
            }

            if (matches.Count != 1)
            {
                string matchingNames = string.Join(", ", matches.Select(profile => profile.name));

                return PreviewEntry.CreateError(consumable, $"Expected exactly one StatusEffectProfile for '{consumableName}', but found {matches.Count}: " + matchingNames);
            }

            StatusEffectProfile expectedStatusEffect = matches[0];

            SerializedObject serializedConsumable = new SerializedObject(consumable);

            SerializedProperty applicationsProperty = serializedConsumable.FindProperty("_statusEffectApplications");

            if (applicationsProperty == null)
            {
                return PreviewEntry.CreateError(consumable, "Serialized property '_statusEffectApplications' could not be found.");
            }

            if (!applicationsProperty.isArray)
            {
                return PreviewEntry.CreateError(consumable, "'_statusEffectApplications' is not an array/list.");
            }

            if (applicationsProperty.arraySize != 1)
            {
                return PreviewEntry.CreateNeedsUpdate(consumable, expectedStatusEffect, $"Application list currently contains {applicationsProperty.arraySize} entries. It will be replaced with exactly one entry.");
            }

            SerializedProperty entryProperty        = applicationsProperty.GetArrayElementAtIndex(0);
            SerializedProperty statusEffectProperty = entryProperty.FindPropertyRelative("_statusEffect");
            SerializedProperty chanceProperty       = entryProperty.FindPropertyRelative("_chance");
            SerializedProperty potencyProperty      = entryProperty.FindPropertyRelative("_potency");

            if (statusEffectProperty == null || chanceProperty == null || potencyProperty == null)
            {
                return PreviewEntry.CreateError(consumable, "One or more StatusEffectApplicationEntry fields could not be found.");
            }

            bool hasCorrectProfile  = statusEffectProperty.objectReferenceValue == expectedStatusEffect;
            bool hasCorrectChance   = Mathf.Approximately(chanceProperty.floatValue, 1f);
            bool hasCorrectPotency  = Mathf.Approximately(potencyProperty.floatValue, 1f);

            if (hasCorrectProfile && hasCorrectChance && hasCorrectPotency)
            {
                return PreviewEntry.CreateCorrect(consumable, expectedStatusEffect);
            }

            List<string> issues = new();

            if (!hasCorrectProfile)
            {
                issues.Add("Status Effect");
            }
            if (!hasCorrectChance)
            {
                issues.Add("Chance");
            }
            if (!hasCorrectPotency)
            {
                issues.Add("Potency");
            }

            return PreviewEntry.CreateNeedsUpdate(consumable, expectedStatusEffect, $"Will update: {string.Join(", ", issues)}.");
        }

        private void ApplyChanges()
        {
            if (_errorCount > 0) return;

            List<PreviewEntry> entriesToUpdate = _previewEntries.Where(entry => entry.Status == PreviewStatus.NeedsUpdate).ToList();
            if (entriesToUpdate.Count == 0) return;

            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Assign Consumable Status Effects");

            try
            {
                foreach (PreviewEntry entry in entriesToUpdate)
                {
                    ConsumableStatusEffectApplicationEditor.ApplyEntry(entry);
                }

                AssetDatabase.SaveAssets();
            }
            finally
            {
                Undo.CollapseUndoOperations(undoGroup);
            }

            Debug.Log($"[{nameof(ConsumableStatusEffectApplicationEditor)}] Finished assigning Consumable Status Effects. Updated: {entriesToUpdate.Count}.");

            this.RefreshPreview();
        }
        private static void ApplyEntry(PreviewEntry previewEntry)
        {
            ConsumableItemProfile consumable = previewEntry.Consumable;

            StatusEffectProfile statusEffect = previewEntry.StatusEffect;

            Undo.RecordObject(consumable, "Assign Consumable Status Effect");

            SerializedObject serializedConsumable = new SerializedObject(consumable);

            SerializedProperty applicationsProperty = serializedConsumable.FindProperty("_statusEffectApplications");

            applicationsProperty.arraySize = 1;

            SerializedProperty applicationProperty  = applicationsProperty.GetArrayElementAtIndex(0);
            SerializedProperty statusEffectProperty = applicationProperty.FindPropertyRelative("_statusEffect");
            SerializedProperty chanceProperty       = applicationProperty.FindPropertyRelative("_chance");
            SerializedProperty potencyProperty      = applicationProperty.FindPropertyRelative("_potency");

            statusEffectProperty.objectReferenceValue = statusEffect;

            chanceProperty.floatValue = 1f;
            potencyProperty.floatValue = 1f;

            serializedConsumable.ApplyModifiedProperties();

            EditorUtility.SetDirty(consumable);
        }

        private static string TryExtractConsumableNameFromConsumable(string assetName)
        {
            const string marker = "_Csm_";

            int markerIndex = assetName.IndexOf(marker, StringComparison.Ordinal);
            if (markerIndex < 0) return null;

            int nameStartIndex = markerIndex + marker.Length;
            if (nameStartIndex >= assetName.Length) return null;

            return assetName[nameStartIndex..];
        }
        private static string TryExtractConsumableNameFromStatusEffect(string assetName)
        {
            const string marker = "_SE_Csm_";

            int markerIndex = assetName.IndexOf(marker, StringComparison.Ordinal);
            if (markerIndex < 0) return null;

            int nameStartIndex = markerIndex + marker.Length;
            if (nameStartIndex >= assetName.Length) return null;

            string remainingName = assetName[nameStartIndex..];

            int separatorIndex = remainingName.IndexOf('_');
            if (separatorIndex <= 0) return null;

            return remainingName[..separatorIndex];
        }

        // --- Models ---
        private enum PreviewStatus
        {
            Correct,
            NeedsUpdate,
            Error
        }
        private sealed class PreviewEntry
        {
            public ConsumableItemProfile Consumable { get; private set; }
            public StatusEffectProfile StatusEffect { get; private set; }
            public PreviewStatus Status { get; private set; }
            public string Message { get; private set; }

            private PreviewEntry()
            {
            }

            public static PreviewEntry CreateCorrect(ConsumableItemProfile consumable, StatusEffectProfile statusEffect)
            {
                return new PreviewEntry
                {
                    Consumable = consumable,
                    StatusEffect = statusEffect,
                    Status = PreviewStatus.Correct,
                    Message ="Exactly one application is configured with the correct Status Effect, Chance = 1 and Potency = 1."
                };
            }
            public static PreviewEntry CreateNeedsUpdate(ConsumableItemProfile consumable, StatusEffectProfile statusEffect, string message)
            {
                return new PreviewEntry
                {
                    Consumable = consumable,
                    StatusEffect = statusEffect,
                    Status = PreviewStatus.NeedsUpdate,
                    Message = message
                };
            }
            public static PreviewEntry CreateError(ConsumableItemProfile consumable, string message)
            {
                return new PreviewEntry
                {
                    Consumable = consumable,
                    StatusEffect = null,
                    Status = PreviewStatus.Error,
                    Message = message
                };
            }
        }
    }
}

#endif
