namespace FlameOfTheLost
{
    public enum CraftingFailureReason
    {
        None = 0,

        CraftingUnavailable = 11,
        InvalidProvider     = 12,
        RecipeUnavailable   = 13,
        InvalidRecipe       = 14,

        InsufficientMaterials       = 21,
        InsufficientLostEmbers      = 22,
        InsufficientInventorySpace  = 23,

        TransactionFailed = 31
    }
}
