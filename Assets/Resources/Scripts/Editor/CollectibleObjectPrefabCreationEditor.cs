#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace FlameOfTheLost
{
    public static class CollectibleObjectPrefabCreationEditor
    {
        // --- Constants ---
        private const string CollectibleObjectProfileFolderPath = "Assets/Resources/Data/World/Environment/Interactables/Collectibles";
        private const string CollectibleObjectPrefabFolderPath = "Assets/Resources/Prefabs/World/Environment/Interactables/Collectibles";

        private const string InteractableLayerName = "Interactable";
        private const string InteractableTagName = "Interactable";
        private const string CollectibleSortingLayerName = "PropBack";

        private const float InteractionTriggerRadius = 1.0f;

        // --- Menu ---
        [MenuItem("FlameOfTheLost/Tools/Assets/Prefabs/Collectibles/Create Material Collectible Prefabs")]
        private static void CreateMaterialCollectiblePrefabs()
        {
            if (!AssetDatabase.IsValidFolder(CollectibleObjectPrefabCreationEditor.CollectibleObjectProfileFolderPath))
            {
                Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Collectible Object Profile folder does not exist at '{CollectibleObjectPrefabCreationEditor.CollectibleObjectProfileFolderPath}'.");
                return;
            }

            if (!AssetDatabase.IsValidFolder(CollectibleObjectPrefabCreationEditor.CollectibleObjectPrefabFolderPath))
            {
                Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Collectible Object Prefab folder does not exist at '{CollectibleObjectPrefabCreationEditor.CollectibleObjectPrefabFolderPath}'.");
                return;
            }

            int interactableLayer = LayerMask.NameToLayer(CollectibleObjectPrefabCreationEditor.InteractableLayerName);
            if (interactableLayer < 0)
            {
                Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Layer '{CollectibleObjectPrefabCreationEditor.InteractableLayerName}' does not exist.");
                return;
            }

            if (!CollectibleObjectPrefabCreationEditor.DoesSortingLayerExist(CollectibleObjectPrefabCreationEditor.CollectibleSortingLayerName))
            {
                Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Sorting Layer '{CollectibleObjectPrefabCreationEditor.CollectibleSortingLayerName}' does not exist.");
                return;
            }

            List<CollectibleObjectProfileAssetEntry> profileEntries = CollectibleObjectPrefabCreationEditor.GetCollectibleObjectProfileEntries();
            if (profileEntries.Count == 0)
            {
                Debug.LogWarning($"[{nameof(CollectibleObjectPrefabCreationEditor)}] No Collectible Object Profiles were found.");
                return;
            }

            int createdCount = 0;
            int skippedCount = 0;
            int failedCount = 0;

            foreach (CollectibleObjectProfileAssetEntry entry in profileEntries)
            {
                string prefabName = CollectibleObjectPrefabCreationEditor.CreatePrefabName(entry.AssetName);
                if (string.IsNullOrWhiteSpace(prefabName))
                {
                    Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Could not determine a prefab name from '{entry.AssetName}'.");

                    failedCount++;
                    continue;
                }

                string prefabPath = $"{CollectibleObjectPrefabCreationEditor.CollectibleObjectPrefabFolderPath}/{prefabName}.prefab";
                if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
                {
                    skippedCount++;
                    continue;
                }

                bool created = CollectibleObjectPrefabCreationEditor.TryCreatePrefab(entry.Profile, prefabName, prefabPath, interactableLayer);
                if (created)
                {
                    createdCount++;
                }
                else
                {
                    failedCount++;
                }
            }

            AssetDatabase.SaveAssets();

            Debug.Log($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Finished creating Material Collectible Object Prefabs. Created: {createdCount}, Skipped: {skippedCount}, Failed: {failedCount}.");
        }

        // --- Profiles ---
        private static List<CollectibleObjectProfileAssetEntry> GetCollectibleObjectProfileEntries()
        {
            string[] guids = AssetDatabase.FindAssets("t:CollectibleObjectProfile", new[] { CollectibleObjectPrefabCreationEditor.CollectibleObjectProfileFolderPath });

            List<CollectibleObjectProfileAssetEntry> entries = new();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                CollectibleObjectProfile profile = AssetDatabase.LoadAssetAtPath<CollectibleObjectProfile>(assetPath);
                if (profile == null)
                {
                    Debug.LogWarning($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Skipped '{assetPath}' because it could not be loaded as a {nameof(CollectibleObjectProfile)}.");
                    continue;
                }

                string assetName = Path.GetFileNameWithoutExtension(assetPath);
                if (!CollectibleObjectPrefabCreationEditor.TryGetLeadingId(assetName, out int id))
                {
                    Debug.LogWarning($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Skipped '{assetPath}' because its Collectible ID could not be determined.");
                    continue;
                }

                entries.Add(new CollectibleObjectProfileAssetEntry(id, assetName, profile));
            }

            return entries.OrderBy(entry => entry.Id).ToList();
        }

        // --- Creation ---
        private static bool TryCreatePrefab(CollectibleObjectProfile profile, string prefabName, string prefabPath, int interactableLayer)
        {
            if (profile == null) return false;
            if (profile.Icon == null)
            {
                Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Collectible Object Profile '{profile.name}' has no icon assigned.");
                return false;
            }

            GameObject root = null;
            try
            {
                root = new GameObject(prefabName);
                CollectibleObjectPrefabCreationEditor.ConfigureTransform(root.transform);
                root.layer = interactableLayer;

                try
                {
                    root.tag = CollectibleObjectPrefabCreationEditor.InteractableTagName;
                }
                catch (UnityException)
                {
                    Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Tag '{CollectibleObjectPrefabCreationEditor.InteractableTagName}' does not exist.");
                    return false;
                }

                // --- Root Controller ---
                CollectibleObjectController controller = root.AddComponent<CollectibleObjectController>();

                // --- Visual ---
                GameObject visualObject = new GameObject("Visual");
                visualObject.transform.SetParent(root.transform, false);
                CollectibleObjectPrefabCreationEditor.ConfigureTransform(visualObject.transform);
                visualObject.layer = interactableLayer;

                SortingGroup sortingGroup = visualObject.AddComponent<SortingGroup>();
                sortingGroup.sortingLayerName = CollectibleObjectPrefabCreationEditor.CollectibleSortingLayerName;
                sortingGroup.sortingOrder = 0;

                SpriteRenderer spriteRenderer = visualObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = profile.Icon;
                spriteRenderer.sortingLayerName = CollectibleObjectPrefabCreationEditor.CollectibleSortingLayerName;
                spriteRenderer.sortingOrder = 0;

                // --- Interaction Trigger Area ---
                GameObject interactionTriggerArea = new GameObject("InteractionTriggerArea");
                interactionTriggerArea.transform.SetParent(root.transform, false);
                CollectibleObjectPrefabCreationEditor.ConfigureTransform(interactionTriggerArea.transform);
                interactionTriggerArea.layer = interactableLayer;

                CircleCollider2D interactionCollider = interactionTriggerArea.AddComponent<CircleCollider2D>();
                interactionCollider.offset = Vector2.zero;
                interactionCollider.radius = CollectibleObjectPrefabCreationEditor.InteractionTriggerRadius;
                interactionCollider.isTrigger = true;

                // --- Controller References ---
                if (!CollectibleObjectPrefabCreationEditor.ConfigureController(controller, profile, visualObject.transform, spriteRenderer)) return false;

                GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);

                if (savedPrefab == null)
                {
                    Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Failed to save prefab '{prefabName}'.");
                    return false;
                }

                return true;
            }
            finally
            {
                if (root != null)
                {
                    UnityEngine.Object.DestroyImmediate(root);
                }
            }
        }

        // --- Controller Configuration ---
        private static bool ConfigureController(CollectibleObjectController controller, CollectibleObjectProfile profile, Transform visualTransform, SpriteRenderer visualSpriteRenderer)
        {
            SerializedObject serializedController = new SerializedObject(controller);
            SerializedProperty profileProperty              = serializedController.FindProperty("_profile");
            SerializedProperty visualTransformProperty      = serializedController.FindProperty("_visualTransform");
            SerializedProperty visualSpriteRendererProperty = serializedController.FindProperty("_visualSpriteRenderer");

            if (profileProperty == null)
            {
                Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Could not find serialized property '_profile' on {nameof(CollectibleObjectController)}.");
                return false;
            }
            if (visualTransformProperty == null)
            {
                Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Could not find serialized property '_visualTransform' on {nameof(CollectibleObjectController)}.");
                return false;
            }
            if (visualSpriteRendererProperty == null)
            {
                Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Could not find serialized property '_visualSpriteRenderer' on {nameof(CollectibleObjectController)}.");
                return false;
            }

            profileProperty.objectReferenceValue                = profile;
            visualTransformProperty.objectReferenceValue        = visualTransform;
            visualSpriteRendererProperty.objectReferenceValue   = visualSpriteRenderer;

            serializedController.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(controller);

            return true;
        }

        // --- Naming ---
        private static string CreatePrefabName(string collectibleProfileAssetName)
        {
            int firstSeparatorIndex = collectibleProfileAssetName.IndexOf('_');

            if (firstSeparatorIndex < 0 || firstSeparatorIndex >= collectibleProfileAssetName.Length - 1) return string.Empty;

            string collectibleName = collectibleProfileAssetName.Substring(firstSeparatorIndex + 1);

            const string CollectibleMaterialPrefix = "Clt_Mtr_";

            if (collectibleName.StartsWith(CollectibleMaterialPrefix, System.StringComparison.Ordinal))
            {
                collectibleName = collectibleName.Substring(CollectibleMaterialPrefix.Length);
            }
            else
            {
                const string CollectiblePrefix = "Clt_";

                if (collectibleName.StartsWith(CollectiblePrefix, System.StringComparison.Ordinal))
                {
                    collectibleName = collectibleName.Substring(CollectiblePrefix.Length);
                }
            }

            if (string.IsNullOrWhiteSpace(collectibleName)) return string.Empty;

            return $"Clt_{collectibleName}";
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

        // --- Validation ---
        private static bool DoesSortingLayerExist(string sortingLayerName)
        {
            return SortingLayer.layers.Any(sortingLayer => sortingLayer.name == sortingLayerName);
        }

        // --- Helpers ---
        private static void ConfigureTransform(Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale    = Vector3.one;
        }

        // --- Data ---
        private readonly struct CollectibleObjectProfileAssetEntry
        {
            public int Id { get; }
            public string AssetName { get; }
            public CollectibleObjectProfile Profile { get; }

            public CollectibleObjectProfileAssetEntry(int id, string assetName, CollectibleObjectProfile profile)
            {
                this.Id = id;
                this.AssetName = assetName;
                this.Profile = profile;
            }
        }
    }
}

#endif