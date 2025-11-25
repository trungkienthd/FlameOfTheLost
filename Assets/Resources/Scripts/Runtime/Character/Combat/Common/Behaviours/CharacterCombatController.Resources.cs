using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract partial class CharacterCombatController : MonoBehaviour
    {
        // Active cast ticket (for reserve/finalize/channel)
        private bool _hasActiveCast;
        private EffectiveResourceCost _activeCost;
        private ResourceCostType _activeCostMode;
        private bool _channeling;
        private float _lastPerSecondTickTime;
        protected bool HasActiveCast
        {
            get => _hasActiveCast;
            set => _hasActiveCast = value;
        }
        protected EffectiveResourceCost ActiveCost
        {
            get => _activeCost;
            set => _activeCost = value;
        }
        protected ref EffectiveResourceCost ActiveCostRef => ref _activeCost; // for in-place modification
        protected ResourceCostType ActiveCostMode
        {
            get => _activeCostMode;
            private set => _activeCostMode = value;
        }
        protected ref ResourceCostType ActiveCostModeRef => ref _activeCostMode; // for in-place modification
        protected bool Channeling
        {
            get => _channeling;
            set => _channeling = value;
        }
        protected ref bool ChannelingRef => ref _channeling; // for in-place modification
        protected float LastPerSecondTickTime
        {
            get => _lastPerSecondTickTime;
            private set => _lastPerSecondTickTime = value;
        }
        protected ref float LastPerSecondTickTimeRef => ref _lastPerSecondTickTime; // for in-place modification

        protected void NotifyGateFail(GateRejectStatus reason)
        {
            switch (reason)
            {
                case GateRejectStatus.OnCooldown:
                    Debug.Log("[CombatController] Blocked: On cooldown");
                    break;
                case GateRejectStatus.Silenced:
                    Debug.Log("[CombatController] Blocked: Silenced");
                    break;
                case GateRejectStatus.NotEnoughStamina:
                    Debug.Log("[CombatController] Blocked: Not enough Stamina");
                    break;
                case GateRejectStatus.NotEnoughFP:
                    Debug.Log("[CombatController] Blocked: Not enough FP");
                    break;
                case GateRejectStatus.ReserveWouldBreakMinStamina:
                    Debug.Log("[CombatController] Blocked: Min Stamina Reserve");
                    break;
                default:
                    Debug.Log("[CombatController] Blocked");
                    break;
            }
        }

        protected void ClearActiveCast()
        {
            this.HasActiveCast = false;
            this.ActiveCost = default;
            this.ActiveCostMode = ResourceCostType.OnStart;
            this.Channeling = false;
            this.LastPerSecondTickTime = 0f;
        }

        // Runs Silence/FP/Stamina gate and tries to reserve/spend up-front (Strict + Reserve).
        // Returns true if reservation succeeded and fills out params for the caller to store as its "active cast ticket".
        protected bool TryGateAndReserveSkill
        (
            SkillProfile skillProfile,
            Action<GateRejectStatus> onFail,
            out EffectiveResourceCost reservationCost,
            out ResourceCostType costMode,
            out bool isChannel,
            out float channelStartTime)
        {
            reservationCost = default;
            costMode = ResourceCostType.OnStart;
            isChannel = false;
            channelStartTime = 0f;

            if (this.CharacterResourcesController == null || skillProfile == null)
            {
                onFail?.Invoke(GateRejectStatus.Unknown);
                return false;
            }

            var gate = SkillResourceGateUtility.Check(this.CharacterResourcesController, skillProfile, cooldownCheck: null);
            if (!gate.Allowed)
            {
                onFail?.Invoke(gate.Reason);
                return false;
            }

            var effectiveCost = this.CharacterResourcesController.PreviewEffectiveResourceCost(skillProfile.ResourceCost);
            if (!this.CharacterResourcesController.TryReserveOrSpend(effectiveCost))
            {
                var freeStamina = this.CharacterResourcesController.CurrentStamina - this.CharacterResourcesController.MinStaminaReserve;
                onFail?.Invoke
                (
                    freeStamina < effectiveCost.Stamina
                        ? GateRejectStatus.ReserveWouldBreakMinStamina
                        : GateRejectStatus.NotEnoughFP
                );
                return false;
            }

            reservationCost = effectiveCost;
            costMode = effectiveCost.CostMode;
            isChannel = (effectiveCost.CostMode == ResourceCostType.PerSecond) && (effectiveCost.PerSecondStamina > 0f || effectiveCost.PerSecondFP > 0f);
            channelStartTime = Time.time;

            return true;
        }

        // Refunds a previously reserved/spent cost (100% by default). Safe to call even if zeroed.
        protected void RefundReservation(in EffectiveResourceCost reservationCost, float refundRatio = 1.0f)
        {
            if (this.CharacterResourcesController == null) return;

            this.CharacterResourcesController.Refund(reservationCost, refundRatio);
        }

        // For PER_SECOND channel skills: drains resources since the last tick.
        // Returns false if the drain failed (ran out), so caller can abort the cast gracefully.
        protected bool TryTickPerSecondDrain(in EffectiveResourceCost activeCost, ref float lastTickTime, float minInterval = 0.02f)
        {
            if (this.CharacterResourcesController == null) return false;

            float currentTime = Time.time;
            float dt = currentTime - lastTickTime;
            if (dt < minInterval) return true; // too soon to tick again

            if (!this.CharacterResourcesController.TryPayPerSecond(dt, activeCost.PerSecondStamina, activeCost.PerSecondFP)) return false;

            lastTickTime = currentTime;
            return true;
        }
    }  
}
