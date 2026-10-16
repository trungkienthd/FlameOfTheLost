# Greenvale Crossroads — Gameplay and Progression Concepts

**Status:** Pre-implementation proposal, v0.1. This document describes intended behaviour, **not** approved component architecture.

## 1. Main experience

`Arrive → Meet NPCs → Shop / Accept Quest → Explore or prepare (optional) → Challenge Boss → Unlock East`.

Trading and side quests should improve readiness, not block the boss route. The map acts as a first demonstration that earlier social interactions can influence later combat.

## 2. NPCs and interactions

**Proposed combat NPC:** *Rowan, the Wandering Swordsman* (**working name**). He is encountered at the Traveller's Camp (R03), offers dialogue and a regional quest, and can become a temporary ally at the boss if an optional preparation condition has been met.

**Proposed merchant NPC:** Separate trader (name TBD) introducing buying and eventually selling. Separating merchant and ally/quest roles makes the settlement feel inhabited and encourages reusable interaction systems.

Possible interaction categories: Talk, Shop, Quest, Leave. The exact dialogue UI, branching logic, and action availability depend on quest state and are TBD.

## 3. Example quest concept (not final content)

**Working title:** *The Blocked Crossing*.

- **Main objective:** Investigate the eastern road and defeat the creature controlling the crossing.
- **Optional preparation:** Help Rowan gather supplies or otherwise prepare for battle.
- **Optional payoff:** Unlock Rowan as temporary assistance for the boss encounter.
- **After victory:** Eastern route opens, quest state updates, NPC dialogue acknowledges the outcome.

Previously suggested quantities (e.g. Wood and Stone), shop stock/prices, and exact quest rewards are illustrative and **not approved balance**.

## 4. Boss and ally encounter model

**Suggested first iteration:** Automatic encounter-scoped ally assistance, not a permanent companion or new permanent player keybind.

```text
Player reaches boss trigger in R10
           |
           +--> Start encounter / activate boss
           |
           +--> Is optional ally unlock earned?
           |          +--> No: solo encounter
           |          +--> Yes: temporary Rowan combat appearance
           |
        Encounter outcome
           +--> Player defeat: ordinary respawn / refresh rules
           +--> Boss defeat: record persistent milestone,
                            resolve quest, remove temporary ally,
                            unlock eastern transition
```

An ally should have autonomous target selection and combat behaviour, but the exact AI reuse/composition must be reviewed against existing Character architecture. Use a persistent **NPC identity/quest state** separate from temporary combat instances. Avoid duplicating NPC progression when the camp representation and combat representation differ.

### Behaviour questions to resolve

- Can the ally be defeated, downed, or retreat? Does that affect quests?
- Does the boss target the player and ally identically? How is aggro decided?
- What happens if the player leaves the arena or dies mid-encounter?
- Can the player repeat the encounter after a partial failure?
- Does Rowan return to camp immediately after victory or narratively travel back?

## 5. Quest, milestone, and save concepts

Keep **per-quest objectives** conceptually separate from **world-changing milestones**. Existing `GameConditionController`/milestone features may expose relevant milestones, while a future Quest System owns objective progress.

Candidate world milestones (names provisional):

```text
Rowan_FirstMet
Rowan_PreparationCompleted
Rowan_AllyUnlocked
GreenvaleCrossroads_BossDefeated
GreenvaleCrossroads_EasternExitUnlocked
```

Candidate quest states: not offered, offered, accepted, in progress, ready to complete, completed. Final state modelling is deferred.

### Reload expectations

- NPC dialogue/quest/ally unlock states are restored if committed as persistent data.
- On a normal world refresh, transient enemy/collectible/environment instances follow the existing refresh rules.
- A **permanently defeated boss does not respawn**; this requires special boss-state persistence beyond generic enemy respawning.
- Temporary combat allies should be recreated only when an active encounter requires them, not restored as permanently spawned duplicates.
- Access to the eastern next-map transition follows persistent boss state.

These are **design requirements**, not a claim that current save code already supports every one of them.

## 6. Priorities

**First playable slice:** camp NPC conversation → optional quest unlock → eastern boss approach → conditional temporary ally → victory milestone + passage unlock.

**Subsequent additions:** full merchant catalogue and selling, more quest variations, enhanced ally behaviours, additional NPC schedules and dynamic reactions.
