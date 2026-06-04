using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterCombatResourcesHandler : CharacterBehaviourBaseController
    {
        // --- States ---
        //  1. References
        public CharacterResourcesController ResourcesController => this.CharacterRootController.ResourcesController;

        //  2. Active reservation IDs
        private readonly List<int> _activeReservationIDs = new();
        private List<int> ActiveReservationIDs => _activeReservationIDs;

        // --- Public APIs ---
        //  0. Pay Cost Immediately
        public bool TryPayImmediateCost(ResourceCost resourceCost)
        {
            if (this.ResourcesController == null) return false;
            if (!resourceCost.HasAnyCost) return true;

            GateResult payResult = this.ResourcesController.TryPayCost(resourceCost, out int reservationId);
            if (!payResult.IsAccepted)
            {
                Debug.Log($"[{this.gameObject.name}.CharacterCombatResourcesHandler] Immediate cost is rejected: {payResult.RejectType}");
                return false;
            }

            if (reservationId >= 1)
            {
                this.ResourcesController.ConsumeReservedCost(reservationId);
            }

            return true;
        }

        //  1. Runs the resource gate and pays up-front according to CostTriggerType:
        //      OnAttackStart: pay immediately
        //      OnAttackEnd/OnHitConfirmed: strict reserve model => reserve now, allow refund if cast fails/cancels
        public bool TryPaySkillCost(RuntimeSkill runtimeSkill, out int reservationID)
        {
            reservationID = -1;

            if (this.ResourcesController == null || runtimeSkill == null) return false;

            GateResult payResult = this.ResourcesController.TryPayCost(runtimeSkill.ResourceCost, out reservationID);
            if (!payResult.IsAccepted)
            {
                Debug.Log($"[{this.gameObject.name}.CharacterCombatResourcesHandler] Cost is Rejected: {payResult.RejectType.ToString()}");
                reservationID = -1;
                return false;
            }

            // Track only real reservations
            if (reservationID >= 1) this.ActiveReservationIDs.Add(reservationID);

            return true;
        }

        //  2. Reservation Resolution
        //      2.1.1. Attack Window End resolution
        public void ResolveOnAttackWindowEnd(SkillStyleType skillStyle)
        {
            if (this.ResourcesController == null) 
            {
                this.ClearAttackReservations();
                return; 
            }

            // Iterate backwards because we may remove items.
            for (int i = this.ActiveReservationIDs.Count - 1; i >= 0; i--)
            {
                int reservationID = this.ActiveReservationIDs[i];
                if (!this.ResourcesController.TryGetReservationRecord(reservationID, out var record))
                {
                    // Already resolved elsewhere
                    this.ActiveReservationIDs.RemoveAt(i);
                    continue;
                }

                switch (record.ReservedCost.CostTriggerType)
                {
                    case ResourceCostTriggerType.OnAttackEnd:
                        // Always consume on smooth window end.
                        this.ResourcesController.ConsumeReservedCost(reservationID);
                        this.ActiveReservationIDs.RemoveAt(i);
                        break;

                    case ResourceCostTriggerType.OnHitConfirmed:
                        switch (skillStyle)
                        {
                            case SkillStyleType.Melee:
                                // Your rule: for melee, if still here at End -> no hit confirmed -> Consume.
                                // (If a hit confirmed earlier, it should have been removed already.)
                                this.ResourcesController.ConsumeReservedCost(reservationID);
                                this.ActiveReservationIDs.RemoveAt(i);
                                break;
                            case SkillStyleType.Ranged:
                                // Ranged: projectile/hitbox decides later. Do nothing here.
                                break;
                            default:
                                Debug.LogWarning($"[{this.gameObject.name}.CharacterCombatResourcesHandler] Unknown SkillStyleType in OnAttackWindowEnd resolution. Consume reservation to avoid free casts.");
                                this.ResourcesController.ConsumeReservedCost(reservationID);
                                this.ActiveReservationIDs.RemoveAt(i);
                                break;
                        }
                        break;

                    default:
                        // Unknown trigger => safest is to consume to avoid free casts.
                        Debug.LogWarning($"[{this.gameObject.name}.CharacterCombatResourcesHandler] Unknown CostTriggerType. Consume reservation to avoid free casts.");
                        this.ResourcesController.ConsumeReservedCost(reservationID);
                        this.ActiveReservationIDs.RemoveAt(i);
                        break;
                }
            }
        }

        //      2.1.2. Attack Window Interrupt resolution
        public void ResolveOnAttackWindowInterrupt(SkillStyleType skillStyle)
        {
            if (this.ResourcesController == null)
            {
                this.ClearAttackReservations();
                return;
            }

            for (int i = this.ActiveReservationIDs.Count - 1; i >= 0; i--)
            {
                int reservationID = this.ActiveReservationIDs[i];

                if (!this.ResourcesController.TryGetReservationRecord(reservationID, out var record))
                {
                    this.ActiveReservationIDs.RemoveAt(i);
                    continue;
                }

                switch (record.ReservedCost.CostTriggerType)
                {
                    case ResourceCostTriggerType.OnAttackEnd:
                        switch (skillStyle)
                        {
                            case SkillStyleType.Melee:
                                // Melee: hitbox was enabled since WindowStart -> Consume
                                this.ResourcesController.ConsumeReservedCost(reservationID);
                                break;
                            case SkillStyleType.Ranged:
                                // Ranged: projectile never created -> Cancel + Refund
                                this.ResourcesController.CancelReservedCost(reservationID, 1f);
                                break;
                            default:
                                Debug.LogWarning($"[{this.gameObject.name}.CharacterCombatResourcesHandler] Unknown SkillStyleType in OnAttackWindowInterrupt resolution. Consume reservation to avoid free casts.");
                                this.ResourcesController.ConsumeReservedCost(reservationID);
                                break;
                        }
                        this.ActiveReservationIDs.RemoveAt(i);
                        break;

                    case ResourceCostTriggerType.OnHitConfirmed:
                        switch (skillStyle)
                        {
                            case SkillStyleType.Melee:
                                // Your rule: if still here at Interrupt -> no hit confirmed -> Consume
                                this.ResourcesController.ConsumeReservedCost(reservationID);
                                break;
                            case SkillStyleType.Ranged:
                                // Ranged: projectile never created -> Cancel + Refund
                                this.ResourcesController.CancelReservedCost(reservationID, 1f);
                                break;
                            default:
                                Debug.LogWarning($"[{this.gameObject.name}.CharacterCombatResourcesHandler] Unknown SkillStyleType in OnAttackWindowInterrupt resolution. Consume reservation to avoid free casts.");
                                this.ResourcesController.ConsumeReservedCost(reservationID);
                                break;
                        }
                        this.ActiveReservationIDs.RemoveAt(i);
                        break;

                    default:
                        // Unknown trigger => safest: cancel on interrupt for ranged, consume for melee.
                        Debug.LogWarning($"[{this.gameObject.name}.CharacterCombatResourcesHandler] Unknown CostTriggerType. Cancel reservation for Ranged, Consume for Melee to avoid free casts.");
                        
                        switch (skillStyle)
                        {
                            case SkillStyleType.Melee:
                                this.ResourcesController.ConsumeReservedCost(reservationID);
                                break;
                            case SkillStyleType.Ranged:
                                this.ResourcesController.CancelReservedCost(reservationID, 1f);
                                break;
                            default:
                                Debug.LogWarning($"[{this.gameObject.name}.CharacterCombatResourcesHandler] Unknown SkillStyleType in Unknown CostTriggerType resolution. Consume reservation to avoid free casts.");
                                this.ResourcesController.ConsumeReservedCost(reservationID);
                                break;
                        }

                        this.ActiveReservationIDs.RemoveAt(i);
                        break;
                }
            }
        }

        //      2.2.1. Resolved when a hit is confirmed by a hitbox (melee or projectile) (any OnHitConfirmed reservations)
        public void ResolveOnHitConfirmed(SkillStyleType skillStyle = SkillStyleType.Default)
        {
            if (this.ResourcesController == null)
            {
                this.ClearAttackReservations();
                return;
            }

            for (int i = this.ActiveReservationIDs.Count - 1; i >= 0; i--)
            {
                int reservationID = this.ActiveReservationIDs[i];

                if (!this.ResourcesController.TryGetReservationRecord(reservationID, out var record))
                {
                    this.ActiveReservationIDs.RemoveAt(i);
                    continue;
                }

                if (record.ReservedCost.CostTriggerType != ResourceCostTriggerType.OnHitConfirmed) continue;

                // Hit happened => consume for both melee and ranged.
                this.ResourcesController.ConsumeReservedCost(reservationID);
                this.ActiveReservationIDs.RemoveAt(i);
            }
        }

        //      2.2.2. Called when a hitbox is disabled without any confirmed hit (OnHitConfirmed reservations)
        //          This is the "miss / no collision / expired" path.
        //          Resolves any OnHitConfirmed reservations.
        //              - Melee: typically already handled by End/Interrupt logic (consume). We keep it safe: consume (to match your current melee rules).
        //              - Ranged: cancel + refund (projectile never hit).
        public void ResolveOnHitboxDisabledWithoutHit(SkillStyleType skillStyle = SkillStyleType.Default)
        {
            if (this.ResourcesController == null)
            {
                this.ClearAttackReservations();
                return;
            }

            for (int i = this.ActiveReservationIDs.Count - 1; i >= 0; i--)
            {
                int reservationID = this.ActiveReservationIDs[i];

                if (!this.ResourcesController.TryGetReservationRecord(reservationID, out var record))
                {
                    this.ActiveReservationIDs.RemoveAt(i);
                    continue;
                }

                if (record.ReservedCost.CostTriggerType != ResourceCostTriggerType.OnHitConfirmed) continue;

                switch (skillStyle)
                {
                    case SkillStyleType.Melee:
                        // Match your melee rule set: no hit by the time it ends => consume.
                        this.ResourcesController.ConsumeReservedCost(reservationID);
                        break;
                    case SkillStyleType.Ranged:
                        // Ranged miss => refund fully.
                        this.ResourcesController.CancelReservedCost(reservationID, 1f);
                        break;
                    default:
                        Debug.LogWarning($"[{this.gameObject.name}.CharacterCombatResourcesHandler] Unknown SkillStyleType in OnHitboxDisabledWithoutHit resolution. Consume reservation to avoid free casts.");
                        this.ResourcesController.ConsumeReservedCost(reservationID);
                        break;
                }

                this.ActiveReservationIDs.RemoveAt(i);
            }    

        }

        //  3. Refresh for Respawn: Clears all runtime reservation
        public void ClearCombatReservationsAfterRespawn()
        {
            this.ClearAttackReservations();
        }

        // --- Helpers ---
        //  1. Clears all runtime reservation tracking for the current cast/attack. Hardly used (ONLY when the whole cast/attack lifecycle is concluded)
        private void ClearAttackReservations()
        {
            this.ActiveReservationIDs.Clear();
        }
    }
}
