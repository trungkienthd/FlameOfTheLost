#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace FlameOfTheLost.Editor
{
    public static class CollectibleObjectPrefabCreationEditor
    {
        // --- Constants ---
        private const string InteractableLayerName = "Interactable";
        private const string InteractableTagName = "Interactable";
        private const string CollectibleSortingLayerName = "PropBack";

        private const string VisualObjectName = "Visual";
        private const string InteractionTriggerAreaObjectName = "InteractionTriggerArea";

        private const float InteractionTriggerRadius = 1.0f;

        // --- Menu ---
        [MenuItem("FlameOfTheLost/Tools/Assets/Prefabs/Collectibles/Create or Update Collectible Prefabs")]
        private static void CreateOrUpdateCollectiblePrefabs()
        {
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

            int createdCount    = 0;
            int updatedCount    = 0;
            int unchangedCount  = 0;
            int failedCount     = 0;

            foreach (CollectibleCategoryDefinition category in CollectibleCreationDefinitions.Categories)
            {
                ProcessResult result = CollectibleObjectPrefabCreationEditor.ProcessCategory(category, interactableLayer);

                createdCount    += result.CreatedCount;
                updatedCount    += result.UpdatedCount;
                unchangedCount  += result.UnchangedCount;
                failedCount     += result.FailedCount;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Finished creating Collectible Object Prefabs. Created: {createdCount}, Updated: {updatedCount}, Unchanged: {unchangedCount}, Failed: {failedCount}.");
        }

        // --- Helpers ---
        //  1. Category Processing
        private static ProcessResult ProcessCategory(CollectibleCategoryDefinition category, int interactableLayer)
        {
            ProcessResult result = new();

            if (!CollectibleObjectPrefabCreationEditor.ValidateCategory(category))
            {
                result.FailedCount++;
                return result;
            }

            List<CollectibleObjectProfileAssetEntry> profileEntries = CollectibleObjectPrefabCreationEditor.GetCollectibleObjectProfileEntries(category.CollectibleProfileFolderPath);

            if (profileEntries.Count == 0)
            {
                Debug.LogWarning($"[{nameof(CollectibleObjectPrefabCreationEditor)}] No {category.DisplayName} Collectible Object Profiles were found.");
                return result;
            }

            Dictionary<CollectibleObjectProfile, List<ExistingPrefabEntry>> existingPrefabsByProfile = CollectibleObjectPrefabCreationEditor.GetExistingPrefabsByProfile(category.CollectiblePrefabFolderPath);

            foreach (CollectibleObjectProfileAssetEntry entry in profileEntries)
            {
                string expectedPrefabName = CollectibleObjectPrefabCreationEditor.CreatePrefabName(entry.AssetName);

                if (string.IsNullOrWhiteSpace(expectedPrefabName))
                {
                    Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Could not determine a prefab name from '{entry.AssetName}'.");
                    result.FailedCount++;
                    continue;
                }

                string expectedPrefabPath = $"{category.CollectiblePrefabFolderPath}/{expectedPrefabName}.prefab";

                if (existingPrefabsByProfile.TryGetValue(entry.Profile, out List<ExistingPrefabEntry> existingPrefabs))
                {
                    ExistingPrefabEntry existingPrefab = CollectibleObjectPrefabCreationEditor.SelectExistingPrefab(entry.Profile, existingPrefabs, expectedPrefabPath);

                    UpdateResult updateResult = CollectibleObjectPrefabCreationEditor.TryUpdateExistingPrefab(entry.Profile, existingPrefab, expectedPrefabName, expectedPrefabPath, interactableLayer);

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

                // No prefab referencing this profile exists.
                if (AssetDatabase.LoadAssetAtPath<GameObject>(expectedPrefabPath) != null)
                {
                    Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Cannot create prefab for '{entry.Profile.name}' because an unrelated prefab already exists at '{expectedPrefabPath}'.");
                    result.FailedCount++;
                    continue;
                }

                bool created = CollectibleObjectPrefabCreationEditor.TryCreatePrefab(entry.Profile, expectedPrefabName, expectedPrefabPath, interactableLayer);
                if (created)
                {
                    result.CreatedCount++;
                }
                else
                {
                    result.FailedCount++;
                }
            }

            Debug.Log($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Finished category '{category.DisplayName}'. Created: {result.CreatedCount}, Updated: {result.UpdatedCount}, Unchanged: {result.UnchangedCount}, Failed: {result.FailedCount}.");

            return result;
        }

        //  2. Validation
        private static bool ValidateCategory(CollectibleCategoryDefinition category)
        {
            if (!AssetDatabase.IsValidFolder(category.CollectibleProfileFolderPath))
            {
                Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] {category.DisplayName} Collectible Object Profile folder does not exist at '{category.CollectibleProfileFolderPath}'.");
                return false;
            }

            if (!AssetDatabase.IsValidFolder(category.CollectiblePrefabFolderPath))
            {
                Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] {category.DisplayName} Collectible Object Prefab folder does not exist at '{category.CollectiblePrefabFolderPath}'.");
                return false;
            }

            return true;
        }

        //  3. Profiles
        private static List<CollectibleObjectProfileAssetEntry> GetCollectibleObjectProfileEntries(string profileFolderPath)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(CollectibleObjectProfile)}", new[] { profileFolderPath });

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

        // 4. Existing Prefabs
        private static Dictionary<CollectibleObjectProfile, List<ExistingPrefabEntry>> GetExistingPrefabsByProfile(string prefabFolderPath)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolderPath });

            Dictionary<CollectibleObjectProfile, List<ExistingPrefabEntry>> result = new();

            foreach (string guid in guids)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab == null) continue;

                CollectibleObjectController controller = prefab.GetComponent<CollectibleObjectController>();
                if (controller == null) continue;

                SerializedObject serializedController = new SerializedObject(controller);
                SerializedProperty profileProperty = serializedController.FindProperty("_profile");
                if (profileProperty == null)
                {
                    Debug.LogWarning($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Prefab '{prefabPath}' has a {nameof(CollectibleObjectController)} but its serialized '_profile' property could not be found.");
                    continue;
                }

                CollectibleObjectProfile profile = profileProperty.objectReferenceValue as CollectibleObjectProfile;
                if (profile == null) continue;

                if (!result.TryGetValue(profile, out List<ExistingPrefabEntry> entries))
                {
                    entries = new List<ExistingPrefabEntry>();

                    result.Add(profile, entries);
                }

                entries.Add(new ExistingPrefabEntry(prefabPath));
            }

            return result;
        }

        // 5. Creation
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
                root = CollectibleObjectPrefabCreationEditor.CreateConfiguredRoot(profile, prefabName, interactableLayer);

                if (root == null) return false;

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

        // 6. Updating
        private static ExistingPrefabEntry SelectExistingPrefab(CollectibleObjectProfile profile, List<ExistingPrefabEntry> existingPrefabs, string expectedPrefabPath)
        {
            ExistingPrefabEntry? expectedPrefab = null;

            foreach (ExistingPrefabEntry prefab in existingPrefabs)
            {
                if (string.Equals(prefab.AssetPath, expectedPrefabPath, StringComparison.Ordinal))
                {
                    expectedPrefab = prefab;
                    break;
                }
            }

            if (existingPrefabs.Count > 1)
            {
                foreach (ExistingPrefabEntry prefab in existingPrefabs)
                {
                    if (string.Equals(prefab.AssetPath, expectedPrefabPath, StringComparison.Ordinal)) continue;

                    Debug.LogWarning($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Legacy or duplicate prefab '{prefab.AssetPath}' also references Collectible Object Profile '{profile.name}'. The correctly named prefab will be used. The duplicate was not deleted automatically.");
                }
            }

            if (expectedPrefab.HasValue)
            {
                return expectedPrefab.Value;
            }

            return existingPrefabs[0];
        }
        private static UpdateResult TryUpdateExistingPrefab(CollectibleObjectProfile profile, ExistingPrefabEntry existingPrefab, string expectedPrefabName, string expectedPrefabPath, int interactableLayer)
        {
            if (profile == null) return UpdateResult.Failed;

            if (profile.Icon == null)
            {
                Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Collectible Object Profile '{profile.name}' has no icon assigned.");
                return UpdateResult.Failed;
            }

            string workingPrefabPath = existingPrefab.AssetPath;

            bool changed = false;

            // Rename / move the prefab when its name does not match the Collectible Profile naming convention.
            if (!string.Equals(workingPrefabPath, expectedPrefabPath, StringComparison.Ordinal))
            {
                UnityEngine.Object targetAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(expectedPrefabPath);

                if (targetAsset != null)
                {
                    Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Could not rename prefab for '{profile.name}' from '{workingPrefabPath}' to '{expectedPrefabPath}' because an asset already exists at the target path.");
                    return UpdateResult.Failed;
                }

                string moveError = AssetDatabase.MoveAsset(workingPrefabPath, expectedPrefabPath);

                if (!string.IsNullOrWhiteSpace(moveError))
                {
                    Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Failed to rename/move prefab from '{workingPrefabPath}' to '{expectedPrefabPath}'. Reason: {moveError}");
                    return UpdateResult.Failed;
                }

                Debug.Log($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Renamed Collectible prefab '{Path.GetFileNameWithoutExtension(workingPrefabPath)}' to '{expectedPrefabName}'.");

                workingPrefabPath = expectedPrefabPath;
                changed = true;
            }

            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(workingPrefabPath);

            if (prefabRoot == null)
            {
                Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Could not load prefab contents from '{workingPrefabPath}'.");
                return UpdateResult.Failed;
            }

            try
            {
                bool prefabChanged = CollectibleObjectPrefabCreationEditor.SynchronizePrefab(prefabRoot, profile, expectedPrefabName, interactableLayer);

                changed |= prefabChanged;
                if (!changed) return UpdateResult.Unchanged;

                GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(prefabRoot, workingPrefabPath);
                if (savedPrefab == null)
                {
                    Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Failed to save updated prefab '{expectedPrefabName}'.");
                    return UpdateResult.Failed;
                }

                return UpdateResult.Updated;
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
        }

        // 7. Root Creation
        private static GameObject CreateConfiguredRoot(CollectibleObjectProfile profile, string prefabName, int interactableLayer)
        {
            GameObject root = new GameObject(prefabName);

            CollectibleObjectPrefabCreationEditor.ConfigureTransform(root.transform);

            root.layer = interactableLayer;

            try
            {
                root.tag = CollectibleObjectPrefabCreationEditor.InteractableTagName;
            }
            catch (UnityException)
            {
                Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] Tag '{CollectibleObjectPrefabCreationEditor.InteractableTagName}' does not exist.");
                UnityEngine.Object.DestroyImmediate(root);
                return null;
            }

            CollectibleObjectController controller = root.AddComponent<CollectibleObjectController>();

            // a. Visual
            GameObject visualObject = new GameObject(CollectibleObjectPrefabCreationEditor.VisualObjectName);
            visualObject.transform.SetParent(root.transform, false);
            CollectibleObjectPrefabCreationEditor.ConfigureTransform(visualObject.transform);
            visualObject.layer = interactableLayer;

            // a.1. Sorting Group
            SortingGroup sortingGroup = visualObject.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerName = CollectibleObjectPrefabCreationEditor.CollectibleSortingLayerName;
            sortingGroup.sortingOrder = 0;

            // a.2. Sprite Renderer
            SpriteRenderer spriteRenderer = visualObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = profile.Icon;
            spriteRenderer.sortingLayerName = CollectibleObjectPrefabCreationEditor.CollectibleSortingLayerName;
            spriteRenderer.sortingOrder = 0;

            // b. Interaction Trigger Area
            GameObject interactionTriggerArea = new GameObject(CollectibleObjectPrefabCreationEditor.InteractionTriggerAreaObjectName);
            interactionTriggerArea.transform.SetParent(root.transform, false);
            CollectibleObjectPrefabCreationEditor.ConfigureTransform(interactionTriggerArea.transform);
            interactionTriggerArea.layer = interactableLayer;

            CircleCollider2D interactionCollider = interactionTriggerArea.AddComponent<CircleCollider2D>();
            interactionCollider.offset = Vector2.zero;
            interactionCollider.radius = CollectibleObjectPrefabCreationEditor.InteractionTriggerRadius;
            interactionCollider.isTrigger = true;

            // c. Controller
            if (!CollectibleObjectPrefabCreationEditor.ConfigureController(controller, profile, visualObject.transform, spriteRenderer))
            {
                UnityEngine.Object.DestroyImmediate(root);
                return null;
            }

            return root;
        }

        // 8. Synchronization
        private static bool SynchronizePrefab(GameObject root, CollectibleObjectProfile profile, string expectedPrefabName, int interactableLayer)
        {
            bool changed = false;

            // a. Root
            if (root.name != expectedPrefabName)
            {
                root.name = expectedPrefabName;
                changed = true;
            }

            changed |= CollectibleObjectPrefabCreationEditor.SynchronizeTransform(root.transform);

            if (root.layer != interactableLayer)
            {
                root.layer = interactableLayer;
                changed = true;
            }
            if (root.tag != CollectibleObjectPrefabCreationEditor.InteractableTagName)
            {
                root.tag = CollectibleObjectPrefabCreationEditor.InteractableTagName;
                changed = true;
            }

            CollectibleObjectController controller = root.GetComponent<CollectibleObjectController>();
            if (controller == null)
            {
                controller = root.AddComponent<CollectibleObjectController>();
                changed = true;
            }

            // b. Visual
            Transform visualTransform = root.transform.Find(CollectibleObjectPrefabCreationEditor.VisualObjectName);

            GameObject visualObject;
            if (visualTransform == null)
            {
                visualObject = new GameObject(CollectibleObjectPrefabCreationEditor.VisualObjectName);
                visualObject.transform.SetParent(root.transform, false);
                visualTransform = visualObject.transform;
                changed = true;
            }
            else
            {
                visualObject = visualTransform.gameObject;
            }

            changed |= CollectibleObjectPrefabCreationEditor.SynchronizeTransform(visualTransform);

            if (visualObject.layer != interactableLayer)
            {
                visualObject.layer = interactableLayer;
                changed = true;
            }

            SortingGroup sortingGroup = visualObject.GetComponent<SortingGroup>();
            if (sortingGroup == null)
            {
                sortingGroup = visualObject.AddComponent<SortingGroup>();
                changed = true;
            }
            if (sortingGroup.sortingLayerName != CollectibleObjectPrefabCreationEditor.CollectibleSortingLayerName)
            {
                sortingGroup.sortingLayerName = CollectibleObjectPrefabCreationEditor.CollectibleSortingLayerName;
                changed = true;
            }
            if (sortingGroup.sortingOrder != 0)
            {
                sortingGroup.sortingOrder = 0;
                changed = true;
            }

            SpriteRenderer spriteRenderer = visualObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = visualObject.AddComponent<SpriteRenderer>();
                changed = true;
            }
            if (spriteRenderer.sprite != profile.Icon)
            {
                spriteRenderer.sprite = profile.Icon;
                changed = true;
            }
            if (spriteRenderer.sortingLayerName != CollectibleObjectPrefabCreationEditor.CollectibleSortingLayerName)
            {
                spriteRenderer.sortingLayerName = CollectibleObjectPrefabCreationEditor.CollectibleSortingLayerName;
                changed = true;
            }
            if (spriteRenderer.sortingOrder != 0)
            {
                spriteRenderer.sortingOrder = 0;
                changed = true;
            }

            // c. Interaction Trigger Area
            Transform interactionTransform = root.transform.Find(CollectibleObjectPrefabCreationEditor.InteractionTriggerAreaObjectName);
            GameObject interactionObject;
            if (interactionTransform == null)
            {
                interactionObject = new GameObject(CollectibleObjectPrefabCreationEditor.InteractionTriggerAreaObjectName);
                interactionObject.transform.SetParent(root.transform, false);
                interactionTransform = interactionObject.transform;
                changed = true;
            }
            else
            {
                interactionObject = interactionTransform.gameObject;
            }

            changed |= CollectibleObjectPrefabCreationEditor.SynchronizeTransform(interactionTransform);

            if (interactionObject.layer != interactableLayer)
            {
                interactionObject.layer = interactableLayer;
                changed = true;
            }

            CircleCollider2D interactionCollider = interactionObject.GetComponent<CircleCollider2D>();
            if (interactionCollider == null)
            {
                interactionCollider = interactionObject.AddComponent<CircleCollider2D>();
                changed = true;
            }
            if (interactionCollider.offset != Vector2.zero)
            {
                interactionCollider.offset = Vector2.zero;
                changed = true;
            }
            if (!Mathf.Approximately(interactionCollider.radius, CollectibleObjectPrefabCreationEditor.InteractionTriggerRadius))
            {
                interactionCollider.radius = CollectibleObjectPrefabCreationEditor.InteractionTriggerRadius;
                changed = true;
            }
            if (!interactionCollider.isTrigger)
            {
                interactionCollider.isTrigger = true;
                changed = true;
            }

            // d. Controller References
            changed |= CollectibleObjectPrefabCreationEditor.SynchronizeController(controller, profile, visualTransform, spriteRenderer);

            return changed;
        }

        // 9. Controller Configuration
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
        private static bool SynchronizeController(CollectibleObjectController controller, CollectibleObjectProfile profile, Transform visualTransform, SpriteRenderer visualSpriteRenderer)
        {
            SerializedObject serializedController = new SerializedObject(controller);
            SerializedProperty profileProperty              = serializedController.FindProperty("_profile");
            SerializedProperty visualTransformProperty      = serializedController.FindProperty("_visualTransform");
            SerializedProperty visualSpriteRendererProperty = serializedController.FindProperty("_visualSpriteRenderer");

            if (profileProperty == null || visualTransformProperty == null || visualSpriteRendererProperty == null)
            {
                Debug.LogError($"[{nameof(CollectibleObjectPrefabCreationEditor)}] One or more required serialized properties could not be found on {nameof(CollectibleObjectController)}.");
                return false;
            }

            bool changed = false;

            if (profileProperty.objectReferenceValue != profile)
            {
                profileProperty.objectReferenceValue = profile;
                changed = true;
            }
            if (visualTransformProperty.objectReferenceValue != visualTransform)
            {
                visualTransformProperty.objectReferenceValue = visualTransform;
                changed = true;
            }
            if (visualSpriteRendererProperty.objectReferenceValue != visualSpriteRenderer)
            {
                visualSpriteRendererProperty.objectReferenceValue = visualSpriteRenderer;
                changed = true;
            }

            if (!changed) return false;

            serializedController.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(controller);

            return true;
        }

        // 10. Naming
        private static string CreatePrefabName(string collectibleProfileAssetName)
        {
            if (string.IsNullOrWhiteSpace(collectibleProfileAssetName)) return string.Empty;

            int firstSeparatorIndex = collectibleProfileAssetName.IndexOf('_');
            if (firstSeparatorIndex < 0 || firstSeparatorIndex >= collectibleProfileAssetName.Length - 1) return string.Empty;

            string collectibleName = collectibleProfileAssetName.Substring(firstSeparatorIndex + 1);
            if (string.IsNullOrWhiteSpace(collectibleName)) return string.Empty;

            return $"{CollectibleCreationDefinitions.PrefabPrefix}{collectibleName}";
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

        // 11. Validation
        private static bool DoesSortingLayerExist(string sortingLayerName)
        {
            return SortingLayer.layers.Any(sortingLayer => sortingLayer.name == sortingLayerName);
        }

        // 12. Transform
        private static void ConfigureTransform(Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale    = Vector3.one;
        }

        private static bool SynchronizeTransform(Transform transform)
        {
            bool changed = false;

            if (transform.localPosition != Vector3.zero)
            {
                transform.localPosition = Vector3.zero;
                changed = true;
            }
            if (transform.localRotation != Quaternion.identity)
            {
                transform.localRotation = Quaternion.identity;
                changed = true;
            }
            if (transform.localScale != Vector3.one)
            {
                transform.localScale = Vector3.one;
                changed = true;
            }

            return changed;
        }

        // --- Model ---
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
        private readonly struct ExistingPrefabEntry
        {
            public string AssetPath { get; }

            public ExistingPrefabEntry(string assetPath)
            {
                this.AssetPath = assetPath;
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