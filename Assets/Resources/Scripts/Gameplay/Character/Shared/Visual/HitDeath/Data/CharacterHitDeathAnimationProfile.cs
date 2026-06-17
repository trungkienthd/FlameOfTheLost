using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "AniHD_New", menuName = "Data/FlameOfTheLost/Character/Shared/Visual/Animation/HitDeath/HitDeathAnimationProfile")]
    public class CharacterHitDeathAnimationProfile : ScriptableObject
    {
        [Header("Character")]
        [SerializeField] private CharacterType _characterType = CharacterType.Default;
        public CharacterType CharacterType => _characterType;

        [Header("Hit / Death Clips")]
        [SerializeField] private CharacterFacingAnimationClipSetEntry _hitAnimationClipSet;
        [SerializeField] private CharacterFacingAnimationClipSetEntry _deathAnimationClipSet;
        public CharacterFacingAnimationClipSetEntry HitAnimationClipSet => _hitAnimationClipSet;
        public CharacterFacingAnimationClipSetEntry DeathAnimationClipSet => _deathAnimationClipSet;

        public AnimationClip GetHitClip(CharacterFacingStatus facingStatus)
        {
            return this.HitAnimationClipSet.GetClip(facingStatus);
        }

        public AnimationClip GetDeathClip(CharacterFacingStatus facingStatus)
        {
            return this.DeathAnimationClipSet.GetClip(facingStatus);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!this.HitAnimationClipSet.IsValid)
            {
                Debug.LogWarning($"[{this.name}] HitAnimationClipSet is not fully assigned.", this);
            }

            if (!this.DeathAnimationClipSet.IsValid)
            {
                Debug.LogWarning($"[{this.name}] DeathAnimationClipSet is not fully assigned.", this);
            }
        }
#endif
    }
}