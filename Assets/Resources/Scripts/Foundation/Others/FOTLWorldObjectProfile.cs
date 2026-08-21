using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class FOTLWorldObjectProfile : FOTLScriptableObjectProfile
    {
        // --- Data / Configs ---
        [Header("Initial Transform")]
        [SerializeField] private bool _configureInitialTransformAtStart = true;

        [SerializeField] private Vector3 _initialPosition;
        [SerializeField] private Vector3 _initialEulerAngles;
        [SerializeField] private Vector3 _initialLocalScale = Vector3.one;

        public bool ConfigureInitialTransformAtStart => _configureInitialTransformAtStart;

        public Vector3 InitialPosition => _initialPosition;
        public Vector3 InitialEulerAngles => _initialEulerAngles;
        public Vector3 InitialLocalScale => _initialLocalScale;
    }
}

