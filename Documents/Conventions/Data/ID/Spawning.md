# Spawning

**Major Range:** `2xxxx`

---

## Asset Type

- ScriptableObject

---

## Profile Types

```text
20xxx   PlayerDefaultSpawnPointProfile

21xxx   CheckpointProfile

22xxx   Enemy Default Spawn Point Profiles
220xx   Territorial Enemy Default Spawn Point Profiles

23xxx   Neutral Character Default Spawn Point Profiles
230xx   Animal Default Spawn Point Profiles

29xxx   DeathTriggerProfile
```

---

## Sub-Ranges

```text
20xxx   Player Default Spawn Point Profiles

21xxx   Player Checkpoint Profiles

22xxx   Enemy Default Spawn Point Profiles

220xx   Territorial Enemy Default Spawn Point Profiles

221xx   Reserved
222xx   Reserved
223xx   Reserved
224xx   Reserved
225xx   Reserved
226xx   Reserved
227xx   Reserved
228xx   Reserved
229xx   Reserved

23xxx   Neutral Character Default Spawn Point Profiles

230xx   Animal Default Spawn Point Profiles

231xx   Reserved
232xx   Reserved
233xx   Reserved
234xx   Reserved
235xx   Reserved
236xx   Reserved
237xx   Reserved
238xx   Reserved
239xx   Reserved

24xxx   Reserved
25xxx   Reserved
26xxx   Reserved
27xxx   Reserved
28xxx   Reserved

29xxx   Death Trigger Profiles
```

---

## Assignment Rules

### Player Default Spawn Points

The ID identifies a **Player Default Spawn Point definition**.

Runtime Player Default Spawn Point behaviours do not own independent IDs.

A runtime Player Default Spawn Point references its corresponding ScriptableObject profile when an ID is required.

Player Default Spawn Points use the `20xxx` range.

---

### Player Checkpoints

The ID identifies a **Player Checkpoint definition**.

Runtime Checkpoint behaviours do not own independent IDs.

A runtime Checkpoint references its corresponding ScriptableObject profile when an ID is required.

Player Checkpoints use the `21xxx` range.

---

### Enemy Default Spawn Points

The ID identifies an **Enemy Default Spawn Point definition**.

Runtime Enemy Default Spawn Point behaviours do not own independent IDs.

An Enemy Default Spawn Point references its corresponding ScriptableObject profile when an ID is required.

Enemy Default Spawn Points use the `22xxx` range.

The third digit further classifies Enemy Default Spawn Point types.

```text
220xx   Territorial Enemy Default Spawn Points
221xx-229xx   Reserved for additional Enemy Default Spawn Point categories
```

---

### Neutral Character Default Spawn Points

The ID identifies a **Neutral Character Default Spawn Point definition**.

Runtime Neutral Character Default Spawn Point behaviours do not own independent IDs.

A Neutral Character Default Spawn Point references its corresponding ScriptableObject profile when an ID is required.

Neutral Character Default Spawn Points use the `23xxx` range.

The third digit further classifies Neutral Character Default Spawn Point types.

```text
230xx   Animal Default Spawn Points
231xx-239xx   Reserved for additional Neutral Character Default Spawn Point categories
```

---

### Death Triggers

The ID identifies a **Death Trigger definition**.

Runtime Death Trigger behaviours do not own independent IDs.

A runtime Death Trigger references its corresponding ScriptableObject profile when an ID is required.

Death Triggers use the `29xxx` range.

---

## Examples

```text
20001   Tutorial Area 1 Player Default Spawn Point
20002   Tutorial Area 2 Player Default Spawn Point

21001   Tutorial Area 2 Player Checkpoint
21002   Tutorial Area 3 Player Checkpoint

22001   Greenvale Feralshroom Den Territorial Enemy Default Spawn Point

23001   Animal Default Spawn Point

29001   Tutorial Pitfall
29002   Deep Water
29003   Lava
```

---

**End of Document**
