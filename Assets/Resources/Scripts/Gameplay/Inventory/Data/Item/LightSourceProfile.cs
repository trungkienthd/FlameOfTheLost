using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "LightSourceProfile", menuName = "Data/FlameOfTheLost/Items/LightSource")]
    public class LightSourceProfile : CarriableItemProfile
    {
        public override bool IsStackable => false;
        public override int MaxStackSize => 1;
        public override ItemType ItemType => ItemType.LightSource;

        [Header("Light source type")]
        [SerializeField] private LightSourceType _lightSourceType = LightSourceType.None;
        public LightSourceType LightSourceType => _lightSourceType;

        [Header("Lighting configuration")]
        [SerializeField, Min(0f)] private float _lightRadius = 3f;
        [SerializeField, Min(0f)] private float _intensity = 1f;

        public float LightRadius => _lightRadius;
        public float Intensity => _intensity;
    }

}