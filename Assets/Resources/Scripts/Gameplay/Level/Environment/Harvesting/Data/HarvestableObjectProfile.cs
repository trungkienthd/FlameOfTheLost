using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Hvt_New", menuName = "Data/FlameOfTheLost/Level/Environment/Harvesting/HarvestableObjectProfile")]
    public class HarvestableObjectProfile : FOTLWorldObjectProfile
    {
        // --- Data / Configs ---
        //  1. Configs
        //      1.1. Durability
        [Header("Durability")]
        [SerializeField, Min(1)] private int _maximumDurability = 10;
        public int MaximumDurability => Mathf.Max(1, _maximumDurability);

        //      1.2. Accepted Impacts
        [Header("Accepted Harvest Impacts")]
        [SerializeField]
        private HarvestImpactType _acceptedImpactTypes = HarvestImpactType.Cut;
        public HarvestImpactType AcceptedImpactTypes => _acceptedImpactTypes;

        //      1.3. Broken State
        [Header("Broken State")]
        [SerializeField] private HarvestableBreakBehaviour _breakBehaviour = HarvestableBreakBehaviour.ReplaceVisual;
        [SerializeField] private Sprite _harvestedSprite;
        public HarvestableBreakBehaviour BreakBehaviour => _breakBehaviour;
        public Sprite HarvestedSprite => _harvestedSprite;

        //      1.4. Drops
        [Header("Drops")]
        [SerializeField]
        private List<HarvestableDropEntry> _dropEntries = new();
        public IReadOnlyList<HarvestableDropEntry> DropEntries => _dropEntries;

        [SerializeField, Min(0f)] private float _randomPositionRadius = 0.35f;
        public float RandomPositionRadius => Mathf.Max(0f, _randomPositionRadius);
    }
}
