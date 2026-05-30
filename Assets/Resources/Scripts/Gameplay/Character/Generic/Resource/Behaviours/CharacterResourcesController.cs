using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(HPController))]
    [RequireComponent(typeof(FPController))]
    [RequireComponent(typeof(StaminaController))]
    public class CharacterResourcesController : CharacterBehaviourBaseController, 
        ICharacterResourcesGateContext,
        ICharacterRespawnRefreshable
    {
        // --- States ---
        //  1. References
        public HPController HP => this.CharacterRootController.HPController;
        public FPController FP => this.CharacterRootController.FPController;
        public StaminaController Stamina => this.CharacterRootController.StaminaController;

        //  2. Runtime Reservations
        private int _nextReservationId = 1;
        private int NextReservationID
        {
            get => _nextReservationId;
            set => _nextReservationId = value;
        }

        private readonly Dictionary<int, ResourceReservationRecord> _reservations = new Dictionary<int, ResourceReservationRecord>(32);
        public Dictionary<int, ResourceReservationRecord> ReservationRecords => _reservations;

        //  3. ICharacterRespawnRefreshable implementation: Respawn Refresh
        public int RespawnRefreshPriority => 10;

        // --- Public APIs ---
        //  1. Gate + spend in one call for OnStart costs. If cost.CostType is OnHit/OnEnd, this will reserve (spend) instead of only spending at end.
        public GateResult TryPayCost(ResourceCost baseCost, out int reservationID)
        {
            reservationID = -1;

            var gate = ResourceCostUtility.Check(this, baseCost);
            if (!gate.IsAccepted) return gate;

            var runtimeCost = gate.RuntimeCost;

            switch (runtimeCost.CostTriggerType)
            {
                case ResourceCostTriggerType.OnAttackStart:
                    if (!this.TrySpendCost(runtimeCost))
                    {
                        return new GateResult
                        {
                            IsAccepted = false,
                            RejectType = GateRejectType.Unknown,
                            RuntimeCost = runtimeCost,
                        };
                    }
                    break;
                case ResourceCostTriggerType.OnHitConfirmed:
                case ResourceCostTriggerType.OnAttackEnd:
                    // strict reserve model
                    if (!this.TryReserveCost(runtimeCost, out reservationID))
                        return new GateResult
                        {
                            IsAccepted = false,
                            RejectType = GateRejectType.Unknown,
                            RuntimeCost = runtimeCost,
                        };
                    break;
            }

            return gate;
        }
        //      1a. Overload without reservation handle output
        public GateResult TryPayCost(ResourceCost baseCost) => this.TryPayCost(baseCost, out _);

        //  2. Get the Reserved Cost's record
        public bool TryGetReservationRecord(int reservationId, out ResourceReservationRecord record)
        {
            record = default;
            return this.ReservationRecords != null && this.ReservationRecords.TryGetValue(reservationId, out record);
        }

        //  3. Handle Reserved Cost (Cancel + Refund or Consume)
        //      3.1. Cancels a reservation and refunds based on ratio, then removes it. 
        public bool CancelReservedCost(int reservationID, float refundRatio = 1.0f)
        {
            if (reservationID < 0) return false;
            if (!this.ReservationRecords.TryGetValue(reservationID, out var record)) return false;

            this.RefundReservedCost(record, refundRatio);
            this.ReservationRecords.Remove(reservationID);
            return true;
        }

        //      3.2. Consumes a reservation without refund, then removes it. 
        public bool ConsumeReservedCost(int reservationID)
        {
            if (reservationID < 0) return false;
            return this.ReservationRecords.Remove(reservationID);
        }

        //  4. ICharacterRespawnRefreshable implementation: on respawn, clear all reservations (no refunds).
        public void RefreshAfterRespawn(CharacterRespawnRefreshContext context)
        {
            this.ClearAllResourceReservations();

            this.HP?.RefreshPoolAfterRespawn(context);
            this.FP?.RefreshPoolAfterRespawn(context);
            this.Stamina?.RefreshPoolAfterRespawn(context);
        }

        // --- Helpers ---
        //  1. Try to spend a cost immediately. (OnStart costs)
        private bool TrySpendCost(RuntimeResourceCost runtimeCost)
        {
            if (!runtimeCost.HasAnyCost) return true;

            if (runtimeCost.StaminaCost > 0 && !this.Stamina.CanSpendCost(runtimeCost.StaminaCost)) return false;
            if (runtimeCost.FPCost > 0      && !this.FP.CanSpendCost(runtimeCost.FPCost))           return false;
            if (runtimeCost.HPCost > 0      && !this.HP.CanSpendCost(runtimeCost.HPCost))           return false;

            // Spend order: Stamina -> FP -> HP
            // (HP last because it's rare and can be self-kill sensitive.)
            if (runtimeCost.StaminaCost > 0)    this.Stamina.TrySpendCost(runtimeCost.StaminaCost);
            if (runtimeCost.FPCost > 0)         this.FP.TrySpendCost(runtimeCost.FPCost);
            if (runtimeCost.HPCost > 0)         this.HP.TrySpendCost(runtimeCost.HPCost);

            return true;
        }

        //  2. Strict reserve model: reserve now == spend now, later it may refund if the skill cancels.
        private bool TryReserveCost(RuntimeResourceCost runtimeCost, out int reservationID)
        {
            reservationID = -1;
            if (!runtimeCost.HasAnyCost) return true;
            if (!this.TrySpendCost(runtimeCost)) return false;

            reservationID = this.NextReservationID++;
            this.ReservationRecords[reservationID] = new ResourceReservationRecord
            {
                ReservedCost = runtimeCost,
                CreatedFrame = Time.frameCount,
            };
            return true;

        }

        //  3. Refund a previously reserved/spent cost:
        //      refundRatio = 1 => full refund;
        //      0.5 => half refund;
        //      0 => none.
        private void RefundReservedCost(ResourceReservationRecord reservationRecord, float refundRatio = 1.0f)
        {
            var reservedCost = reservationRecord.ReservedCost;
            if (!reservedCost.HasAnyCost) return;

            refundRatio = Mathf.Clamp01(refundRatio);

            int staminaRefund = Mathf.RoundToInt(reservedCost.StaminaCost * refundRatio);
            int fpRefund = Mathf.RoundToInt(reservedCost.FPCost * refundRatio);
            int hpRefund = Mathf.RoundToInt(reservedCost.HPCost * refundRatio);

            if (staminaRefund > 0 && this.Stamina != null) this.Stamina.Restore(staminaRefund);
            if (fpRefund > 0 && this.FP != null) this.FP.Restore(fpRefund);
            if (hpRefund > 0 && this.HP != null) this.HP.Restore(hpRefund);
        }

        //  4. Clear all reservations.
        private void ClearAllResourceReservations()
        {
            this.ReservationRecords.Clear();
            this.NextReservationID = 1;
        }
    }
}
