
using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCInputHandler : CharacterInputHandler
    {
        // --- Public APIs ---
        //  1. Movement Input
        public override void SetMoveInput(Vector2 moveInput)
        {
        }

        //  2. Dodging Input
        public override void RequestDodge()
        {
        }

        //  3. Combat Input
        //      3.1. Attack Input
        public override void RequestNormalAttack(AttackSide attackSide)
        {
        }

        public override void RequestSpecialAttack(AttackSide attackSide)
        {
        }

        //      3.2. Combat Loadout Input
        public override void RequestCycleWeapon(int direction)
        {
        }

        public override void RequestQuickEquipWeapon()
        {
        }

        public override void RequestCycleConsumableItem(int direction)
        {
        }

        public override void RequestConsumeConsumableItem()
        {
        }

        public override void RequestSetActiveWeaponSpecialSkill(SkillProfile skill)
        {
        }
    }
}
