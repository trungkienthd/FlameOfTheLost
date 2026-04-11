using System;

namespace FlameOfTheLost
{
    public readonly struct CharacterCombatAnimationKey : IEquatable<CharacterCombatAnimationKey>
    {
        private readonly int _skillID;
        private readonly int _weaponID;
        private readonly CharacterTagType _tag;

        private int SkillID => _skillID;
        private int WeaponID => _weaponID;
        private CharacterTagType Tag => _tag;

        public CharacterCombatAnimationKey(int skillID, int weaponID, CharacterTagType tag)
        {
            _skillID = skillID;
            _weaponID = weaponID;
            _tag = tag;
        }

        public bool Equals(CharacterCombatAnimationKey other) => this.SkillID == other.SkillID && this.WeaponID == other.WeaponID && this.Tag == other.Tag;
        public override bool Equals(object obj) => obj is CharacterCombatAnimationKey other && this.Equals(other);
        public override int GetHashCode() => HashCode.Combine(this.SkillID, this.WeaponID, (int)this.Tag);
    }
}
