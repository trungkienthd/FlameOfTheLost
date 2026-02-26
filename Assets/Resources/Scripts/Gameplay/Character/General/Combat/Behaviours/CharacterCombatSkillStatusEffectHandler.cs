using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(StatusEffectController))]
    public class CharacterCombatSkillStatusEffectHandler : MonoBehaviour
    {
        // --- References
        [Header("References")]
        [SerializeField] private StatusEffectController _statusEffectController;
        public StatusEffectController StatusEffectController
        {
            get => _statusEffectController;
            private set => _statusEffectController = value;
        }

        // --- Limit tracking (per attack-instance = hitboxObject)
        // hitboxId -> firstHitApplied (for FirstHitOnly)
        private readonly Dictionary<int, HashSet<StatusEffectProfile>> _firstHitAppliedMap = new();
        private Dictionary<int, HashSet<StatusEffectProfile>> FirstHitAppliedMap => _firstHitAppliedMap;

        // hitboxId -> (targetInstanceId -> applied profiles) (for OncePerTargetPerAttack)
        private readonly Dictionary<int, Dictionary<int, HashSet<StatusEffectProfile>>> _oncePerTargetMap = new();
        private Dictionary<int, Dictionary<int, HashSet<StatusEffectProfile>>> OncePerTargetMap => _oncePerTargetMap;

        // --- Unity methods
        private void Awake()
        {
            if (this.StatusEffectController == null) this.StatusEffectController = this.GetComponent<StatusEffectController>();
        }

        // --- Public APIs
        //  1. Apply to Self (OnAttackStart, OnAttackEnd)
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

        //  2. Apply when the hit is confirmed (OnHitConfirmed)
        public void ApplySkillStatusEffectsOnHitConfirmed(RuntimeSkill runtimeSkill, GameObject targetGameObject, GameObject hitboxGameObject)
        {
            if (runtimeSkill == null) return;
            if (targetGameObject == null) return;
            if (hitboxGameObject == null) return;

            int hitboxId = hitboxGameObject.GetInstanceID();
            int targetId = targetGameObject.GetInstanceID();

            // Apply to TARGET
            var receiver = targetGameObject.GetComponentInParent<StatusEffectController>() ?? targetGameObject.GetComponentInChildren<StatusEffectController>(true);
            this.ApplyToReceiverOnHitConfirmed
            (
                receiverStatusEffectController: receiver,
                receiverType: SkillStatusEffectReceiverType.Target,
                runtimeSkill: runtimeSkill,
                hitboxId: hitboxId,
                targetId: targetId
            );

            // Apply to SELF
            this.ApplyToReceiverOnHitConfirmed
            (
                receiverStatusEffectController: this.StatusEffectController,
                receiverType: SkillStatusEffectReceiverType.Self,
                runtimeSkill: runtimeSkill,
                hitboxId: hitboxId,
                targetId: targetId
            );
        }

        //  3. Clear tracking for a specific hitbox 
        public void ClearTrackingForHitbox(GameObject hitboxGameObject)
        {
            if (hitboxGameObject == null) return;
            int id = hitboxGameObject.GetInstanceID();
            this.FirstHitAppliedMap.Remove(id);
            this.OncePerTargetMap.Remove(id);
        }

        // --- Helpers
        private void ApplyToReceiverOnHitConfirmed(StatusEffectController receiverStatusEffectController, SkillStatusEffectReceiverType receiverType, RuntimeSkill runtimeSkill, int hitboxId, int targetId)
        {
            if (receiverStatusEffectController == null) return;

            foreach (var application in runtimeSkill.EnumerateStatusEffectApplications(SkillStatusEffectTriggerType.OnHitConfirmed, receiverType))
            {
                if (application == null || !application.IsValid) continue;

                // Limits
                if (!this.PassesLimit(application.Limit, application.StatusEffect, hitboxId, targetId)) continue;

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
                        this.FirstHitAppliedMap[hitboxID] = firstHitAppliedSet;
                    }

                    // FirstHitOnly can still mean "first time for this statusEffect". This lets multiple FirstHitOnly effects each apply once.
                    if (firstHitAppliedSet.Contains(statusEffect)) return false;

                    firstHitAppliedSet.Add(statusEffect);
                    return true;

                case SkillStatusEffectApplicationLimitType.OncePerTargetPerAttack:
                    if (!this.OncePerTargetMap.TryGetValue(hitboxID, out var perTarget))
                    {
                        perTarget = new Dictionary<int, HashSet<StatusEffectProfile>>();
                        this.OncePerTargetMap[hitboxID] = perTarget;
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
                    Debug.LogWarning($"[{this.gameObject.name}.CharacterCombatSkillStatusEffectHandler] PassesLimit: Unhandled limit type {limit}");
                    return false;
            }
        }
    }
}
