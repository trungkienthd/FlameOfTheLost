# Spawning

**Major Range:** `2xxxx`

---

## Asset Type

- ScriptableObject

---

## Profile Types

```text
200xx   PlayerDefaultSpawnPointProfile
201xx   EnemyDefaultSpawnPointProfile

210xx   DeathTriggerProfile
```

---

## Sub-Ranges

```text
200xx   Player Default Spawn Point Profiles
201xx   Enemy Default Spawn Point Profiles

202xx   Reserved
203xx   Reserved
204xx   Reserved
205xx   Reserved
206xx   Reserved
207xx   Reserved
208xx   Reserved
209xx   Reserved

210xx   Death Trigger Profiles

211xx   Reserved
212xx   Reserved
213xx   Reserved
214xx   Reserved
215xx   Reserved
216xx   Reserved
217xx   Reserved
218xx   Reserved
219xx   Reserved

22xxx   Reserved
23xxx   Reserved
24xxx   Reserved
25xxx   Reserved
26xxx   Reserved
27xxx   Reserved
28xxx   Reserved
29xxx   Reserved
```

---

## Assignment Rules

### Default Spawn Points

The ID identifies a **default spawn point definition**.

Runtime spawn point behaviours do not own independent IDs.

A runtime spawn point references its corresponding ScriptableObject profile when an ID is required.

---

### Death Triggers

The ID identifies a **death trigger definition**.

Runtime death trigger behaviours do not own independent IDs.

A runtime death trigger references its corresponding ScriptableObject profile when an ID is required.

---

## Examples

```text
20001   Tutorial Area 1 Player Spawn
20002   Tutorial Area 2 Player Spawn

20101   Tutorial Area 3 Enemy Spawn
20102   Forest Enemy Spawn

21001   Tutorial Pitfall
21002   Deep Water
21003   Lava
```

---

**End of Document**
