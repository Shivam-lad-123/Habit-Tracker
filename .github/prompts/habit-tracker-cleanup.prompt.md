---
description: "Use when reviewing Habit Tracker endpoints, services, validators, or DTOs for a safe cleanup pass."
name: "Habit Tracker Cleanup"
argument-hint: "feature, file, or endpoint"
agent: "C#/.NET Janitor"
---

Review the selected Habit Tracker code or the nearest related files in the workspace.

- Keep behavior and public API shape unchanged unless the task explicitly asks for a functional change.
- Prefer idiomatic modern C# and match the existing project style.
- Remove dead code, simplify logic, and call out any validation or test updates that should accompany the change.
- Return a concise summary with the safest change you would make first.