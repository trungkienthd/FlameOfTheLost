using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Ddg_New", menuName = "Data/FlameOfTheLost/Character/Shared/Dodging/CharacterDodgeConfig")]
    public class CharacterDodgeConfig : ScriptableObject
    {
        [Header("Timing")]
        [SerializeField] private float _dodgeDuration = 0.25f;
        [SerializeField] private float _recoveryDuration = 0.15f;
        public float DodgeDuration => _dodgeDuration;
        public float RecoveryDuration => _recoveryDuration;

        [Header("Movement")]
        [SerializeField] private float _dodgeSpeed = 8f;
        [SerializeField] private bool _useLastFacingDirectionWhenNoInput = true;
        public float DodgeSpeed => _dodgeSpeed;
        public bool UseLastFacingDirectionWhenNoInput => _useLastFacingDirectionWhenNoInput;

        [Header("Hit Invulnerability")]
        [SerializeField] private bool _isHitInvulnerableDuringDodge = true;
        [SerializeField] private bool _isHitInvulnerableDuringRecovery = false;
        public bool IsHitInvulnerableDuringDodge => _isHitInvulnerableDuringDodge;
        public bool IsHitInvulnerableDuringRecovery => _isHitInvulnerableDuringRecovery;
    }
}
