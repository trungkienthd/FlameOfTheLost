using System;

namespace FlameOfTheLost
{
    public readonly struct CollectibleCategoryDefinition
    {
        // --- Identity ---
        public string DisplayName { get; }
        public Type ItemProfileType { get; }
        public string ItemPrefix { get; }

        // --- Paths ---
        public string ItemProfileFolderPath { get; }
        public string CollectibleProfileFolderPath { get; }
        public string CollectiblePrefabFolderPath { get; }
        public string OutlineSpriteSheetPath { get; }

        public CollectibleCategoryDefinition(string displayName, Type itemProfileType, string itemPrefix, string itemProfileFolderPath, string collectibleProfileFolderPath, string collectiblePrefabFolderPath, string outlineSpriteSheetPath)
        {
            this.DisplayName = displayName;
            this.ItemProfileType = itemProfileType;
            this.ItemPrefix = itemPrefix;
            this.ItemProfileFolderPath = itemProfileFolderPath;
            this.CollectibleProfileFolderPath = collectibleProfileFolderPath;
            this.CollectiblePrefabFolderPath = collectiblePrefabFolderPath;
            this.OutlineSpriteSheetPath = outlineSpriteSheetPath;
        }
    }
}
