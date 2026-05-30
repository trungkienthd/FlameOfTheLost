using FlameOfTheLost;
using UnityEngine;

public class ToxicschroomBrainController : EnemyBrainController
{
    [Header("Default Roaming")]
    [SerializeField] private Vector2 _patrolMin;
    [SerializeField] private Vector2 _patrolMax;
    [SerializeField] private float _walkDurationMin = 1.0f;
    [SerializeField] private float _walkDurationMax = 2.5f;
    [SerializeField] private float _idleDurationMin = 0.8f;
    [SerializeField] private float _idleDurationMax = 1.8f;
    [SerializeField] private float _targetReachDistance = 0.15f;

    [Header("Attack Approach")]
    [SerializeField] private float _idealAttackRange = 1.2f;
    [SerializeField] private float _attackCooldownSeconds = 1.5f;
    [SerializeField] private AttackSide _normalAttackSide = AttackSide.Left;

    private CharacterCombatLoadoutController CombatLoadoutController => this.CharacterRootController.CombatLoadoutController;

    private void Start()
    {
        this.CombatLoadoutController?.TryQuickEquipWeapon();   
    }
}
