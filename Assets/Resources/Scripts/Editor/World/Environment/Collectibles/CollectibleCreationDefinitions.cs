#if UNITY_EDITOR

using UnityEngine;

namespace FlameOfTheLost.Editor
{
    public static class CollectibleCreationDefinitions
    {
        // --- Constants ---
        //  1. Root Paths
        public const string CollectibleObjectProfileRootFolderPath = "Assets/Resources/Data/World/Environment/Interactables/Collectibles";
        public const string CollectibleObjectPrefabRootFolderPath = "Assets/Resources/Prefabs/World/Environment/Interactables/Collectibles";

        //  2. Naming
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

                collectibleIdRangeStart: 32401,
                collectibleIdRangeEnd:   32499,

                itemProfileFolderPath:          "Assets/Resources/Data/Inventory/Items/Materials",
                collectibleProfileFolderPath:   "Assets/Resources/Data/World/Environment/Interactables/Collectibles/Materials",
                collectiblePrefabFolderPath:    "Assets/Resources/Prefabs/World/Environment/Interactables/Collectibles/Materials",
                outlineSpriteSheetPath:         "Assets/Resources/Art/Sprites/Inventory/Items/Materials/CFRPG16_Resources_Icons_Outline.png"
            ),

            // Consumable
            new CollectibleCategoryDefinition
            (
                displayName: "Consumable", itemProfileType: typeof(ConsumableItemProfile), itemPrefix: "Csm_",

                collectibleIdRangeStart: 33301,
                collectibleIdRangeEnd:   33399,

                itemProfileFolderPath:          "Assets/Resources/Data/Inventory/Items/Consumables",
                collectibleProfileFolderPath:   "Assets/Resources/Data/World/Environment/Interactables/Collectibles/Consumables",
                collectiblePrefabFolderPath:    "Assets/Resources/Prefabs/World/Environment/Interactables/Collectibles/Consumables",
                outlineSpriteSheetPath:         "Assets/Resources/Art/Sprites/Inventory/Items/Consumables/CFRPG16_Food_Icons_Outline.png"
            )
        };
    }
}

#endif
