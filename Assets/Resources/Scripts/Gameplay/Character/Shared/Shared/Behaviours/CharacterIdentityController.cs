using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterIdentityController : MonoBehaviour, IIdentityProvider
    {
        // --- States ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _id;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(5, 5)] private string _description;

        public int ID => _id;
        public string DisplayName => _displayName;
        public string Description => _description;

        //  2. Classification
        [Header("Classification")]
        [SerializeField] private CharacterTagType _characterTag = CharacterTagType.Default;
        [SerializeField] private CharacterFactionType _faction = CharacterFactionType.Neutral;
        public CharacterTagType CharacterTag => _characterTag;
        public CharacterFactionType Faction => _faction;

        // --- Public APIs ---
        public bool IsPlayer => _faction == CharacterFactionType.Player;
        public bool IsEnemy => _faction == CharacterFactionType.Enemy;
        public bool IsAlly => _faction == CharacterFactionType.Ally;
    }
}
