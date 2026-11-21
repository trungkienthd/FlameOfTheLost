#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlameOfTheLost.Editor
{
    public sealed class ConsumableStatusEffectProfileGeneratorEditor : EditorWindow
    {
        // --- Constants ---
        private const string TargetFolderPath = "Assets/Resources/Data/Character/Shared/StatusEffects/ConsumableItem";
        private const string StatusEffectRootFolderPath = "Assets/Resources/Data/Character/Shared/StatusEffects";

        // --- Planned Profiles ---
        private static readonly ConsumableStatusEffectDefinition[] Definitions =
        {
            // Meat, Poultry, Eggs
            new(53001, "RawPoultry",     "RecoveryFatigue",            "Raw Poultry",     "Recovery Fatigue"),
            new(53002, "CookedPoultry",  "Recovery",                   "Cooked Poultry",  "Recovery"),
            new(53003, "RawMeat",        "RecoveryVulnerability",      "Raw Meat",        "Recovery Vulnerability"),
            new(53004, "CookedMeat",     "Recovery",                   "Cooked Meat",     "Recovery"),
            new(53005, "RawSteak",       "BerserkRecovery",            "Raw Steak",       "Berserk Recovery"),
            new(53006, "CookedSteak",    "PowerRecovery",              "Cooked Steak",    "Power Recovery"),
            new(53007, "RawMeatLeg",     "HeavyRecovery",              "Raw Meat Leg",    "Heavy Recovery"),
            new(53008, "CookedMeatLeg",  "FortifiedRecovery",          "Cooked Meat Leg", "Fortified Recovery"),
            new(53009, "Egg",            "LightRecovery",              "Egg",              "Light Recovery"),
            new(53010, "FriedEgg",       "StaminaEfficiencyRecovery",  "Fried Egg",        "Stamina Efficiency Recovery"),

            // Mushrooms
            new(53011, "BrownMushroom",  "StaminaRecovery",            "Brown Mushroom",  "Stamina Recovery"),
            new(53012, "PurpleMushroom", "FPRecoverySlow",             "Purple Mushroom", "FP Recovery Slow"),
            new(53013, "RedMushroom",    "GlassCannon",                "Red Mushroom",    "Glass Cannon"),
            new(53014, "BlueMushroom",   "FPEfficiency",               "Blue Mushroom",   "FP Efficiency"),

            // Dairy
            new(53015, "Cheese",         "FortifiedRecovery",          "Cheese",          "Fortified Recovery"),
            new(53016, "Milk",           "BalancedRecovery",           "Milk",            "Balanced Recovery"),

            // Fish
            new(53017, "BlueFish",       "FPEfficiencyRecovery",       "Blue Fish",       "FP Efficiency Recovery"),
            new(53018, "CyanFish",       "MobilityRecovery",           "Cyan Fish",       "Mobility Recovery"),
            new(53019, "DarkBlueFish",   "DefensiveRecovery",          "Dark Blue Fish",  "Defensive Recovery"),
            new(53020, "GreenFish",      "StaminaRecovery",            "Green Fish",      "Stamina Recovery"),
            new(53021, "BrownFish",      "KnockbackResistanceRecovery","Brown Fish",      "Knockback Resistance Recovery"),
            new(53022, "RedFish",        "OffensiveRecovery",          "Red Fish",        "Offensive Recovery"),
            new(53023, "YellowFish",     "StaminaEfficiency",          "Yellow Fish",     "Stamina Efficiency"),
            new(53024, "GreyFish",       "CombatStability",            "Grey Fish",       "Combat Stability"),

            // Grain and Large Produce
            new(53025, "Wheat",          "StaminaRecovery",            "Wheat",           "Stamina Recovery"),
            new(53026, "GreenSquash",    "BalancedRecovery",           "Green Squash",    "Balanced Recovery"),
            new(53027, "Cabbage",        "DefensiveRecovery",          "Cabbage",         "Defensive Recovery"),
            new(53028, "Pumpkin",        "FortifiedRecovery",          "Pumpkin",         "Fortified Recovery"),
            new(53029, "Corn",           "StaminaEfficiencyRecovery",  "Corn",            "Stamina Efficiency Recovery"),
            new(53030, "Parsnip",        "RecoveryRegen",              "Parsnip",         "Recovery Regen"),
            new(53031, "Watermelon",     "BalancedRecovery",           "Watermelon",      "Balanced Recovery"),

            // Peppers and Chili
            new(53032, "RedPepper",      "OffensiveStamina",           "Red Pepper",      "Offensive Stamina"),
            new(53033, "OrangePepper",   "MobilityStaminaRegen",       "Orange Pepper",   "Mobility Stamina Regen"),
            new(53034, "GreenPepper",    "DefensiveEfficiency",        "Green Pepper",    "Defensive Efficiency"),
            new(53035, "Chili",          "Berserk",                    "Chili",           "Berserk"),

            // Ordinary Vegetables
            new(53036, "Tomato",         "PeriodicHeal",               "Tomato",          "Periodic Heal"),
            new(53037, "Carrot",         "MobilityStamina",            "Carrot",          "Mobility Stamina"),
            new(53038, "Cucumber",       "StaminaRecovery",            "Cucumber",        "Stamina Recovery"),

            // Fruit
            new(53039, "Apple",          "BalancedRecovery",           "Apple",           "Balanced Recovery"),
            new(53040, "Peach",          "HealingRegen",               "Peach",           "Healing Regen"),
            new(53041, "Grapes",         "FPStaminaRecovery",          "Grapes",          "FP Stamina Recovery"),
            new(53042, "Pear",           "DefensiveRecovery",          "Pear",            "Defensive Recovery"),
            new(53043, "Cherry",         "PeriodicHeal",               "Cherry",          "Periodic Heal"),
            new(53044, "Strawberry",     "HPFPRecovery",               "Strawberry",      "HP FP Recovery"),

            // Drink
            new(53045, "OrangeJuice",    "UtilityRecovery",            "Orange Juice",    "Utility Recovery"),
        };

        // --- Runtime ---
        private Vector2 _scrollPosition;

        // --- Menu ---
        [MenuItem("FlameOfTheLost/Tools/Assets/Data/Character/StatusEffect/Consumable Status Effects")]
        public static void OpenWindow()
        {
            ConsumableStatusEffectProfileGeneratorEditor window = ConsumableStatusEffectProfileGeneratorEditor.GetWindow<ConsumableStatusEffectProfileGeneratorEditor>();

            window.titleContent = new GUIContent("Consumable Status Effects");
            window.minSize = new Vector2(760f, 500f);
            window.Show();
        }

        // --- GUI ---
        private void OnGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Consumable Status Effect Profile Generator", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Creates the base StatusEffectProfile assets for the 45 currently designed Consumables.\n\nExisting Status Effects are detected by ID and will NOT be overwritten.\n\nThe generator configures Identity and shared/general fields only. Affinity, Duration, Stat Modifiers, and Stat Periodics must still be authored manually.", MessageType.Info);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Target Folder", EditorStyles.boldLabel);
            EditorGUILayout.SelectableLabel(TargetFolderPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));

            EditorGUILayout.Space();

            this.DrawSharedGeneratedConfiguration();

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Missing Consumable Status Effects", GUILayout.Height(32f)))
            {
                ConsumableStatusEffectProfileGeneratorEditor.CreateMissingProfiles();
            }

            EditorGUILayout.Space();

            this.DrawProfilePreview();
        }

        // --- Helpers ---
        //  1. GUI Draw
        private void DrawSharedGeneratedConfiguration()
        {
            EditorGUILayout.LabelField("Automatically Configured", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("• ID");
            EditorGUILayout.LabelField("• Key");
            EditorGUILayout.LabelField("• Display Name");
            EditorGUILayout.LabelField("• Max Stacks = 1");
            EditorGUILayout.LabelField("• Duration Refresh Type = Refresh");
            EditorGUILayout.LabelField("• Stat Modifier Combination Type = AddStacks");
            EditorGUILayout.LabelField("• Flags = empty");
            EditorGUILayout.LabelField("• Stat Modifiers = empty");
            EditorGUILayout.LabelField("• Stat Periodics = empty");
            EditorGUILayout.LabelField("• VFX = none");
            EditorGUILayout.LabelField("• VFX Sorting Order Offset = 5");

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Affinity and Duration intentionally remain at StatusEffectProfile defaults. Configure them manually together with Modifiers and Periodics.", MessageType.Warning);
        }
        private void DrawProfilePreview()
        {
            bool databaseIsValid =ConsumableStatusEffectProfileGeneratorEditor.TryFindExistingStatusEffectPathsByID(out Dictionary<int, string> existingByID);
            if (!databaseIsValid)
            {
                EditorGUILayout.HelpBox("Duplicate StatusEffectProfile IDs exist. Resolve them before generating Consumable Status Effects.", MessageType.Error);
            }

            EditorGUILayout.LabelField($"Profiles ({ConsumableStatusEffectProfileGeneratorEditor.Definitions.Length})", EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            for (int i = 0; i < ConsumableStatusEffectProfileGeneratorEditor.Definitions.Length; i++)
            {
                ConsumableStatusEffectDefinition definition = ConsumableStatusEffectProfileGeneratorEditor.Definitions[i];

                bool exists = existingByID.TryGetValue(definition.ID, out string existingPath);

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                GUILayout.Label(definition.ID.ToString(), GUILayout.Width(60f));
                GUILayout.Label(definition.ProfileName, GUILayout.MinWidth(350f));

                if (exists)
                {
                    GUIStyle existingStyle = new GUIStyle(EditorStyles.label)
                    {
                        fontStyle = FontStyle.Bold
                    };

                    GUILayout.Label("EXISTS", existingStyle, GUILayout.Width(70f));

                    if (GUILayout.Button("Ping", GUILayout.Width(50f)))
                    {
                        UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(existingPath);

                        if (asset != null)
                        {
                            EditorGUIUtility.PingObject(asset);
                            Selection.activeObject = asset;
                        }
                    }
                }
                else
                {
                    GUILayout.Label("MISSING", GUILayout.Width(70f));
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        //  2. Generation
        private static void CreateMissingProfiles()
        {
            ConsumableStatusEffectProfileGeneratorEditor.EnsureFolderExists(TargetFolderPath);

            if (!ConsumableStatusEffectProfileGeneratorEditor.TryFindExistingStatusEffectPathsByID(out Dictionary<int, string> existingByID))
            {
                Debug.LogError($"[{nameof(ConsumableStatusEffectProfileGeneratorEditor)}] Generation aborted because duplicate StatusEffectProfile IDs exist.");
                return;
            }

            int createdCount = 0;
            int skippedCount = 0;
            int blockedCount = 0;

            AssetDatabase.StartAssetEditing();

            try
            {
                for (int i = 0; i < ConsumableStatusEffectProfileGeneratorEditor.Definitions.Length; i++)
                {
                    ConsumableStatusEffectDefinition definition = ConsumableStatusEffectProfileGeneratorEditor.Definitions[i];

                    // a. Skip any StatusEffectProfile already using this ID.
                    if (existingByID.TryGetValue(definition.ID, out string existingPath))
                    {
                        Debug.Log($"[{nameof(ConsumableStatusEffectProfileGeneratorEditor)}] Skipped ID {definition.ID}: existing StatusEffectProfile found at '{existingPath}'.");
                        skippedCount++;
                        continue;
                    }

                    string assetPath = $"{TargetFolderPath}/{definition.AssetFileName}";

                    // b. Never overwrite an occupied path.
                    UnityEngine.Object existingAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                    if (existingAsset != null)
                    {
                        Debug.Log($"[{nameof(ConsumableStatusEffectProfileGeneratorEditor)}] Cannot create '{definition.ProfileName}'. The target path is already occupied:\n{assetPath}");
                        blockedCount++;
                        continue;
                    }

                    // c. Create base StatusEffectProfile.
                    StatusEffectProfile profile = ScriptableObject.CreateInstance<StatusEffectProfile>();

                    profile.name = definition.AssetName;

                    ConsumableStatusEffectProfileGeneratorEditor.ConfigureBaseProfile(profile, definition);

                    AssetDatabase.CreateAsset(profile, assetPath);

                    createdCount++;

                    Debug.Log($"[{nameof(ConsumableStatusEffectProfileGeneratorEditor)}] Created {definition.ID}: {definition.ProfileName}");
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log($"[{nameof(ConsumableStatusEffectProfileGeneratorEditor)}] Finished. Created: {createdCount}, Skipped existing IDs: {skippedCount}, Blocked paths: {blockedCount}.");
        }
        private static void ConfigureBaseProfile(StatusEffectProfile profile, ConsumableStatusEffectDefinition definition)
        {
            SerializedObject serializedObject = new SerializedObject(profile);

            // a. Identity 
            serializedObject.FindProperty("_id").intValue               = definition.ID;
            serializedObject.FindProperty("_key").stringValue           = definition.Key;
            serializedObject.FindProperty("_displayName").stringValue   = definition.DisplayName;
            serializedObject.FindProperty("_description").stringValue   = string.Empty;
            serializedObject.FindProperty("_icon").objectReferenceValue = null;

            // b. Lifetime / Stacking
            serializedObject.FindProperty("_maxStacks").intValue                    = 1;
            serializedObject.FindProperty("_durationRefreshType").enumValueIndex    = (int)StatusEffectDurationRefreshType.Refresh;

            // c. Contributions
            SerializedProperty flags        = serializedObject.FindProperty("_flags");
            SerializedProperty modifiers    = serializedObject.FindProperty("_statModifiers");
            SerializedProperty periodics    = serializedObject.FindProperty("_statPeriodics");

            if (flags != null)
            {
                flags.arraySize = 0;
            }
            if (modifiers != null)
            {
                modifiers.arraySize = 0;
            }
            if (periodics != null)
            {
                periodics.arraySize = 0;
            }

            serializedObject.FindProperty("_combinationType").enumValueIndex = (int)StatusEffectStatModifierCombinationType.AddStacks;

            // d. VFX
            serializedObject.FindProperty("_vfxClip").objectReferenceValue = null;
            serializedObject.FindProperty("_vfxSortingOrderOffset").intValue = 5;

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        //  3. Existing Asset Detection
        private static bool TryFindExistingStatusEffectPathsByID(out Dictionary<int, string> result)
        {
            result = new();

            bool hasDuplicateID = false;

            string[] guids = AssetDatabase.FindAssets("t:StatusEffectProfile", new[] { ConsumableStatusEffectProfileGeneratorEditor.StatusEffectRootFolderPath });

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);

                StatusEffectProfile profile = AssetDatabase.LoadAssetAtPath<StatusEffectProfile>(path);
                if (profile == null) continue;

                int id = profile.Id;
                if (result.TryGetValue(id, out string duplicatePath))
                {
                    Debug.LogError($"[{nameof(ConsumableStatusEffectProfileGeneratorEditor)}] Duplicate StatusEffectProfile ID detected: {id}\nExisting: {duplicatePath}\nDuplicate: {path}");
                    hasDuplicateID = true;
                    continue;
                }

                result.Add(id, path);
            }

            return !hasDuplicateID; 
        }

        //  4. Folder
        private static void EnsureFolderExists(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath)) return;

            string normalizedPath = folderPath.Replace("\\", "/");
            string[] parts = normalizedPath.Split('/');
            if (parts.Length <= 1 || parts[0] != "Assets") throw new InvalidOperationException($"Invalid Assets folder path: {folderPath}");

            string currentPath = "Assets";

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

        // --- Definition ---
        private readonly struct ConsumableStatusEffectDefinition
        {
            public int ID { get; }

            public string ConsumableName { get; }
            public string EffectName { get; }

            public string ConsumableDisplayName { get; }
            public string EffectDisplayName { get; }

            public string ProfileName => $"SE_Csm_{this.ConsumableName}_{this.EffectName}";
            public string AssetName => $"{this.ID}_{this.ProfileName}";
            public string AssetFileName => $"{this.AssetName}.asset";

            public string Key => $"{ToUpperSnakeCase(this.ConsumableName)}_{ToUpperSnakeCase(this.EffectName)}";
            public string DisplayName => $"{this.EffectDisplayName} ({this.ConsumableDisplayName})";

            public ConsumableStatusEffectDefinition(int id, string consumableName, string effectName, string consumableDisplayName, string effectDisplayName)
            {
                this.ID = id;

                this.ConsumableName = consumableName;
                this.EffectName = effectName;

                this.ConsumableDisplayName = consumableDisplayName;
                this.EffectDisplayName = effectDisplayName;
            }

            private static string ToUpperSnakeCase(string value)
            {
                if (string.IsNullOrWhiteSpace(value)) return string.Empty;

                List<char> characters = new();

                for (int i = 0; i < value.Length; i++)
                {
                    char current = value[i];

                    if (i > 0 && char.IsUpper(current) && (char.IsLower(value[i - 1]) || (i + 1 < value.Length && char.IsLower(value[i + 1]))))
                    {
                        characters.Add('_');
                    }

                    characters.Add(char.ToUpperInvariant(current));
                }

                return new string(characters.ToArray());
            }
        }
    }
}

#endif
