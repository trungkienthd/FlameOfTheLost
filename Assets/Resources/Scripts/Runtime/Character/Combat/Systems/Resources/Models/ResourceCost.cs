using UnityEngine;

namespace FlameOfTheLost
{
    [System.Serializable]
    public struct ResourceCost                      // Authoring-time cost, on the skill asset
    {
        [SerializeField, Min(0f)] private int _stamina;
        [SerializeField, Min(0f)] private int _fp;

        public int Stamina
        {
            get => _stamina;
            set => _stamina = value;
        }
        public int FP
        {
            get => _fp;
            set => _fp = value;
        }

        [Tooltip("Optional extra cost as % of Max resources (0..1).")]
        [SerializeField, Range(0f, 1f)] private float _staminaPercentOfMax;
        [SerializeField, Range(0f, 1f)] private float _fpPercentOfMax;

        public float StaminaPercentOfMax
        {
            get => _staminaPercentOfMax;
            set => _staminaPercentOfMax = value;
        }
        public float FPPercentOfMax
        {
            get => _fpPercentOfMax;
            set => _fpPercentOfMax = value;
        }

        [SerializeField] private ResourceCostType _costMode;
        public ResourceCostType CostMode
        {
            get => _costMode;
            set => _costMode = value;
        }

        [Tooltip("If true, this skill is blocked by 'Silenced'.")] 
        [SerializeField] private bool _requiresFP;
        public bool RequiresFP
        {
            get => _requiresFP;
            set => _requiresFP = value;
        }

        [Tooltip("For channels only: cost per second while active.")]
        [SerializeField, Min(0)] private float _perSecondStamina;
        [SerializeField, Min(0)] private float _perSecondFP;

        public float PerSecondStamina
        {
            get => _perSecondStamina;
            set => _perSecondStamina = value;
        }
        public float PerSecondFP
        {
            get => _perSecondFP;
            set => _perSecondFP = value;
        }
    }
}
