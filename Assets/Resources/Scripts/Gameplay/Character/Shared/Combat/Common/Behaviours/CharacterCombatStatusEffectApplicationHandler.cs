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
        // hitboxId -> firstHitApplied (for FirstHitOnly)
        private readonly Dictionary<int, HashSet<StatusEffectProfile>> _firstHitAppliedMap = new();
        private IReadOnlyDictionary<int, HashSet<StatusEffectProfile>> FirstHitAppliedMap => _firstHitAppliedMap;

        // hitboxId -> (targetInstanceId -> applied profiles) (for OncePerTargetPerAttack)
        private readonly Dictionary<int, Dictionary<int, HashSet<StatusEffectProfile>>> _oncePerTargetMap = new();
        private IReadOnlyDictionary<int, Dictionary<int, HashSet<StatusEffectProfile>>> OncePerTargetMap => _oncePerTargetMap;

        // --- Public APIs ---
        //  1. Generic apply-to-self for non-skill systems (e.g. ConsumableItem, Terrain)
        public bool TryApplyStatusEffectApplicationToSelf(StatusEffectApplication application)
        {
            return this.TryApplyStatusEffectApplicationToSelf(application, this.gameObject);
        }
        public bool TryApplyStatusEffectApplicationToSelf(StatusEffectApplication application, GameObject sourceOverride)
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
        public int TryApplyStatusEffectApplicationsToSelf(IEnumerable<StatusEffectApplication> applications)
        {
            return this.TryApplyStatusEffectApplicationsToSelf(applications, this.gameObject);
        }
        public int TryApplyStatusEffectApplicationsToSelf(IEnumerable<StatusEffectApplication> applications, GameObject sourceOverride)
        {
            if (applications == null) return 0;
            if (this.StatusEffectController == null) return 0;

            int appliedCount = 0;

            foreach (StatusEffectApplication application in applications)
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

                this.StatusEffectController.ApplyStatusEffect(application.StatusEffect, this.gameObject, application.Potency);
            }
        }

        //  3. Apply when the hit is confirmed: SkillStatusEffectApplication (OnHitConfirmed)
        public void ApplySkillStatusEffectsOnHitConfirmed(RuntimeSkill runtimeSkill, CharacterRootController target, GameObject hitboxGameObject)
        {
            if (runtimeSkill == null) return;
            if (target == null) return;
            if (hitboxGameObject == null) return;

            int hitboxId = hitboxGameObject.GetInstanceID();
            int targetId = target.GetInstanceID();

            // Apply to TARGET
            var receiver = target.StatusEffectController;
            this.ApplyToReceiverOnHitConfirmed
            (
                receiverStatusEffectController: receiver,
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
        public void ClearRuntimeTrackingAfterCharacterPlacement()
        {
            _firstHitAppliedMap.Clear();
            _oncePerTargetMap.Clear();
        }

        // --- Helpers ---
        private void ApplyToReceiverOnHitConfirmed(StatusEffectController receiverStatusEffectController, SkillStatusEffectReceiverType receiverType, RuntimeSkill runtimeSkill, int hitboxID, int targetID)
        {
            if (receiverStatusEffectController == null) return;

            foreach (var application in runtimeSkill.EnumerateStatusEffectApplications(SkillStatusEffectTriggerType.OnHitConfirmed, receiverType))
            {
                if (application == null || !application.IsValid) continue;

                // Limits
                if (!this.PassesLimit(application.Limit, application.StatusEffect, hitboxID, targetID)) continue;

                // Chance
                float chance = Mathf.Clamp01(application.Chance);
                if (chance < 1f && Random.value > chance) continue;

                // Apply
                receiverStatusEffectController.ApplyStatusEffect(application.StatusEffect, this.gameObject, application.Potency);
            }
        }   
        private bool PassesLimit(SkillStatusEffectApplicationLimitType limit, StatusEffectProfile statusEffect, int hitboxID, int targetID)
        {
            if (statusEffect == null) return false;

            switch (limit)
            {
                case SkillStatusEffectApplicationLimitType.EveryValidHit:
                    return true;

                case SkillStatusEffectApplicationLimitType.FirstHitOnly:
                    if (!this.FirstHitAppliedMap.TryGetValue(hitboxID, out var firstHitAppliedSet))
                    { 
                        firstHitAppliedSet = new HashSet<StatusEffectProfile>();
                        _firstHitAppliedMap[hitboxID] = firstHitAppliedSet;
                    }

                    // FirstHitOnly can still mean "first time for this statusEffect". This lets multiple FirstHitOnly effects each apply once.
                    if (firstHitAppliedSet.Contains(statusEffect)) return false;

                    firstHitAppliedSet.Add(statusEffect);
                    return true;

                case SkillStatusEffectApplicationLimitType.OncePerTargetPerAttack:
                    if (!this.OncePerTargetMap.TryGetValue(hitboxID, out var perTarget))
                    {
                        perTarget = new Dictionary<int, HashSet<StatusEffectProfile>>();
                        _oncePerTargetMap[hitboxID] = perTarget;
                    }

                    if (!perTarget.TryGetValue(targetID, out var oncePerTargetSet))
                    {
                        oncePerTargetSet = new HashSet<StatusEffectProfile>();
                        perTarget[targetID] = oncePerTargetSet;
                    }

                    if (oncePerTargetSet.Contains(statusEffect)) return false;

                    oncePerTargetSet.Add(statusEffect);
                    return true;

                default:
                    Debug.LogWarning($"[{this.gameObject.name}.CharacterCombatStatusEffectHandler] PassesLimit: Unhandled limit type {limit}");
                    return false;
            }
        }
    }
}
