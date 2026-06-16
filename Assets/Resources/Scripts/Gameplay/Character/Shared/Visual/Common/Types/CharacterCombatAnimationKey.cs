using System;

namespace FlameOfTheLost
{
    public readonly struct CharacterCombatAnimationKey : IEquatable<CharacterCombatAnimationKey>
    {
        private readonly int _skillID;
        private readonly int _weaponID;
        private readonly CharacterType _type;

        private int SkillID => _skillID;
        private int WeaponID => _weaponID;
        private CharacterType Type => _type;

        public CharacterCombatAnimationKey(int skillID, int weaponID, CharacterType type)
        {
            _skillID = skillID;
            _weaponID = weaponID;
            _type = type;
        }

        public bool Equals(CharacterCombatAnimationKey other) => this.SkillID == other.SkillID && this.WeaponID == other.WeaponID && this.Type == other.Type;
        public override bool Equals(object obj) => obj is CharacterCombatAnimationKey other && this.Equals(other);
        public override int GetHashCode() => HashCode.Combine(this.SkillID, this.WeaponID, (int)this.Type);
    }
}
