#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace FlameOfTheLost.Editor
{
    public static class SpriteSheetVariantNamingEditor
    {
        // --- Entry ---
        private readonly struct SourceSpriteNamingEntry
        {
            public int Index { get; }
            public string SemanticName { get; }
            public Rect Rect { get; }

            public SourceSpriteNamingEntry(int index, string semanticName, Rect rect)
            {
                Index = index;
                SemanticName = semanticName;
                Rect = rect;
            }
        }

        // --- Configuration ---
        private readonly struct SpriteSheetRenameConfig
        {
            public string DisplayName { get; }
            public string SourceSpriteSheetPath { get; }
            public string TargetSpriteSheetPath { get; }

            public SpriteSheetRenameConfig(string displayName, string sourceSpriteSheetPath, string targetSpriteSheetPath)
            {
                DisplayName = displayName;
                SourceSpriteSheetPath = sourceSpriteSheetPath;
                TargetSpriteSheetPath = targetSpriteSheetPath;
            }
        }

        private static readonly SpriteSheetRenameConfig MaterialConfig = new
        (
            "Material",
            "Assets/Resources/Art/Sprites/Inventory/Items/Materials/CFRPG16_Resources_Icons_NO_Outline.png",
            "Assets/Resources/Art/Sprites/Inventory/Items/Materials/CFRPG16_Resources_Icons_Outline.png"
        );
        private static readonly SpriteSheetRenameConfig ConsumableConfig = new
        (
            "Consumable",
            "Assets/Resources/Art/Sprites/Inventory/Items/Consumables/CFRPG16_Food_Icons_NO_Outline.png",
            "Assets/Resources/Art/Sprites/Inventory/Items/Consumables/CFRPG16_Food_Icons_Outline.png"
        );
        private static readonly IReadOnlyList<SpriteSheetRenameConfig> AllConfigs = new[]
        {
            SpriteSheetVariantNamingEditor.MaterialConfig,
            SpriteSheetVariantNamingEditor.ConsumableConfig
        };

        // --- Menu ---

        [MenuItem("FlameOfTheLost/Tools/Assets/Art/Sprites/Items/Rename Outline Sprites/Materials")]
        private static void RenameMaterialOutlineSprites()
        {
            SpriteSheetVariantNamingEditor.RenameTargetSpriteSheet(MaterialConfig);
        }
        [MenuItem("FlameOfTheLost/Tools/Assets/Art/Sprites/Items/Rename Outline Sprites/Consumables")]
        private static void RenameConsumableOutlineSprites()
        {
            SpriteSheetVariantNamingEditor.RenameTargetSpriteSheet(ConsumableConfig);
        }
        [MenuItem("FlameOfTheLost/Tools/Assets/Art/Sprites/Items/Rename Outline Sprites/All")]
        private static void RenameAllOutlineSprites()
        {
            int successfulCount = 0;

            foreach (SpriteSheetRenameConfig config in AllConfigs)
            {
                if (SpriteSheetVariantNamingEditor.RenameTargetSpriteSheet(config))
                {
                    successfulCount++;
                }
            }

            Debug.Log($"[{nameof(SpriteSheetVariantNamingEditor)}] Finished processing all configured sprite sheets. {successfulCount}/{AllConfigs.Count} sheet pair(s) processed successfully.");
        }

        // --- Helpers ---
        //  1. Rename
        private static bool RenameTargetSpriteSheet(SpriteSheetRenameConfig config)
        {
            if (!SpriteSheetVariantNamingEditor.TryGetSpriteRects(config.SourceSpriteSheetPath, out _, out _, out SpriteRect[] sourceSpriteRects))
            {
                Debug.LogError($"[{nameof(SpriteSheetVariantNamingEditor)}] Could not load source sprite sheet for '{config.DisplayName}':\n" + config.SourceSpriteSheetPath);
                return false;
            }
            if (!SpriteSheetVariantNamingEditor.TryGetSpriteRects(config.TargetSpriteSheetPath, out TextureImporter targetImporter, out ISpriteEditorDataProvider targetDataProvider, out SpriteRect[] targetSpriteRects))
            {
                Debug.LogError($"[{nameof(SpriteSheetVariantNamingEditor)}] Could not load target sprite sheet for '{config.DisplayName}':\n" + config.TargetSpriteSheetPath);
                return false;
            }
            if (sourceSpriteRects.Length != targetSpriteRects.Length)
            {
                Debug.LogError($"[{nameof(SpriteSheetVariantNamingEditor)}] Sprite count mismatch for '{config.DisplayName}'. Source has {sourceSpriteRects.Length} sprite(s), target has {targetSpriteRects.Length} sprite(s).");
                return false;
            }

            string sourcePrefix = Path.GetFileNameWithoutExtension(config.SourceSpriteSheetPath);
            string targetPrefix = Path.GetFileNameWithoutExtension(config.TargetSpriteSheetPath);

            Dictionary<int, SourceSpriteNamingEntry> sourceEntriesByIndex = SpriteSheetVariantNamingEditor.BuildSourceNamesByIndex(config.DisplayName, sourcePrefix, sourceSpriteRects);
            if (sourceEntriesByIndex.Count == 0)
            {
                Debug.LogWarning($"[{nameof(SpriteSheetVariantNamingEditor)}] No semantic sprite names were found in source sheet '{config.DisplayName}'. Nothing to rename.");
                return false;
            }
            Dictionary<Rect, SpriteRect> targetSpritesByRect = SpriteSheetVariantNamingEditor.BuildTargetSpritesByRect(config.DisplayName, targetSpriteRects);

            int renamedCount = 0;
            int unchangedCount = 0;
            int skippedCount = 0;

            foreach (SourceSpriteNamingEntry sourceEntry in sourceEntriesByIndex.Values)
            {
                if (!targetSpritesByRect.TryGetValue(sourceEntry.Rect, out SpriteRect targetSpriteRect))
                {
                    Debug.LogWarning($"[{nameof(SpriteSheetVariantNamingEditor)}] [{config.DisplayName}] Could not find target sprite matching source index {sourceEntry.Index} at rect {sourceEntry.Rect}.");
                    skippedCount++;
                    continue;
                }

                string expectedName = $"{targetPrefix}_{sourceEntry.Index}_{sourceEntry.SemanticName}";
                if (targetSpriteRect.name == expectedName)
                {
                    unchangedCount++;
                    continue;
                }

                Debug.Log($"[{nameof(SpriteSheetVariantNamingEditor)}] [{config.DisplayName}] Renaming '{targetSpriteRect.name}' -> '{expectedName}'.");

                targetSpriteRect.name = expectedName;
                renamedCount++;
            }

            targetDataProvider.SetSpriteRects(targetSpriteRects);
            targetDataProvider.Apply();

            targetImporter.SaveAndReimport();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[{nameof(SpriteSheetVariantNamingEditor)}] Finished '{config.DisplayName}'. Renamed: {renamedCount}, Already correct: {unchangedCount}, Skipped: {skippedCount}.");

            return true;
        }

        //  2. Source Name Resolution
        private static Dictionary<int, SourceSpriteNamingEntry> BuildSourceNamesByIndex(string displayName, string spriteSheetPrefix, IEnumerable<SpriteRect> spriteRects)
        {
            Dictionary<int, SourceSpriteNamingEntry> entriesByIndex = new();

            foreach (SpriteRect spriteRect in spriteRects)
            {
                if (!SpriteSheetVariantNamingEditor.TryGetSpriteIndexAndSemanticName(spriteRect.name, spriteSheetPrefix, out int index, out string semanticName))
                {
                    Debug.Log($"[{nameof(SpriteSheetVariantNamingEditor)}] [{displayName}] Skipping source sprite '{spriteRect.name}' because it does not contain a semantic name after its index.");
                    continue;
                }
                if (entriesByIndex.ContainsKey(index))
                {
                    Debug.LogWarning($"[{nameof(SpriteSheetVariantNamingEditor)}] [{displayName}] Duplicate source sprite index {index} detected.");
                    continue;
                }

                SourceSpriteNamingEntry entry = new SourceSpriteNamingEntry(index, semanticName, spriteRect.rect);
                entriesByIndex.Add(index, entry);
            }

            return entriesByIndex;
        }
        private static Dictionary<Rect, SpriteRect> BuildTargetSpritesByRect(string displayName, IEnumerable<SpriteRect> spriteRects)
        {
            Dictionary<Rect, SpriteRect> spritesByRect = new();

            foreach (SpriteRect spriteRect in spriteRects)
            {
                Rect currentRect = spriteRect.rect;
                if (spritesByRect.ContainsKey(currentRect))
                {
                    Debug.LogWarning($"[{nameof(SpriteSheetVariantNamingEditor)}] [{displayName}] Duplicate target sprite rectangle detected for sprite '{spriteRect.name}'.");
                    continue;
                }

                spritesByRect.Add(currentRect, spriteRect);
            }

            return spritesByRect;
        }

        //  3. Sprite Name Parsing
        private static bool TryGetSpriteIndexAndSemanticName(string spriteName, string spriteSheetPrefix, out int index, out string semanticName)
        {
            index = -1;
            semanticName = null;

            if (string.IsNullOrWhiteSpace(spriteName)) return false;
            if (string.IsNullOrWhiteSpace(spriteSheetPrefix)) return false;

            string expectedPrefix = $"{spriteSheetPrefix}_";
            if (!spriteName.StartsWith(expectedPrefix, StringComparison.Ordinal)) return false;

            string remainder = spriteName.Substring(expectedPrefix.Length);

            int separatorIndex = remainder.IndexOf('_');
            if (separatorIndex < 0) return false;

            string indexText = remainder.Substring(0, separatorIndex);
            if (!int.TryParse(indexText, out index))
            {
                index = -1;
                return false;
            }

            semanticName = remainder.Substring(separatorIndex + 1);
            if (string.IsNullOrWhiteSpace(semanticName))
            {
                index = -1;
                semanticName = null;
                return false;
            }

            return true;
        }

        //  4. Sprite Data
        private static bool TryGetSpriteRects(string spriteSheetPath, out TextureImporter importer, out ISpriteEditorDataProvider dataProvider, out SpriteRect[] spriteRects)
        {
            importer = AssetImporter.GetAtPath(spriteSheetPath) as TextureImporter;

            dataProvider = null;
            spriteRects = null;

            if (importer == null)
            {
                Debug.LogError($"[{nameof(SpriteSheetVariantNamingEditor)}] Could not find TextureImporter at:\n{spriteSheetPath}");
                return false;
            }

            SpriteDataProviderFactories providerFactories = new SpriteDataProviderFactories();
            providerFactories.Init();

            dataProvider = providerFactories.GetSpriteEditorDataProviderFromObject(importer);

            if (dataProvider == null)
            {
                Debug.LogError($"[{nameof(SpriteSheetVariantNamingEditor)}] Could not obtain sprite editor data provider for:\n" + spriteSheetPath);
                return false;
            }

            dataProvider.InitSpriteEditorDataProvider();

            spriteRects = dataProvider.GetSpriteRects();
            if (spriteRects == null || spriteRects.Length == 0)
            {
                Debug.LogError($"[{nameof(SpriteSheetVariantNamingEditor)}] No sliced sprites were found in:\n{spriteSheetPath}");
                return false;
            }

            return true;
        }
    }
}

#endif