using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterProgressionController : CharacterBehaviourBaseController
    {
        // --- States ---
        [Header("Progression Attributes")]
        [SerializeField] private List<CharacterProgressionAttributeEntry> _attributes = new();
        public IReadOnlyList<CharacterProgressionAttributeEntry> Attributes => _attributes;

        private HPController HPController => this.CharacterRootController.HPController;
        private FPController FPController => this.CharacterRootController.FPController;
        private StaminaController StaminaController => this.CharacterRootController.StaminaController;
        private CharacterCombatLoadoutController CombatLoadoutController => this.CharacterRootController.CombatLoadoutController;
        private CharacterRewardWalletController RewardWalletController => this.CharacterRootController.RewardWalletController;

        // --- Events ---
        public event Action<CharacterProgressionAttributeEntry, int> OnAttributeValueChanged;
        public event Action<CharacterProgressionAttributeEntry, int> OnAttributeLevelChanged;

        // --- Unity Callbacks ---
#if UNITY_EDITOR
        private void OnValidate()
        {
            
        }
#endif
    }
}
