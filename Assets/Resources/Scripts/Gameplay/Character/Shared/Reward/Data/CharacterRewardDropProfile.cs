using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "RwdDrop_Character", menuName = "Data/FlameOfTheLost/Character/Shared/Reward/CharacterRewardDropProfile")]
    public class CharacterRewardDropProfile : CharacterDataAsset
    {
        // --- Data / Configs ---
        //  1. Lost Embers
        [Header("Lost Embers")]
        [SerializeField, Min(0)] private int _amount = 1;
        public int Amount => Mathf.Max(0, _amount);

        //  2. Item Drops
        [Header("Item Drops")]
        [SerializeField] private List<CharacterRewardItemDropEntry> _itemDropEntries = new();
        public IReadOnlyList<CharacterRewardItemDropEntry> ItemDropEntries => _itemDropEntries;

        //  3. Drop Position
        [Header("Drop Position")]
        [SerializeField, Min(0f)] private float _randomPositionRadius = 0.35f;
        public float RandomPositionRadius => Mathf.Max(0f, _randomPositionRadius);
    }
}
