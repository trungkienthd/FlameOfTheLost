using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "RwdDrop_Character", menuName = "Data/FlameOfTheLost/Character/Shared/Reward/CharacterRewardDropProfile")]
    public class CharacterRewardDropProfile : CharacterDataAsset
    {
        [Header("Reward")]
        [SerializeField, Min(0)] private int _amount = 1;
        public int Amount => _amount;
    }
}
