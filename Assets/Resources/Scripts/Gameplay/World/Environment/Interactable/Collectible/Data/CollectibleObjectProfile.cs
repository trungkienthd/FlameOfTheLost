using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "CltItm_New", menuName = "Data/FlameOfTheLost/World/Environment/Interactable/Collectible/CollectibleItemProfile")]
    public class CollectibleObjectProfile : FOTLWorldObjectProfile
    {
        // --- Data / Configs ---
        //  1. Item
        [Header("Item")]
        [SerializeField] private ItemProfile _item;
        [SerializeField, Min(1)] private int _quantity = 1;
        public ItemProfile Item => _item;
        public int Quantity => Mathf.Max(1, _quantity);

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