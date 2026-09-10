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

            List<MaterialProfileAssetEntry> materialEntries = CollectibleObjectProfileCreationEditor.GetMaterialProfileEntries();
            if (materialEntries.Count == 0)
            {
                Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] No Material Profiles were found.");
                return;
            }

            HashSet<ItemProfile> existingItems = CollectibleObjectProfileCreationEditor.GetExistingCollectibleItems();

            HashSet<int> existingIds = CollectibleObjectProfileCreationEditor.GetExistingCollectibleIds();
            int nextCollectibleId = CollectibleObjectProfileCreationEditor.GetNextAvailableCollectibleId(existingIds, CollectibleObjectProfileCreationEditor.CollectibleIdRangeStart);
            if (nextCollectibleId < 0)
            {
                Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] No IDs remain in the {CollectibleObjectProfileCreationEditor.CollectibleIdRangeStart}-{CollectibleObjectProfileCreationEditor.CollectibleIdRangeEnd} range.");
                return;
            }

            int createdCount = 0;
            int skippedCount = 0;
            int failedCount = 0;

            AssetDatabase.StartAssetEditing();

            try
            {
                foreach (MaterialProfileAssetEntry entry in materialEntries)
                {
                    if (existingItems.Contains(entry.Profile))
                    {
                        skippedCount++;
                        continue;
                    }

                    if (nextCollectibleId < 0)
                    {
                        Debug.LogError($"[{nameof(CollectibleObjectProfileCreationEditor)}] Stopped because the Collectible Object ID range is full.");
                        failedCount++;
                        break;
                    }

                    bool created = CollectibleObjectProfileCreationEditor.TryCreateCollectibleProfile(entry, nextCollectibleId);

                    if (!created)
                    {
                        failedCount++;

                        existingIds.Add(nextCollectibleId);

                        nextCollectibleId = CollectibleObjectProfileCreationEditor.GetNextAvailableCollectibleId(existingIds, nextCollectibleId + 1);
                        continue;
                    }

                    existingItems.Add(entry.Profile);
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

            Debug.Log($"[{nameof(CollectibleObjectProfileCreationEditor)}] Finished creating Material Collectible Object Profiles. Created: {createdCount}, Skipped: {skippedCount}, Failed: {failedCount}.");
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

        // --- Existing Collectibles ---
        private static HashSet<ItemProfile> GetExistingCollectibleItems()
        {
            string[] guids = AssetDatabase.FindAssets("t:CollectibleObjectProfile", new[] { CollectibleObjectProfileCreationEditor.CollectibleObjectProfileFolderPath });

            HashSet<ItemProfile> items = new();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                CollectibleObjectProfile profile = AssetDatabase.LoadAssetAtPath<CollectibleObjectProfile>(assetPath);

                if (profile == null || profile.Item == null) continue;

                items.Add(profile.Item);
            }

            return items;
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
        private static bool TryCreateCollectibleProfile(MaterialProfileAssetEntry materialEntry, int collectibleId)
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
            CollectibleObjectProfileCreationEditor.CopyObjectReferenceProperty(materialSerializedObject, collectibleSerializedObject, "_icon");
            CollectibleObjectProfileCreationEditor.SetBoolProperty(collectibleSerializedObject, "_configureIconAtStart", CollectibleObjectProfileCreationEditor.DefaultConfigureIconAtStart);
            CollectibleObjectProfileCreationEditor.SetObjectReferenceProperty(collectibleSerializedObject, "_item", materialEntry.Profile);
            CollectibleObjectProfileCreationEditor.SetIntProperty(collectibleSerializedObject, "_interactionPromptPriority", CollectibleObjectProfileCreationEditor.DefaultInteractionPromptPriority);
            CollectibleObjectProfileCreationEditor.SetBoolProperty(collectibleSerializedObject, "_destroyAfterCollection", CollectibleObjectProfileCreationEditor.DefaultDestroyAfterCollection);

            collectibleSerializedObject.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(collectibleProfile);

            return true;
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

        private static void CopyObjectReferenceProperty(SerializedObject source, SerializedObject destination, string propertyName)
        {
            SerializedProperty sourceProperty = source.FindProperty(propertyName);
            SerializedProperty destinationProperty = destination.FindProperty(propertyName);

            if (sourceProperty == null || destinationProperty == null)
            {
                Debug.LogWarning($"[{nameof(CollectibleObjectProfileCreationEditor)}] Could not copy serialized property '{propertyName}'.");
                return;
            }

            destinationProperty.objectReferenceValue = sourceProperty.objectReferenceValue;
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
    }
}

#endif