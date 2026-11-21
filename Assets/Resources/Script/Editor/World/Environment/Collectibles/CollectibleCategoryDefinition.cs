using System;

namespace FlameOfTheLost.Editor
{
    public readonly struct CollectibleCategoryDefinition
    {
        // --- Properties ---
        // 1. Identity
        public string DisplayName { get; }
        public Type ItemProfileType { get; }
        public string ItemPrefix { get; }

        // 2. ID Range
        public int CollectibleIdRangeStart { get; }
        public int CollectibleIdRangeEnd { get; }

        // 3. Paths
        public string ItemProfileFolderPath { get; }
        public string CollectibleProfileFolderPath { get; }
        public string CollectiblePrefabFolderPath { get; }
        public string OutlineSpriteSheetPath { get; }


        public CollectibleCategoryDefinition(string displayName, Type itemProfileType, string itemPrefix, int collectibleIdRangeStart, int collectibleIdRangeEnd, string itemProfileFolderPath, string collectibleProfileFolderPath, string collectiblePrefabFolderPath, string outlineSpriteSheetPath)
        {
            this.DisplayName = displayName;
            this.ItemProfileType = itemProfileType;
            this.ItemPrefix = itemPrefix;

            this.CollectibleIdRangeStart = collectibleIdRangeStart;
            this.CollectibleIdRangeEnd = collectibleIdRangeEnd;

            this.ItemProfileFolderPath = itemProfileFolderPath;
            this.CollectibleProfileFolderPath = collectibleProfileFolderPath;
            this.CollectiblePrefabFolderPath = collectiblePrefabFolderPath;
            this.OutlineSpriteSheetPath = outlineSpriteSheetPath;
        }
    }
}
