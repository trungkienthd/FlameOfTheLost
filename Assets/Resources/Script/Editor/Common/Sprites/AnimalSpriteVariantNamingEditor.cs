#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace FlameOfTheLost.Editor
{
    public sealed class AnimalSpriteVariantNamingEditor : EditorWindow
    {
        // --- Constants ---
        private const string AnimalSpriteRoot = "Assets/Resources/Art/Sprites/Characters/AI/Neutral/Animals";

        // --- Configs ---
        private AnimalType _selectedAnimal = AnimalType.Cow;

        // --- Runtime ---
        private Vector2 _scrollPosition;

        private static readonly IReadOnlyDictionary<AnimalType, AnimalNamingDefinition> AnimalDefinitions = new Dictionary<AnimalType, AnimalNamingDefinition>
        {
            {
                AnimalType.Cow,
                new AnimalNamingDefinition
                (
                    animalName: "Cow",
                    sourceVariantName: "CowCream",
                    variantNames: new Dictionary<int, string>
                    {
                        { 1, "CowCream" },
                        { 2, "CowTan" },
                        { 3, "CowOrange" },
                        { 4, "CowBrown" },
                        { 5, "CowDarkBrown" },
                        { 6, "CowBurgundy" },
                        { 7, "CowGrey" },
                        { 8, "CowBlue" },
                        { 9, "CowBlack" }
                    })
            },

            {
                AnimalType.Sheep,
                new AnimalNamingDefinition
                (
                    animalName: "Sheep",
                    sourceVariantName: "SheepWhite",
                    variantNames: new Dictionary<int, string>
                    {
                        { 1, "SheepWhite" },
                        { 2, "SheepRed" },
                        { 3, "SheepOrange" },
                        { 4, "SheepYellow" },
                        { 5, "SheepGreen" },
                        { 6, "SheepBlue" },
                        { 7, "SheepPurple" },
                        { 8, "SheepPink" },
                        { 9, "SheepBlack" }
                    })
            }
        };

        // --- Menu ---
        [MenuItem("FlameOfTheLost/Tools/Assets/Art/Sprites/Animal Variant Naming")]
        private static void OpenWindow()
        {
            AnimalSpriteVariantNamingEditor window = AnimalSpriteVariantNamingEditor.GetWindow<AnimalSpriteVariantNamingEditor>();

            window.titleContent = new GUIContent("Animal Sprite Naming");

            window.minSize = new Vector2(520f, 420f);

            window.Show();
        }

        // --- GUI ---
        private void OnGUI()
        {
            EditorGUILayout.Space(8f);

            EditorGUILayout.LabelField("Animal Sprite Variant Naming", EditorStyles.boldLabel);

            EditorGUILayout.Space(4f);

            EditorGUILayout.HelpBox("Uses Variant 01 as the naming template and applies its animation names to the other sprite sheets while replacing the animal variant name.", MessageType.Info);

            EditorGUILayout.Space(8f);

            _selectedAnimal = (AnimalType)EditorGUILayout.EnumPopup("Animal", _selectedAnimal);

            AnimalNamingDefinition definition = AnimalSpriteVariantNamingEditor.GetDefinition(_selectedAnimal);

            EditorGUILayout.Space(8f);

            this.DrawDefinition(definition);

            EditorGUILayout.Space(12f);

            using (new EditorGUI.DisabledScope(definition == null))
            {
                if (GUILayout.Button($"Rename {definition?.AnimalName} Variants 02 - 09", GUILayout.Height(32f)))
                {
                    AnimalSpriteVariantNamingEditor.RenameAllTargetVariants(definition);
                }
            }

            EditorGUILayout.Space(6f);

            if (GUILayout.Button($"Validate {definition?.AnimalName} Sprite Sheets", GUILayout.Height(26f)))
            {
                AnimalSpriteVariantNamingEditor.ValidateDefinition(definition);
            }
        }

        // --- Helpers ---
        //  1. Drawing
        private void DrawDefinition(AnimalNamingDefinition definition)
        {
            if (definition == null) return;

            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Folder", definition.FolderPath);
            EditorGUILayout.LabelField("Template", Path.GetFileName(definition.GetSpriteSheetPath(1)));

            EditorGUILayout.Space(6f);

            EditorGUILayout.LabelField("Variant Mapping", EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.MaxHeight(210f));

            foreach (KeyValuePair<int, string> pair in definition.VariantNames.OrderBy(pair => pair.Key))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField($"{definition.AnimalName}_{pair.Key:00}", GUILayout.Width(120f));

                    EditorGUILayout.LabelField(pair.Value);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        //  2. Definition Getter
        private static AnimalNamingDefinition GetDefinition(AnimalType animalType)
        {
            return AnimalDefinitions.TryGetValue(animalType, out AnimalNamingDefinition definition)
                ? definition
                : null;
        }

        //  3. Rename
        private static void RenameAllTargetVariants(AnimalNamingDefinition definition)
        {
            if (definition == null) return;

            string sourcePath = definition.GetSpriteSheetPath(1);

            if (!AnimalSpriteVariantNamingEditor.TryGetSpriteRects(sourcePath, out _, out _, out SpriteRect[] sourceSpriteRects))
            {
                Debug.LogError($"[{nameof(AnimalSpriteVariantNamingEditor)}] Could not load source sprite sheet:\n{sourcePath}");
                return;
            }

            string sourceSheetPrefix = Path.GetFileNameWithoutExtension(sourcePath);
            Dictionary<Rect, SourceSpriteEntry> sourceEntriesByRect = AnimalSpriteVariantNamingEditor.BuildSourceEntriesByRect(definition, sourceSheetPrefix, sourceSpriteRects);

            if (sourceEntriesByRect.Count == 0)
            {
                Debug.LogError($"[{nameof(AnimalSpriteVariantNamingEditor)}] No valid source sprite names found for {definition.AnimalName}.");
                return;
            }

            int successfulSheets = 0;

            foreach (KeyValuePair<int, string> variant in definition.VariantNames.OrderBy(pair => pair.Key))
            {
                // Variant 01 is our authoritative source/template.
                if (variant.Key == 1) continue;

                if (AnimalSpriteVariantNamingEditor.RenameTargetVariant(definition, variant.Key, variant.Value, sourceEntriesByRect))
                {
                    successfulSheets++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[{nameof(AnimalSpriteVariantNamingEditor)}] Finished {definition.AnimalName} variant naming. Successfully processed {successfulSheets} target sheet(s).");
        }
        private static bool RenameTargetVariant(AnimalNamingDefinition definition, int variantIndex, string variantName, IReadOnlyDictionary<Rect, SourceSpriteEntry> sourceEntriesByRect)
        {
            string targetPath = definition.GetSpriteSheetPath(variantIndex);
            if (!AnimalSpriteVariantNamingEditor.TryGetSpriteRects(targetPath, out TextureImporter targetImporter, out ISpriteEditorDataProvider targetDataProvider, out SpriteRect[] targetSpriteRects))
            {
                Debug.LogError($"[{nameof(AnimalSpriteVariantNamingEditor)}] Could not load target sprite sheet:\n{targetPath}");
                return false;
            }

            if (targetSpriteRects.Length != sourceEntriesByRect.Count)
            {
                Debug.LogError($"[{nameof(AnimalSpriteVariantNamingEditor)}] {definition.AnimalName}_{variantIndex:00} has {targetSpriteRects.Length} sprite(s), while the source contains {sourceEntriesByRect.Count}.");
                return false;
            }

            string targetSheetPrefix = Path.GetFileNameWithoutExtension(targetPath);

            int renamedCount = 0;
            int unchangedCount = 0;
            int skippedCount = 0;

            foreach (SpriteRect targetSpriteRect in targetSpriteRects)
            {
                if (!sourceEntriesByRect.TryGetValue(targetSpriteRect.rect, out SourceSpriteEntry sourceEntry))
                {
                    Debug.LogWarning($"[{nameof(AnimalSpriteVariantNamingEditor)}] [{definition.AnimalName}_{variantIndex:00}] No source sprite found at rect {targetSpriteRect.rect}.");
                    skippedCount++;
                    continue;
                }

                string expectedName = $"{targetSheetPrefix}_{sourceEntry.Index}_{variantName}_{sourceEntry.AnimationSuffix}";

                if (targetSpriteRect.name == expectedName)
                {
                    unchangedCount++;
                    continue;
                }

                Debug.Log($"[{nameof(AnimalSpriteVariantNamingEditor)}] [{definition.AnimalName}_{variantIndex:00}] '{targetSpriteRect.name}' -> '{expectedName}'");

                targetSpriteRect.name = expectedName;
                renamedCount++;
            }

            targetDataProvider.SetSpriteRects(targetSpriteRects);
            targetDataProvider.Apply();

            targetImporter.SaveAndReimport();

            Debug.Log($"[{nameof(AnimalSpriteVariantNamingEditor)}] Finished {definition.AnimalName}_{variantIndex:00}. Renamed: {renamedCount}, Already correct: {unchangedCount}, Skipped: {skippedCount}.");

            return skippedCount == 0;
        }

        //  4. Source Parsing
        private static Dictionary<Rect, SourceSpriteEntry> BuildSourceEntriesByRect(AnimalNamingDefinition definition, string sourceSheetPrefix, IEnumerable<SpriteRect> spriteRects)
        {
            Dictionary<Rect, SourceSpriteEntry> entries = new();

            foreach (SpriteRect spriteRect in spriteRects)
            {
                if (!AnimalSpriteVariantNamingEditor.TryParseSourceSpriteName(spriteRect.name, sourceSheetPrefix, definition.SourceVariantName, out int spriteIndex, out string animationSuffix))
                {
                    Debug.LogWarning($"[{nameof(AnimalSpriteVariantNamingEditor)}] Could not parse source sprite name '{spriteRect.name}'.");
                    continue;
                }

                if (entries.ContainsKey(spriteRect.rect))
                {
                    Debug.LogWarning($"[{nameof(AnimalSpriteVariantNamingEditor)}] Duplicate source rect detected: {spriteRect.rect}");
                    continue;
                }

                var sourceSprite = new SourceSpriteEntry(spriteIndex, spriteRect.rect, animationSuffix);
                entries.Add(spriteRect.rect, sourceSprite);
            }

            return entries;
        }
        private static bool TryParseSourceSpriteName(string spriteName, string spriteSheetPrefix, string sourceVariantName, out int spriteIndex, out string animationSuffix)
        {
            spriteIndex = -1;
            animationSuffix = null;

            string requiredPrefix = $"{spriteSheetPrefix}_";

            if (!spriteName.StartsWith(requiredPrefix, StringComparison.Ordinal)) return false;

            string remaining = spriteName.Substring(requiredPrefix.Length);

            int firstUnderscore = remaining.IndexOf('_');
            if (firstUnderscore <= 0) return false;

            string indexText = remaining.Substring(0, firstUnderscore);
            if (!int.TryParse(indexText, out spriteIndex)) return false;

            remaining = remaining.Substring(firstUnderscore + 1);

            string variantPrefix = $"{sourceVariantName}_";
            if (!remaining.StartsWith(variantPrefix, StringComparison.Ordinal)) return false;

            animationSuffix = remaining.Substring(variantPrefix.Length);

            return !string.IsNullOrWhiteSpace(animationSuffix);
        }

        //  5. Validation
        private static void ValidateDefinition(AnimalNamingDefinition definition)
        {
            if (definition == null) return;

            int? expectedSpriteCount = null;
            bool valid = true;

            foreach (KeyValuePair<int, string> variant in definition.VariantNames.OrderBy(pair => pair.Key))
            {
                string path = definition.GetSpriteSheetPath(variant.Key);
                if (!AnimalSpriteVariantNamingEditor.TryGetSpriteRects(path, out _, out _, out SpriteRect[] spriteRects))
                {
                    Debug.LogError($"[{nameof(AnimalSpriteVariantNamingEditor)}] Missing or invalid sprite sheet:\n{path}");
                    valid = false;
                    continue;
                }

                expectedSpriteCount ??= spriteRects.Length;

                if (spriteRects.Length != expectedSpriteCount.Value)
                {
                    Debug.LogError($"[{nameof(AnimalSpriteVariantNamingEditor)}] {definition.AnimalName}_{variant.Key:00} contains {spriteRects.Length} sprites; expected {expectedSpriteCount.Value}.");
                    valid = false;
                }
            }

            if (valid)
            {
                Debug.Log($"[{nameof(AnimalSpriteVariantNamingEditor)}] {definition.AnimalName} validation successful. All {definition.VariantNames.Count} sprite sheets have {expectedSpriteCount} sprite(s).");
            }
        }

        //  6. Sprite Editor Access
        private static bool TryGetSpriteRects(string assetPath, out TextureImporter textureImporter, out ISpriteEditorDataProvider dataProvider, out SpriteRect[] spriteRects)
        {
            textureImporter = null;
            dataProvider = null;
            spriteRects = null;

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

            if (texture == null) return false;

            textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (textureImporter == null) return false;

            SpriteDataProviderFactories factories = new SpriteDataProviderFactories();

            factories.Init();

            dataProvider = factories.GetSpriteEditorDataProviderFromObject(textureImporter);
            if (dataProvider == null) return false;

            dataProvider.InitSpriteEditorDataProvider();

            spriteRects = dataProvider.GetSpriteRects();

            return spriteRects != null;
        }

        // --- Modeling ---
        private sealed class AnimalNamingDefinition
        {
            public string AnimalName { get; }
            public string SourceVariantName { get; }
            public IReadOnlyDictionary<int, string> VariantNames { get; }

            public AnimalNamingDefinition(string animalName, string sourceVariantName, IReadOnlyDictionary<int, string> variantNames)
            {
                AnimalName = animalName;
                SourceVariantName = sourceVariantName;
                VariantNames = variantNames;
            }

            public string FolderPath => $"{AnimalSpriteVariantNamingEditor.AnimalSpriteRoot}/{this.AnimalName}";
            public string GetSpriteSheetPath(int variantIndex)
            {
                return $"{this.FolderPath}/CFRPG16_{this.AnimalName}_{variantIndex:00}.png";
            }
        }
        private sealed class SourceSpriteEntry
        {
            public int Index { get; }
            public Rect Rect { get; }
            public string AnimationSuffix { get; }

            public SourceSpriteEntry(int index, Rect rect, string animationSuffix)
            {
                Index = index;
                Rect = rect;
                AnimationSuffix = animationSuffix;
            }
        }

    }
}

#endif