using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterSpawnPointProfile : FOTLWorldObjectProfile
    {
        // --- Data / Configs ---
        //  1. Spawn Settings
        [Header("Spawn Settings")]
        [SerializeField] private CharacterFacingStatus _defaultFacing = CharacterFacingStatus.Down;
        public CharacterFacingStatus DefaultFacing => _defaultFacing;

        //  2. Spawn Anchor Transform
        [Header("Spawn Anchor Transform")]
        [SerializeField] private bool _configureSpawnAnchorTransformAtStart = true;
        [SerializeField] private Vector3 _initialSpawnAnchorLocalPosition;
        [SerializeField] private Vector3 _initialSpawnAnchorLocalEulerAngles;
        [SerializeField] private Vector3 _initialSpawnAnchorLocalScale = Vector3.one;
        public bool ConfigureSpawnAnchorTransformAtStart => _configureSpawnAnchorTransformAtStart;
        public Vector3 InitialSpawnAnchorLocalPosition => _initialSpawnAnchorLocalPosition;
        public Vector3 InitialSpawnAnchorLocalEulerAngles => _initialSpawnAnchorLocalEulerAngles;
        public Vector3 InitialSpawnAnchorLocalScale => _initialSpawnAnchorLocalScale;
    }
}