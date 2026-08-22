using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class FOTLWorldObjectProfile : FOTLScriptableObjectProfile
    {
        // --- Data / Configs ---
        [Header("Initial Transform")]
        //  1. Initial Transform
        //      1.0. Boolean Gate
        [SerializeField] private bool _configureInitialTransformAtStart = true;
        public bool ConfigureInitialTransformAtStart => _configureInitialTransformAtStart;

        //      1.1. Root
        [SerializeField] private Vector3 _initialPosition;
        [SerializeField] private Vector3 _initialEulerAngles;
        [SerializeField] private Vector3 _initialLocalScale = Vector3.one;
        public Vector3 InitialPosition => _initialPosition;
        public Vector3 InitialEulerAngles => _initialEulerAngles;
        public Vector3 InitialLocalScale => _initialLocalScale;

        //      1.2. Visual
        [Header("Visual")]
        [SerializeField] private Vector3 _initialVisualLocalPosition;
        [SerializeField] private Vector3 _initialVisualLocalEulerAngles;
        [SerializeField] private Vector3 _initialVisualLocalScale = Vector3.one;
        public Vector3 InitialVisualLocalPosition => _initialVisualLocalPosition;
        public Vector3 InitialVisualLocalEulerAngles => _initialVisualLocalEulerAngles;
        public Vector3 InitialVisualLocalScale => _initialVisualLocalScale;

        //  2. Initial Visual
        [Header("Initial Visual")]
        [SerializeField] private bool _configureIconAtStart = false;
        public bool ConfigureIconAtStart => _configureIconAtStart;
    }
}

