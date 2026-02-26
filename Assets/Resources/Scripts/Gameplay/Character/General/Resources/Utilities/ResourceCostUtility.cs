using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class ResourceCostUtility
    {
        // cooldownCheck: () => true if the skill is currently on cooldown
        public static GateResult Check(ICharacterResourcesGateContext context, ResourceCost resourceCost)
        {
            // Compute cost (percent-of-max -> flat)
            var runtimeCost = ResourceCostUtility.GetRuntimeResourceCost(resourceCost, context.HP.MaxValue, context.FP.MaxValue, context.Stamina.MaxValue);

            // Silence check
            if (runtimeCost.IsSilencedBlocked && context.FP.IsSilenced) return ResourceCostUtility.GetRejectedGateResult(GateRejectType.Silenced, runtimeCost);

            // Stamina check: strict reserve + affordability
            if (runtimeCost.StaminaCost > 0)
            {
                if (context.Stamina.CurrentValue < runtimeCost.StaminaCost)
                    return ResourceCostUtility.GetRejectedGateResult(GateRejectType.NotEnoughStamina, runtimeCost);

                // Authoritative check (includes SE scaling + reserve enforcement inside StaminaController)
                if (!context.Stamina.CanSpendCost(runtimeCost.StaminaCost))
                    return ResourceCostUtility.GetRejectedGateResult(GateRejectType.ReserveWouldBreakMinStamina, runtimeCost);
            }

            // FP check: affordability
            if (runtimeCost.FPCost > 0 && context.FP.CurrentValue < runtimeCost.FPCost)
            {
                return ResourceCostUtility.GetRejectedGateResult(GateRejectType.NotEnoughFP, runtimeCost);
            }

            // HP check: affordability + self-kill rule
            if (runtimeCost.HPCost > 0)
            {
                if (context.HP.CurrentValue < runtimeCost.HPCost)
                    return ResourceCostUtility.GetRejectedGateResult(GateRejectType.NotEnoughHP, runtimeCost);

                int hpAfterCost = context.HP.CurrentValue - runtimeCost.HPCost;
                if (hpAfterCost <= 0 && !runtimeCost.AllowSelfKillWithHPCost)
                    return ResourceCostUtility.GetRejectedGateResult(GateRejectType.WouldSelfKill, runtimeCost);
            }

            return ResourceCostUtility.GetAcceptedGateResult(runtimeCost);
        }

        public static GateResult GetAcceptedGateResult(RuntimeResourceCost runtimeCost)
        {
            return new GateResult
            {
                IsAccepted = true,
                RejectType = GateRejectType.None,
                RuntimeCost = runtimeCost
            };
        }

        public static GateResult GetRejectedGateResult(GateRejectType rejectType, RuntimeResourceCost runtimeCost = default)
        {
            return new GateResult
            {
                IsAccepted = false,
                RejectType = rejectType,
                RuntimeCost = runtimeCost
            };
        }

        public static RuntimeResourceCost GetRuntimeResourceCost(ResourceCost baseCost, int maxHP, int maxFP, int maxStamina)
        {
            // Convert percent-of-max into integer costs (ceil so small % still costs at least 1 when >0).
            int hpFromPercentOfMax      = baseCost.HPPercentOfMaxCost       > 0f ? Mathf.CeilToInt(baseCost.HPPercentOfMaxCost      * maxHP)        : 0;
            int fpFromPercentOfMax      = baseCost.FPPercentOfMaxCost       > 0f ? Mathf.CeilToInt(baseCost.FPPercentOfMaxCost      * maxFP)        : 0;
            int staminaFromPercentOfMax = baseCost.StaminaPercentOfMaxCost  > 0f ? Mathf.CeilToInt(baseCost.StaminaPercentOfMaxCost * maxStamina)   : 0;

            return new RuntimeResourceCost
            {
                HPCost      = Mathf.Clamp(baseCost.HPFlatCost       + hpFromPercentOfMax,       0, maxHP),
                FPCost      = Mathf.Clamp(baseCost.FPFlatCost       + fpFromPercentOfMax,       0, maxFP),
                StaminaCost = Mathf.Clamp(baseCost.StaminaFlatCost  + staminaFromPercentOfMax,  0, maxStamina),

                CostTriggerType = baseCost.CostTriggerType,

                IsSilencedBlocked = baseCost.IsSilenceBlocked,
                AllowSelfKillWithHPCost = baseCost.AllowSelfKillWithHPCost
            };
        }

    }
}
