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
        [SerializeField] private CharacterType _characterType = CharacterType.Default;
        [SerializeField] private CharacterFaction _faction = CharacterFaction.Neutral;
        public CharacterType CharacterType => _characterType;
        public CharacterFaction Faction => _faction;

        // --- Public APIs ---
        public bool IsPlayer => _faction == CharacterFaction.Player;
        public bool IsEnemy => _faction == CharacterFaction.Enemy;
        public bool IsAlly => _faction == CharacterFaction.Ally;
    }
}
