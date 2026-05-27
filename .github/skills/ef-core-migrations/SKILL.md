---
name: ef-core-migrations
description: "Use when creating, reviewing, or validating EF Core migrations, DbContext changes, or seed updates for the Habit Tracker API."
---

# EF Core Migrations

Use this skill when the requested change affects the database schema, entity configuration, migration history, or seed data.

## When To Use

- Adding or changing properties on `Habit` or other EF Core entities.
- Updating `HabitTrackerDbContext`, model configuration, or seeding logic.
- Creating a new migration after a schema change.
- Checking whether the generated migration matches the model snapshot.

## Workflow

1. Inspect the model change first, especially the entity, `DbContext`, and any seeding code.
2. Generate the migration with `scripts/New-Migration.ps1`.
3. Review the generated migration and snapshot for destructive changes, nullable changes, and default values.
4. Validate the project with `scripts/Validate-Migration.ps1`.
5. If the app starts the database automatically, confirm startup still succeeds with the new migration.

## Guardrails

- Prefer generated migrations over hand-editing the snapshot.
- Keep migration names specific to the schema change.
- Avoid changing unrelated tables or seeding data unless the model change requires it.
- If a migration looks unsafe, stop and verify the model change before applying it.

## Helpful Commands

- `scripts/New-Migration.ps1 -Name AddHabitReminder`
- `scripts/Validate-Migration.ps1`
