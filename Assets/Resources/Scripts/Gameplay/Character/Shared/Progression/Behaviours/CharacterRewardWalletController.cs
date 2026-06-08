using FlameOfTheLost;
using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterRewardWalletController : CharacterBehaviourBaseController
    {
        // --- States ---
        [Header("Amount (Lost Embers)")]
        [SerializeField, Min(0)] private int _currentAmount = 0;
        public int CurrentAmount
        {
            get => _currentAmount;
            private set
            {
                _currentAmount = Mathf.Max(0, value);
                this.OnChanged?.Invoke(this.CurrentAmount);
            }
        }

        // --- Events ---
        public event Action<int> OnChanged;
        public event Action<int> OnGained;
        public event Action<int> OnSpent;

        // --- Public APIs ---
        public void Add(int amount)
        {
            if (amount <= 0) return;

            this.CurrentAmount += amount;

            this.OnGained?.Invoke(amount);
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

            this.OnSpent?.Invoke(amount);

            return true;
        }

        // --- Helpers ---
        private void OnSpentSafe(int amount)
        {
            this.OnSpent?.Invoke(amount);
        }
    }
}
