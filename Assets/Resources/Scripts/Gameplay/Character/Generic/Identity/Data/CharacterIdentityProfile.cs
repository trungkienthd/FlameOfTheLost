using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "CID_CharacterIdentity", menuName = "Data/FlameOfTheLost/Character/Identity")]
    public class CharacterIdentityProfile : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private int _id;
        [SerializeField] private string _displayName;
        [TextArea]
        [SerializeField] private string _description;

        [Header("Classification")]
        [SerializeField] private CharacterType _characterType;
        [SerializeField] private CharacterFaction _faction;

        public int ID => _id;
        public string DisplayName => _displayName;
        public string Description => _description;
        public CharacterType CharacterType => _characterType;
        public CharacterFaction Faction => _faction;
    }
}