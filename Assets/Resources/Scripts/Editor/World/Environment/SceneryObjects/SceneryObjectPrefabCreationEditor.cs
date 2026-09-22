#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace FlameOfTheLost.Editor
{
    public static class SceneryObjectPrefabCreationEditor
    {
        // --- Constants ---
        private const string SpriteSheetPath = "Assets/Resources/Art/Sprites/World/Environment/Other/CFRPG16_Outdoor_Decor.png";
        private const string PrefabOutputFolder = "Assets/Resources/Prefabs/World/Environment/SceneryObjects";

        private const string SpriteSceneryMarker = "_Scn_";
        private const string SceneryPrefix = "Scn_";
        private const string PrefabPrefix = "Prefab_";

        private const string VisualObjectName = "Visual";
        private const string SortingLayerName = "PropBack";

        // --- Menu ---
        [MenuItem("FlameOfTheLost/Tools/Assets/Prefabs/World/Environment/Create Scenery Object Prefabs")]
        private static void CreateSceneryObjectPrefabs()
        {
            if (!SceneryObjectPrefabCreationEditor.ValidateConfiguration()) return;

            SceneryObjectPrefabCreationEditor.EnsureFolderExists(SceneryObjectPrefabCreationEditor.PrefabOutputFolder);

            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(SceneryObjectPrefabCreationEditor.SpriteSheetPath);
            Sprite[] scenerySprites = assets
                .OfType<Sprite>()
                .Where(IsScenerySprite)
                .OrderBy(sprite => sprite.name, StringComparer.Ordinal)
                .ToArray();

            if (scenerySprites.Length == 0)
            {
                Debug.LogWarning($"[{nameof(SceneryObjectPrefabCreationEditor)}] No scenery sprites were found in:\n{SceneryObjectPrefabCreationEditor.SpriteSheetPath}");
                return;
            }

            int createdCount = 0;
            int repairedCount = 0;
            int skippedCount = 0;
            int failedCount = 0;

            foreach (Sprite sprite in scenerySprites)
            {
                string sceneryName = SceneryObjectPrefabCreationEditor.ExtractSceneryName(sprite.name);

                if (string.IsNullOrWhiteSpace(sceneryName))
                {
                    Debug.LogWarning($"[{nameof(SceneryObjectPrefabCreationEditor)}] Could not extract scenery name from sprite '{sprite.name}'.");
                    failedCount++;
                    continue;
                }

                string prefabName = $"{SceneryObjectPrefabCreationEditor.PrefabPrefix}{sceneryName}";
                string prefabPath = $"{SceneryObjectPrefabCreationEditor.PrefabOutputFolder}/{prefabName}.prefab";

                GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (existingPrefab != null)
                {
                    if (existingPrefab.name == prefabName)
                    {
                        Debug.Log($"[{nameof(SceneryObjectPrefabCreationEditor)}] Skipped existing prefab: {prefabName}");
                        skippedCount++;
                        continue;
                    }

                    if (SceneryObjectPrefabCreationEditor.RepairExistingPrefabRootName(prefabPath, prefabName))
                    {
                        repairedCount++;
                    }
                    else
                    {
                        failedCount++;
                    }

                    continue;
                }

                if (SceneryObjectPrefabCreationEditor.CreatePrefab(sprite, prefabName, prefabPath))
                {
                    createdCount++;
                }
                else
                {
                    failedCount++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[{nameof(SceneryObjectPrefabCreationEditor)}] Finished.\nCreated: {createdCount}\nRepaired root names: {repairedCount}\nSkipped existing: {skippedCount}\nFailed: {failedCount}");
        }

        private static bool IsScenerySprite(Sprite sprite)
        {
            if (sprite == null) return false;

            return sprite.name.Contains(SceneryObjectPrefabCreationEditor.SpriteSceneryMarker, StringComparison.Ordinal);
        }

        private static string ExtractSceneryName(string spriteName)
        {
            if (string.IsNullOrWhiteSpace(spriteName)) return null;

            int markerIndex = spriteName.IndexOf(SceneryObjectPrefabCreationEditor.SpriteSceneryMarker, StringComparison.Ordinal);

            if (markerIndex < 0) return null;

            // "_Scn_" -> start after the leading underscore, giving "Scn_..."
            int sceneryNameStart = markerIndex + 1;
            string sceneryName = spriteName.Substring(sceneryNameStart);

            return sceneryName.StartsWith(SceneryObjectPrefabCreationEditor.SceneryPrefix, StringComparison.Ordinal)
                ? sceneryName
                : null;
        }

        private static bool CreatePrefab(Sprite sprite, string prefabName, string prefabPath)
        {
            GameObject root = null;

            try
            {
                root = new GameObject(prefabName);

                GameObject visual = new GameObject(SceneryObjectPrefabCreationEditor.VisualObjectName);
                visual.transform.SetParent(root.transform, false);

                SortingGroup sortingGroup = visual.AddComponent<SortingGroup>();
                sortingGroup.sortingLayerName = SceneryObjectPrefabCreationEditor.SortingLayerName;
                sortingGroup.sortingOrder = 0;

                SpriteRenderer spriteRenderer = visual.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprite;
                spriteRenderer.sortingLayerName = SceneryObjectPrefabCreationEditor.SortingLayerName;
                spriteRenderer.sortingOrder = 0;

                GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath, out bool success);
                if (!success || savedPrefab == null)
                {
                    Debug.LogError($"[{nameof(SceneryObjectPrefabCreationEditor)}] Failed to save prefab: {prefabName}");
                    return false;
                }

                Debug.Log($"[{nameof(SceneryObjectPrefabCreationEditor)}] Created: {prefabName} <- {sprite.name}");
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"[{nameof(SceneryObjectPrefabCreationEditor)}] Failed to create '{prefabName}'.\n{exception}");
                return false;
            }
            finally
            {
                if (root != null) UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static bool RepairExistingPrefabRootName(string prefabPath, string expectedRootName)
        {
            GameObject prefabContents = null;

            try
            {
                prefabContents = PrefabUtility.LoadPrefabContents(prefabPath);

                string previousName = prefabContents.name;

                prefabContents.name = expectedRootName;

                GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath, out bool success);
                if (!success || savedPrefab == null)
                {
                    Debug.LogError($"[{nameof(SceneryObjectPrefabCreationEditor)}] Failed to save repaired prefab: {prefabPath}");
                    return false;
                }

                Debug.Log($"[{nameof(SceneryObjectPrefabCreationEditor)}] Repaired prefab root name: '{previousName}' -> '{expectedRootName}'");
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"[{nameof(SceneryObjectPrefabCreationEditor)}] Failed to repair prefab '{prefabPath}'.\n{exception}");
                return false;
            }
            finally
            {
                if (prefabContents != null)
                {
                    PrefabUtility.UnloadPrefabContents(prefabContents);
                }
            }
        }

        private static bool ValidateConfiguration()
        {
            if (AssetDatabase.LoadMainAssetAtPath(SceneryObjectPrefabCreationEditor.SpriteSheetPath) == null)
            {
                Debug.LogError($"[{nameof(SceneryObjectPrefabCreationEditor)}] Sprite sheet could not be found:\n{SceneryObjectPrefabCreationEditor.SpriteSheetPath}");
                return false;
            }

            bool sortingLayerExists = SortingLayer.layers.Any(layer => layer.name == SceneryObjectPrefabCreationEditor.SortingLayerName);

            if (!sortingLayerExists)
            {
                Debug.LogError($"[{nameof(SceneryObjectPrefabCreationEditor)}] Required sorting layer '{SceneryObjectPrefabCreationEditor.SortingLayerName}' does not exist.");
                return false;
            }

            return true;
        }

        private static void EnsureFolderExists(string folderPath)
        {
            string[] parts = folderPath.Split('/');

            if (parts.Length == 0 || parts[0] != "Assets")
            {
                throw new ArgumentException($"Invalid Unity asset folder path: {folderPath}");
            }

            string currentPath = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string nextPath = $"{currentPath}/{parts[i]}";

                if (!AssetDatabase.IsValidFolder(nextPath))
                {
                    AssetDatabase.CreateFolder(currentPath, parts[i]);
                }

                currentPath = nextPath;
            }
        }
    }
}

#endif
