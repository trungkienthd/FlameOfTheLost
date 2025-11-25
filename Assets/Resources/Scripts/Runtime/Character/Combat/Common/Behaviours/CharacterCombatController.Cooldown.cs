using UnityEngine;

namespace FlameOfTheLost
{
    public abstract partial class CharacterCombatController : MonoBehaviour
    {
        [Header("Attack Cooldown")]
        [Tooltip("Block new attacks until End event fires.")]
        [SerializeField] private bool _requireEndBeforeNext = true;

        [Tooltip("Cooldown AFTER End (seconds). Tuned in Inspector.")]
        [SerializeField] private float _cooldownAfterEnd = 0.50f;

        protected bool RequiredEndBeforeNext => _requireEndBeforeNext;
        protected float CooldownAfterEnd => _cooldownAfterEnd;

        // state
        private float _nextStartAllowedAt = 0f;
        private bool _attackActive = false;
        private float NextStartAllowedAt
        {
            get => _nextStartAllowedAt;
            set => _nextStartAllowedAt = value;
        }
        private bool AttackActive
        {
            get => _attackActive;
            set => _attackActive = value;
        }

        // Cooldown clock for HUD
        private float _cooldownStartAt = 0f; // when the visible cooldown started
        private float _cooldownTargetAt = 0f; // when you’re allowed to start next (max of all gates)

        protected float CooldownStartAt
        {
            get => _cooldownStartAt;
            private set => _cooldownStartAt = value;
        }
        protected float CooldownTargetAt
        {
            get => _cooldownTargetAt;
            private set => _cooldownTargetAt = value;
        }
        public float CooldownNormalized
        {
            get
            {
                // While attack is active (before End), keep overlay at full (1.0)
                if (this.AttackActive) return 1f;

                // If not cooling, hide overlay
                if (Time.time >= this.CooldownTargetAt) return 0f;

                // Cooling down
                float dur = Mathf.Max(0.0001f, this.CooldownTargetAt - this.CooldownStartAt);
                float rem = Mathf.Clamp(this.CooldownTargetAt - Time.time, 0f, dur);
                return rem / dur;
            }
        }

        protected bool TryBeginAttack(AttackSlotType slot, int skillId)
        {
            // Block if still cooling down by time
            if (Time.time < this.NextStartAllowedAt) return false;

            // Optionally require End before next attack, even if min-interval passed
            if (this.RequiredEndBeforeNext && this.AttackActive) return false;

            this.LastSlot = slot;
            this.LastSkillId = skillId;

            this.AttackActive = true;

            return true;
        }

        public abstract void DoAttack(AttackSlotType slot, int skillId);

        protected void MarkAttackEnded()
        {
            if (!this.AttackActive) return;

            this.AttackActive = false;

            // Start cooldown now (Inspector-tuned)
            this.CooldownStartAt = Time.time;
            this.CooldownTargetAt = Time.time + Mathf.Max(0f, this.CooldownAfterEnd);
            this.NextStartAllowedAt = this.CooldownTargetAt;
        }
    }
}
