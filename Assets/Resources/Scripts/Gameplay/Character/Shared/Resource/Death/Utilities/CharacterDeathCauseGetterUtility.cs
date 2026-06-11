using UnityEngine;

namespace FlameOfTheLost
{
    public static class CharacterDeathCauseGetterUtility
    {
        public static CharacterDeathCauseEntry Unknown(string reason = null)
        {
            return new CharacterDeathCauseEntry
            (
                causeType: CharacterDeathCauseType.Unknown,
                sourceCharacter: null,
                sourceObject: null,
                reason: reason
            );
        }

        public static CharacterDeathCauseEntry Environment(GameObject sourceObject, string reason = null)
        {
            return new CharacterDeathCauseEntry
            (
                causeType: CharacterDeathCauseType.Environment,
                sourceCharacter: null,
                sourceObject: sourceObject,
                reason: reason
            );
        }

        public static CharacterDeathCauseEntry Self(CharacterRootController selfCharacter, GameObject sourceObject = null, string reason = null)
        {
            return new CharacterDeathCauseEntry
            (
                causeType: CharacterDeathCauseType.Self,
                sourceCharacter: selfCharacter,
                sourceObject: sourceObject != null ? sourceObject : selfCharacter != null ? selfCharacter.gameObject : null,
                reason: reason
            );
        }

        public static CharacterDeathCauseEntry OtherCharacter(CharacterRootController sourceCharacter, GameObject sourceObject = null, string reason = null)
        {
            if (sourceCharacter == null)
            {
                return CharacterDeathCauseGetterUtility.Unknown(reason);
            }

            return new CharacterDeathCauseEntry
            (
                causeType: CharacterDeathCauseType.OtherCharacter,
                sourceCharacter: sourceCharacter,
                sourceObject: sourceObject != null ? sourceObject : sourceCharacter.gameObject,
                reason: reason
            );
        }

        public static CharacterDeathCauseEntry FromSourceCharacter(CharacterRootController victim, CharacterRootController sourceCharacter, GameObject sourceObject = null, string reason = null)
        {
            if (sourceCharacter == null)
            {
                return CharacterDeathCauseGetterUtility.Unknown(reason);
            }

            if (victim != null && sourceCharacter == victim)
            {
                return CharacterDeathCauseGetterUtility.Self(victim, sourceObject, reason);
            }

            return CharacterDeathCauseGetterUtility.OtherCharacter(sourceCharacter, sourceObject, reason);
        }
    }
}
