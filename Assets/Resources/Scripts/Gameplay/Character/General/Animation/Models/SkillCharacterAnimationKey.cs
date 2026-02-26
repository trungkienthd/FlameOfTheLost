using System;

namespace FlameOfTheLost
{
    public readonly struct SkillCharacterAnimationKey : IEquatable<SkillCharacterAnimationKey>
    {
        private readonly int _skillID;
        private readonly CharacterTagType _tag;

        private int SkillID => _skillID;
        private CharacterTagType Tag => _tag;

        public SkillCharacterAnimationKey(int skillID, CharacterTagType tag)
        {
            _skillID = skillID;
            _tag = tag;
        }

        public bool Equals(SkillCharacterAnimationKey other) => this.SkillID == other.SkillID && this.Tag == other.Tag;
        public override bool Equals(object obj) => obj is SkillCharacterAnimationKey other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(this.SkillID, (int)this.Tag);
    }
}
