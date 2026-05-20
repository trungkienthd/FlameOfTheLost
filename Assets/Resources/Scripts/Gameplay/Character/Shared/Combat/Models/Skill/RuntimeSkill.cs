using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class RuntimeSkill
    {
        // --- States ---
        //  1. Base Skill
        [Header("Base Skill")]
        [SerializeField] private SkillProfile _profile;
        [SerializeField] private SkillProvisionType _provisionType;
        public SkillProfile Profile
        {
            get => _profile;
            private set => _profile = value;
        }
        public SkillProvisionType ProvisionType
        {
            get => _provisionType;
            private set => _provisionType = value;
        }

        //  2. Non-serialized runtime data
        [NonSerialized] private GameObject _owner;
        [NonSerialized] private CharacterTagType _characterTag;
        [NonSerialized] private CharacterCombatAnimationClipHitboxSetProfile _animationProfile;
        public GameObject Owner
        {
            get => _owner;
            private set => _owner = value;
        }
        public CharacterTagType CharacterTag
        {
            get => _characterTag;
            private set => _characterTag = value;
        }
        public CharacterCombatAnimationClipHitboxSetProfile CombatAnimationClipHitboxSetProfile
        {
            get => _animationProfile;
            private set => _animationProfile = value;
        }

        public int SkillID => this.Profile != null ? this.Profile.ID : -1;

        // --- Constructors ---
        public RuntimeSkill(SkillProfile profile, WeaponProfile weapon, SkillProvisionType provisionType, GameObject owner, CharacterTagType characterTag, CharacterCombatAnimationClipHitboxSetProfile animationProfile)
        {
            this.Profile = profile;
            this.ProvisionType = provisionType;
            this.Owner = owner;
            this.CharacterTag = characterTag;
            this.CombatAnimationClipHitboxSetProfile = animationProfile;
        }

        // --- Public APIs ---
        public SkillStyleType Style => this.Profile != null ? this.Profile.Style : SkillStyleType.Default;
        public int DamageAmount => this.Profile != null ? this.Profile.DamageAmount : 0;
        public float KnockbackForce => this.Profile != null ? this.Profile.KnockbackForce : 0f;
        public ResourceCost ResourceCost => this.Profile != null ? this.Profile.ResourceCost : default;
        public IReadOnlyList<SkillStatusEffectApplication> StatusEffectApplications => this.Profile != null ? this.Profile.StatusEffectApplications : Array.Empty<SkillStatusEffectApplication>();
        public IEnumerable<SkillStatusEffectApplication> EnumerateStatusEffectApplications(SkillStatusEffectTriggerType trigger, SkillStatusEffectReceiverType receiver)
        {
            if (this.StatusEffectApplications == null) yield break;

            for (int i = 0; i < this.StatusEffectApplications.Count; i++)
            {
                var application = this.StatusEffectApplications[i];
                if (application == null) continue;
                if (!application.IsValid) continue;
                if (application.Trigger != trigger) continue;
                if (application.Receiver != receiver) continue;
                yield return application;
            }
        }
        public bool TryGetMeleeHitboxShape(AttackSideType attackSideType, out MeleeHitboxShape hitboxShape)
        {
            hitboxShape = default;

            // 1) Must be a melee skill profile (ranged / others return false)
            if (this.Profile is not MeleeSkillProfile) return false;

            // 2) Must have an animation profile for (skill + character)
            if (this.CombatAnimationClipHitboxSetProfile == null) return false;

            // 3) Fetch per-side hitbox shape from the animation profile
            // (Assumes you added left/right MeleeHitboxShape to SkillCharacterAnimationProfile)
            hitboxShape = this.CombatAnimationClipHitboxSetProfile.GetMeleeHitboxShape(attackSideType);

            // Optional: if you have an "empty" shape concept, validate here.
            // e.g. return hitboxShape.HasAnyShape;
            return true;
        }
    }
}
