namespace FlameOfTheLost
{
    public enum GameConditionKey
    {
        None = 0,

        // --- Tutorial: Equipment ---
        PlayerHasMultipleWeapons = 100,
        PlayerReadyToCycleWeapons = 101,

        // --- Tutorial: Combat ---
        PlayerHitByFirstEnemy = 200,
        FirstEnemyDefeated = 201,

        // --- NPC Quests: The Silent Altar ---
        QuestTheForgottenFlameAccepted = 14041110,
        QuestTheForgottenFlameCompleted = 14041111,

        QuestTheFallenRadianceAccepted = 14041120,
        QuestTheFallenRadianceCompleted = 14041121,

        // --- World Progression: Forgotten Sanctum ---
        ForgottenSanctumLanternsActivated = 140411120,
        ForgottenSanctumGorVakhDefeated = 140411121,
        ForgottenSanctumEmberboundSigilAcquired = 140411122,

        // --- World Progression: Eastern Altar ---
        EasternAltarLanternsActivated = 140411221,
        EasternAltarAurelianDefeated = 140411222,

        // --- NPC Quest Reward Claims: Seraphine ---
        QuestTheForgottenFlameTorchClaimed = 140411131,
        QuestTheForgottenFlameCompletionRewardClaimed = 140411132,
        QuestTheFallenRadianceWeaponClaimed = 140411232,
    }
}
