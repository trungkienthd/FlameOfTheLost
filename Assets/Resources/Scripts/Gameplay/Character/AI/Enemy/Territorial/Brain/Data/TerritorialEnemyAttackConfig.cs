using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "TEAttack_New", menuName = "Data/FlameOfTheLost/Character/Enemy/Territorial/AttackConfig")]
    public class TerritorialEnemyAttackConfig : ScriptableObject
    {
        [Header("Attack Range")]
        [SerializeField] private float _idealAttackRange = 1.4f;
        public float IdealAttackRange => _idealAttackRange;

        [Header("Attack Timing")]
        [SerializeField] private float _attackDecisionCooldown = 1.5f;
        [SerializeField] private float _attackingStateDuration = 0.7f;
        public float AttackDecisionCooldown => _attackDecisionCooldown;
        public float AttackingStateDuration => _attackingStateDuration;

        [Header("Attack Input")]
        [SerializeField] private AttackSide _normalAttackSide = AttackSide.Left;
        public AttackSide NormalAttackSide => _normalAttackSide;
    }
}
