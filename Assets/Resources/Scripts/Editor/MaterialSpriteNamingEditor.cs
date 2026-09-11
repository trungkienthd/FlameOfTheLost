#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class MaterialSpriteNamingEditor
    {
        // --- Constants ---
        private const string OutlineSpriteSheetPath = "Assets/Resources/Art/Sprites/Inventory/Items/Materials/CFRPG16_Resources_Icons_Outline.png";
        private const string OutlineSpritePrefix = "CFRPG16_Resources_Icons_Outline";

        // --- Data ---
        private static readonly IReadOnlyDictionary<int, string> MaterialNames = new Dictionary<int, string>
        {
            {  0, "Mtr_IronChunks" },
            {  1, "Mtr_IronOre" },
            {  2, "Mtr_IronBar" },

            {  3, "Mtr_AquamarineChunks" },
            {  4, "Mtr_AquamarineCrystal" },
            {  5, "Mtr_AquamarineBlock" },

            {  6, "Mtr_CopperChunks" },
            {  7, "Mtr_CopperOre" },
            {  8, "Mtr_CopperBar" },

            {  9, "Mtr_AmberChunks" },
            { 10, "Mtr_AmberCrystal" },
            { 11, "Mtr_AmberBlock" },

            { 12, "Mtr_GoldChunks" },
            { 13, "Mtr_GoldOre" },
            { 14, "Mtr_GoldBar" },

            { 15, "Mtr_RubyChunks" },
            { 16, "Mtr_RubyCrystal" },
            { 17, "Mtr_RubyBlock" },

            { 18, "Mtr_EmeraldChunks" },
            { 19, "Mtr_EmeraldCrystal" },
            { 20, "Mtr_EmeraldBlock" },

            { 21, "Mtr_AmethystChunks" },
            { 22, "Mtr_AmethystCrystal" },
            { 23, "Mtr_AmethystBlock" },

            { 24, "Mtr_Wood" },
            { 25, "Mtr_Bone" },
            { 26, "Mtr_Twig" },

            // 27 and 28 are intentionally left unnamed for now.

            { 29, "Mtr_PurpleOrb" },
            { 30, "Mtr_Stone" }
        };

        // --- Menu ---
        [MenuItem("FlameOfTheLost/Tools/Assets/Art/Sprites/Item/Material/Rename Outline Sprites")]
        private static void RenameOutlineMaterialSprites()
        {
            TextureImporter importer = AssetImporter.GetAtPath(MaterialSpriteNamingEditor.OutlineSpriteSheetPath) as TextureImporter;

            if (importer == null)
            {
                Debug.LogError($"[{nameof(MaterialSpriteNamingEditor)}] Could not find TextureImporter at:\n{MaterialSpriteNamingEditor.OutlineSpriteSheetPath}");
                return;
            }

            SpriteDataProviderFactories providerFactories = new SpriteDataProviderFactories();

            providerFactories.Init();

            ISpriteEditorDataProvider dataProvider = providerFactories.GetSpriteEditorDataProviderFromObject(importer);

            if (dataProvider == null)
            {
                Debug.LogError($"[{nameof(MaterialSpriteNamingEditor)}] Could not obtain sprite editor data provider.");
                return;
            }

            dataProvider.InitSpriteEditorDataProvider();

            SpriteRect[] spriteRects = dataProvider.GetSpriteRects();

            if (spriteRects == null || spriteRects.Length == 0)
            {
                Debug.LogError($"[{nameof(MaterialSpriteNamingEditor)}] No sliced sprites were found in the outline sprite sheet.");
                return;
            }

            if (spriteRects.Length <= 30)
            {
                Debug.LogError($"[{nameof(MaterialSpriteNamingEditor)}] Expected at least 31 sprites, but found {spriteRects.Length}.");
                return;
            }

            // The sprites are arranged/indexed according to their current
            // numeric suffixes, so resolve the index from the existing name.
            Dictionary<int, SpriteRect> spritesByIndex = new();

            foreach (SpriteRect spriteRect in spriteRects)
            {
                if (!TryGetSpriteIndex(spriteRect.name, out int index))
                {
                    Debug.LogWarning($"[{nameof(MaterialSpriteNamingEditor)}] Could not determine index from sprite '{spriteRect.name}'.");
                    continue;
                }

                spritesByIndex[index] = spriteRect;
            }

            int renamedCount = 0;

            foreach (KeyValuePair<int, string> pair in MaterialNames)
            {
                int index = pair.Key;
                string materialName = pair.Value;

                if (!spritesByIndex.TryGetValue(index, out SpriteRect spriteRect))
                {
                    Debug.LogWarning($"[{nameof(MaterialSpriteNamingEditor)}] Could not find sprite index {index}.");
                    continue;
                }

                string expectedName = $"{MaterialSpriteNamingEditor.OutlineSpritePrefix}_{index}_{materialName}";

                if (spriteRect.name == expectedName) continue;

                Debug.Log($"[{nameof(MaterialSpriteNamingEditor)}] Renaming '{spriteRect.name}' -> '{expectedName}'.");

                spriteRect.name = expectedName;
                renamedCount++;
            }

            dataProvider.SetSpriteRects(spriteRects);
            dataProvider.Apply();

            importer.SaveAndReimport();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[{nameof(MaterialSpriteNamingEditor)}] Finished renaming outline material sprites. Renamed {renamedCount} sprite(s).");
        }

        // --- Helpers ---
        private static bool TryGetSpriteIndex(string spriteName, out int index)
        {
            index = -1;

            if (string.IsNullOrWhiteSpace(spriteName)) return false;

            string[] parts = spriteName.Split('_');

            // Search from the end because the names contain many underscores.
            // We want the first numeric component encountered from the right that corresponds to the sprite's original index.
            for (int i = parts.Length - 1; i >= 0; i--)
            {
                if (int.TryParse(parts[i], out index)) return true;
            }

            return false;
        }
    }
}

#endif