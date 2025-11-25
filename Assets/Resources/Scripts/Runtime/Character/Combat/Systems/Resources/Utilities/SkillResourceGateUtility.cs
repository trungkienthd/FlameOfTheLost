namespace FlameOfTheLost
{
    public static class SkillResourceGateUtility
    {
        // cooldownCheck: () => true if the skill is currently on cooldown
        public static GateResult Check(CharacterResourcesController characterResourcesController, SkillProfile skill, System.Func<bool> cooldownCheck)
        {
            if (cooldownCheck != null && cooldownCheck()) return GateResult.GetRejectedGateResult(GateRejectStatus.OnCooldown);

            var erc = characterResourcesController.PreviewEffectiveResourceCost(skill.ResourceCost);

            if (erc.RequiresFP && characterResourcesController.IsSilenced) return GateResult.GetRejectedGateResult(GateRejectStatus.Silenced);

            // Always require up-front affordability (Strict model), regardless of cost timing
            // We will RESERVE now (Step 4) so there is no "fail at End" surprise.
            if (!characterResourcesController.CanAffordResourceCost(erc, enforceReserve: true))
            {
                // pick the reason that failed
                int freeStamina = characterResourcesController.CurrentStamina - characterResourcesController.MinStaminaReserve;
                if (freeStamina < erc.Stamina) 
                    return GateResult.GetRejectedGateResult(GateRejectStatus.ReserveWouldBreakMinStamina);

                if (characterResourcesController.CurrentFP < erc.FP) 
                    return GateResult.GetRejectedGateResult(GateRejectStatus.NotEnoughFP);

                return GateResult.GetRejectedGateResult(GateRejectStatus.Unknown);
            }

            return GateResult.GetAllowedGateResult();
        }
    }
}
