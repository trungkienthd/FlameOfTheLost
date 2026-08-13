using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "DSp_Generic_New", menuName = "Data/FlameOfTheLost/Level/Spawning/DefaultSpawnPoints/CharacterDefaultSpawnPointProfile")]
    public class CharacterDefaultSpawnPointProfile : ScriptableObject,
        IIdentityProvider,
        IIconProvider
    {
        // --- Data / Configs ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _id;
        [SerializeField] private string _key;
        [SerializeField] private string _displayName = "Default Spawn Point";
        [SerializeField, TextArea(5, 5)] private string _description;
        public int Id => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        //  2. Display
        [Header("Display")]
        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;

        //  3. Spawn Settings
        [Header("Spawn Settings")]
        [SerializeField] private CharacterFacingStatus _defaultFacing = CharacterFacingStatus.Down;
        public CharacterFacingStatus DefaultFacing => _defaultFacing;
    }
}
