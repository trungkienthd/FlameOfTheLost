using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(OldStatusEffectController))]
    [RequireComponent(typeof(CharacterResourcesController))]
    public class StatusEffectsResourcesBridgeHandler : MonoBehaviour
    {
        //[Header("Targets")]
        //[SerializeField] private CharacterResourcesController _resources;
        //private CharacterResourcesController CharacterResourcesController
        //{
        //    get => _resources;
        //    set => _resources = value;
        //}

        //private StatusEffectController _statusEffect;
        //private StatusEffectController StatusEffectController
        //{
        //    get => _statusEffect;
        //    set => _statusEffect = value;
        //}

        //private void Awake()
        //{
        //    this.StatusEffectController = this.GetComponent<StatusEffectController>();
        //    if (!this.CharacterResourcesController) this.CharacterResourcesController = this.GetComponent<CharacterResourcesController>();

        //    this.StatusEffectController.OnStatusEffectAggregatesChanged.AddListener(this.ApplyAggregates);
        //}

        //private void ApplyAggregates(StatusEffectAggregates sa)
        //{
        //    if (this.CharacterResourcesController == null) return;

        //    // Temporary override of the multipliers.
        //    this.CharacterResourcesController.SetCostScales(sa.StaminaCostScale, sa.FPCostScale);
        //    this.CharacterResourcesController.SetRegenAdjustments
        //    (
        //        sa.StaminaRegenPerSecondDelta,  sa.FPRegenPerSecondDelta,
        //        sa.StaminaRegenDelayDelta,      sa.FPRegenDelayDelta
        //    );
        //    this.CharacterResourcesController.SetMinStaminaReserveBonus(sa.MinStaminaReserveFlat);
        //    this.CharacterResourcesController.SetSilenced(sa.Silenced);
        //    this.CharacterResourcesController.SetFatigued(sa.Fatigued);
        //}
    }
}
