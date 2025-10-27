using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "InputConfig", menuName = "FlameOfTheLost/Game/Input/InputConfig")]
    public class InputConfig : ScriptableObject
    {
        [Header("Aiming")]
        [SerializeField] private bool _mouseAimingOverridesFacing = true;
        [SerializeField] private bool _lockFacingDuringActiveFrames = true;
        [SerializeField] private bool _eightWaySnap = false;
        [SerializeField, Range(0.01f, 1f)] private float _cursorDeadRadius = 0.15f;
        [SerializeField, Range(0f, 45f)] private float _comboTurnAllowanceDeg = 20f;
        public bool MouseAimingOverridesFacing => _mouseAimingOverridesFacing;
        public bool LockFacingDuringActiveFrames => _lockFacingDuringActiveFrames;
        public bool EightWaySnap => _eightWaySnap;
        public float CursorDeadRadius => _cursorDeadRadius;
        public float ComboTurnAllowanceDeg => _comboTurnAllowanceDeg;

        [Header("Modifiers")]
        [SerializeField] private KeyCode _faceLockKey = KeyCode.LeftShift;
        public KeyCode FaceLockKey => _faceLockKey;
    }
}
