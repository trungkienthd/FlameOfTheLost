using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FlameOfTheLost
{
    public class HarvestableObjectPrefabCreationEditor : EditorWindow
    {
        // --- Constants ---
        //  1. Paths
        private const string PROFILE_FOLDER_PATH = "Assets/Resources/Data/World/Environment/Harvestables";
        private const string PREFAB_FOLDER_PATH = "Assets/Resources/Prefabs/World/Environment/Harvestables";
        private const string TEMPLATE_PREFAB_PATH = HarvestableObjectPrefabCreationEditor.PREFAB_FOLDER_PATH + "/Prefab_Hvt.prefab";

        //  2. Naming
        private const string HARVESTABLE_NAME_MARKER = "_Hvt_";
        private const string PREFAB_PREFIX = "Prefab_Hvt_";

        //  3. Hierarchy Names
        private const string VISUAL_OBJECT_NAME = "Visual";
        private const string DAMAGE_RECEIVER_OBJECT_NAME = "DamageReceiver";
        private const string BROKEN_COLLIDER_PRESET_OBJECT_NAME = "BrokenColliderPreset";
        private const string DROP_ANCHORS_OBJECT_NAME = "DropAnchors";

        //  4. Serialized Property Names
        //      4.1. HarvestableObjectController / FOTLWorldObjectProfileBehaviour
        private const string PROFILE_PROPERTY = "_profile";
        private const string VISUAL_TRANSFORM_PROPERTY = "_visualTransform";
        private const string VISUAL_SPRITE_RENDERER_PROPERTY = "_visualSpriteRenderer";
        private const string BROKEN_STATE_HANDLER_PROPERTY = "_brokenStateHandler";
        private const string DROP_HANDLER_PROPERTY = "_dropHandler";

        //      4.2. HarvestableDamageReceivingHandler
        private const string HARVESTABLE_OBJECT_CONTROLLER_PROPERTY = "_harvestableObjectController";

        //      4.3. HarvestableBrokenStateHandler
        private const string BROKEN_VISUAL_SPRITE_RENDERER_PROPERTY = "_visualSpriteRenderer";
        private const string ACTIVE_COLLIDER_PROPERTY = "_activeCollider";
        private const string BROKEN_COLLIDER_PRESET_PROPERTY = "_brokenColliderPreset";

        //      4.4. HarvestableDropHandler
        private const string DEFAULT_DROP_ORIGIN_PROPERTY = "_defaultDropOrigin";
        private const string DROP_ANCHORS_PROPERTY = "_dropAnchors";

        // --- Runtime ---
        private Vector2 _scrollPosition;

        private readonly List<HarvestableObjectProfile> _profiles = new();

        // --- Menu ---
        [MenuItem("FlameOfTheLost/Tools/Assets/Prefabs/World/Environment/Create Harvestable Prefabs")]
        private static void OpenWindow()
        {
            HarvestableObjectPrefabCreationEditor window = HarvestableObjectPrefabCreationEditor.GetWindow<HarvestableObjectPrefabCreationEditor>();

            window.titleContent = new GUIContent("Harvestable Prefabs");
            window.minSize = new Vector2(600f, 480f);

            window.RefreshProfiles();
        }

        // --- Unity Callbacks ---
        private void OnEnable()
        {
            this.RefreshProfiles();
        }


        // --- GUI ---
        private void OnGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Harvestable Object Prefab Creation", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            this.DrawConfiguration();

            EditorGUILayout.Space();

            this.DrawProfilePreview();

            EditorGUILayout.Space();

            this.DrawActions();
        }

        // --- Helpers ---
        //  1. Draw
        private void DrawConfiguration()
        {
            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField("Profile Folder", HarvestableObjectPrefabCreationEditor.PROFILE_FOLDER_PATH);
                EditorGUILayout.TextField("Prefab Folder",  HarvestableObjectPrefabCreationEditor.PREFAB_FOLDER_PATH);
                EditorGUILayout.TextField("Template",       HarvestableObjectPrefabCreationEditor.TEMPLATE_PREFAB_PATH);
            }

            GameObject templatePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(HarvestableObjectPrefabCreationEditor.TEMPLATE_PREFAB_PATH);

            if (templatePrefab == null)
            {
                EditorGUILayout.HelpBox($"Template prefab was not found at:\n{HarvestableObjectPrefabCreationEditor.TEMPLATE_PREFAB_PATH}", MessageType.Error);
            }
        }
        private void DrawProfilePreview()
        {
            EditorGUILayout.LabelField($"Harvestable Profiles ({_profiles.Count})", EditorStyles.boldLabel);

            if (_profiles.Count == 0)
            {
                EditorGUILayout.HelpBox("No HarvestableObjectProfile assets were found.", MessageType.Warning);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(240f));

            for (int i = 0; i < _profiles.Count; i++)
            {
                HarvestableObjectProfile profile = _profiles[i];

                if (profile == null) continue;

                string prefabName = HarvestableObjectPrefabCreationEditor.GetPrefabName(profile);

                string prefabPath = HarvestableObjectPrefabCreationEditor.GetPrefabPath(profile);

                bool prefabExists = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null;

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.ObjectField(profile, typeof(HarvestableObjectProfile),false);

                    GUILayout.Label(prefabName, GUILayout.Width(230f));

                    GUILayout.Label(prefabExists ? "Exists" : "Missing", GUILayout.Width(60f));
                }
            }

            EditorGUILayout.EndScrollView();
        }
        private void DrawActions()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Refresh Profiles", GUILayout.Height(32f)))
                {
                    this.RefreshProfiles();
                }

                using (new EditorGUI.DisabledScope(!CanGeneratePrefabs()))
                {
                    if (GUILayout.Button("Create / Update All Prefabs", GUILayout.Height(32f)))
                    {
                        this.CreateOrUpdateAllPrefabs();
                    }
                }
            }
        }

        //  2. Validation
        private bool CanGeneratePrefabs()
        {
            if (_profiles.Count == 0) return false;

            return AssetDatabase.LoadAssetAtPath<GameObject>(HarvestableObjectPrefabCreationEditor.TEMPLATE_PREFAB_PATH) != null;
        }

        //  3. Profile Discovery
        private void RefreshProfiles()
        {
            _profiles.Clear();

            string[] guids = AssetDatabase.FindAssets("t:HarvestableObjectProfile", new[] { HarvestableObjectPrefabCreationEditor.PROFILE_FOLDER_PATH });

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);

                HarvestableObjectProfile profile = AssetDatabase.LoadAssetAtPath<HarvestableObjectProfile>(assetPath);

                if (profile == null) continue;

                _profiles.Add(profile);
            }

            _profiles.Sort((a, b) => string.CompareOrdinal(a.name, b.name));

            this.Repaint();
        }

        //  4. Generation
        private void CreateOrUpdateAllPrefabs()
        {
            GameObject templatePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(HarvestableObjectPrefabCreationEditor.TEMPLATE_PREFAB_PATH);
            if (templatePrefab == null)
            {
                Debug.LogError($"[{nameof(HarvestableObjectPrefabCreationEditor)}] Template prefab not found at '{HarvestableObjectPrefabCreationEditor.TEMPLATE_PREFAB_PATH}'.");
                return;
            }

            HarvestableObjectPrefabCreationEditor.EnsureFolderExists(HarvestableObjectPrefabCreationEditor.PREFAB_FOLDER_PATH);

            int createdCount = 0;
            int updatedCount = 0;
            int failedCount = 0;

            for (int i = 0; i < _profiles.Count; i++)
            {
                HarvestableObjectProfile profile = _profiles[i];

                if (profile == null) continue;

                bool success = HarvestableObjectPrefabCreationEditor.CreateOrUpdatePrefab(profile, out bool created);

                if (!success)
                {
                    failedCount++;
                    continue;
                }

                if (created)
                {
                    createdCount++;
                }
                else
                {
                    updatedCount++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[{nameof(HarvestableObjectPrefabCreationEditor)}] Finished Harvestable prefab generation. Created: {createdCount}, Updated: {updatedCount}, Failed: {failedCount}.");

            this.RefreshProfiles();
        }
        private static bool CreateOrUpdatePrefab(HarvestableObjectProfile profile, out bool created)
        {
            created = false;

            string prefabPath = HarvestableObjectPrefabCreationEditor.GetPrefabPath(profile);

            GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            bool isNewPrefab = existingPrefab == null;

            if (isNewPrefab)
            {
                bool copied = AssetDatabase.CopyAsset(HarvestableObjectPrefabCreationEditor.TEMPLATE_PREFAB_PATH, prefabPath);
                if (!copied)
                {
                    Debug.LogError($"[{nameof(HarvestableObjectPrefabCreationEditor)}] Failed to create prefab '{prefabPath}' from the Harvestable template.");
                    return false;
                }

                AssetDatabase.ImportAsset(prefabPath, ImportAssetOptions.ForceUpdate);
            }

            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
            if (prefabRoot == null)
            {
                if (isNewPrefab)
                {
                    AssetDatabase.DeleteAsset(prefabPath);
                }

                Debug.LogError($"[{nameof(HarvestableObjectPrefabCreationEditor)}] Failed to load prefab contents for '{prefabPath}'.");
                return false;
            }

            bool success = false;

            try
            {
                prefabRoot.name = HarvestableObjectPrefabCreationEditor.GetPrefabName(profile);

                if (!HarvestableObjectPrefabCreationEditor.ConfigurePrefab(prefabRoot, profile)) return false;

                GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
                if (savedPrefab == null)
                {
                    Debug.LogError($"[{nameof(HarvestableObjectPrefabCreationEditor)}] Failed to save prefab '{prefabPath}'.");
                    return false;
                }

                success = true;
                created = isNewPrefab;

                return true;
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(prefabRoot);

                if (!success && isNewPrefab)
                {
                    AssetDatabase.DeleteAsset(prefabPath);
                }
            }
        }

        //  5. Prefab Configuration
        private static bool ConfigurePrefab(GameObject root, HarvestableObjectProfile profile)
        {
            if (root == null || profile == null) return false;

            // Root Components
            HarvestableObjectController     controller          = root.GetComponent<HarvestableObjectController>();
            HarvestableDropHandler          dropHandler         = root.GetComponent<HarvestableDropHandler>();
            HarvestableBrokenStateHandler   brokenStateHandler  = root.GetComponent<HarvestableBrokenStateHandler>();
            Collider2D                      activeCollider      = root.GetComponent<Collider2D>();

            // Child Objects
            Transform visualTransform               = root.transform.Find(HarvestableObjectPrefabCreationEditor.VISUAL_OBJECT_NAME);
            Transform damageReceiverTransform       = root.transform.Find(HarvestableObjectPrefabCreationEditor.DAMAGE_RECEIVER_OBJECT_NAME);
            Transform brokenColliderPresetTransform = root.transform.Find(HarvestableObjectPrefabCreationEditor.BROKEN_COLLIDER_PRESET_OBJECT_NAME);
            Transform dropAnchorsTransform          = root.transform.Find(HarvestableObjectPrefabCreationEditor.DROP_ANCHORS_OBJECT_NAME);

            // Validate required root components.
            if (controller          == null) return HarvestableObjectPrefabCreationEditor.LogMissing(root, nameof(HarvestableObjectController));
            if (dropHandler         == null) return HarvestableObjectPrefabCreationEditor.LogMissing(root, nameof(HarvestableDropHandler));
            if (brokenStateHandler  == null) return HarvestableObjectPrefabCreationEditor.LogMissing(root, nameof(HarvestableBrokenStateHandler));
            if (activeCollider      == null) return HarvestableObjectPrefabCreationEditor.LogMissing(root, "Root Collider2D");

            // Validate required children.
            if (visualTransform                 == null) return HarvestableObjectPrefabCreationEditor.LogMissing(root, HarvestableObjectPrefabCreationEditor.VISUAL_OBJECT_NAME);
            if (damageReceiverTransform         == null) return HarvestableObjectPrefabCreationEditor.LogMissing(root, HarvestableObjectPrefabCreationEditor.DAMAGE_RECEIVER_OBJECT_NAME);
            if (brokenColliderPresetTransform   == null) return HarvestableObjectPrefabCreationEditor.LogMissing(root, HarvestableObjectPrefabCreationEditor.BROKEN_COLLIDER_PRESET_OBJECT_NAME);
            if (dropAnchorsTransform            == null) return HarvestableObjectPrefabCreationEditor.LogMissing(root, HarvestableObjectPrefabCreationEditor.DROP_ANCHORS_OBJECT_NAME);

            // Child Components
            SpriteRenderer visualSpriteRenderer = visualTransform.GetComponent<SpriteRenderer>();
            if (visualSpriteRenderer    == null) return HarvestableObjectPrefabCreationEditor.LogMissing(root, "Visual/SpriteRenderer");

            HarvestableDamageReceivingHandler damageReceivingHandler = damageReceiverTransform.GetComponent<HarvestableDamageReceivingHandler>();
            if (damageReceivingHandler  == null) return HarvestableObjectPrefabCreationEditor.LogMissing(root, nameof(HarvestableDamageReceivingHandler));

            Collider2D brokenColliderPreset = brokenColliderPresetTransform.GetComponent<Collider2D>();
            if (brokenColliderPreset    == null) return HarvestableObjectPrefabCreationEditor.LogMissing(root, "BrokenColliderPreset/Collider2D");

            List<Transform> dropAnchors = HarvestableObjectPrefabCreationEditor.GetDropAnchors(dropAnchorsTransform);
            if (dropAnchors.Count == 0)
            {
                Debug.LogError($"[{nameof(HarvestableObjectPrefabCreationEditor)}] Prefab '{root.name}' does not contain any DropAnchor_* children under '{HarvestableObjectPrefabCreationEditor.DROP_ANCHORS_OBJECT_NAME}'.", root);
                return false;
            }

            // Assign all serialized references.
            if (!HarvestableObjectPrefabCreationEditor.ConfigureController(controller, profile, visualTransform, visualSpriteRenderer, brokenStateHandler, dropHandler)) return false;
            if (!HarvestableObjectPrefabCreationEditor.ConfigureDamageReceiver(damageReceivingHandler, controller)) return false;
            if (!HarvestableObjectPrefabCreationEditor.ConfigureBrokenStateHandler(brokenStateHandler, visualSpriteRenderer, activeCollider, brokenColliderPreset)) return false;
            if (!HarvestableObjectPrefabCreationEditor.ConfigureDropHandler(dropHandler, dropAnchors)) return false;

            HarvestableObjectPrefabCreationEditor.ConfigureVisual(visualSpriteRenderer, profile);

            return true;
        }

        //  6. Configuration
        //      6.1. Controller
        private static bool ConfigureController(HarvestableObjectController controller, HarvestableObjectProfile profile, Transform visualTransform, SpriteRenderer visualSpriteRenderer, HarvestableBrokenStateHandler brokenStateHandler, HarvestableDropHandler dropHandler)
        {
            SerializedObject serializedObject = new SerializedObject(controller);

            if (!HarvestableObjectPrefabCreationEditor.TrySetObjectReference(serializedObject, HarvestableObjectPrefabCreationEditor.PROFILE_PROPERTY, profile, controller)) return false;
            if (!HarvestableObjectPrefabCreationEditor.TrySetObjectReference(serializedObject, HarvestableObjectPrefabCreationEditor.VISUAL_TRANSFORM_PROPERTY, visualTransform, controller)) return false;
            if (!HarvestableObjectPrefabCreationEditor.TrySetObjectReference(serializedObject, HarvestableObjectPrefabCreationEditor.VISUAL_SPRITE_RENDERER_PROPERTY, visualSpriteRenderer, controller)) return false;
            if (!HarvestableObjectPrefabCreationEditor.TrySetObjectReference(serializedObject, HarvestableObjectPrefabCreationEditor.BROKEN_STATE_HANDLER_PROPERTY, brokenStateHandler, controller)) return false;
            if (!HarvestableObjectPrefabCreationEditor.TrySetObjectReference(serializedObject, HarvestableObjectPrefabCreationEditor.DROP_HANDLER_PROPERTY, dropHandler, controller)) return false;

            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            return true;
        }

        //      6.2. Damage Receiver
        private static bool ConfigureDamageReceiver(HarvestableDamageReceivingHandler handler, HarvestableObjectController controller)
        {
            SerializedObject serializedObject = new SerializedObject(handler);

            if (!HarvestableObjectPrefabCreationEditor.TrySetObjectReference(serializedObject, HarvestableObjectPrefabCreationEditor.HARVESTABLE_OBJECT_CONTROLLER_PROPERTY, controller, handler)) return false;

            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            return true;
        }

        //      6.3. Broken State Handler
        private static bool ConfigureBrokenStateHandler(HarvestableBrokenStateHandler handler, SpriteRenderer visualSpriteRenderer, Collider2D activeCollider, Collider2D brokenColliderPreset)
        {
            SerializedObject serializedObject = new SerializedObject(handler);

            if (!HarvestableObjectPrefabCreationEditor.TrySetObjectReference(serializedObject, HarvestableObjectPrefabCreationEditor.BROKEN_VISUAL_SPRITE_RENDERER_PROPERTY, visualSpriteRenderer, handler)) return false;
            if (!HarvestableObjectPrefabCreationEditor.TrySetObjectReference(serializedObject, HarvestableObjectPrefabCreationEditor.ACTIVE_COLLIDER_PROPERTY, activeCollider, handler)) return false;
            if (!HarvestableObjectPrefabCreationEditor.TrySetObjectReference(serializedObject, HarvestableObjectPrefabCreationEditor.BROKEN_COLLIDER_PRESET_PROPERTY, brokenColliderPreset, handler)) return false;

            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            // The preset is data only.
            // It must remain disabled at runtime.
            brokenColliderPreset.enabled = false;

            EditorUtility.SetDirty(brokenColliderPreset);

            return true;
        }

        //      6.4. Drop Handler
        private static bool ConfigureDropHandler(HarvestableDropHandler handler, IReadOnlyList<Transform> dropAnchors)
        {
            SerializedObject serializedObject = new SerializedObject(handler);

            SerializedProperty defaultOriginProperty = serializedObject.FindProperty(HarvestableObjectPrefabCreationEditor.DEFAULT_DROP_ORIGIN_PROPERTY);
            if (defaultOriginProperty == null) return HarvestableObjectPrefabCreationEditor.LogMissingSerializedProperty(handler, HarvestableObjectPrefabCreationEditor.DEFAULT_DROP_ORIGIN_PROPERTY);

            // Intentionally null.
            // HarvestableDropHandler falls back to its own Transform.
            defaultOriginProperty.objectReferenceValue = null;

            SerializedProperty anchorsProperty = serializedObject.FindProperty(HarvestableObjectPrefabCreationEditor.DROP_ANCHORS_PROPERTY);
            if (anchorsProperty == null) return HarvestableObjectPrefabCreationEditor.LogMissingSerializedProperty(handler, HarvestableObjectPrefabCreationEditor.DROP_ANCHORS_PROPERTY);

            anchorsProperty.arraySize = dropAnchors.Count;

            for (int i = 0; i < dropAnchors.Count; i++)
            {
                anchorsProperty.GetArrayElementAtIndex(i).objectReferenceValue = dropAnchors[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            return true;
        }

        //      6.5. Visual
        private static void ConfigureVisual(SpriteRenderer spriteRenderer, HarvestableObjectProfile profile)
        {
            if (spriteRenderer == null || profile == null) return;

            spriteRenderer.sprite = profile.Icon;

            EditorUtility.SetDirty(spriteRenderer);
        }

        //  7. Drop Anchors
        private static List<Transform> GetDropAnchors(Transform dropAnchorsRoot)
        {
            List<Transform> anchors = new();

            if (dropAnchorsRoot == null) return anchors;

            for (int i = 0; i < dropAnchorsRoot.childCount; i++)
            {
                Transform child = dropAnchorsRoot.GetChild(i);

                if (child == null) continue;
                if (!child.name.StartsWith("DropAnchor_", StringComparison.Ordinal)) continue;

                anchors.Add(child);
            }

            anchors.Sort((a, b) => string.CompareOrdinal(a.name, b.name));

            return anchors;
        }

        //  8. Serialized Properties
        private static bool TrySetObjectReference(SerializedObject serializedObject, string propertyName, UnityEngine.Object value, UnityEngine.Object context)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null) return HarvestableObjectPrefabCreationEditor.LogMissingSerializedProperty(context, propertyName);

            property.objectReferenceValue = value;

            return true;
        }

        //  9. Debug
        private static bool LogMissingSerializedProperty(UnityEngine.Object context, string propertyName)
        {
            Debug.LogError($"[{nameof(HarvestableObjectPrefabCreationEditor)}] Serialized property '{propertyName}' was not found on '{context?.GetType().Name ?? "Unknown"}'.", context);
            return false;
        }
        private static bool LogMissing(GameObject prefabRoot, string requiredObject)
        {
            Debug.LogError($"[{nameof(HarvestableObjectPrefabCreationEditor)}] Prefab '{prefabRoot.name}' is missing required object/component '{requiredObject}'.", prefabRoot);
            return false;
        }

        //  10. Naming
        private static string GetPrefabName(HarvestableObjectProfile profile)
        {
            string profileName = profile.name;

            int markerIndex = profileName.IndexOf(HarvestableObjectPrefabCreationEditor.HARVESTABLE_NAME_MARKER, StringComparison.Ordinal);
            if (markerIndex < 0)
            {
                Debug.LogWarning($"[{nameof(HarvestableObjectPrefabCreationEditor)}] Harvestable profile '{profileName}' does not follow the expected '<ID>_Hvt_<Name>' naming convention.");
                return HarvestableObjectPrefabCreationEditor.PREFAB_PREFIX + profileName;
            }

            string harvestableName = profileName.Substring(markerIndex + HarvestableObjectPrefabCreationEditor.HARVESTABLE_NAME_MARKER.Length);

            return HarvestableObjectPrefabCreationEditor.PREFAB_PREFIX + harvestableName;
        }
        private static string GetPrefabPath(HarvestableObjectProfile profile)
        {
            return $"{HarvestableObjectPrefabCreationEditor.PREFAB_FOLDER_PATH}/{HarvestableObjectPrefabCreationEditor.GetPrefabName(profile)}.prefab";
        }

        //  11. Folder Validation
        private static void EnsureFolderExists(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath)) return;

            string parent = Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
            string folderName = Path.GetFileName(folderPath);

            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            {
                HarvestableObjectPrefabCreationEditor.EnsureFolderExists(parent);
            }

            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
