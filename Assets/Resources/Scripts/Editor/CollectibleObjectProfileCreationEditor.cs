#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class CollectibleObjectProfileCreationEditor
    {
        // --- Constants ---
        private const string MaterialProfileFolderPath = "Assets/Resources/Data/Inventory/Items/Materials";
        private const string CollectibleObjectProfileFolderPath = "Assets/Resources/Data/World/Environment/Interactables/Collectibles";
        private const string MaterialOutlineSpriteSheetPath = "Assets/Resources/Art/Sprites/Inventory/Items/Materials/CFRPG16_Resources_Icons_Outline.png";

        private const int CollectibleIdRangeStart = 32401;
        private const int CollectibleIdRangeEnd = 32499;

        private const int DefaultInteractionPromptPriority = 100;
        private const bool DefaultDestroyAfterCollection = true;
        private const bool DefaultConfigureIconAtStart = false;

        // --- Menu ---
        [MenuItem("FlameOfTheLost/Tools/Assets/Data/Collectibles/Create Material Collectible Profiles")]
        private static void CreateMaterialCollectibleProfiles()
        {
            if (!AssetDatabase.IsValidFolder(CollectibleObjectProfileCreationEditor.MaterialProfileFolderPath))
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Material Profile folder does not exist at '{CollectibleObjectProfileCreationEditor.MaterialProfileFolderPath}'.");
                return;
            }

            if (!AssetDatabase.IsValidFolder(CollectibleObjectProfileCreationEditor.CollectibleObjectProfileFolderPath))
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Collectible Object Profile folder does not exist at '{CollectibleObjectProfileCreationEditor.CollectibleObjectProfileFolderPath}'.");
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<Texture2D>(CollectibleObjectProfileCreationEditor.MaterialOutlineSpriteSheetPath) == null)
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Material outline sprite sheet could not be found at '{CollectibleObjectProfileCreationEditor.MaterialOutlineSpriteSheetPath}'.");
                return;
            }

            List<MaterialProfileAssetEntry> materialEntries = CollectibleObjectProfileCreationEditor.GetMaterialProfileEntries();
            if (materialEntries.Count == 0)
            {
                Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] No Material Profiles were found.");
                return;
            }

            Dictionary<string, Sprite> outlineSprites = CollectibleObjectProfileCreationEditor.GetMaterialOutlineSprites();
            if (outlineSprites.Count == 0)
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] No Material sprites were found in the outline sprite sheet.");
                return;
            }

            Dictionary<ItemProfile, CollectibleObjectProfile> existingCollectibles = CollectibleObjectProfileCreationEditor.GetExistingCollectiblesByItem();

            HashSet<int> existingIds = CollectibleObjectProfileCreationEditor.GetExistingCollectibleIds();

            int nextCollectibleId = CollectibleObjectProfileCreationEditor.GetNextAvailableCollectibleId(existingIds, CollectibleObjectProfileCreationEditor.CollectibleIdRangeStart);

            int createdCount = 0;
            int updatedCount = 0;
            int unchangedCount = 0;
            int failedCount = 0;

            AssetDatabase.StartAssetEditing();

            try
            {
                foreach (MaterialProfileAssetEntry entry in materialEntries)
                {
                    string materialName = CollectibleObjectProfileCreationEditor.RemoveLeadingId(entry.AssetName);

                    if (string.IsNullOrWhiteSpace(materialName))
                    {
                        Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Could not determine the Material name from '{entry.AssetName}'.");
                        failedCount++;
                        continue;
                    }

                    if (!outlineSprites.TryGetValue(materialName, out Sprite outlineSprite))
                    {
                        Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Could not find an outlined Material sprite for '{materialName}'.");
                        failedCount++;
                        continue;
                    }

                    // Existing collectible: synchronize its sprite instead of creating another profile.
                    if (existingCollectibles.TryGetValue(entry.Profile, out CollectibleObjectProfile existingCollectible))
                    {
                        UpdateResult updateResult =
                            CollectibleObjectProfileCreationEditor.UpdateCollectibleSprite(
                                existingCollectible,
                                outlineSprite);

                        switch (updateResult)
                        {
                            case UpdateResult.Updated:
                                updatedCount++;
                                break;

                            case UpdateResult.Unchanged:
                                unchangedCount++;
                                break;

                            case UpdateResult.Failed:
                                failedCount++;
                                break;
                        }

                        continue;
                    }

                    // New collectible.
                    if (nextCollectibleId < 0)
                    {
                        Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Stopped because the Collectible Object ID range is full.");

                        failedCount++;
                        break;
                    }

                    bool created = CollectibleObjectProfileCreationEditor.TryCreateCollectibleProfile(entry, outlineSprite, nextCollectibleId);
                    if (!created)
                    {
                        failedCount++;

                        existingIds.Add(nextCollectibleId);

                        nextCollectibleId = CollectibleObjectProfileCreationEditor.GetNextAvailableCollectibleId(existingIds, nextCollectibleId + 1);
                        continue;
                    }

                    existingIds.Add(nextCollectibleId);

                    createdCount++;
                    nextCollectibleId = CollectibleObjectProfileCreationEditor.GetNextAvailableCollectibleId(existingIds, nextCollectibleId + 1);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
            }

            Debug.Log($"[{nameof(CollectibleObjectProfileCreationEditor)}] Finished processing Material Collectible Object Profiles. Created: {createdCount}, Updated: {updatedCount}, Unchanged: {unchangedCount}, Failed: {failedCount}.");
        }

        // --- Material Profiles ---
        private static List<MaterialProfileAssetEntry> GetMaterialProfileEntries()
        {
            string[] guids = AssetDatabase.FindAssets("t:MaterialProfile", new[] { CollectibleObjectProfileCreationEditor.MaterialProfileFolderPath });

            List<MaterialProfileAssetEntry> entries = new();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                MaterialProfile profile = AssetDatabase.LoadAssetAtPath<MaterialProfile>(assetPath);
                if (profile == null)
                {
                    Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Skipped '{assetPath}' because it could not be loaded as a MaterialProfile.");
                    continue;
                }

                string assetName = Path.GetFileNameWithoutExtension(assetPath);
                if (!CollectibleObjectProfileCreationEditor.TryGetLeadingId(assetName, out int materialId))
                {
                    Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Skipped '{assetPath}' because its Material ID could not be determined.");
                    continue;
                }

                entries.Add(new MaterialProfileAssetEntry(materialId, assetName, profile));
            }

            return entries.OrderBy(entry => entry.MaterialId).ToList();
        }

        // --- Material Sprites ---
        private static Dictionary<string, Sprite> GetMaterialOutlineSprites()
        {
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(CollectibleObjectProfileCreationEditor.MaterialOutlineSpriteSheetPath);

            Dictionary<string, Sprite> sprites = new();

            foreach (UnityEngine.Object asset in assets)
            {
                if (asset is not Sprite sprite) continue;
                if (!CollectibleObjectProfileCreationEditor.TryGetMaterialNameFromSpriteName(sprite.name, out string materialName)) continue;

                if (sprites.ContainsKey(materialName))
                {
                    Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Multiple outlined sprites were found for '{materialName}'. Keeping the first one.");
                    continue;
                }

                sprites.Add(materialName, sprite);
            }

            return sprites;
        }
        private static bool TryGetMaterialNameFromSpriteName(string spriteName, out string materialName)
        {
            materialName = string.Empty;

            if (string.IsNullOrWhiteSpace(spriteName)) return false;

            int materialPrefixIndex = spriteName.IndexOf("Mtr_", StringComparison.Ordinal);
            if (materialPrefixIndex < 0) return false;

            materialName = spriteName.Substring(materialPrefixIndex);

            return !string.IsNullOrWhiteSpace(materialName);
        }

        // --- Existing Collectibles ---
        private static Dictionary<ItemProfile, CollectibleObjectProfile> GetExistingCollectiblesByItem()
        {
            string[] guids = AssetDatabase.FindAssets("t:CollectibleObjectProfile", new[] { CollectibleObjectProfileCreationEditor.CollectibleObjectProfileFolderPath });

            Dictionary<ItemProfile, CollectibleObjectProfile> collectibles =
                new();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                CollectibleObjectProfile profile = AssetDatabase.LoadAssetAtPath<CollectibleObjectProfile>(assetPath);

                if (profile == null || profile.Item == null) continue;
                if (collectibles.ContainsKey(profile.Item))
                {
                    Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Multiple Collectible Object Profiles reference '{profile.Item.name}'. Keeping the first one.");
                    continue;
                }

                collectibles.Add(profile.Item, profile);
            }

            return collectibles;
        }

        private static HashSet<int> GetExistingCollectibleIds()
        {
            string[] guids = AssetDatabase.FindAssets("t:CollectibleObjectProfile", new[] { CollectibleObjectProfileCreationEditor.CollectibleObjectProfileFolderPath });

            HashSet<int> ids = new();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string assetName = Path.GetFileNameWithoutExtension(assetPath);

                if (!CollectibleObjectProfileCreationEditor.TryGetLeadingId(assetName, out int id)) continue;
                if (id < CollectibleObjectProfileCreationEditor.CollectibleIdRangeStart || id > CollectibleObjectProfileCreationEditor.CollectibleIdRangeEnd) continue;

                ids.Add(id);
            }

            return ids;
        }

        private static int GetNextAvailableCollectibleId(HashSet<int> existingIds, int startId)
        {
            int firstCandidateId = Mathf.Max(startId, CollectibleObjectProfileCreationEditor.CollectibleIdRangeStart);

            for (int id = firstCandidateId; id <= CollectibleObjectProfileCreationEditor.CollectibleIdRangeEnd; id++)
            {
                if (!existingIds.Contains(id)) return id;
            }

            return -1;
        }

        // --- Creation ---
        private static bool TryCreateCollectibleProfile(MaterialProfileAssetEntry materialEntry, Sprite outlineSprite, int collectibleId)
        {
            string materialName = CollectibleObjectProfileCreationEditor.RemoveLeadingId(materialEntry.AssetName);

            if (string.IsNullOrWhiteSpace(materialName))
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Could not determine the Material name from '{materialEntry.AssetName}'.");
                return false;
            }

            string collectibleAssetName = $"{collectibleId}_Clt_{materialName}";
            string collectibleAssetPath = $"{CollectibleObjectProfileCreationEditor.CollectibleObjectProfileFolderPath}/{collectibleAssetName}.asset";

            if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(collectibleAssetPath) != null)
            {
                Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Skipped '{collectibleAssetName}' because an asset already exists at the target path.");
                return false;
            }

            CollectibleObjectProfile collectibleProfile = ScriptableObject.CreateInstance<CollectibleObjectProfile>();
            AssetDatabase.CreateAsset(collectibleProfile, collectibleAssetPath);

            SerializedObject collectibleSerializedObject = new SerializedObject(collectibleProfile);
            SerializedObject materialSerializedObject = new SerializedObject(materialEntry.Profile);

            CollectibleObjectProfileCreationEditor.SetIntProperty(collectibleSerializedObject, "_id", collectibleId);
            CollectibleObjectProfileCreationEditor.SetStringProperty(collectibleSerializedObject, "_key", CollectibleObjectProfileCreationEditor.CreateCollectibleKey(materialName));
            CollectibleObjectProfileCreationEditor.SetStringProperty(collectibleSerializedObject, "_displayName", CollectibleObjectProfileCreationEditor.CreateCollectibleDisplayName(materialSerializedObject, materialName));
            CollectibleObjectProfileCreationEditor.SetObjectReferenceProperty(collectibleSerializedObject, "_icon", outlineSprite);
            CollectibleObjectProfileCreationEditor.SetBoolProperty(collectibleSerializedObject, "_configureIconAtStart", CollectibleObjectProfileCreationEditor.DefaultConfigureIconAtStart);
            CollectibleObjectProfileCreationEditor.SetObjectReferenceProperty(collectibleSerializedObject, "_item", materialEntry.Profile);
            CollectibleObjectProfileCreationEditor.SetIntProperty(collectibleSerializedObject, "_interactionPromptPriority", CollectibleObjectProfileCreationEditor.DefaultInteractionPromptPriority);
            CollectibleObjectProfileCreationEditor.SetBoolProperty(collectibleSerializedObject, "_destroyAfterCollection", CollectibleObjectProfileCreationEditor.DefaultDestroyAfterCollection);

            collectibleSerializedObject.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(collectibleProfile);

            return true;
        }

        // --- Updating ---
        private static UpdateResult UpdateCollectibleSprite(CollectibleObjectProfile collectibleProfile, Sprite outlineSprite)
        {
            SerializedObject serializedObject = new SerializedObject(collectibleProfile);

            SerializedProperty iconProperty = serializedObject.FindProperty("_icon");

            if (iconProperty == null)
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Could not find serialized property '_icon' on '{collectibleProfile.name}'.");
                return UpdateResult.Failed;
            }

            if (iconProperty.objectReferenceValue == outlineSprite)
            {
                return UpdateResult.Unchanged;
            }

            iconProperty.objectReferenceValue = outlineSprite;

            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(collectibleProfile);

            return UpdateResult.Updated;
        }

        // --- Naming ---
        private static string CreateCollectibleKey(string materialName)
        {
            const string MaterialPrefix = "Mtr_";

            string baseName = materialName.StartsWith(MaterialPrefix, StringComparison.Ordinal)
                ? materialName.Substring(MaterialPrefix.Length)
                : materialName;

            return $"COLLECTIBLE_{CollectibleObjectProfileCreationEditor.ConvertPascalCaseToUpperSnakeCase(baseName)}";
        }

        private static string CreateCollectibleDisplayName(SerializedObject materialSerializedObject, string materialName)
        {
            SerializedProperty displayNameProperty = materialSerializedObject.FindProperty("_displayName");

            if (displayNameProperty != null && !string.IsNullOrWhiteSpace(displayNameProperty.stringValue))
            {
                return $"Collectible - {displayNameProperty.stringValue}";
            }

            const string MaterialPrefix = "Mtr_";

            string baseName = materialName.StartsWith(MaterialPrefix, StringComparison.Ordinal)
                ? materialName.Substring(MaterialPrefix.Length)
                : materialName;

            return $"Collectible - {CollectibleObjectProfileCreationEditor.SplitPascalCase(baseName)}";
        }

        private static string RemoveLeadingId(string assetName)
        {
            int separatorIndex = assetName.IndexOf('_');

            if (separatorIndex < 0 || separatorIndex >= assetName.Length - 1) return string.Empty;

            return assetName.Substring(separatorIndex + 1);
        }

        private static bool TryGetLeadingId(string assetName, out int id)
        {
            id = 0;

            if (string.IsNullOrWhiteSpace(assetName)) return false;

            int separatorIndex = assetName.IndexOf('_');

            if (separatorIndex <= 0) return false;

            string idText = assetName.Substring(0, separatorIndex);

            return int.TryParse(idText, out id);
        }

        private static string ConvertPascalCaseToUpperSnakeCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            List<char> characters = new();

            for (int i = 0; i < value.Length; i++)
            {
                char current = value[i];

                if (i > 0 && char.IsUpper(current) && !char.IsUpper(value[i - 1]))
                {
                    characters.Add('_');
                }

                characters.Add(char.ToUpperInvariant(current));
            }

            return new string(characters.ToArray());
        }

        private static string SplitPascalCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            List<char> characters = new();

            for (int i = 0; i < value.Length; i++)
            {
                char current = value[i];

                if (i > 0 && char.IsUpper(current) && !char.IsWhiteSpace(value[i - 1]))
                {
                    characters.Add(' ');
                }

                characters.Add(current);
            }

            return new string(characters.ToArray());
        }

        // --- Serialized Properties ---
        private static void SetIntProperty(SerializedObject serializedObject, string propertyName, int value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);

            if (property == null)
            {
                Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Could not find serialized property '{propertyName}'.");
                return;
            }

            property.intValue = value;
        }

        private static void SetBoolProperty(SerializedObject serializedObject, string propertyName, bool value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);

            if (property == null)
            {
                Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Could not find serialized property '{propertyName}'.");
                return;
            }

            property.boolValue = value;
        }

        private static void SetStringProperty(SerializedObject serializedObject, string propertyName, string value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);

            if (property == null)
            {
                Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Could not find serialized property '{propertyName}'.");
                return;
            }

            property.stringValue = value;
        }

        private static void SetObjectReferenceProperty(SerializedObject serializedObject, string propertyName, UnityEngine.Object value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);

            if (property == null)
            {
                Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Could not find serialized property '{propertyName}'.");
                return;
            }

            property.objectReferenceValue = value;
        }

        // --- Data ---
        private readonly struct MaterialProfileAssetEntry
        {
            public int MaterialId { get; }
            public string AssetName { get; }
            public MaterialProfile Profile { get; }

            public MaterialProfileAssetEntry(int materialId, string assetName, MaterialProfile profile)
            {
                this.MaterialId = materialId;
                this.AssetName = assetName;
                this.Profile = profile;
            }
        }

        private enum UpdateResult
        {
            Updated,
            Unchanged,
            Failed
        }
    }
}

#endif