# Tutorial

**Major Range:** `4xxxx`

---

## Asset Type

- ScriptableObject

---

## Profile Types

```text
400xx   TutorialObjectProfile
41xxx   PromptTriggerProfile
```

---

## Sub-Ranges

```text
400xx   Tutorial Object Profiles

401xx   Reserved
402xx   Reserved
403xx   Reserved
404xx   Reserved
405xx   Reserved
406xx   Reserved
407xx   Reserved
408xx   Reserved
409xx   Reserved

41xxx   Tutorial Prompt Trigger Profiles

42xxx   Reserved
43xxx   Reserved
44xxx   Reserved
45xxx   Reserved
46xxx   Reserved
47xxx   Reserved
48xxx   Reserved
49xxx   Reserved
```

---

## Assignment Rules

### Tutorial Objects

The ID identifies a **reusable tutorial object definition**.

All instances using the same profile share the same ID.

---

### Tutorial Prompt Triggers

The ID identifies a **tutorial prompt trigger definition**.

Runtime prompt trigger behaviours do not own independent IDs.

A runtime prompt trigger references its corresponding ScriptableObject profile when an ID is required.

---

## Examples

```text
40001   Movement Tutorial Object
40002   Combat Tutorial Object

41001   Movement Input Prompt
41002   Loadout Input Prompt
41003   Normal Attack Input Prompt
41004   Dodge Input Prompt
```

---

**End of Document**
