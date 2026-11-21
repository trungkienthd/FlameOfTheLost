using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Clt_New", menuName = "Data/FlameOfTheLost/World/Environment/Interactable/Collectible/CollectibleItemProfile")]
    public class CollectibleObjectProfile : FOTLWorldObjectProfile
    {
        // --- Data / Configs ---
        //  1. Item
        [Header("Item")]
        [SerializeField] private ItemProfile _item;
        public ItemProfile Item => _item;

        //  2. Interaction
        [Header("Interaction")]
        [SerializeField] private int _interactionPromptPriority = 100;
        public int InteractionPromptPriority => _interactionPromptPriority;

        //  3. Collection
        [Header("Collection")]
        [SerializeField] private bool _destroyAfterCollection = true;
        public bool DestroyAfterCollection => _destroyAfterCollection;
    }
}