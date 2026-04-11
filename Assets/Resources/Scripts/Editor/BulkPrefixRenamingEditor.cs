using System.IO;
using UnityEditor;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class BulkPrefixRenamingEditor
    {
        private const string PREFIX = "CFRPG16_";


        [MenuItem("Tools/Assets/Add Prefix to Selected Folder (CFRPG16_)")]
        public static void AddPrefixToSelectedFolder()
        {
            Object obj = Selection.activeObject;
            if (obj == null)
            {
                Debug.LogError("Select a folder in the Project window first.");
                return;
            }

            string folderPath = AssetDatabase.GetAssetPath(obj);
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("Selected object is not a folder. Please select a folder.");
                return;
            }

            // Get all assets (including subfolders). This returns GUIDs.
            string[] guids = AssetDatabase.FindAssets("", new[] { folderPath });

            int renamed = 0;
            int skipped = 0;
            int failed = 0;

            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                    // Skip folders
                    if (AssetDatabase.IsValidFolder(assetPath))
                    {
                        skipped++;
                        continue;
                    }

                    // Skip meta / non-assets just in case
                    if (assetPath.EndsWith(".meta"))
                    {
                        skipped++;
                        continue;
                    }

                    string fileName = Path.GetFileNameWithoutExtension(assetPath);
                    string ext = Path.GetExtension(assetPath);

                    // Only rename if missing prefix
                    if (fileName.StartsWith(PREFIX))
                    {
                        skipped++;
                        continue;
                    }

                    string newName = PREFIX + fileName;

                    // Collision check (Unity rename fails if same name exists in folder)
                    string dir = Path.GetDirectoryName(assetPath)?.Replace("\\", "/");
                    string candidatePath = $"{dir}/{newName}{ext}";
                    if (File.Exists(candidatePath))
                    {
                        Debug.LogWarning($"Skip (name collision): {assetPath} -> {candidatePath}");
                        failed++;
                        continue;
                    }

                    string error = AssetDatabase.RenameAsset(assetPath, newName);
                    if (string.IsNullOrEmpty(error))
                    {
                        renamed++;
                    }
                    else
                    {
                        Debug.LogError($"Failed: {assetPath} -> {newName} | {error}");
                        failed++;
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log($"Bulk rename done. Renamed={renamed}, Skipped={skipped}, Failed={failed}");
        }
    }
}
