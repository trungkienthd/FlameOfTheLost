using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class CharacterRewardAmount
    {
        [SerializeField, Min(0)] private int _amount;
        public int Amount
        {
            get => _amount;
            private set => _amount = Mathf.Max(0, value);
        }

        public CharacterRewardAmount(int amount)
        {
            this.Amount = amount;
        }
    }
}
