using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class HarvestableColliderConfigurationRecord
    {
        // --- Configs ---
        [Header("Active Collider")]
        [SerializeField]
        private ColliderConfigurationEntry _activeCollider = new();
        public ColliderConfigurationEntry ActiveCollider => _activeCollider;

        [Header("Damage Receiver Collider")]
        [SerializeField]
        private ColliderConfigurationEntry _damageReceiverCollider = new();
        public ColliderConfigurationEntry DamageReceiverCollider => _damageReceiverCollider;

        [Header("Broken Collider Preset")]
        [SerializeField]
        private ColliderConfigurationEntry _brokenColliderPreset = new();
        public ColliderConfigurationEntry BrokenColliderPreset => _brokenColliderPreset;
    }
}