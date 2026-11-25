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

        // --- NPC Quests: Seraphine ---
        //  1. Quest ID 1404111: The Forgotten Flame
        QuestTheForgottenFlameAccepted = 14041110,
        QuestTheForgottenFlameCompleted = 14041111,

        //      1.1. World Progression: Forgotten Sanctum
        ForgottenSanctumLanternsActivated = 140411120,
        ForgottenSanctumGorVakhDefeated = 140411121,
        ForgottenSanctumEmberboundSigilAcquired = 140411122,

        //      1.2. Reward Claims
        QuestTheForgottenFlameTorchClaimed = 140411131,
        QuestTheForgottenFlameCompletionRewardClaimed = 140411132,

        //  2. Quest ID 1404112: The Fallen Radiance
        QuestTheFallenRadianceAccepted = 14041120,
        QuestTheFallenRadianceCompleted = 14041121,

        //      2.1. World Progression: Eastern Altar
        EasternAltarLanternsActivated = 140411220,
        EasternAltarAurelianDefeated = 140411221,

        //      2.2. Reward Claims
        QuestTheFallenRadianceWeaponClaimed = 140411232,

        // --- NPC Quests: Sir Aldric ---
        //  1. Quest ID 1420111: An Oath Unbroken
        QuestAnOathUnbrokenAccepted = 14201110,
        QuestAnOathUnbrokenCompleted = 14201111,

        //      1.1. World Progression
        QuestAnOathUnbrokenAldricFreed = 142011120,
        QuestAnOathUnbrokenWeaponRecovered = 142011121,
        QuestAnOathUnbrokenPuzzleDiscovered = 142011122,
        QuestAnOathUnbrokenPuzzleSolved = 142011123,

        //      1.2. Reward Claims
        QuestAnOathUnbrokenCompletionRewardClaimed = 142011130,

        //      1.3. NPC Status
        SirAldricBetrayed = 14201901,

    }
}
