using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "TutObj_New", menuName = "Data/FlameOfTheLost/Level/Tutorial/TutorialObjectProfile")]
    public class TutorialObjectProfile : ScriptableObject,
        IIdentityProvider,
        IIconProvider
    {
        // --- Data / Configs ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _id = 40000;
        [SerializeField] private string _key;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(5, 5)] private string _description;
        public int Id => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        //  2. Display
        [Header("Display")]
        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;

        //  3. Settings
        [Header("Settings")]
        [SerializeField] private bool _visibleOnStart = true;
        public bool VisibleOnStart => _visibleOnStart;
    }
}
