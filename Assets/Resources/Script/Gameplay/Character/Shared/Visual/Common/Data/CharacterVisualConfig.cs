using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Vis_Character", menuName = "Data/FlameOfTheLost/Character/Shared/Visual/CharacterVisualConfig")]
    public class CharacterVisualConfig : CharacterData
    {
        //  1. General Visual 
        //      1.1. Color
        [Header("Visual Color")]
        [SerializeField] private Color _normalColor = Color.white;
        public Color NormalColor => _normalColor;

        //      1.2. Facing
        [Header("Facing")]
        [SerializeField] private float _speedForOne = 10f;
        [SerializeField] private float _speedDampTime = 0.05f;
        public float SpeedForOne => _speedForOne;
        public float SpeedDampTime => _speedDampTime;

        //  2. Animation
        //      2.1. Profiles
        [Header("Animation Profiles")]
        [SerializeField] private CharacterLocomotionAnimationProfile _locomotionAnimationProfile;
        [SerializeField] private CharacterAttackAnimationProfile _attackAnimationProfile;
        [SerializeField] private CharacterHitDeathAnimationProfile _hitDeathAnimationProfile;
        [SerializeField] private CharacterDodgeAnimationProfile _dodgeAnimationProfile;
        public CharacterLocomotionAnimationProfile LocomotionAnimationProfile => _locomotionAnimationProfile;
        public CharacterAttackAnimationProfile AttackAnimationProfile => _attackAnimationProfile;
        public CharacterHitDeathAnimationProfile HitDeathAnimationProfile => _hitDeathAnimationProfile;
        public CharacterDodgeAnimationProfile DodgeAnimationProfile => _dodgeAnimationProfile;

        //  2. Hit / Death timing
        [Header("Hit / Death Animation Timing")]
        [SerializeField] private float _hitCompletionFallbackDelay = 0.3f;
        [SerializeField] private float _hitCompletionPadding = 0.03f;
        [SerializeField] private float _hitToDeathDelay = 0.15f;
        [SerializeField] private float _deathCompletionFallbackDelay = 1.0f;
        [SerializeField] private float _deathCompletionPadding = 0.03f;
        public float HitCompletionFallbackDelay => _hitCompletionFallbackDelay;
        public float HitCompletionPadding => _hitCompletionPadding;
        public float HitToDeathAnimationDelay => _hitToDeathDelay;
        public float DeathCompletionFallbackDelay => _deathCompletionFallbackDelay;
        public float DeathCompletionPadding => _deathCompletionPadding;
    }
}