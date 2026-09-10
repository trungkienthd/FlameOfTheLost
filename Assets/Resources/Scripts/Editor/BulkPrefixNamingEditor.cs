#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class BulkPrefixNamingEditor
    {
        private const string Prefix = "CFRPG16_";

        [MenuItem("FlameOfTheLost/Tools/Asset/Sprite/Common/Add Prefix to Selected Folder")]
        public static void AddPrefixToSelectedFolder()
        {
            Object obj = Selection.activeObject;
            if (obj == null)
            {
                Debug.LogError($"[{nameof(BulkPrefixNamingEditor)}] Select a folder in the Project window first.");
                return;
            }

            string folderPath = AssetDatabase.GetAssetPath(obj);
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError($"[{nameof(BulkPrefixNamingEditor)}] Selected object is not a folder. Please select a folder.");
                return;
            }

            // Get all assets (including subfolders). This returns GUIDs.
            string[] guids = AssetDatabase.FindAssets("", new[] { folderPath });

            int renamedCount = 0;
            int skippedCount = 0;
            int failedCount = 0;

            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                    // Skip folders
                    if (AssetDatabase.IsValidFolder(assetPath))
                    {
                        skippedCount++;
                        continue;
                    }

                    // Skip meta / non-assets just in case
                    if (assetPath.EndsWith(".meta"))
                    {
                        skippedCount++;
                        continue;
                    }

                    string fileName = Path.GetFileNameWithoutExtension(assetPath);
                    string ext = Path.GetExtension(assetPath);

                    // Only rename if missing prefix
                    if (fileName.StartsWith(BulkPrefixNamingEditor.Prefix))
                    {
                        skippedCount++;
                        continue;
                    }

                    string newName = BulkPrefixNamingEditor.Prefix + fileName;

                    // Collision check (Unity rename fails if same name exists in folder)
                    string dir = Path.GetDirectoryName(assetPath)?.Replace("\\", "/");
                    string candidatePath = $"{dir}/{newName}{ext}";
                    if (File.Exists(candidatePath))
                    {
                        Debug.LogWarning($"[{nameof(BulkPrefixNamingEditor)}] Skipped '{assetPath}' because '{candidatePath}' already exists.");
                        failedCount++;
                        continue;
                    }

                    string error = AssetDatabase.RenameAsset(assetPath, newName);
                    if (string.IsNullOrEmpty(error))
                    {
                        renamedCount++;
                    }
                    else
                    {
                        Debug.LogError($"Failed: {assetPath} -> {newName} | {error}");
                        failedCount++;
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log($"[{nameof(BulkPrefixNamingEditor)}] Finished adding asset prefixes. Renamed: {renamedCount}, Skipped: {skippedCount}, Failed: {failedCount}.");
        }
    }
}

#endif