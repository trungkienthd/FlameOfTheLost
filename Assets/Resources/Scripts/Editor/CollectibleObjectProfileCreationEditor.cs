#if UNITY_EDITOR

namespace FlameOfTheLost
{
    public static class CollectibleObjectProfileCreationEditor
    {
        // --- Constants ---
        private const string MaterialProfileFolderPath = "Assets/Resources/Data/Inventory/Items/Materials";
        private const string CollectibleObjectProfileFolderPath = "Assets/Resources/Data/World/Environment/Interactables/Collectibles";

        private const int CollectibleIdRangeStart = 32401;
        private const int CollectibleIdRangeEnd = 32499;

        private const int DefaultQuantity = 1;
        private const int DefaultInteractionPromptPriority = 100;
        private const bool DefaultDestroyAfterCollection = true;
        private const bool DefaultConfigureIconAtStart = true;
    }
}

#endif