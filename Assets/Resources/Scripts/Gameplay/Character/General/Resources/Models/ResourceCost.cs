using System;
using UnityEngine;

namespace FlameOfTheLost
{
    // Authoring-time cost, on the skill asset
    [Serializable]
    public struct ResourceCost                      
    {
        // Flat costs: direct subtraction from current resource
        [Header("Flat Costs")]
        [SerializeField, Min(0f)] private int _hp;
        [SerializeField, Min(0f)] private int _fp;
        [SerializeField, Min(0f)] private int _stamina;
        public int HPFlatCost
        {
            get => _hp;
            set => _hp = value;
        }
        public int FPFlatCost
        {
            get => _fp;
            set => _fp = value;
        }
        public int StaminaFlatCost
        {
            get => _stamina;
            set => _stamina = value;
        }

        // Percentage costs: percentage of max resource
        [Header("Percent of Max Costs")]
        [Tooltip("Optional extra cost as % of Max resources (0..1).")]
        [SerializeField, Range(0f, 1f)] private float _hpPercentOfMax;
        [SerializeField, Range(0f, 1f)] private float _fpPercentOfMax;
        [SerializeField, Range(0f, 1f)] private float _staminaPercentOfMax;
        public float HPPercentOfMaxCost
        {
            get => _hpPercentOfMax;
            set => _hpPercentOfMax = value;
        }
        public float FPPercentOfMaxCost
        {
            get => _fpPercentOfMax;
            set => _fpPercentOfMax = value;
        }
        public float StaminaPercentOfMaxCost
        {
            get => _staminaPercentOfMax;
            set => _staminaPercentOfMax = value;
        }

        // Cost mode: determines how the cost is applied
        [Header("Cost Type")]
        [SerializeField] private ResourceCostTriggerType _costType;
        public ResourceCostTriggerType CostTriggerType
        {
            get => _costType;
            set => _costType = value;
        }

        [Header("Silence Gate")]
        [Tooltip("If true, this skill is blocked by 'Silenced'. Prefer using this field going forward.")]
        [SerializeField] private bool _blockedBySilence;
        public bool BlockedBySilence
        {
            get => _blockedBySilence;
            set => _blockedBySilence = value;
        }

        [Header("Resource Requirements")]  
        [Tooltip("If true, this skill is blocked by 'Silenced'.")] 
        [SerializeField] private bool _requiresFP;
        public bool RequiresFP
        {
            get => _requiresFP;
            set => _requiresFP = value;
        }

        [Header("HP Sacrifice Safety")]
        [SerializeField] private bool _allowSelfKillWithHPCost;
        public bool AllowSelfKillWithHPCost
        {
            get => _allowSelfKillWithHPCost;
            set => _allowSelfKillWithHPCost = value;
        }

        public bool HasAnyCost
        {
            get
            {
                return (_hp > 0 || _fp > 0 || _stamina > 0 || _hpPercentOfMax > 0f || _fpPercentOfMax > 0f || _staminaPercentOfMax > 0f);
            }
        }

        public bool IsSilenceBlocked
        {
            get => (this.BlockedBySilence || this.RequiresFP);
        }
    }
}
