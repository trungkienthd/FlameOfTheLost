using UnityEngine;

namespace FlameOfTheLost
{
    public class UICanvasHUDCombatLoadoutConsumableItemSlotHandler : UICanvasHUDCombatLoadoutItemSlotHandlerBase
    {
        protected override Sprite GetItemIcon(CharacterCombatLoadoutController combatLoadoutController)
        {
            if (combatLoadoutController == null) return null;

            return combatLoadoutController.GetActiveConsumableItemIcon();
        }

        protected override InventoryItemEntry GetItemEntry(CharacterCombatLoadoutController combatLoadoutController)
        {
            if (combatLoadoutController == null) return null;

            return combatLoadoutController.ActiveEquippedConsumableItemEntry;
        }
    }
}