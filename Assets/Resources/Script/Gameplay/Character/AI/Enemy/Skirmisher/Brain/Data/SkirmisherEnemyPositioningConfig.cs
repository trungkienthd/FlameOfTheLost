using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Pos_Variant_Chr_AI_Enemy_Sks_New", menuName = "Data/FlameOfTheLost/Character/AI/Enemy/Skirmisher/PositioningConfig")]
    public class SkirmisherEnemyPositioningConfig : CharacterData
    {
        //  1. Combat Distance
        [Header("Combat Distance")]
        [SerializeField, Min(0f)] private float _preferredCombatDistance = 1.1f;
        [SerializeField, Min(0f)] private float _combatDistanceTolerance = 0.3f;
        public float PreferredCombatDistance => _preferredCombatDistance;
        public float CombatDistanceTolerance => _combatDistanceTolerance;

        public float MinimumCombatDistance => Mathf.Max(0f, this.PreferredCombatDistance - this.CombatDistanceTolerance);
        public float MaximumCombatDistance => this.PreferredCombatDistance + this.CombatDistanceTolerance;

        //   2. Strafing
        [Header("Strafing")]
        [SerializeField, Min(0f)] private float _minStrafeDuration = 0.5f;
        [SerializeField, Min(0f)] private float _maxStrafeDuration = 1.5f;
        [SerializeField, Min(0f)] private float _strafeSpeedMultiplier = 0.75f;
        [SerializeField, Range(0f, 1f)] private float _directionChangeChance = 0.3f;
        public float MinStrafeDuration => _minStrafeDuration;
        public float MaxStrafeDuration => _maxStrafeDuration;
        public float StrafeSpeedMultiplier => _strafeSpeedMultiplier;
        public float DirectionChangeChance => _directionChangeChance;

        //  3. Tactical Movement Weights
        [Header("Tactical Movement Weights")]
        [SerializeField, Min(0f)] private float _strafeWeight = 1f;
        [SerializeField, Min(0f)] private float _approachWeight = 1f;
        [SerializeField, Min(0f)] private float _withdrawWeight = 1f;
        public float StrafeWeight => _strafeWeight;
        public float ApproachWeight => _approachWeight;
        public float WithdrawWeight => _withdrawWeight;

        public float GetRandomStrafeDuration()
        {
            return Random.Range(this.MinStrafeDuration, Mathf.Max(this.MinStrafeDuration, this.MaxStrafeDuration));
        }
    }
}