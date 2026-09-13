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

        private const int DefaultInteractionPromptPriority = 100;
        private const bool DefaultDestroyAfterCollection = true;
        private const bool DefaultConfigureIconAtStart = false;

        // --- Menu ---
        [MenuItem("FlameOfTheLost/Tools/Assets/Data/Collectibles/Create Material Collectible Profiles")]
        private static void CreateCollectibleProfiles()
        {
            if (!AssetDatabase.IsValidFolder(CollectibleCreationDefinitions.CollectibleObjectProfileRootFolderPath))
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Collectible Object Profile root folder does not exist at '{CollectibleCreationDefinitions.CollectibleObjectProfileRootFolderPath}'.");
                return;
            }

            Dictionary<ItemProfile, CollectibleObjectProfile> existingCollectibles = CollectibleObjectProfileCreationEditor.GetExistingCollectiblesByItem();
            HashSet<int> existingIds = CollectibleObjectProfileCreationEditor.GetExistingCollectibleIds();

            int createdCount    = 0;
            int updatedCount    = 0;
            int unchangedCount  = 0;
            int failedCount     = 0;

            foreach (CollectibleCategoryDefinition category
                     in CollectibleCreationDefinitions.Categories)
            {
                ProcessResult result = CollectibleObjectProfileCreationEditor.ProcessCategory(category, existingCollectibles, existingIds);

                createdCount    += result.CreatedCount;
                updatedCount    += result.UpdatedCount;
                unchangedCount  += result.UnchangedCount;
                failedCount     += result.FailedCount;
            }

            AssetDatabase.SaveAssets();

            Debug.Log($"[{nameof(CollectibleObjectProfileCreationEditor)}] Finished processing Collectible Object Profiles. Created: {createdCount}, Updated: {updatedCount}, Unchanged: {unchangedCount}, Failed: {failedCount}.");
        }

        // --- Category Processing ---
        private static ProcessResult ProcessCategory(CollectibleCategoryDefinition category, Dictionary<ItemProfile, CollectibleObjectProfile> existingCollectibles, HashSet<int> existingIds)
        {
            ProcessResult result = new();

            if (!CollectibleObjectProfileCreationEditor.ValidateCategory(category))
            {
                result.FailedCount++;
                return result;
            }

            List<ItemProfileAssetEntry> itemEntries = CollectibleObjectProfileCreationEditor.GetItemProfileEntries(category);
            if (itemEntries.Count == 0)
            {
                Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] No {category.DisplayName} Item Profiles were found.");
                return result;
            }

            Dictionary<string, Sprite> outlineSprites =
                CollectibleObjectProfileCreationEditor.GetOutlineSprites(category);

            if (outlineSprites.Count == 0)
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] No outlined sprites were found for category '{category.DisplayName}'.");
                result.FailedCount++;
                return result;
            }

            foreach (ItemProfileAssetEntry entry in itemEntries)
            {
                string itemName = CollectibleObjectProfileCreationEditor.RemoveLeadingId(entry.AssetName);
                if (string.IsNullOrWhiteSpace(itemName))
                {
                    Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Could not determine the Item name from '{entry.AssetName}'.");
                    result.FailedCount++;
                    continue;
                }

                if (!outlineSprites.TryGetValue(itemName, out Sprite outlineSprite))
                {
                    Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Could not find an outlined sprite for '{itemName}' in category '{category.DisplayName}'.");
                    result.FailedCount++;
                    continue;
                }

                // Existing Collectible Profile: synchronize data rather than creating another one.
                if (existingCollectibles.TryGetValue(entry.Profile, out CollectibleObjectProfile existingCollectible))
                {
                    UpdateResult updateResult = CollectibleObjectProfileCreationEditor.UpdateExistingCollectible(existingCollectible, outlineSprite);
                    switch (updateResult)
                    {
                        case UpdateResult.Updated:
                            result.UpdatedCount++;
                            break;

                        case UpdateResult.Unchanged:
                            result.UnchangedCount++;
                            break;

                        case UpdateResult.Failed:
                            result.FailedCount++;
                            break;
                    }

                    continue;
                }

                int nextCollectibleId = CollectibleObjectProfileCreationEditor.GetNextAvailableCollectibleId(existingIds, CollectibleCreationDefinitions.CollectibleIdRangeStart);
                if (nextCollectibleId < 0)
                {
                    Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Stopped because the Collectible Object ID range {CollectibleCreationDefinitions.CollectibleIdRangeStart}-{CollectibleCreationDefinitions.CollectibleIdRangeEnd} is full.");
                    result.FailedCount++;
                    break;
                }

                bool created = CollectibleObjectProfileCreationEditor.TryCreateCollectibleProfile(category, entry, outlineSprite, nextCollectibleId, out CollectibleObjectProfile createdProfile);
                if (!created)
                {
                    result.FailedCount++;
                    continue;
                }

                existingIds.Add(nextCollectibleId);
                existingCollectibles.Add(entry.Profile, createdProfile);

                result.CreatedCount++;
            }

            Debug.Log($"[{nameof(CollectibleObjectProfileCreationEditor)}] Finished category '{category.DisplayName}'. Created: {result.CreatedCount}, Updated: {result.UpdatedCount}, Unchanged: {result.UnchangedCount}, Failed: {result.FailedCount}.");
            return result;
        }

        // --- Validation ---
        private static bool ValidateCategory(CollectibleCategoryDefinition category)
        {
            if (category.ItemProfileType == null)
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Category '{category.DisplayName}' has no Item Profile type.");
                return false;
            }

            if (!typeof(ItemProfile).IsAssignableFrom(category.ItemProfileType))
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Profile type '{category.ItemProfileType.Name}' in category '{category.DisplayName}' does not inherit from {nameof(ItemProfile)}.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(category.ItemPrefix))
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Category '{category.DisplayName}' has no Item prefix.");
                return false;
            }

            if (!AssetDatabase.IsValidFolder(category.ItemProfileFolderPath))
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] {category.DisplayName} Item Profile folder does not exist at '{category.ItemProfileFolderPath}'.");
                return false;
            }

            if (!AssetDatabase.IsValidFolder(category.CollectibleProfileFolderPath))
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] {category.DisplayName} Collectible Object Profile folder does not exist at '{category.CollectibleProfileFolderPath}'.");
                return false;
            }

            if (AssetDatabase.LoadAssetAtPath<Texture2D>(category.OutlineSpriteSheetPath) == null)
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] {category.DisplayName} outline sprite sheet could not be found at '{category.OutlineSpriteSheetPath}'.");
                return false;
            }

            return true;
        }


        // --- Item Profiles ---
        private static List<ItemProfileAssetEntry> GetItemProfileEntries(CollectibleCategoryDefinition category)
        {
            string filter = $"t:{category.ItemProfileType.Name}";

            string[] guids = AssetDatabase.FindAssets(filter, new[] { category.ItemProfileFolderPath });

            List<ItemProfileAssetEntry> entries = new();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                ItemProfile profile = AssetDatabase.LoadAssetAtPath<ItemProfile>(assetPath);

                if (profile == null)
                {
                    Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Skipped '{assetPath}' because it could not be loaded as an {nameof(ItemProfile)}.");
                    continue;
                }
                if (!category.ItemProfileType.IsInstanceOfType(profile))
                {
                    Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Skipped '{assetPath}' because it is not of expected type '{category.ItemProfileType.Name}'.");
                    continue;
                }

                string assetName = Path.GetFileNameWithoutExtension(assetPath);
                if (!CollectibleObjectProfileCreationEditor.TryGetLeadingId(assetName, out int itemId))
                {
                    Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Skipped '{assetPath}' because its Item ID could not be determined.");
                    continue;
                }

                entries.Add(new ItemProfileAssetEntry(itemId, assetName, profile));
            }

            return entries.OrderBy(entry => entry.ItemId).ToList();
        }

        // --- Outline Sprites ---
        private static Dictionary<string, Sprite> GetOutlineSprites(CollectibleCategoryDefinition category)
        {
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(category.OutlineSpriteSheetPath);

            Dictionary<string, Sprite> sprites = new();

            foreach (UnityEngine.Object asset in assets)
            {
                if (asset is not Sprite sprite) continue;
                if (!CollectibleObjectProfileCreationEditor.TryGetItemNameFromSpriteName(sprite.name, category.ItemPrefix, out string itemName)) continue;

                if (sprites.ContainsKey(itemName))
                {
                    Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Multiple outlined sprites were found for '{itemName}'. Keeping the first one.");
                    continue;
                }

                sprites.Add(itemName, sprite);
            }

            return sprites;
        }
        private static bool TryGetItemNameFromSpriteName(string spriteName, string itemPrefix, out string itemName)
        {
            itemName = string.Empty;

            if (string.IsNullOrWhiteSpace(spriteName)) return false;

            int itemPrefixIndex = spriteName.IndexOf(itemPrefix, StringComparison.Ordinal);
            if (itemPrefixIndex < 0) return false;

            itemName = spriteName.Substring(itemPrefixIndex);

            return !string.IsNullOrWhiteSpace(itemName);
        }

        // --- Existing Collectibles ---
        private static Dictionary<ItemProfile, CollectibleObjectProfile> GetExistingCollectiblesByItem()
        {
            string[] guids = AssetDatabase.FindAssets("t:CollectibleObjectProfile", new[] { CollectibleCreationDefinitions.CollectibleObjectProfileRootFolderPath });

            Dictionary<ItemProfile, CollectibleObjectProfile> collectibles = new();

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
            string[] guids = AssetDatabase.FindAssets("t:CollectibleObjectProfile", new[] { CollectibleCreationDefinitions.CollectibleObjectProfileRootFolderPath });

            HashSet<int> ids = new();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string assetName = Path.GetFileNameWithoutExtension(assetPath);

                if (!CollectibleObjectProfileCreationEditor.TryGetLeadingId(assetName, out int id)) continue;
                if (id < CollectibleCreationDefinitions.CollectibleIdRangeStart || id > CollectibleCreationDefinitions.CollectibleIdRangeEnd) continue;

                ids.Add(id);
            }

            return ids;
        }
        private static int GetNextAvailableCollectibleId(HashSet<int> existingIds, int startId)
        {
            int firstCandidateId = Mathf.Max(startId, CollectibleCreationDefinitions.CollectibleIdRangeStart);

            for (int id = firstCandidateId; id <= CollectibleCreationDefinitions.CollectibleIdRangeEnd; id++)
            {
                if (!existingIds.Contains(id))
                {
                    return id;
                }
            }

            return -1;
        }

        // --- Creation ---
        private static bool TryCreateCollectibleProfile(CollectibleCategoryDefinition category, ItemProfileAssetEntry itemEntry, Sprite outlineSprite, int collectibleId, out CollectibleObjectProfile createdProfile)
        {
            createdProfile = null;

            string itemName = CollectibleObjectProfileCreationEditor.RemoveLeadingId( itemEntry.AssetName);
            if (string.IsNullOrWhiteSpace(itemName))
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Could not determine the Item name from '{itemEntry.AssetName}'.");
                return false;
            }

            string collectibleAssetName = $"{collectibleId}_{CollectibleCreationDefinitions.CollectiblePrefix}{itemName}";
            string collectibleAssetPath = $"{category.CollectibleProfileFolderPath}/{collectibleAssetName}.asset";

            if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(collectibleAssetPath) != null)
            {
                Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Skipped '{collectibleAssetName}' because an asset already exists at the target path.");
                return false;
            }

            CollectibleObjectProfile collectibleProfile = ScriptableObject.CreateInstance<CollectibleObjectProfile>();
            AssetDatabase.CreateAsset(collectibleProfile, collectibleAssetPath);
            SerializedObject collectibleSerializedObject = new SerializedObject(collectibleProfile);
            SerializedObject itemSerializedObject = new SerializedObject(itemEntry.Profile);

            CollectibleObjectProfileCreationEditor.SetIntProperty(collectibleSerializedObject, "_id", collectibleId);
            CollectibleObjectProfileCreationEditor.SetStringProperty(collectibleSerializedObject, "_key", CollectibleObjectProfileCreationEditor.CreateCollectibleKey(itemName, category.ItemPrefix));
            CollectibleObjectProfileCreationEditor.SetStringProperty(collectibleSerializedObject, "_displayName", CollectibleObjectProfileCreationEditor.CreateCollectibleDisplayName(itemSerializedObject, itemName, category.ItemPrefix));
            CollectibleObjectProfileCreationEditor.SetObjectReferenceProperty(collectibleSerializedObject, "_icon", outlineSprite);
            CollectibleObjectProfileCreationEditor.SetBoolProperty(collectibleSerializedObject, "_configureIconAtStart", CollectibleObjectProfileCreationEditor.DefaultConfigureIconAtStart);
            CollectibleObjectProfileCreationEditor.SetObjectReferenceProperty(collectibleSerializedObject, "_item", itemEntry.Profile);
            CollectibleObjectProfileCreationEditor.SetIntProperty(collectibleSerializedObject, "_interactionPromptPriority", CollectibleObjectProfileCreationEditor.DefaultInteractionPromptPriority);
            CollectibleObjectProfileCreationEditor.SetBoolProperty(collectibleSerializedObject, "_destroyAfterCollection", CollectibleObjectProfileCreationEditor.DefaultDestroyAfterCollection);

            collectibleSerializedObject.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(collectibleProfile);

            createdProfile = collectibleProfile;

            return true;
        }

        // --- Updating ---
        private static UpdateResult UpdateExistingCollectible(CollectibleObjectProfile collectibleProfile, Sprite outlineSprite)
        {
            if (collectibleProfile == null) return UpdateResult.Failed;

            SerializedObject serializedObject = new SerializedObject(collectibleProfile);
            SerializedProperty iconProperty = serializedObject.FindProperty("_icon");

            if (iconProperty == null)
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Could not find serialized property '_icon' on '{collectibleProfile.name}'.");
                return UpdateResult.Failed;
            }
            if (iconProperty.objectReferenceValue == outlineSprite) return UpdateResult.Unchanged;

            iconProperty.objectReferenceValue = outlineSprite;

            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(collectibleProfile);

            return UpdateResult.Updated;
        }

        // --- Naming ---
        private static string CreateCollectibleKey(string itemName, string itemPrefix)
        {
            string baseName = CollectibleObjectProfileCreationEditor.RemovePrefix(itemName, itemPrefix);

            return $"COLLECTIBLE_{CollectibleObjectProfileCreationEditor.ConvertPascalCaseToUpperSnakeCase(baseName)}";
        }

        private static string CreateCollectibleDisplayName(SerializedObject itemSerializedObject, string itemName, string itemPrefix)
        {
            SerializedProperty displayNameProperty = itemSerializedObject.FindProperty("_displayName");

            if (displayNameProperty != null && !string.IsNullOrWhiteSpace(displayNameProperty.stringValue))
            {
                return $"Collectible - {displayNameProperty.stringValue}";
            }

            string baseName = CollectibleObjectProfileCreationEditor.RemovePrefix(itemName, itemPrefix);

            return $"Collectible - {CollectibleObjectProfileCreationEditor.SplitPascalCase(baseName)}";
        }

        private static string RemovePrefix(string value, string prefix)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            if (string.IsNullOrWhiteSpace(prefix)) return value;

            return value.StartsWith(prefix, StringComparison.Ordinal)
                ? value.Substring(prefix.Length)
                : value;
        }

        private static string RemoveLeadingId(string assetName)
        {
            if (string.IsNullOrWhiteSpace(assetName)) return string.Empty;

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

        private static string SplitPascalCase( string value)
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
                CollectibleObjectProfileCreationEditor.LogMissingProperty(propertyName);
                return;
            }

            property.intValue = value;
        }
        private static void SetBoolProperty(SerializedObject serializedObject, string propertyName, bool value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                CollectibleObjectProfileCreationEditor.LogMissingProperty(propertyName);
                return;
            }

            property.boolValue = value;
        }
        private static void SetStringProperty(SerializedObject serializedObject, string propertyName, string value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                CollectibleObjectProfileCreationEditor.LogMissingProperty(propertyName);
                return;
            }

            property.stringValue = value;
        }
        private static void SetObjectReferenceProperty(SerializedObject serializedObject, string propertyName, UnityEngine.Object value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                CollectibleObjectProfileCreationEditor.LogMissingProperty(propertyName);
                return;
            }

            property.objectReferenceValue = value;
        }
        private static void LogMissingProperty(string propertyName)
        {
            Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Could not find serialized property '{propertyName}'.");
        }


        // --- Data ---
        private readonly struct ItemProfileAssetEntry
        {
            public int ItemId { get; }
            public string AssetName { get; }
            public ItemProfile Profile { get; }

            public ItemProfileAssetEntry(
                int itemId,
                string assetName,
                ItemProfile profile)
            {
                this.ItemId = itemId;
                this.AssetName = assetName;
                this.Profile = profile;
            }
        }

        private struct ProcessResult
        {
            public int CreatedCount;
            public int UpdatedCount;
            public int UnchangedCount;
            public int FailedCount;
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