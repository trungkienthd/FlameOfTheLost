using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterAITargetDetectionHandler : CharacterBehaviourControllerBase
    {
        private CharacterFaction Faction => this.CharacterRootController.IdentityController.Faction;

        private readonly HashSet<CharacterRootController> _alertTargets = new();
        private readonly HashSet<CharacterRootController> _attackRangeTargets = new();
        public bool HasAlertTarget => this.CurrentAlertTarget != null;
        public bool HasAttackRangeTarget => this.CurrentAttackRangeTarget != null;

        public CharacterRootController CurrentAlertTarget => this.ResolveNearestValidTarget(_alertTargets);
        public CharacterRootController CurrentAttackRangeTarget => this.ResolveNearestValidTarget(_attackRangeTargets);

        public CharacterRootController CurrentTarget
        {
            get
            {
                if (this.CurrentAttackRangeTarget != null) return this.CurrentAttackRangeTarget;
                return this.CurrentAlertTarget;
            }
        }

        public Transform CurrentTargetTransform => this.CurrentTarget != null
            ? this.CurrentTarget.transform
            : null;

        public void RegisterTarget(CharacterRootController target, CharacterAITargetDetectionRangeType rangeType)
        {
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

        private CharacterRootController ResolveNearestValidTarget(HashSet<CharacterRootController> targets)
        {
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
            if (target == null) return false;
            if (target == this.CharacterRootController) return false;

            return CharacterAITargetValidationUtility.CanTarget(this.Faction, target.IdentityController.Faction);
        }
    }
}
