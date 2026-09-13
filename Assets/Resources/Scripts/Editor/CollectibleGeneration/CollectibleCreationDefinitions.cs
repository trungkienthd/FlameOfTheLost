#if UNITY_EDITOR

using UnityEngine;

namespace FlameOfTheLost
{
    public static class CollectibleCreationDefinitions
    {
        // --- Root Paths ---
        public const string CollectibleObjectProfileRootFolderPath = "Assets/Resources/Data/World/Environment/Interactables/Collectibles";
        public const string CollectibleObjectPrefabRootFolderPath = "Assets/Resources/Prefabs/World/Environment/Interactables/Collectibles";

        // --- Collectible ID Range ---
        public const int CollectibleIdRangeStart = 32401;
        public const int CollectibleIdRangeEnd = 32499;

        // --- Naming ---
        public const string CollectiblePrefix = "Clt_";
        public const string PrefabPrefix = "Prefab_";

        // --- Category Definitions ---
        // To support another collectible family, add another entry here.
        // The generators themselves should not contain category-specific logic.
        public static readonly CollectibleCategoryDefinition[] Categories =
        {
            // Material
            new CollectibleCategoryDefinition
            (
                displayName: "Material", itemProfileType: typeof(MaterialProfile), itemPrefix: "Mtr_",
                itemProfileFolderPath:          "Assets/Resources/Data/Inventory/Items/Materials",
                collectibleProfileFolderPath:   "Assets/Resources/Data/World/Environment/Interactables/Collectibles/Materials",
                collectiblePrefabFolderPath:    "Assets/Resources/Prefabs/World/Environment/Interactables/Collectibles/Materials",
                outlineSpriteSheetPath:         "Assets/Resources/Art/Sprites/Inventory/Items/Materials/CFRPG16_Resources_Icons_Outline.png"
            ),

            // Consumable
            new CollectibleCategoryDefinition
            (
                displayName: "Consumable", itemProfileType: typeof(ConsumableItemProfile), itemPrefix: "Csm_",
                itemProfileFolderPath:          "Assets/Resources/Data/Inventory/Items/Consumables",
                collectibleProfileFolderPath:   "Assets/Resources/Data/World/Environment/Interactables/Collectibles/Consumables",
                collectiblePrefabFolderPath:    "Assets/Resources/Prefabs/World/Environment/Interactables/Collectibles/Consumables",
                outlineSpriteSheetPath:         "Assets/Resources/Art/Sprites/Inventory/Items/Consumables/CFRPG16_Consumables_Icons_Outline.png"
            )
        };
    }
}

#endif
