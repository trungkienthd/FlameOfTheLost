
using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCInputHandler : CharacterInputHandler
    {
        [Header("Runtime - AI Input")]
        [SerializeField] private bool _allowAIInput;
        public bool AllowAIInput => _allowAIInput;

        // --- Public APIs ---
        //  1. Movement Input
        public void SetAIInputEnabled(bool enabled)
        {
            _allowAIInput = enabled;

            if (!enabled)
            {
                base.ClearMoveInput();
            }
        }

        public override void SetMoveInput(Vector2 moveInput)
        {
            if (!this.AllowAIInput)
            {
                base.SetMoveInput(Vector2.zero);
                return;
            }

            base.SetMoveInput(moveInput);
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
