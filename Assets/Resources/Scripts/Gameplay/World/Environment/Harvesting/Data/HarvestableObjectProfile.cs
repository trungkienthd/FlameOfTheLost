using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Hvt_New", menuName = "Data/FlameOfTheLost/World/Environment/Harvesting/HarvestableObjectProfile")]
    public class HarvestableObjectProfile : FOTLWorldObjectProfile
    {
        // --- Data / Configs ---
        //  1. Configs
        //      1.1. Durability
        [Header("Durability")]
        [SerializeField, Min(1)] private int _maximumDurability = 10;
        public int MaximumDurability => Mathf.Max(1, _maximumDurability);

        //      1.2. Harvest Impact Responses
        [Header("Harvest Impact Responses")]
        [SerializeField, Min(0f)] private float _defaultImpactDamageMultiplier = 0.25f;
        [SerializeField] private List<HarvestImpactResponseEntry> _impactResponses = new();
        public float DefaultImpactDamageMultiplier => Mathf.Max(0f, _defaultImpactDamageMultiplier);
        public IReadOnlyList<HarvestImpactResponseEntry> ImpactResponses => _impactResponses;

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

        //      1.5. Colliders
        [Header("Colliders")]
        [SerializeField]
        private HarvestableColliderConfigurationRecord _colliderConfiguration = new();
        public HarvestableColliderConfigurationRecord ColliderConfiguration => _colliderConfiguration;

        // --- Public APIs ---
        public float GetImpactDamageMultiplier(HarvestImpactType impactType)
        {
            if (impactType == HarvestImpactType.None) return 0f;

            if (this.ImpactResponses != null)
            {
                for (int i = 0; i < this.ImpactResponses.Count; i++)
                {
                    HarvestImpactResponseEntry entry = this.ImpactResponses[i];

                    if (entry == null) continue;

                    if (entry.ImpactType == impactType)
                    {
                        return entry.DamageMultiplier;
                    }
                }
            }

            return this.DefaultImpactDamageMultiplier;
        }

    }
}
