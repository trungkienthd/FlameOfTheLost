# Repository Guidelines

## Project Structure & Module Organization
`FlameOfTheLost` is a Unity 6 project (`ProjectSettings/ProjectVersion.txt` shows `6000.3.9f1`). Core game code lives in `Assets/Resources/Scripts` under the `FlameOfTheLost` namespace. Use the existing split:
- `Gameplay/` for runtime systems such as character, combat, inventory, camera, and UI.
- `Core/` for shared utilities and serializable dictionaries.
- `Editor/` for Unity editor tooling only.

Data assets and prefabs live under `Assets/Resources/Data` and other `Assets/Resources/*` folders. Project docs and diagrams live in `Documents/`. Treat `Library/`, `Logs/`, `Temp/`, `obj/`, and `ProfilerCaptures/` as generated output, not review targets.

## Build, Test, and Development Commands
Open the project in Unity Hub with editor `6000.3.9f1`, or from a shell:

```powershell
Unity.exe -projectPath D:\Trungkienthd\FlameOfTheLost
```

Useful local commands:
- `dotnet build Assembly-CSharp.csproj` checks runtime C# compilation outside the editor.
- `dotnet build Assembly-CSharp-Editor.csproj` checks editor scripts.
- Unity Test Runner or `Unity.exe -batchmode -projectPath . -runTests -testPlatform EditMode -logFile Logs\editmode-tests.log` runs automated tests.

## Coding Style & Naming Conventions
Follow the current C# style: 4-space indentation, braces on new lines, PascalCase for types/methods/properties, `_camelCase` for serialized private fields, and `ALL_CAPS` only when Unity or constants justify it. Keep runtime scripts in the `FlameOfTheLost` namespace.

Asset naming is repository-specific. Follow `Documents/AssetNamingConvention.md`: third-party assets keep source prefixes such as `BGE118_` or `CFRPG16_`; custom assets use `FTL_`. Never rename `.meta` files manually.

## Testing Guidelines
`com.unity.test-framework` is installed, but this repo currently has no committed test suites. Add new tests under `Assets/Resources/Scripts/Tests` with clear `*Tests.cs` names and prefer Edit Mode tests for pure logic. Cover gameplay utilities, status-effect logic, and inventory/loadout rules when changing those systems.

## Commit & Pull Request Guidelines
Recent history uses direct subject lines like `Commit - Design and Implement Inventory & Loadout Panels`. Keep commits imperative, scoped, and readable: `Commit - Add status effect purge guard`. PRs should include a short summary, affected scenes/prefabs, linked task or issue, and screenshots or GIFs for UI/visual changes.
