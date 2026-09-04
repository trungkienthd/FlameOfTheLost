using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterAITargetDetectionHandler : CharacterBehaviourControllerBase,
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- State ---
        //  1. Faction
        private CharacterFaction Faction => this.CharacterRootController.Faction;

        //  2. Detected Targets
        private readonly HashSet<CharacterRootController> _alertTargets = new();
        private readonly HashSet<CharacterRootController> _attackRangeTargets = new();
        public bool HasAlertTarget => this.CurrentAlertTarget != null;
        public bool HasAttackRangeTarget => this.CurrentAttackRangeTarget != null;

        public CharacterRootController CurrentAlertTarget => this.ResolveNearestValidTarget(_alertTargets);
        public CharacterRootController CurrentAttackRangeTarget => this.ResolveNearestValidTarget(_attackRangeTargets);

        //  3. References - Current Target
        public CharacterRootController CurrentTarget
        {
            get
            {
                if (this.IsDeathDisabled) return null;
                if (this.CurrentAttackRangeTarget != null) return this.CurrentAttackRangeTarget;
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
            }
        }
        public void ClearAllTargets()
        {
            _alertTargets.Clear();
            _attackRangeTargets.Clear();
        }

        //  2. ICharacterRefreshable Implementation
        public int RefreshmentConfigurePriority => 85;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.IsDeathDisabled = false;
            this.ClearAllTargets();
        }

        //  3. ICharacterDeathDisableable Implementation
        public int DeathDisablePriority => 5;
        public void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;
            this.ClearAllTargets();
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
