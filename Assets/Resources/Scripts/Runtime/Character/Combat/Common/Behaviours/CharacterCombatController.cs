using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract partial class CharacterCombatController : MonoBehaviour
    {
        [Header("Common Refs")]
        [SerializeField] private CharacterAnimationHandler _animationHandler;
        [SerializeField] private CharacterCombatLoadoutController _combatLoadoutController;
        [SerializeField] private CharacterResourcesController _characterResourcesController;
        [SerializeField] private AudioHandler _audioHandler;

        protected CharacterAnimationHandler AnimationHandler
        {
            get => _animationHandler;
            private set => _animationHandler = value;
        }
        protected CharacterCombatLoadoutController CombatLoadoutController
        {
            get => _combatLoadoutController;
            private set => _combatLoadoutController = value;
        }
        protected CharacterResourcesController CharacterResourcesController
        {
            get => _characterResourcesController;
            private set => _characterResourcesController = value;
        }
        protected AudioHandler AudioHandler
        {
            get => _audioHandler;
            private set => _audioHandler = value;
        }

        private AttackSlotType _lastSlot = AttackSlotType.Left;
        private int _lastSkillId = 0;
        protected AttackSlotType LastSlot
        {
            get => _lastSlot;
            private set => _lastSlot = value;
        }
        protected int LastSkillId
        {
            get => _lastSkillId;
            private set => _lastSkillId = value;
        }

        protected virtual void Awake()
        {
            if (!this.AnimationHandler) this.AnimationHandler = this.GetComponentInChildren<CharacterAnimationHandler>();
            if (!this.CombatLoadoutController) this.CombatLoadoutController = this.GetComponent<CharacterCombatLoadoutController>();
            if (!this.CharacterResourcesController) this.CharacterResourcesController = this.GetComponent<CharacterResourcesController>();
            if (!this.AudioHandler) this.AudioHandler = this.GetComponentInChildren<AudioHandler>();

            this.AwakeAiming();
        }
    }
}
