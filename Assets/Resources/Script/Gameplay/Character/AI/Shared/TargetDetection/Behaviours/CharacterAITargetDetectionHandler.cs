using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterAITargetDetectionHandler : CharacterBehaviourControllerBase,
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- Runtime ---
        //  1. Faction
        private CharacterFaction Faction => this.CharacterRootController.Faction;

        //  2. Detected Targets
        private readonly HashSet<CharacterRootController> _alertTargets = new();
        private readonly HashSet<CharacterRootController> _attackRangeTargets = new();
        private readonly HashSet<CharacterRootController> _fleeRangeTargets = new();
        public bool HasAlertTarget          => this.CurrentAlertTarget != null;
        public bool HasAttackRangeTarget    => this.CurrentAttackRangeTarget != null;
        public bool HasFleeRangeTarget      => this.CurrentFleeRangeTarget != null;

        public CharacterRootController CurrentAlertTarget       => this.ResolveNearestValidTarget(_alertTargets);
        public CharacterRootController CurrentAttackRangeTarget => this.ResolveNearestValidTarget(_attackRangeTargets);
        public CharacterRootController CurrentFleeRangeTarget   => this.ResolveNearestValidTarget(_fleeRangeTargets);

        //  3. References - Current Target
        public CharacterRootController CurrentTarget
        {
            get
            {
                if (this.IsDeathDisabled) return null;

                if (this.CurrentAttackRangeTarget   != null) return this.CurrentAttackRangeTarget;
                if (this.CurrentFleeRangeTarget     != null) return this.CurrentFleeRangeTarget;

                return this.CurrentAlertTarget;
            }
        }
        public Transform CurrentTargetTransform => this.CurrentTarget != null
            ? this.CurrentTarget.transform
            : null;

        //  4. Death Disable State (Runtime)
        [Header("Death Disable (Runtime)")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled
        {
            get => _isDeathDisabled;
            private set => _isDeathDisabled = value;
        }

        // --- Public APIs ---
        //  0.. Interface implementation
        //      0.1. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 85;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.IsDeathDisabled = false;
            this.ClearAllTargets();
        }

        //      0.2. ICharacterDeathDisableable implementation
        public int DeathDisablePriority => 5;
        public void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;
            this.ClearAllTargets();
        }

        //  1. Target Registration
        public void RegisterTarget(CharacterRootController target, CharacterAITargetDetectionRangeType rangeType)
        {
            if (this.IsDeathDisabled) return;
            if (!this.IsValidTarget(target)) return;

            switch (rangeType)
            {
                case CharacterAITargetDetectionRangeType.Alert:
                    _alertTargets.Add(target);
                    break;

                case CharacterAITargetDetectionRangeType.AttackRange:
                    _attackRangeTargets.Add(target);
                    break;

                case CharacterAITargetDetectionRangeType.FleeRange:
                    _fleeRangeTargets.Add(target);
                    break;
            }
        }
        public void UnregisterTarget(CharacterRootController target, CharacterAITargetDetectionRangeType rangeType)
        {
            if (target == null) return;

            switch (rangeType)
            {
                case CharacterAITargetDetectionRangeType.Alert:
                    _alertTargets.Remove(target);
                    break;

                case CharacterAITargetDetectionRangeType.AttackRange:
                    _attackRangeTargets.Remove(target);
                    break;

                case CharacterAITargetDetectionRangeType.FleeRange:
                    _fleeRangeTargets.Remove(target);
                    break;
            }
        }
        public void ClearAllTargets()
        {
            _alertTargets.Clear();
            _attackRangeTargets.Clear();
            _fleeRangeTargets.Clear();
        }

        //  2. Validation
        public bool IsTargetDetected(CharacterRootController target)
        {
            if (this.IsDeathDisabled) return false;
            if (!this.IsValidTarget(target)) return false;

            return _alertTargets.Contains(target) || _attackRangeTargets.Contains(target) || _fleeRangeTargets.Contains(target);
        }

        // --- Helpers ---
        private CharacterRootController ResolveNearestValidTarget(HashSet<CharacterRootController> targets)
        {
            if (this.IsDeathDisabled) return null;

            CharacterRootController nearestTarget = null;
            float nearestSqrDistance = float.MaxValue;

            Vector2 selfPosition = this.CharacterRootController.transform.position;

            foreach (CharacterRootController target in targets)
            {
                if (!this.IsValidTarget(target)) continue;

                float sqrDistance = ((Vector2)target.transform.position - selfPosition).sqrMagnitude;

                if (sqrDistance < nearestSqrDistance)
                {
                    nearestSqrDistance = sqrDistance;
                    nearestTarget = target;
                }
            }

            return nearestTarget;
        }

        private bool IsValidTarget(CharacterRootController target)
        {
            if (this.IsDeathDisabled) return false;
            if (target == null) return false;
            if (target == this.CharacterRootController) return false;

            return CharacterAITargetValidationUtility.CanTarget(this.Faction, target.Faction);
        }
    }
}
