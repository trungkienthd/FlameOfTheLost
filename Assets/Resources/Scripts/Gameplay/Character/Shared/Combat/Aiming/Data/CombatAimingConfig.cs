using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "CbAim_New", menuName = "Data/FlameOfTheLost/Character/Shared/Combat/Aiming/Config/CombatAimingConfig")]
    public class CombatAimingConfig : ScriptableObject
    {
        [Header("Routing")]
        [SerializeField] private bool _aimingOverridesFacing = true;            // if true, aim decides Facing/DirX/DirY
        [SerializeField] private bool _lockFacingDuringActiveFrames = true;
        public bool AimingOverridesFacing => _aimingOverridesFacing;
        public bool LockFacingDuringActiveFrames => _lockFacingDuringActiveFrames;

        [Header("Snap & Dead Zones")]
        [SerializeField] private FacingSnapType _facingSnapType = FacingSnapType.FourWay;
        [SerializeField] private bool _eightWaySnap = false;                  // if true, 8-way snap is used instead of 4-way
        [SerializeField, Min(0f)] private float _deadzoneRadius = 0.1f;        // radius of dead zone for aiming input
        [SerializeField, Range(0f, 180f)] private float _comboTurnAllowanceDegree = 50f;
        public FacingSnapType FacingSnapType => _facingSnapType;
        public bool EightWaySnap => _eightWaySnap;
        public float DeadzoneRadius => _deadzoneRadius;
        public float ComboTurnAllowanceDegree => _comboTurnAllowanceDegree;

        [Header("Misc")]
        [SerializeField, Min(0f)] private float _maxAimVectorLength = 0f;
        public float MaxAimVectorLength => _maxAimVectorLength;
    }
}