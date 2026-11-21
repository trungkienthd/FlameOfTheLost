using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterCombatStatusEffectApplicationHandler : CharacterBehaviourControllerBase
    {
        // --- States ---
        //  1. References
        private StatusEffectController StatusEffectController => this.CharacterRootController.StatusEffectController;

        //  2. Limit tracking (per attack-instance = hitboxObject)
        // hitboxID
        //     → receiver type
        //         → applied status-effect profiles
        private readonly Dictionary<int, Dictionary<SkillStatusEffectReceiverType, HashSet<StatusEffectProfile>>> _firstHitAppliedMap = new();
        private IReadOnlyDictionary<int, Dictionary<SkillStatusEffectReceiverType, HashSet<StatusEffectProfile>>> FirstHitAppliedMap => _firstHitAppliedMap;

        // hitboxID
        //     → receiver type
        //         → target ID
        //             → applied status-effect profiles
        private readonly Dictionary<int, Dictionary<SkillStatusEffectReceiverType, Dictionary<int, HashSet<StatusEffectProfile>>>> _oncePerTargetMap = new();
        private IReadOnlyDictionary<int, Dictionary<SkillStatusEffectReceiverType, Dictionary<int, HashSet<StatusEffectProfile>>>> OncePerTargetMap => _oncePerTargetMap;

        // --- Public APIs ---
        //  1. Generic apply-to-self for non-skill systems (e.g. ConsumableItem, Terrain)
        public bool TryApplyStatusEffectApplicationToSelf(StatusEffectApplicationEntry application)
        {
            return this.TryApplyStatusEffectApplicationToSelf(application, this.gameObject);
        }
        public bool TryApplyStatusEffectApplicationToSelf(StatusEffectApplicationEntry application, GameObject sourceOverride)
        {
            if (application == null) return false;
            if (!application.IsValid) return false;
            if (this.StatusEffectController == null) return false;

            GameObject source = sourceOverride != null ? sourceOverride : this.gameObject;

            float chance = Mathf.Clamp01(application.Chance);
            if (chance < 1f && Random.value > chance) return false;

            uint runtimeId = this.StatusEffectController.ApplyStatusEffect(application.StatusEffect, source, application.Potency);

            return runtimeId != 0;
        }

        //      1.1. 
        public int TryApplyStatusEffectApplicationsToSelf(IEnumerable<StatusEffectApplicationEntry> applications)
        {
            return this.TryApplyStatusEffectApplicationsToSelf(applications, this.gameObject);
        }
        public int TryApplyStatusEffectApplicationsToSelf(IEnumerable<StatusEffectApplicationEntry> applications, GameObject sourceOverride)
        {
            if (applications == null) return 0;
            if (this.StatusEffectController == null) return 0;

            int appliedCount = 0;

            foreach (StatusEffectApplicationEntry application in applications)
            {
                if (this.TryApplyStatusEffectApplicationToSelf(application, sourceOverride))
                {
                    appliedCount++;
                }
            }

            return appliedCount;
        }

        //  2. Apply to Self: SkillStatusEffectApplication (OnAttackStart, OnAttackEnd)
        public void ApplySkillStatusEffectsToSelf(RuntimeSkill runtimeSkill, SkillStatusEffectTriggerType trigger)
        {
            if (runtimeSkill == null) return;
            if (this.StatusEffectController == null) return;
            if (trigger != SkillStatusEffectTriggerType.OnAttackStart && trigger != SkillStatusEffectTriggerType.OnAttackEnd) return;

            foreach (var application in runtimeSkill.EnumerateStatusEffectApplications(trigger, SkillStatusEffectReceiverType.Self))
            {
                if (application == null || !application.IsValid) continue;

                float chance = Mathf.Clamp01(application.Chance);
                if (chance < 1f && Random.value > chance) continue;

                this.StatusEffectController.ApplyStatusEffect(application.StatusEffect, this.CharacterRootController.gameObject, application.Potency);
            }
        }

        //  3. Apply when the hit is confirmed: SkillStatusEffectApplication (OnHitConfirmed)
        public void ApplySkillStatusEffectsToSelfOnHitConfirmed(RuntimeSkill runtimeSkill, GameObject hitboxGameObject, int hitTargetID)
        {
            if (runtimeSkill == null) return;
            if (hitboxGameObject == null) return;
            if (this.StatusEffectController == null) return;

            int hitboxID = hitboxGameObject.GetInstanceID();

            this.ApplyToReceiverOnHitConfirmed
            (
                receiverStatusEffectController: this.StatusEffectController,
                receiverType: SkillStatusEffectReceiverType.Self,
                runtimeSkill: runtimeSkill,
                hitboxID: hitboxID,
                targetID: hitTargetID
            );
        }
        public void ApplySkillStatusEffectsToTargetOnHitConfirmed(RuntimeSkill runtimeSkill, CharacterRootController characterTarget, GameObject hitboxGameObject)
        {
            if (runtimeSkill == null) return;
            if (characterTarget == null) return;
            if (hitboxGameObject == null) return;

            // Apply to TARGET
            StatusEffectController receiverStatusEffectController = characterTarget.StatusEffectController;

            if (receiverStatusEffectController == null) return;

            int hitboxId = hitboxGameObject.GetInstanceID();
            int targetId = characterTarget.GetInstanceID();

            this.ApplyToReceiverOnHitConfirmed
            (
                receiverStatusEffectController: receiverStatusEffectController,
                receiverType: SkillStatusEffectReceiverType.Target,
                runtimeSkill: runtimeSkill,
                hitboxID: hitboxId,
                targetID: targetId
            );

            // Apply to SELF
            this.ApplyToReceiverOnHitConfirmed
            (
                receiverStatusEffectController: this.StatusEffectController,
                receiverType: SkillStatusEffectReceiverType.Self,
                runtimeSkill: runtimeSkill,
                hitboxID: hitboxId,
                targetID: targetId
            );
        }

        //      3.1. Clear tracking for a specific hitbox 
        public void ClearTrackingForHitbox(GameObject hitboxGameObject)
        {
            if (hitboxGameObject == null) return;
            int id = hitboxGameObject.GetInstanceID();
            _firstHitAppliedMap.Remove(id);
            _oncePerTargetMap.Remove(id);
        }

        //  4. Refresh for respawn: Clear all tracking maps
        public void ClearRuntimeTrackingAfterCharacterRefreshment()
        {
            _firstHitAppliedMap.Clear();
            _oncePerTargetMap.Clear();
        }

        // --- Helpers ---
        private void ApplyToReceiverOnHitConfirmed(StatusEffectController receiverStatusEffectController, SkillStatusEffectReceiverType receiverType, RuntimeSkill runtimeSkill, int hitboxID, int targetID)
        {
            if (receiverStatusEffectController == null) return;
            if (runtimeSkill == null) return;

            foreach (var application in runtimeSkill.EnumerateStatusEffectApplications(SkillStatusEffectTriggerType.OnHitConfirmed, receiverType))
            {
                if (application == null || !application.IsValid) continue;

                // Limits
                if (!this.PassesLimit(application.Limit, receiverType, application.StatusEffect, hitboxID, targetID)) continue;

                // Chance
                float chance = Mathf.Clamp01(application.Chance);
                if (chance < 1f && Random.value > chance) continue;

                // Apply
                receiverStatusEffectController.ApplyStatusEffect(application.StatusEffect, this.CharacterRootController.gameObject, application.Potency);
            }
        }   
        private bool PassesLimit(SkillStatusEffectApplicationLimitType limit, SkillStatusEffectReceiverType receiverType, StatusEffectProfile statusEffect, int hitboxID, int targetID)
        {
            if (statusEffect == null) return false;

            switch (limit)
            {
                case SkillStatusEffectApplicationLimitType.EveryValidHit:
                    return true;

                case SkillStatusEffectApplicationLimitType.FirstHitOnly:
                    return this.TryRegisterFirstHitApplication(hitboxID, receiverType, statusEffect);

                case SkillStatusEffectApplicationLimitType.OncePerTargetPerAttack:
                    return this.TryRegisterOncePerTargetApplication(hitboxID, receiverType, targetID, statusEffect);

                default:
                    Debug.LogWarning
                    (
                        $"[{this.gameObject.name}." +
                        $"{nameof(CharacterCombatStatusEffectApplicationHandler)}] " +
                        $"{nameof(PassesLimit)}: " +
                        $"Unhandled limit type {limit}.",
                        this
                    );
                    return false;
            }
        }
        private bool TryRegisterFirstHitApplication(int hitboxID, SkillStatusEffectReceiverType receiverType, StatusEffectProfile statusEffect)
        {
            if (!this.FirstHitAppliedMap.TryGetValue(hitboxID, out Dictionary<SkillStatusEffectReceiverType, HashSet<StatusEffectProfile>> receiverMap))
            {
                receiverMap = new Dictionary<SkillStatusEffectReceiverType, HashSet<StatusEffectProfile>>();
                _firstHitAppliedMap[hitboxID] = receiverMap;
            }

            if (!receiverMap.TryGetValue(receiverType, out HashSet<StatusEffectProfile> appliedSet))
            {
                appliedSet = new HashSet<StatusEffectProfile>();
                receiverMap[receiverType] = appliedSet;
            }

            return appliedSet.Add(statusEffect);
        }

        private bool TryRegisterOncePerTargetApplication(int hitboxID, SkillStatusEffectReceiverType receiverType, int targetID, StatusEffectProfile statusEffect)
        {
            if (!this.OncePerTargetMap.TryGetValue(hitboxID, out Dictionary<SkillStatusEffectReceiverType, Dictionary<int, HashSet<StatusEffectProfile>>> receiverMap))
            {
                receiverMap = new Dictionary<SkillStatusEffectReceiverType, Dictionary<int, HashSet<StatusEffectProfile>>>();
                _oncePerTargetMap[hitboxID] = receiverMap;
            }

            if (!receiverMap.TryGetValue(receiverType, out Dictionary<int, HashSet<StatusEffectProfile>> targetMap))
            {
                targetMap = new Dictionary<int, HashSet<StatusEffectProfile>>();
                receiverMap[receiverType] = targetMap;
            }

            if (!targetMap.TryGetValue(targetID, out HashSet<StatusEffectProfile> appliedSet))
            {
                appliedSet = new HashSet<StatusEffectProfile>();
                targetMap[targetID] = appliedSet;
            }
            return appliedSet.Add(statusEffect);
        }
    }
}
