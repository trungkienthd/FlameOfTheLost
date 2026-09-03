using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class EnemyVariantProfile : ScriptableObject
    {
        // --- References ---
        //  1. Enemy Prefab
        [Header("References - Enemy Prefab")]
        [SerializeField] private GameObject _enemyPrefab;
        public GameObject EnemyPrefab => _enemyPrefab;

        // --- Data / Configs ---
        //  1. Initial Inventory
        [Header("Configs - (EnemyVariantProfile) Initial Inventory")]
        [SerializeField] private List<InventoryItemEntry> _initialInventoryEntries = new();
        public IReadOnlyList<InventoryItemEntry> InitialInventoryEntries => _initialInventoryEntries;

        // --- Public APIs ---
        public virtual bool IsValid => this.EnemyPrefab != null;
    }
}
