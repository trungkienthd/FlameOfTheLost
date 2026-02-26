using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class SkillCharacterAnimationProfileUtility
    {
        // Put all your SkillCharacterAnimationProfile assets under:
        // Assets/Resources/Data/Combat/SkillCharacterAnimations/...
        private const string RESOURCES_FOLDER = "Data/Combat/SkillCharacterAnimations";

        private static bool _isBuilt;
        public static bool IsBuilt
        {
            get => _isBuilt;
            private set => _isBuilt = value;
        }
        private static Dictionary<SkillCharacterAnimationKey, SkillCharacterAnimationProfile> _map;
        public static Dictionary<SkillCharacterAnimationKey, SkillCharacterAnimationProfile> Map
        {
            get => _map;
            private set => _map = value;
        }

        // Call once at boot if you want deterministic timing; otherwise it lazy-builds on first query.
        public static void WarmUp()
        {
            SkillCharacterAnimationProfileUtility.EnsureBuilt();
        }

        public static bool TryGet(SkillProfile skill, CharacterTagType tag, out SkillCharacterAnimationProfile profile)
        {
            SkillCharacterAnimationProfileUtility.EnsureBuilt();

            profile = null;
            if (skill == null) return false;

            // Exact match first
            if (SkillCharacterAnimationProfileUtility.Map.TryGetValue(new SkillCharacterAnimationKey(skill.ID, tag), out profile)) return true;

            // Optional fallback: (skill, Default)
            if (tag != CharacterTagType.Default && SkillCharacterAnimationProfileUtility.Map.TryGetValue(new SkillCharacterAnimationKey(skill.ID, CharacterTagType.Default), out profile)) return true;

            return false;
        }

        public static SkillCharacterAnimationProfile GetOrNull(SkillProfile skill, CharacterTagType tag)
        {
            SkillCharacterAnimationProfileUtility.TryGet(skill, tag, out var skillCharacterAnimation);
            return skillCharacterAnimation;
        }

        // --- Helpers
        private static void EnsureBuilt()
        {
            if (SkillCharacterAnimationProfileUtility.IsBuilt) return;

            SkillCharacterAnimationProfileUtility.Map = new Dictionary<SkillCharacterAnimationKey, SkillCharacterAnimationProfile>(512);

            var skillCharacterAnimations = Resources.LoadAll<SkillCharacterAnimationProfile>(SkillCharacterAnimationProfileUtility.RESOURCES_FOLDER);
            if (skillCharacterAnimations == null || skillCharacterAnimations.Length == 0)
            {
                Debug.LogWarning($"[SkillCharacterAnimationProfileResolver] No SkillCharacterAnimationProfile assets found at Resources/{RESOURCES_FOLDER}.");
                
                SkillCharacterAnimationProfileUtility.IsBuilt = true;
                return;
            }

            foreach (var skillCharacterAnimation in skillCharacterAnimations)
            {
                if (skillCharacterAnimation.Skill == null)
                {
                    Debug.LogError($"[SkillCharacterAnimationProfileResolver] SkillCharacterAnimationProfile asset '{skillCharacterAnimation.name}' has no Skill assigned; skipping.");
                    continue;
                }

                // These property names must exist on your SkillCharacterAnimationProfile.
                // If yours differ, just rename here.
                var skill = skillCharacterAnimation.Skill;
                var tag = skillCharacterAnimation.CharacterTag;

                if (skill == null)
                {
                    Debug.LogWarning($"[SkillCharacterAnimationProfileResolver] Asset '{skillCharacterAnimation.name}' has null SkillProfile.", skillCharacterAnimation);
                    continue;
                }

                var key = new SkillCharacterAnimationKey(skill.ID, tag);
                if (SkillCharacterAnimationProfileUtility.Map.TryGetValue(key, out var existingSkillCharacterAnimation) && existingSkillCharacterAnimation != null)
                {
                    Debug.LogError
                    (
                        $"[SkillCharacterAnimationProfileResolver] Duplicate mapping for SkillID={skill.ID} ('{skill.name}'), Tag={tag}. " +
                        $"Existing='{existingSkillCharacterAnimation.name}', Duplicate='{skillCharacterAnimation.name}'.",
                        skillCharacterAnimation
                    );
                    continue;
                }

                SkillCharacterAnimationProfileUtility.Map[key] = skillCharacterAnimation;
            }

            SkillCharacterAnimationProfileUtility.IsBuilt = true;
        }
    }
}
