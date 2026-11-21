using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Cbt_Character", menuName = "Data/FlameOfTheLost/Character/Shared/Combat/CharacterCombatConfig")]
    public class CharacterCombatConfig : CharacterData
    {
        [Header("Aiming - Routing")]
        [SerializeField] private bool _aimingOverridesFacing = true;
        [SerializeField] private bool _lockFacingDuringActiveFrames = true;
        public bool AimingOverridesFacing => _aimingOverridesFacing;
        public bool LockFacingDuringActiveFrames => _lockFacingDuringActiveFrames;

        [Header("Aiming - Snap & Dead Zones")]
        [SerializeField] private FacingSnapType _facingSnapType = FacingSnapType.FourWay;
        [SerializeField] private bool _eightWaySnap = false;
        [SerializeField, Min(0f)] private float _deadzoneRadius = 0.1f;
        [SerializeField, Range(0f, 180f)] private float _comboTurnAllowanceDegree = 50f;
        public FacingSnapType FacingSnapType => _facingSnapType;
        public bool EightWaySnap => _eightWaySnap;
        public float DeadzoneRadius => _deadzoneRadius;
        public float ComboTurnAllowanceDegree => _comboTurnAllowanceDegree;

        [Header("Aiming - Misc")]
        [SerializeField, Min(0f)] private float _maxAimVectorLength = 0;
        public float MaxAimVectorLength => _maxAimVectorLength;
    }
}
