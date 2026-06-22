using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Ddg_New", menuName = "Data/FlameOfTheLost/Character/Shared/Dodging/CharacterDodgeConfig")]
    public class CharacterDodgeConfig : ScriptableObject
    {
        [Header("Timing")]
        [SerializeField] private float _dodgeDuration = 0.25f;
        [SerializeField] private float _recoveryDuration = 1f;
        public float DodgeDuration => _dodgeDuration;
        public float RecoveryDuration => _recoveryDuration;

        [Header("Movement")]
        [SerializeField] private float _dodgeSpeed = 10f;
        [SerializeField] private bool _useLastFacingDirectionWhenNoInput = true;
        public float DodgeSpeed => _dodgeSpeed;
        public bool UseLastFacingDirectionWhenNoInput => _useLastFacingDirectionWhenNoInput;

        [Header("Resource Cost")]
        [SerializeField, Min(0)] private int _staminaCost = 15;
        public int StaminaCost => _staminaCost;

        public ResourceCostEntry DodgeCost
        {
            get
            {
                return new ResourceCostEntry
                {
                    HPFlatCost = 0,
                    FPFlatCost = 0,
                    StaminaFlatCost = this.StaminaCost,

                    HPPercentOfMaxCost = 0f,
                    FPPercentOfMaxCost = 0f,
                    StaminaPercentOfMaxCost = 0f,

                    // Dodge is always paid immediately.
                    // No reservation. No refund.
                    CostTriggerType = ResourceCostTriggerType.OnAttackStart,

                    // Dodge should not be blocked by Silence.
                    IsSilenceBlocked = false,

                    // No HP cost, so irrelevant.
                    AllowSelfKillWithHPCost = false,
                };
            }
        }

        [Header("Hit Invulnerability")]
        [SerializeField] private bool _isHitInvulnerableDuringDodge = true;
        [SerializeField] private bool _isHitInvulnerableDuringRecovery = false;
        public bool IsHitInvulnerableDuringDodge => _isHitInvulnerableDuringDodge;
        public bool IsHitInvulnerableDuringRecovery => _isHitInvulnerableDuringRecovery;
    }
}
