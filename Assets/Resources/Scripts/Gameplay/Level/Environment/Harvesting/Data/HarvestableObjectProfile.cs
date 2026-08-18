using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Hvt_New", menuName = "Data/FlameOfTheLost/Level/Environment/Harvesting/HarvestableObjectProfile")]
    public class HarvestableObjectProfile : ScriptableObject,
        IIdentityProvider,
        IIconProvider
    {
        // --- States ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField, Min(1)] private int _id = 32200;
        [SerializeField] private string _key;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(5, 5)] private string _description;
        public int Id => Mathf.Max(1, _id);
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        //  2. Icon
        [Header("Icon")]
        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;

        //  3. Configs
        //      3.1. Durability
        [Header("Durability")]
        [SerializeField, Min(1)] private int _maximumDurability = 10;
        public int MaximumDurability => Mathf.Max(1, _maximumDurability);

        //      3.2. Accepted Impacts
        [Header("Accepted Harvest Impacts")]
        [SerializeField]
        private HarvestImpactType _acceptedImpactTypes = HarvestImpactType.Cut;
        public HarvestImpactType AcceptedImpactTypes => _acceptedImpactTypes;

        //      3.3. Broken State
        [Header("Broken State")]
        [SerializeField] private HarvestableBreakBehaviour _breakBehaviour = HarvestableBreakBehaviour.ReplaceVisual;
        [SerializeField] private Sprite _harvestedSprite;
        public HarvestableBreakBehaviour BreakBehaviour => _breakBehaviour;
        public Sprite HarvestedSprite => _harvestedSprite;

        //      3.4. Drops
        [Header("Drops")]
        [SerializeField]
        private List<HarvestableDropEntry> _dropEntries = new();
        public IReadOnlyList<HarvestableDropEntry> DropEntries => _dropEntries;

        [SerializeField, Min(0f)] private float _randomPositionRadius = 0.35f;
        public float RandomPositionRadius => Mathf.Max(0f, _randomPositionRadius);
    }
}
