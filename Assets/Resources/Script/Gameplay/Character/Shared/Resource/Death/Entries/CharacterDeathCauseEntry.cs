using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class CharacterDeathCauseEntry
    {
        [SerializeField] private CharacterDeathCauseType _causeType;
        [SerializeField] private CharacterRootController _sourceCharacter;
        [SerializeField] private GameObject _sourceObject;
        [SerializeField] private string _reason;
        public CharacterDeathCauseType CauseType => _causeType;
        public CharacterRootController SourceCharacter => _sourceCharacter;
        public GameObject SourceObject => _sourceObject;
        public string Reason => _reason;

        public bool HasSourceCharacter => this.SourceCharacter != null;

        public CharacterDeathCauseEntry(CharacterDeathCauseType causeType, CharacterRootController sourceCharacter, GameObject sourceObject, string reason)
        {
            _causeType = causeType;
            _sourceCharacter = sourceCharacter;
            _sourceObject = sourceObject;
            _reason = reason;
        }


    }
}
