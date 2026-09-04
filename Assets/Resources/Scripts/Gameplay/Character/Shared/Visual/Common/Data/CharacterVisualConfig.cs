using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Vis_Character", menuName = "Data/FlameOfTheLost/Character/Shared/Visual/CharacterVisualConfig")]
    public class CharacterVisualConfig : CharacterDataAsset
    {
        [Header("Visual Color")]
        [SerializeField] private Color _normalColor = Color.white;
        public Color NormalColor => _normalColor;

        [Header("Animation")]
        [SerializeField] private CharacterLocomotionAnimationProfile _locomotionAnimationProfile;
        [SerializeField] private CharacterAttackAnimationProfile _attackAnimationProfile;
        [SerializeField] private CharacterHitDeathAnimationProfile _hitDeathAnimationProfile;
        [SerializeField] private CharacterDodgeAnimationProfile _dodgeAnimationProfile;
        public CharacterLocomotionAnimationProfile LocomotionAnimationProfile => _locomotionAnimationProfile;
        public CharacterAttackAnimationProfile AttackAnimationProfile => _attackAnimationProfile;
        public CharacterHitDeathAnimationProfile HitDeathAnimationProfile => _hitDeathAnimationProfile;
        public CharacterDodgeAnimationProfile DodgeAnimationProfile => _dodgeAnimationProfile;
    }
}