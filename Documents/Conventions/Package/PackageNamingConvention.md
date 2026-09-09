# Package Naming Convention
**Project:** FlameOfTheLost  
**Version:** 1.0  
**Scope:** Third-party packages imported through Unity Package Manager or Git dependencies

---

## 1. Purpose

This document defines the naming and usage convention for third-party packages integrated into the project through Unity Package Manager, including Git-based packages.

### Objectives

- Keep external package dependencies traceable
- Avoid modifying package-managed files directly
- Prevent merge conflicts caused by package updates
- Maintain clear separation between external package files and project-owned assets
- Provide a consistent prefix rule when package content is copied or adapted into `Assets/Resources/`
- Provide a consistent wrapper rule when package behaviours are exposed through project-owned scripts

---

## 2. Core Rule

Packages imported through Unity Package Manager and stored under `Packages/` should preserve their original package structure and filenames.

Do not rename files directly inside `Packages/`, because package updates may overwrite changes, break package references, or create unnecessary merge conflicts.

If any package asset is copied, modified, wrapped, or adapted into `Assets/Resources/`, the copied project-owned version must follow the project asset naming convention.

If any package behaviour is exposed through a project-owned C# wrapper class, the wrapper class should be placed under the appropriate `Assets/Resources/Scripts/` folder and inside the `FlameOfTheLost` namespace.

---

## 3. Naming Relationship with Asset Naming Convention

The asset naming convention applies to files placed inside `Assets/Resources/`.

Package files under `Packages/` are external dependencies and should not be renamed just to match the asset naming convention.

### Recommended Usage Pattern

```text
Packages/<PackageName>/...             Keep original package names.
Assets/Resources/... copied package    Use the registered package prefix.
Assets/Resources/... project asset     Use the project asset naming convention.
Assets/Resources/Scripts/... wrapper   Use normal project C# naming inside the FlameOfTheLost namespace.