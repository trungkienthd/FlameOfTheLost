using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "TutObj_New", menuName = "Data/FlameOfTheLost/World/Tutorial/TutorialObjectProfile")]
    public class TutorialObjectProfile : FOTLWorldObjectProfile,
        IIdentityProvider,
        IIconProvider
    {
        // --- Data / Configs ---
        //  1. Settings
        [Header("Settings")]
        [SerializeField] private bool _visibleOnStart = true;
        public bool VisibleOnStart => _visibleOnStart;
    }
}
