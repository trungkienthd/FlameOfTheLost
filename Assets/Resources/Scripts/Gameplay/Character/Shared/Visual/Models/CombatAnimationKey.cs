using System;

namespace FlameOfTheLost
{
    public readonly struct CombatAnimationKey : IEquatable<CombatAnimationKey>
    {
        private readonly int _skillId;
        private readonly int _weaponId;
        private readonly AttackSide _side;

        public int SkillID => _skillId;
        public int WeaponID => _weaponId;
        public AttackSide Side => _side;

        public CombatAnimationKey(int skillId, int weaponId, AttackSide side)
        {
            _skillId = skillId;
            _weaponId = weaponId;
            _side = side;
        }

        public bool Equals(CombatAnimationKey other) => this.SkillID == other.SkillID && this.WeaponID == other.WeaponID && this.Side == other.Side;
        public override bool Equals(object obj) => obj is CombatAnimationKey other && this.Equals(other);
        public override int GetHashCode() => HashCode.Combine(this.SkillID, this.WeaponID, (int)this.Side);
    }
}