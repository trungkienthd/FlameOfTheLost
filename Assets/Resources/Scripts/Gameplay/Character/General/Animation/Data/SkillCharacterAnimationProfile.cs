using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "SkillAnimationProfile", menuName = "Data/FlameOfTheLost/Animation/SkillAnimationProfile")]
    public class SkillCharacterAnimationProfile : ScriptableObject
    {
        [Header("Match Keys")]
        [SerializeField] private SkillProfile _skill;
        [SerializeField] private CharacterTagType _characterTag;
        public SkillProfile Skill => _skill;
        public CharacterTagType CharacterTag => _characterTag;

        [Header("Attack Clips")]
        [SerializeField] private AnimationClip _attackDown;
        [SerializeField] private AnimationClip _attackSide;
        [SerializeField] private AnimationClip _attackUp;

        public AnimationClip AttackDown => _attackDown;
        public AnimationClip AttackSide => _attackSide;
        public AnimationClip AttackUp => _attackUp;

        public AnimationClip GetAttackClip(FacingStatusType facing)
        {
            return facing switch
            {
                FacingStatusType.Down => _attackDown,
                FacingStatusType.Side => _attackSide,
                FacingStatusType.Up => _attackUp,
                _ => _attackDown
            };
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.Skill == null) Debug.LogWarning($"[{this.name}.SkillAnimationProfile] Missing Skill reference.", this);
        }
#endif
    }
}
