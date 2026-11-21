using UnityEngine;

namespace FlameOfTheLost
{
    public class UICanvasHUDCombatLoadoutProjectileSlotHandler : UICanvasHUDCombatLoadoutItemSlotHandlerBase
    {
        protected override Sprite GetItemIcon(CharacterCombatLoadoutController combatLoadoutController)
        {
            if (combatLoadoutController == null) return null;

            return combatLoadoutController.GetActiveEquippedProjectileIcon();
        }

        protected override InventoryItemEntry GetItemEntry(CharacterCombatLoadoutController combatLoadoutController)
        {
            if (combatLoadoutController == null) return null;

            return combatLoadoutController.ActiveEquippedProjectileEntry;
        }
    }
}