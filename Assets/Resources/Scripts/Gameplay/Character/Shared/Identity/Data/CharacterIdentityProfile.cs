using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "CID_CharacterIdentity", menuName = "Data/FlameOfTheLost/Character/Shared/Identity")]
    public class CharacterIdentityProfile : ScriptableObject, IIdentityProvider
    {
        [Header("Identity")]
        [SerializeField] private int _id;
        [SerializeField] private string _key;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(5, 5)] private string _description;
        public int Id => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        [Header("Classification")]
        [SerializeField] private CharacterType _characterType;
        [SerializeField] private CharacterFaction _faction;
        [SerializeField] private CharacterTraversalType _traversalType;
        public CharacterType CharacterType => _characterType;
        public CharacterFaction Faction => _faction;
        public CharacterTraversalType TraversalType => _traversalType;
    }
}