using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterRewardWalletController : CharacterBehaviourControllerBase, ISaveDataHandler<PlayerRewardSaveData>
    {
        // --- States ---
        [Header("Lost Embers Amount (Runtime)")]
        [SerializeField, Min(0)] private int _currentAmount = 0;
        public int CurrentAmount
        {
            get => _currentAmount;
            private set
            {
                int clampedValue = Mathf.Max(0, value);

                if (_currentAmount == clampedValue) return;

                _currentAmount = clampedValue;

                this.AmountChanged?.Invoke(this.CurrentAmount);
            }
        }

        // --- Events ---
        public event Action<int> AmountChanged;

        // --- Public APIs ---
        //  1. Reward Modification
        public void Add(int amount)
        {
            if (amount <= 0) return;

            this.CurrentAmount += amount;
        }

        public bool CanSpend(int amount)
        {
            if (amount <= 0) return false;

            return this.CurrentAmount >= amount;
        }

        public bool TrySpend(int amount)
        {
            if (!this.CanSpend(amount)) return false;

            this.CurrentAmount -= amount;

            return true;
        }

        public void SetAmount(int amount)
        {
            this.CurrentAmount = Mathf.Max(0, amount);
        }

        //  2. ISaveDataHandler<PlayerRewardSaveData>
        public PlayerRewardSaveData CaptureSaveData()
        {
            return new PlayerRewardSaveData
            {
                LostEmbers = this.CurrentAmount
            };
        }
        public void RestoreSaveData(PlayerRewardSaveData saveData)
        {
            if (saveData == null) return;

            this.SetAmount(Mathf.Max(0, saveData.LostEmbers));
        }
    }
}
