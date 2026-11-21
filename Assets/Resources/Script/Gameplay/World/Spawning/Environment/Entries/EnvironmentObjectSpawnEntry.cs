using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class EnvironmentObjectSpawnEntry
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private GameObject _prefab;
        public GameObject Prefab => _prefab;

        // --- Configs ---
        //  1. Initial Transform
        [Header("Initial Transform")]
        [SerializeField] private Vector3 _initialPosition;
        [SerializeField] private Vector3 _initialEulerAngles;
        [SerializeField] private Vector3 _initialLocalScale = Vector3.one;
        public Vector3 InitialPosition => _initialPosition;
        public Vector3 InitialEulerAngles => _initialEulerAngles;
        public Vector3 InitialLocalScale => _initialLocalScale;

        public Quaternion InitialRotation => Quaternion.Euler(this.InitialEulerAngles);

        //  2. Spawn Settings
        [Header("Spawn Settings")]
        [SerializeField] private bool _spawnOnStart = true;
        [SerializeField] private bool _spawnOnWorldRefresh = true;
        public bool SpawnOnStart => _spawnOnStart;
        public bool SpawnOnWorldRefresh => _spawnOnWorldRefresh;

        // --- Public APIs ---
        public bool IsValid => this.Prefab != null;
    }
}