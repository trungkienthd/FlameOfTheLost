using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FlameOfTheLost
{
    public class OldStatusEffectController : MonoBehaviour
    {
        //[Serializable] public class StatusEffectEvent : UnityEvent<AppliedStatusEffect> { }

        //[Serializable] public class AggregatesEvent : UnityEvent<StatusEffectAggregates> { }

        //[Serializable] public class IntEvent : UnityEvent<int> { }

        //public class AppliedStatusEffect
        //{
        //    public uint RuntimeID 
        //    {
        //        get; 
        //        internal set; 
        //    }
        //    public StatusEffectProfile StatusEffectProfile 
        //    {
        //        get; 
        //        internal set; 
        //    }
        //    public GameObject Source 
        //    {
        //        get; 
        //        internal set; 
        //    }

        //    // resolved at apply (BasePotency * external scale)
        //    public float PotencyScale                                        
        //    {
        //        get; 
        //        internal set; 
        //    }
        //    // 1..MaxStacks
        //    public int Stacks                                           
        //    {
        //        get; 
        //        internal set; 
        //    }

        //    // seconds left
        //    public float RemainingDuration                                      
        //    {
        //        get; 
        //        internal set; 
        //    }

        //    // for periodic effects
        //    public float TickAccumulator                                
        //    { 
        //        get; 
        //        internal set; 
        //    } 
        //}

        //[Header("Ticking")]
        //[SerializeField] private bool _useUnscaledTime = false;         // If true, tick and decay use unscaled time (ignore slow-motion).
        //private bool UseUnscaledTime => _useUnscaledTime;

        //[Header("Events")]
        //[SerializeField] private StatusEffectEvent _onStatusEffectApplied = new StatusEffectEvent();
        //[SerializeField] private StatusEffectEvent _onStatusEffectUpdated = new StatusEffectEvent();
        //[SerializeField] private StatusEffectEvent _onStatusEffectRemoved = new StatusEffectEvent();
        //public StatusEffectEvent OnStatusEffectApplied => _onStatusEffectApplied;
        //public StatusEffectEvent OnStatusEffectUpdated => _onStatusEffectUpdated;
        //public StatusEffectEvent OnStatusEffectRemoved => _onStatusEffectRemoved;

        //[SerializeField] private AggregatesEvent _onAggregatesChanged = new AggregatesEvent();
        //public AggregatesEvent OnStatusEffectAggregatesChanged => _onAggregatesChanged;

        //[SerializeField] private IntEvent _onDoTTick = new IntEvent();  // Raised for DoT ticks (damage amount per tick). Hook to HealthController.TakeDamage bridge.
        //[SerializeField] private IntEvent _onHoTTick = new IntEvent();  // Raised for HoT ticks (heal amount per tick). Hook to HealthController.Heal.
        //public IntEvent OnDamageOverTimeTick => _onDoTTick;
        //public IntEvent OnHealOverTimeTick => _onHoTTick;

        //private StatusEffectAggregates _currentAggregates = StatusEffectAggregates.Default();
        //public StatusEffectAggregates CurrentStatusEffectAggregatesSnapshot
        //{
        //    get => _currentAggregates;
        //    private set => _currentAggregates = value;
        //}

        //private readonly List<AppliedStatusEffect> _active = new List<AppliedStatusEffect>();
        //private List<AppliedStatusEffect> ActiveAppliedStatusEffects => _active;

        //private bool _aggregatesDirty = true;
        //private bool StatusEffectAggregatesDirty
        //{
        //    get => _aggregatesDirty;
        //    set => _aggregatesDirty = value;
        //}

        //private uint _nextID = 1;
        //public uint NextRuntimeID
        //{
        //    get => _nextID;
        //    private set => _nextID = value;
        //}

        //// Apply or re-apply an effect. Returns true if accepted.
        //public bool TryApply(StatusEffectProfile statusEffectProfile, GameObject source = null, float potencyScale = 1f, float? durationOverrideInSecs = null, int stacksToAdd = 1)
        //{
        //    if (statusEffectProfile == null || stacksToAdd <= 0) return false;

        //    var existing = this.ActiveAppliedStatusEffects.Find(activeStatusEffect => activeStatusEffect.StatusEffectProfile == statusEffectProfile);
        //    switch (statusEffectProfile.StatusEffectStackingPolicy)
        //    {
        //        case StatusEffectStackingPolicy.RefreshDuration:
        //            if (existing == null)
        //            {
        //                var newAppliedStatusEffect = this.CreateNew(statusEffectProfile, source, potencyScale, durationOverrideInSecs, 1);
        //                this.ActiveAppliedStatusEffects.Add(newAppliedStatusEffect);
        //                this.OnStatusEffectApplied.Invoke(newAppliedStatusEffect);
        //            }
        //            else
        //            {
        //                existing.Stacks = 1;
        //                existing.PotencyScale = StatusEffectController.ResolvePotency(statusEffectProfile, source, potencyScale);
        //                existing.RemainingDuration = StatusEffectController.ResolveDuration(statusEffectProfile, durationOverrideInSecs);
        //                existing.TickAccumulator = 0f;
        //                this.OnStatusEffectUpdated.Invoke(existing);
        //            }
        //            this.StatusEffectAggregatesDirty = true;
        //            break;

        //        case StatusEffectStackingPolicy.AdditiveStacks:
        //            if (existing == null)
        //            {
        //                int stacks = Mathf.Clamp(stacksToAdd, 1, statusEffectProfile.MaxStacks);
        //                var newAppliedStatusEffect = this.CreateNew(statusEffectProfile, source, potencyScale, durationOverrideInSecs, stacks);
        //                this.ActiveAppliedStatusEffects.Add(newAppliedStatusEffect);
        //                this.OnStatusEffectApplied.Invoke(newAppliedStatusEffect);
        //            }
        //            else
        //            {
        //                int newStacks = Mathf.Clamp(existing.Stacks + stacksToAdd, 1, statusEffectProfile.MaxStacks);
        //                existing.Stacks = newStacks;
        //                existing.PotencyScale = StatusEffectController.ResolvePotency(statusEffectProfile, source, potencyScale);
        //                existing.RemainingDuration = StatusEffectController.ResolveDuration(statusEffectProfile, durationOverrideInSecs); // Refresh on re-apply
        //                existing.TickAccumulator = 0f;
        //                this.OnStatusEffectUpdated.Invoke(existing);
        //            }
        //            this.StatusEffectAggregatesDirty = true;
        //            break;

        //        case StatusEffectStackingPolicy.StrongestOnly:
        //            if (existing == null)
        //            {
        //                var newAppliedStatusEffect = this.CreateNew(statusEffectProfile, source, potencyScale, durationOverrideInSecs, 1);
        //                this.ActiveAppliedStatusEffects.Add(newAppliedStatusEffect);
        //                this.OnStatusEffectApplied.Invoke(newAppliedStatusEffect);
        //            }
        //            else
        //            {
        //                float newPotency = StatusEffectController.ResolvePotency(statusEffectProfile, source, potencyScale);
        //                if (newPotency >= existing.PotencyScale)
        //                {
        //                    existing.PotencyScale = newPotency;
        //                    existing.RemainingDuration = StatusEffectController.ResolveDuration(statusEffectProfile, durationOverrideInSecs);
        //                    existing.TickAccumulator = 0f;
        //                }
        //                //else
        //                //{
        //                //    // Optional: even weaker reapply still refreshes duration
        //                //    existing.RemainingDuration = StatusEffectController.ResolveDuration(statusEffectProfile, durationOverrideInSecs);
        //                //    existing.TickAccumulator = 0f;
        //                //}
        //                this.OnStatusEffectUpdated.Invoke(existing);
        //            }
        //            this.StatusEffectAggregatesDirty = true;
        //            break;  
        //    }

        //    this.RecomputeAggregatesIfDirty();
        //    return true;
        //}

        //public bool RemoveById(uint runtimeId)
        //{
        //    int idx = this.ActiveAppliedStatusEffects.FindIndex(activeAppliedStatusEffect => activeAppliedStatusEffect.RuntimeID == runtimeId);
        //    if (idx < 0) return false;

        //    var chosenActiveAppliedStatusEffect = this.ActiveAppliedStatusEffects[idx];
        //    this.ActiveAppliedStatusEffects.RemoveAt(idx);
        //    this.OnStatusEffectRemoved.Invoke(chosenActiveAppliedStatusEffect);

        //    this.StatusEffectAggregatesDirty = true;
        //    this.RecomputeAggregatesIfDirty();

        //    return true;
        //}

        //private int RemoveActiveStatusesByCondition(Func<AppliedStatusEffect, bool> predicate)
        //{
        //    if (predicate == null) return 0;

        //    int removed = 0;

        //    for (int i = this.ActiveAppliedStatusEffects.Count - 1; i >= 0; i--)
        //    {
        //        var chosenActiveAppliedStatusEffect = this.ActiveAppliedStatusEffects[i];
        //        if (!predicate(chosenActiveAppliedStatusEffect)) continue;

        //        this.ActiveAppliedStatusEffects.RemoveAt(i);
        //        this.OnStatusEffectRemoved.Invoke(chosenActiveAppliedStatusEffect);
        //        removed++;
        //    } 

        //    if (removed > 0)
        //    {
        //        this.StatusEffectAggregatesDirty = true;
        //        this.RecomputeAggregatesIfDirty();
        //    }

        //    return removed;
        //}

        //public int RemoveByTag(StatusTag tag)
        //{

        //    return this.RemoveActiveStatusesByCondition(activeAppliedStatusEffect => StatusEffectController.StatusEffectProfileHasTag(activeAppliedStatusEffect.StatusEffectProfile, tag));
        //}

        //public int Dispel(DispelClass dispelClass, bool onlyDebuffs = true)
        //{
        //    return this.RemoveActiveStatusesByCondition
        //    (
        //        activeAppliedStatusEffect =>
        //        {
        //            var statusEffectProfile = activeAppliedStatusEffect.StatusEffectProfile;
        //            if (statusEffectProfile == null) return true;                 // defensive cleanup
        //            if (statusEffectProfile.Undispellable) return false;          // can't dispel these
        //            if (onlyDebuffs && statusEffectProfile.IsBeneficial) return false;
        //            if (dispelClass == DispelClass.All) return true;
        //            return statusEffectProfile.DispelClass == dispelClass;
        //        }
        //    );
        //}

        //public bool HasTag(StatusTag targetTag)
        //{
        //    for (int i = 0; i < this.ActiveAppliedStatusEffects.Count; i++)
        //    {
        //        if (StatusEffectController.StatusEffectProfileHasTag(this.ActiveAppliedStatusEffects[i].StatusEffectProfile, targetTag)) return true;
        //    }

        //    return false;
        //}

        //private void Start()
        //{
        //    // Emit initial aggregates so listeners initialize correctly
        //    this.CurrentStatusEffectAggregatesSnapshot = this.BuildStatusEffectAggregatesSnapshot();
        //    this.OnStatusEffectAggregatesChanged.Invoke(this.CurrentStatusEffectAggregatesSnapshot);
        //}

        //private void Update()
        //{
        //    float dt = this.UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        //    if (dt <= 0 || this.ActiveAppliedStatusEffects.Count == 0) return;

        //    for (int i = this.ActiveAppliedStatusEffects.Count - 1; i >= 0; i--)
        //    {
        //        var activeAppliedStatusEffect = this.ActiveAppliedStatusEffects[i];

        //        if (activeAppliedStatusEffect.StatusEffectProfile == null)
        //        {
        //            this.ActiveAppliedStatusEffects.RemoveAt(i);
        //            this.OnStatusEffectRemoved.Invoke(activeAppliedStatusEffect);
        //            this.StatusEffectAggregatesDirty = true;
        //            continue;
        //        }

        //        activeAppliedStatusEffect.RemainingDuration -= dt;

        //        // Periodic ticking
        //        if (activeAppliedStatusEffect.StatusEffectProfile.TickSchedule != TickSchedule.None && activeAppliedStatusEffect.StatusEffectProfile.TickMode != PeriodicStatusEffectTickMode.None)
        //        {
        //            float interval = (activeAppliedStatusEffect.StatusEffectProfile.TickSchedule == TickSchedule.EverySecond) 
        //                ? 1f 
        //                : Mathf.Max(activeAppliedStatusEffect.StatusEffectProfile.TickInterval, 0.01f);

        //            activeAppliedStatusEffect.TickAccumulator += dt;
        //            while (activeAppliedStatusEffect.TickAccumulator >= interval)
        //            {
        //                activeAppliedStatusEffect.TickAccumulator -= interval;
        //                this.DoTick(activeAppliedStatusEffect);
        //            }
        //        }

        //        // Expiry
        //        if (activeAppliedStatusEffect.RemainingDuration <= 0f)
        //        {
        //            this.ActiveAppliedStatusEffects.RemoveAt(i);
        //            this.OnStatusEffectRemoved.Invoke(activeAppliedStatusEffect);
        //            this.StatusEffectAggregatesDirty = true;
        //        }
        //    }

        //    this.RecomputeAggregatesIfDirty();
        //}

        //private AppliedStatusEffect CreateNew(StatusEffectProfile statusEffectProfile, GameObject source, float potencyScale, float? durationOverrideInSecs, int stacks)
        //{
        //    return new AppliedStatusEffect
        //    {
        //        RuntimeID = this.NextRuntimeID++,
        //        StatusEffectProfile = statusEffectProfile,
        //        Source = source,
        //        PotencyScale = StatusEffectController.ResolvePotency(statusEffectProfile, source, potencyScale),
        //        Stacks = Mathf.Clamp(stacks, 1, statusEffectProfile.MaxStacks),
        //        RemainingDuration = StatusEffectController.ResolveDuration(statusEffectProfile, durationOverrideInSecs),
        //        TickAccumulator = 0f
        //    };
        //}

        //private const float MAX_POTENCY = 100f;

        //private static float ResolvePotency(StatusEffectProfile statusEffectProfile, GameObject source, float potencyScale)
        //{
        //    var value = statusEffectProfile.BasePotencyScale * Mathf.Max(potencyScale, 0f);
        //    return Mathf.Clamp(value, 0f, MAX_POTENCY);
        //}

        //private static float ResolveDuration(StatusEffectProfile statusEffectProfile, float? durationOverrideInSecs)
        //{
        //    return Mathf.Max(durationOverrideInSecs ?? statusEffectProfile.BaseDurationInSeconds, 0f);
        //}

        //private void DoTick(AppliedStatusEffect appliedStatusEffect)
        //{
        //    // Integer-safe base amount
        //    long baseAmount64 = (long)appliedStatusEffect.StatusEffectProfile.TickAmount * (long)appliedStatusEffect.Stacks;
        //    int baseAmount = baseAmount64 > int.MaxValue ? int.MaxValue
        //                   : baseAmount64 < 0 ? 0
        //                   : (int)baseAmount64;

        //    float scaled = baseAmount * Mathf.Max(0f, appliedStatusEffect.PotencyScale);
        //    int amount = Mathf.Clamp(Mathf.RoundToInt(scaled), 0, int.MaxValue);
        //    if (amount == 0) return;

        //    if (appliedStatusEffect.StatusEffectProfile.TickMode == PeriodicStatusEffectTickMode.DamageOverTime)
        //    {
        //        this.OnDamageOverTimeTick.Invoke(amount);
        //    }
        //    else if (appliedStatusEffect.StatusEffectProfile.TickMode == PeriodicStatusEffectTickMode.HealOverTime)
        //    {
        //        this.OnHealOverTimeTick.Invoke(amount);
        //    }
        //}

        //private static bool StatusEffectProfileHasTag(StatusEffectProfile statusEffectProfile, StatusTag targetTag)
        //{
        //    if (statusEffectProfile == null || statusEffectProfile.StatusTags == null) return false;

        //    var tags = statusEffectProfile.StatusTags;
        //    for (int i = 0; i < tags.Length; i++) 
        //    {
        //        if (tags[i] == targetTag) return true;
        //    }

        //    return false;
        //}

        //private const float MULTIPLIER_FLOOR = 0.01f;

        //// --- Per-(Key,Formula) accumulator for stacking policy ---
        //// We’ll fold at the end for each stat that uses it.
        //// Note: values remain “percentage deltas” (e.g., +0.10f == +10%) for AddPercentage,
        //// and “mult deltas” (e.g., +0.10f means x1.10 after folding) for Multiply.
        //private static void Accumulate(ref Dictionary<(StatModifierKey, StatModifierFormula), List<float>> map, StatModifierKey smKey, StatModifierFormula formula, float value)
        //{
        //    map ??= new Dictionary<(StatModifierKey, StatModifierFormula), List<float>>(32);
        //    var mapKey = (smKey, formula);
        //    if (!map.TryGetValue(mapKey, out var list))
        //    {
        //        list = new List<float>(4);
        //        map[mapKey] = list;
        //    }

        //    list.Add(value);
        //}


        //// Folding with stacking policy
        //private float Fold(List<float> values, StatModifierStackingPolicy policy, StatModifierFormula formula)
        //{
        //    if (values == null || values.Count == 0) return 0f;

        //    switch (policy)
        //    {
        //        case StatModifierStackingPolicy.HighestOnly:
        //            float max = values[0];
        //            for (int i = 1; i < values.Count; i++)
        //            {
        //                if (values[i] > max) 
        //                    max = values[i];
        //            }
        //            return max;
        //        case StatModifierStackingPolicy.LowestOnly:
        //            float min = values[0];
        //            for (int i = 1; i < values.Count; i++)
        //            {
        //                if (values[i] < min) 
        //                    min = values[i];
        //            }
        //            return min;
        //        default: // StackAll
        //            if (formula == StatModifierFormula.Multiply)
        //            {
        //                float m = 1f;
        //                for (int i = 0; i < values.Count; i++)
        //                {
        //                    m *= (1f + values[i]);
        //                }
        //                return m - 1f; // return as net percentage delta
        //            }

        //            float s = 0f;
        //            for (int i = 0; i < values.Count; i++)
        //            {
        //                s += values[i];
        //            }
        //            return s;
        //    }
        //}

        //// Helper to read folded percent/mult for a stat using your existing StatModifier struct (has StackingPolicy)
        //private float ReadPercent(ref Dictionary<(StatModifierKey, StatModifierFormula), List<float>> perStatMap, StatModifierKey smKey, StatModifierStackingPolicy policy)
        //{
        //    List<float> list = null;
        //    if (perStatMap != null)
        //        perStatMap.TryGetValue((smKey, StatModifierFormula.AddPercentage), out list);

        //    return this.Fold(list, policy, StatModifierFormula.AddPercentage);
        //}
        //private float ReadMultiply(ref Dictionary<(StatModifierKey, StatModifierFormula), List<float>> perStatMap, StatModifierKey smKey, StatModifierStackingPolicy pol)
        //{
        //    List<float> list = null;
        //    if (perStatMap != null)
        //        perStatMap.TryGetValue((smKey, StatModifierFormula.Multiply), out list);

        //    return this.Fold(list, pol, StatModifierFormula.Multiply);
        //}

        //private StatusEffectAggregates BuildStatusEffectAggregatesSnapshot()
        //{
        //    // Build a fresh snapshot to avoid mutating old instance before emitting
        //    var sa = StatusEffectAggregates.Default();

        //    // Collectors
        //    float percentDamageTaken    = 0f, multiplyDamageTaken       = 1f;
        //    float percentDamageDealt    = 0f, multiplyDamageDealt       = 1f;
        //    float percentKnockbackTaken = 0f, multiplyKnockbackTaken    = 1f;
        //    float percentMoveSpeed      = 0f, multiplyMoveSpeed     = 1f;
        //    float percentAttackSpeed    = 0f, multiplyAttackSpeed   = 1f;
        //    float percentPoiseDamageTaken = 0f, multiplyPoiseDamageTaken = 1f;

        //    // Flats
        //    int flatMinStaminaReserve = 0;
        //    float flatStaminaRegenPerSec    = 0f, flatFPRegenPerSec     = 0f;
        //    float flatStaminaRegenDelaySecs = 0f, flatFPRegenDelaySecs  = 0f;
        //    float flatMaxHP = 0f;
        //    float flatPoiseRegen = 0f;

        //    // Cost multipliers are stored as percent-add deltas in aggregates
        //    float percentStaminaCost = 0f, percentFPCost = 0f;

        //    Dictionary<(StatModifierKey, StatModifierFormula), List<float>> perStatMap = null;

        //    // Iterate statuses
        //    for (int i = 0; i < this.ActiveAppliedStatusEffects.Count; i++)
        //    {
        //        var activeAppliedStatusEffect = this.ActiveAppliedStatusEffects[i];
        //        if (activeAppliedStatusEffect.StatusEffectProfile == null) continue;

        //        // CC flags
        //        if (activeAppliedStatusEffect.StatusEffectProfile.AppliesSilence) sa.Silenced = true;
        //        if (activeAppliedStatusEffect.StatusEffectProfile.AppliesFatigue) sa.Fatigued = true;
        //        if (activeAppliedStatusEffect.StatusEffectProfile.AppliesStun) sa.Stunned = true;

        //        // Apply stat modifiers (stacking per-entry; when you switch to ModifierStackingPolicy we can refine)
        //        var statModifiers = activeAppliedStatusEffect.StatusEffectProfile.StatModifiers;
        //        if (statModifiers == null || statModifiers.Length <= 0) continue;

        //        // Apply stat modifiers (stack-scaled, NaN/Inf guarded)
        //        int stacks = Mathf.Max(1, activeAppliedStatusEffect.Stacks);
        //        for (int j = 0; j < statModifiers.Length; j++)
        //        {
        //            var statModifier = statModifiers[j];
        //            float value = statModifier.Value * stacks; // Stack-scaled
        //            if (float.IsNaN(value) || float.IsInfinity(value)) continue;

        //            switch (statModifier.TargetStat)
        //            {
        //                // --- Costs ---
        //                case StatModifierKey.StaminaCostMultiplier:
        //                    if (statModifier.Formula == StatModifierFormula.AddPercentage) percentStaminaCost += value;
        //                    else if (statModifier.Formula == StatModifierFormula.Multiply) StatusEffectController.Accumulate(ref perStatMap, statModifier.TargetStat, statModifier.Formula, value);
        //                    break;
        //                case StatModifierKey.FPCostMultiplier:
        //                    if (statModifier.Formula == StatModifierFormula.AddPercentage) percentFPCost += value;
        //                    else if (statModifier.Formula == StatModifierFormula.Multiply) StatusEffectController.Accumulate(ref perStatMap, statModifier.TargetStat, statModifier.Formula, value);
        //                    break;

        //                // --- Reserve/regen/delay flats ---
        //                case StatModifierKey.MinStaminaReserveFlat:         flatMinStaminaReserve       += Mathf.RoundToInt(value); break;
        //                case StatModifierKey.StaminaRegenPerSecondFlat:     flatStaminaRegenPerSec      += value; break;
        //                case StatModifierKey.FPRegenPerSecondFlat:          flatFPRegenPerSec           += value; break;
        //                case StatModifierKey.StaminaRegenDelaySecondsFlat:  flatStaminaRegenDelaySecs   += value; break;
        //                case StatModifierKey.FPRegenDelaySecondsFlat:       flatFPRegenDelaySecs        += value; break;

        //                case StatModifierKey.MaxHealthFlat: flatMaxHP += value; break;

        //                case StatModifierKey.PoiseRegenPerSecondFlat: flatPoiseRegen += value; break;

        //                // --- Damage model multipliers ---
        //                case StatModifierKey.DamageTakenMultiplier:
        //                case StatModifierKey.DamageDealtMultiplier:
        //                case StatModifierKey.KnockbackTakenMultiplier:
        //                case StatModifierKey.MoveSpeedMultiplier:
        //                case StatModifierKey.AttackSpeedMultiplier:
        //                case StatModifierKey.PoiseDamageTakenMultiplier:
        //                    StatusEffectController.Accumulate(ref perStatMap, statModifier.TargetStat, statModifier.Formula, value);
        //                    break;
        //            }
        //        }
        //    }

        //    // Assemble final aggregated snapshot

        //    //  Cost scales: percent adds + treat multiply deltas as extra percent
        //    sa.StaminaCostScale = percentStaminaCost
        //                        + this.ReadPercent  (ref perStatMap, StatModifierKey.StaminaCostMultiplier, StatModifierStackingPolicy.StackAll)
        //                        + this.ReadMultiply (ref perStatMap, StatModifierKey.StaminaCostMultiplier, StatModifierStackingPolicy.StackAll);
        //    sa.FPCostScale  = percentFPCost
        //                    + this.ReadPercent  (ref perStatMap, StatModifierKey.FPCostMultiplier, StatModifierStackingPolicy.StackAll)
        //                    + this.ReadMultiply (ref perStatMap, StatModifierKey.FPCostMultiplier, StatModifierStackingPolicy.StackAll);

        //    //  Flats
        //    sa.MinStaminaReserveFlat        = flatMinStaminaReserve;
        //    sa.StaminaRegenPerSecondDelta   = flatStaminaRegenPerSec;
        //    sa.FPRegenPerSecondDelta        = flatFPRegenPerSec;
        //    sa.StaminaRegenDelayDelta       = flatStaminaRegenDelaySecs;
        //    sa.FPRegenDelayDelta            = flatFPRegenDelaySecs;

        //    sa.MaxHealthDelta = flatMaxHP;

        //    sa.PoiseRegenDelta = flatPoiseRegen;

        //    //  Percent/multiply families
        //    percentDamageTaken += this.ReadPercent(ref perStatMap, StatModifierKey.DamageTakenMultiplier, StatModifierStackingPolicy.StackAll);
        //    multiplyDamageTaken *= 1f + this.ReadMultiply(ref perStatMap, StatModifierKey.DamageTakenMultiplier, StatModifierStackingPolicy.StackAll);

        //    percentDamageDealt += this.ReadPercent(ref perStatMap, StatModifierKey.DamageDealtMultiplier, StatModifierStackingPolicy.StackAll);
        //    multiplyDamageDealt *= 1f + this.ReadMultiply(ref perStatMap, StatModifierKey.DamageDealtMultiplier, StatModifierStackingPolicy.StackAll);

        //    percentKnockbackTaken += this.ReadPercent(ref perStatMap, StatModifierKey.KnockbackTakenMultiplier, StatModifierStackingPolicy.StackAll);
        //    multiplyKnockbackTaken *= 1f + this.ReadMultiply(ref perStatMap, StatModifierKey.KnockbackTakenMultiplier, StatModifierStackingPolicy.StackAll);

        //    percentMoveSpeed += this.ReadPercent(ref perStatMap, StatModifierKey.MoveSpeedMultiplier, StatModifierStackingPolicy.StackAll);
        //    multiplyMoveSpeed *= 1f + this.ReadMultiply(ref perStatMap, StatModifierKey.MoveSpeedMultiplier, StatModifierStackingPolicy.StackAll);

        //    percentAttackSpeed += this.ReadPercent(ref perStatMap, StatModifierKey.AttackSpeedMultiplier, StatModifierStackingPolicy.StackAll);
        //    multiplyAttackSpeed *= 1f + this.ReadMultiply(ref perStatMap, StatModifierKey.AttackSpeedMultiplier, StatModifierStackingPolicy.StackAll);

        //    percentPoiseDamageTaken += this.ReadPercent(ref perStatMap, StatModifierKey.PoiseDamageTakenMultiplier, StatModifierStackingPolicy.StackAll);
        //    multiplyPoiseDamageTaken *= 1f + this.ReadMultiply(ref perStatMap, StatModifierKey.PoiseDamageTakenMultiplier, StatModifierStackingPolicy.StackAll);

        //    sa.DamageTakenMultiplier    = Mathf.Max(MULTIPLIER_FLOOR, (1f + percentDamageTaken) * multiplyDamageTaken);
        //    sa.DamageDealtMultiplier    = Mathf.Max(MULTIPLIER_FLOOR, (1f + percentDamageDealt) * multiplyDamageDealt);
        //    sa.KnockbackTakenMultiplier = Mathf.Max(MULTIPLIER_FLOOR, (1f + percentKnockbackTaken) * multiplyKnockbackTaken);

        //    sa.MoveSpeedMultiplier          = Mathf.Max(MULTIPLIER_FLOOR, (1f + percentMoveSpeed) * multiplyMoveSpeed);
        //    sa.AttackSpeedMultiplier        = Mathf.Max(MULTIPLIER_FLOOR, (1f + percentAttackSpeed) * multiplyAttackSpeed);
        //    sa.PoiseDamageTakenMultiplier   = Mathf.Max(MULTIPLIER_FLOOR, (1f + percentPoiseDamageTaken) * multiplyPoiseDamageTaken);

        //    return sa;
        //}

        //// Compare a few hot fields first 
        //private static bool AggregatesAreDifferent(StatusEffectAggregates a, StatusEffectAggregates b)
        //{
        //    if (a.Silenced != b.Silenced || a.Fatigued != b.Fatigued || a.Stunned != b.Stunned) return true;

        //    if (!Mathf.Approximately(a.DamageTakenMultiplier,       b.DamageTakenMultiplier))       return true;
        //    if (!Mathf.Approximately(a.DamageDealtMultiplier,       b.DamageDealtMultiplier))       return true;
        //    if (!Mathf.Approximately(a.KnockbackTakenMultiplier,    b.KnockbackTakenMultiplier))    return true;
        //    if (!Mathf.Approximately(a.MoveSpeedMultiplier,     b.MoveSpeedMultiplier))     return true;
        //    if (!Mathf.Approximately(a.AttackSpeedMultiplier,   b.AttackSpeedMultiplier))   return true;
        //    if (!Mathf.Approximately(a.PoiseDamageTakenMultiplier, b.PoiseDamageTakenMultiplier)) return true;

        //    if (!Mathf.Approximately(a.StaminaCostScale,    b.StaminaCostScale))    return true;
        //    if (!Mathf.Approximately(a.FPCostScale,         b.FPCostScale))         return true;

        //    if (a.MinStaminaReserveFlat != b.MinStaminaReserveFlat) return true;

        //    if (!Mathf.Approximately(a.StaminaRegenPerSecondDelta,  b.StaminaRegenPerSecondDelta))  return true;
        //    if (!Mathf.Approximately(a.FPRegenPerSecondDelta,       b.FPRegenPerSecondDelta))       return true;
        //    if (!Mathf.Approximately(a.StaminaRegenDelayDelta,  b.StaminaRegenDelayDelta))  return true;
        //    if (!Mathf.Approximately(a.FPRegenDelayDelta,       b.FPRegenDelayDelta))       return true;
        //    if (!Mathf.Approximately(a.PoiseRegenDelta, b.PoiseRegenDelta)) return true;
        //    if (!Mathf.Approximately(a.MaxHealthDelta, b.MaxHealthDelta)) return true;

        //    // ...add any other fields that drive visuals/gameplay
        //    return false;
        //}

        //private void RecomputeAggregatesIfDirty()
        //{
        //    if (!this.StatusEffectAggregatesDirty) return;
        //    this.StatusEffectAggregatesDirty = false;

        //    var nextStatusEffectAggregates = this.BuildStatusEffectAggregatesSnapshot();
        //    if (!StatusEffectController.AggregatesAreDifferent(this.CurrentStatusEffectAggregatesSnapshot, nextStatusEffectAggregates)) return;

        //    // Swap snapshot and notify
        //    this.CurrentStatusEffectAggregatesSnapshot = nextStatusEffectAggregates;
        //    this.OnStatusEffectAggregatesChanged.Invoke(this.CurrentStatusEffectAggregatesSnapshot);
        //}
    }
}
