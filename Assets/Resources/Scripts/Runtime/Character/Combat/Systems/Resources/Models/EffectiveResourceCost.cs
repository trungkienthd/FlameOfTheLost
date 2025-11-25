using UnityEngine;

namespace FlameOfTheLost
{
    public struct EffectiveResourceCost
    {
        private int _stamina;
        private int _fp;
        public int Stamina
        {
            get => _stamina;
            set => _stamina = Mathf.Max(0, value);
        }
        public int FP
        {
            get => _fp;
            set => _fp = Mathf.Max(0, value);
        }

        private ResourceCostType _costMode;
        public ResourceCostType CostMode
        {
            get => _costMode;
            set => _costMode = value;
        }

        private bool _requiresFP;
        public bool RequiresFP
        {
            get => _requiresFP;
            set => _requiresFP = value;
        }

        private float _perSecondStamina;
        private float _perSecondFP;
        public float PerSecondStamina
        {
            get => _perSecondStamina;
            set => _perSecondStamina = Mathf.Max(0f, value);
        }
        public float PerSecondFP
        {
            get => _perSecondFP;
            set => _perSecondFP = Mathf.Max(0f, value);    
        }
    }
}
