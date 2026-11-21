using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "AniDdg_New", menuName = "Data/FlameOfTheLost/Character/Shared/Visual/Animation/Dodge/CharacterDodgeAnimationProfile")]
    public class CharacterDodgeAnimationProfile : ScriptableObject
    {
        [Header("Character")]
        [SerializeField] private CharacterType _characterType = CharacterType.Default;
        public CharacterType CharacterType => _characterType;

        [Header("Dodge Clips")]
        [SerializeField] private CharacterFacingAnimationClipSetEntry _dodgeAnimationClipSet;
        public CharacterFacingAnimationClipSetEntry DodgeAnimationClipSet => _dodgeAnimationClipSet;

        public AnimationClip GetDodgeClip(CharacterFacingStatus facingStatus)
        {
            return this.DodgeAnimationClipSet.GetClip(facingStatus);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!this.DodgeAnimationClipSet.IsValid)
            {
                Debug.LogWarning($"[{this.name}] DodgeAnimationClipSet is not fully assigned.", this);
            }
        }
#endif
    }
}
