using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class CollectibleObjectSpawnEntry : EnvironmentObjectSpawnEntry
    {
        // --- Configs ---
        //  1. Initial Quantity
        [Header("Initial Quantity")]
        [SerializeField, Min(1)] private int _initialQuantity = 1;
        public int InitialQuantity => Mathf.Max(1, _initialQuantity);
    }
}