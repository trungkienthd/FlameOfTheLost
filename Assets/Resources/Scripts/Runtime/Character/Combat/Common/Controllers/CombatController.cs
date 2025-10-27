using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CombatController : MonoBehaviour
    {
        [Header("Common Refs")]
        [SerializeField] private CharacterAnimationHandler _animationHandler;
        [SerializeField] private CombatLoadoutController _combatLoadoutController;
        [SerializeField] private CharacterResourcesController _characterResourcesController;
        [SerializeField] private AudioHandler _audioHandler;

        protected CharacterAnimationHandler AnimationHandler
        {
            get => _animationHandler;
            private set => _animationHandler = value;
        }
        protected CombatLoadoutController CombatLoadoutController
        {
            get => _combatLoadoutController;
            private set => _combatLoadoutController = value;
        }
        protected CharacterResourcesController CharacterResourcesController
        {
            get => _characterResourcesController;
            private set => _characterResourcesController = value;
        }
        protected AudioHandler AudioHandler
        {
            get => _audioHandler;
            private set => _audioHandler = value;
        }

        private AttackSlot _lastSlot = AttackSlot.Left;
        private int _lastSkillId = 0;
        protected AttackSlot LastSlot
        {
            get => _lastSlot;
            private set => _lastSlot = value;
        }
        protected int LastSkillId
        {
            get => _lastSkillId;
            private set => _lastSkillId = value;
        }

        [Header("Input")]
        [SerializeField] private InputConfig _inputConfig;
        public InputConfig InputConfig
        {
            get => _inputConfig;
            private set => _inputConfig = value;
        }

        private bool _facingLocked = false;                 // Runtime aim/facing
        private Vector2 _attackSnapshotDir = Vector2.right; // used if Hitbox needs direction
        protected bool FacingLocked
        {
            get => _facingLocked;
            private set => _facingLocked = value;
        }
        protected Vector2 AttackSnapshotDir
        {
            get => _attackSnapshotDir;
            private set => _attackSnapshotDir = value;
        }

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

        // Active cast ticket (for reserve/finalize/channel)
        private bool _hasActiveCast;
        private EffectiveResourceCost _activeCost;
        private ResourceCostMode _activeCostMode;
        private bool _channeling;
        private float _lastPerSecondTickTime;
        protected bool HasActiveCast
        {
            get => _hasActiveCast;
            set => _hasActiveCast = value;
        }
        protected EffectiveResourceCost ActiveCost
        {
            get => _activeCost;
            set => _activeCost = value;
        }
        protected ref EffectiveResourceCost ActiveCostRef => ref _activeCost; // for in-place modification
        protected ResourceCostMode ActiveCostMode
        {
            get => _activeCostMode;
            set => _activeCostMode = value;
        }
        protected ref ResourceCostMode ActiveCostModeRef => ref _activeCostMode; // for in-place modification
        protected bool Channeling
        {
            get => _channeling;
            set => _channeling = value;
        }
        protected ref bool ChannelingRef => ref _channeling; // for in-place modification
        protected float LastPerSecondTickTime
        {
            get => _lastPerSecondTickTime;
            set => _lastPerSecondTickTime = value;
        }
        protected ref float LastPerSecondTickTimeRef => ref _lastPerSecondTickTime; // for in-place modification

        protected virtual void Awake()
        {
            if (!this.AnimationHandler) this.AnimationHandler = this.GetComponentInChildren<CharacterAnimationHandler>();
            if (!this.CombatLoadoutController) this.CombatLoadoutController = this.GetComponent<CombatLoadoutController>();
            if (!this.CharacterResourcesController) this.CharacterResourcesController = this.GetComponent<CharacterResourcesController>();
            if (!this.AudioHandler) this.AudioHandler = this.GetComponentInChildren<AudioHandler>();
        }

        protected bool TryBeginAttack(AttackSlot slot, int skillId)
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

        public abstract void DoAttack(AttackSlot slot, int skillId);

        protected void TryApplyAimFacingOverride()
        {
            if (!this.InputConfig) return;
            if (!this.InputConfig.MouseAimingOverridesFacing) return;
            if (this.FacingLocked) return; // in case of mis-ordered calls

            // Face-lock modifier (e.g., Shift) to keep current facing
            if (Input.GetKey(this.InputConfig.FaceLockKey)) return;

            // Detect mouse presence (safe for both legacy & new input setups)
            if (!Input.mousePresent) return;

            // Compute mouse-based aim direction in world space
            bool valid;
            Vector2 aimDir = AimHelper.TryGetMouseAimDir2D
            (
                this.transform,
                Camera.main,
                this.InputConfig.CursorDeadRadius,
                out valid
            );

            // Snap to 4-way or 8-way and set facing/flip
            bool flipDummy;
            var facing = AimHelper.GetFacingFromVector(aimDir, this.InputConfig.EightWaySnap, out flipDummy);

            // Update anim-facing
            this.AnimationHandler?.SetFacing(facing);

            // Ensure Animator DirX/DirY + FlipGroup update immediately (idle-friendly)
            // Keep AnimationHandler’s internal direction consistent
            // NOTE: your AnimationHandler.ForceFacing takes Vector2 dir
            Vector2 dirForFacing =
                (facing == FacingStatus.Side) ? new Vector2(Mathf.Sign(aimDir.x), 0f) :
                (facing == FacingStatus.Up) ? Vector2.up :
                                                Vector2.down;

            this.AnimationHandler?.ForceFacing(dirForFacing);

            // Cache for hitbox orientation use
            this.AttackSnapshotDir = aimDir;
        }

        protected Vector2 GetCurrentAttackDirection()
        {
            // Prefer last computed aim; if none, derive from current facing
            if (this.AttackSnapshotDir.sqrMagnitude > 0.0001f) return this.AttackSnapshotDir;

            // Fallback: map current facing to a vector (Right/Left handled by flipX in AnimationHandler)
            // You may adjust if your AnimationHandler exposes current flip state.
            switch (this.AnimationHandler?.FacingStatus ?? FacingStatus.Side)
            {
                case FacingStatus.Down: return Vector2.down;
                case FacingStatus.Up: return Vector2.up;
                case FacingStatus.Side:
                    return (this.AnimationHandler != null && this.AnimationHandler.IsFacingRight)
                        ? Vector2.right
                        : Vector2.left;
                default: return Vector2.right;
            }
        }

        protected void LockFacingAndSnapshotDirection()
        {
            // Lock facing during active frames if configured
            if (this.InputConfig && this.InputConfig.LockFacingDuringActiveFrames)
            {
                this.FacingLocked = true;
                // If your AnimationHandler supports it, you can also notify it:
                // this.AnimationHandler?.SetFacingLocked(true);
            }

            // Snapshot current attack direction for hitbox / projectile alignment
            this.AttackSnapshotDir = this.GetCurrentAttackDirection(); // TryApplyAimFacingOverride() should've run in DoAttack()
        }

        protected void UnlockFacingAfterActiveWindow()
        {
            if (this.FacingLocked)
            {
                this.FacingLocked = false;
            }
        }

        protected void MarkAttackEnded()
        {
            if (!this.AttackActive) return;

            this.AttackActive = false;

            // Start cooldown now (Inspector-tuned)
            this.CooldownStartAt = Time.time;
            this.CooldownTargetAt = Time.time + Mathf.Max(0f, this.CooldownAfterEnd);
            this.NextStartAllowedAt = this.CooldownTargetAt;
        }

        protected void NotifyGateFail(GateRejectReason reason)
        {
            switch (reason)
            {
                case GateRejectReason.OnCooldown:
                    Debug.Log("[CombatController] Blocked: On cooldown");
                    break;
                case GateRejectReason.Silenced:
                    Debug.Log("[CombatController] Blocked: Silenced");
                    break;
                case GateRejectReason.NotEnoughStamina:
                    Debug.Log("[CombatController] Blocked: Not enough Stamina");
                    break;
                case GateRejectReason.NotEnoughFP:
                    Debug.Log("[CombatController] Blocked: Not enough FP");
                    break;
                case GateRejectReason.ReserveWouldBreakMinStamina:
                    Debug.Log("[CombatController] Blocked: Min Stamina Reserve");
                    break;
                default:
                    Debug.Log("[CombatController] Blocked");
                    break;
            }
        }

        protected void ClearActiveCast()
        {
            this.HasActiveCast = false;
            this.ActiveCost = default;
            this.ActiveCostMode = ResourceCostMode.OnStart;
            this.Channeling = false;
            this.LastPerSecondTickTime = 0f;
        }

        // Runs Silence/FP/Stamina gate and tries to reserve/spend up-front (Strict + Reserve).
        // Returns true if reservation succeeded and fills out params for the caller to store as its "active cast ticket".
        protected bool TryGateAndReserveSkill
        (
            SkillProfile skillProfile,
            Action<GateRejectReason> onFail,
            out EffectiveResourceCost reservationCost,
            out ResourceCostMode costMode,
            out bool isChannel,
            out float channelStartTime)
        {
            reservationCost = default;
            costMode = ResourceCostMode.OnStart;
            isChannel = false;
            channelStartTime = 0f;

            if (this.CharacterResourcesController == null || skillProfile == null)
            {
                onFail?.Invoke(GateRejectReason.Unknown);
                return false;
            }

            var gate = AttackGatekeeper.Check(this.CharacterResourcesController, skillProfile, cooldownCheck: null);
            if (!gate.Allowed)
            {
                onFail?.Invoke(gate.Reason);
                return false;
            }

            var effectiveCost = this.CharacterResourcesController.PreviewEffectiveResourceCost(skillProfile.ResourceCost);
            if (!this.CharacterResourcesController.TryReserveOrSpend(effectiveCost))
            {
                var freeStamina = this.CharacterResourcesController.CurrentStamina - this.CharacterResourcesController.MinStaminaReserve;
                onFail?.Invoke
                (
                    freeStamina < effectiveCost.Stamina 
                        ? GateRejectReason.ReserveWouldBreakMinStamina
                        : GateRejectReason.NotEnoughFP
                );
                return false;
            }

            reservationCost = effectiveCost;
            costMode = effectiveCost.CostMode;
            isChannel = (effectiveCost.CostMode == ResourceCostMode.PerSecond) && (effectiveCost.PerSecondStamina > 0f || effectiveCost.PerSecondFP > 0f);
            channelStartTime = Time.time;

            return true;
        }

        // Refunds a previously reserved/spent cost (100% by default). Safe to call even if zeroed.
        protected void RefundReservation(in EffectiveResourceCost reservationCost, float refundRatio = 1.0f)
        {
            if (this.CharacterResourcesController == null) return;
            this.CharacterResourcesController.Refund(reservationCost, refundRatio);
        }

        // For PER_SECOND channel skills: drains resources since the last tick.
        // Returns false if the drain failed (ran out), so caller can abort the cast gracefully.
        protected bool TryTickPerSecondDrain
        (
            in EffectiveResourceCost activeCost,
            ref float lastTickTime, 
            float minInterval = 0.02f
        )
        {
            if (this.CharacterResourcesController == null) return false;

            float currentTime = Time.time;
            float dt = currentTime - lastTickTime;
            if (dt < minInterval) return true; // too soon to tick again

            if (!this.CharacterResourcesController.TryPayPerSecond(dt, activeCost.PerSecondStamina, activeCost.PerSecondFP)) return false;

            lastTickTime = currentTime;
            return true;
        }
    }
}
