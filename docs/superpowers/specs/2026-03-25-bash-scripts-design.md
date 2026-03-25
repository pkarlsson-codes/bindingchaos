# Cross-Platform Scripts: PowerShell → Bash

**Date:** 2026-03-25
**Status:** Approved

## Problem

All npm scripts in `scripts/` are PowerShell (`.ps1`), which requires PowerShell to be available. On Linux this fails unless PowerShell Core (`pwsh`) is separately installed. The immediate trigger is `reset:all` failing on first Linux run because Keycloak's schema wasn't seeded.

## Solution

Replace all `.ps1` scripts in `scripts/` with equivalent `.sh` bash scripts. Update `package.json` to invoke `bash` instead of `powershell`. Delete the `.ps1` files.

Windows users with WSL installed get transparent compatibility: npm invokes `bash.exe` (the WSL launcher, placed on PATH by WSL setup), which resolves the working directory automatically — no special configuration needed.

## File Mapping

Every `.ps1` in `scripts/` is replaced 1:1:

| Old | New |
|-----|-----|
| `scripts/reset-seed/orchestration/reset-area.ps1` | `scripts/reset-seed/orchestration/reset-area.sh` |
| `scripts/reset-seed/shared/reset-shared.ps1` | `scripts/reset-seed/shared/reset-shared.sh` |
| `scripts/reset-seed/app/reset-app.ps1` | `scripts/reset-seed/app/reset-app.sh` |
| `scripts/reset-seed/keycloak/reset-keycloak.ps1` | `scripts/reset-seed/keycloak/reset-keycloak.sh` |
| `scripts/reset-seed/keycloak/db-migrate-keycloak.ps1` | `scripts/reset-seed/keycloak/db-migrate-keycloak.sh` |
| `scripts/reset-seed/redis/reset-redis.ps1` | `scripts/reset-seed/redis/reset-redis.sh` |
| `scripts/reset-seed/rabbitmq/reset-rabbitmq.ps1` | `scripts/reset-seed/rabbitmq/reset-rabbitmq.sh` |
| `scripts/generate-api-clients.ps1` | `scripts/generate-api-clients.sh` |
| `scripts/add-migration.ps1` | `scripts/add-migration.sh` |

## Script Conventions

- `set -euo pipefail` at the top of every script (equivalent to `$ErrorActionPreference = "Stop"`)
- Shared functions live in `reset-shared.sh` and are sourced by other scripts via `. "$(dirname "$0")/../shared/reset-shared.sh"`
- Parameters passed as positional arguments (bash has no named parameters like PowerShell)
- Wait loops use `sleep 1` + a counter variable
- `docker compose` (v2) used, with a fallback to `docker-compose` (v1) — same logic as the current shared script
- All scripts made executable (`chmod +x`)

## package.json Changes

All `powershell -ExecutionPolicy Bypass -File` invocations replaced with `bash ./scripts/...`:

```
reset:app       → bash ./scripts/reset-seed/orchestration/reset-area.sh app
reset:keycloak  → bash ./scripts/reset-seed/orchestration/reset-area.sh keycloak
reset:redis     → bash ./scripts/reset-seed/orchestration/reset-area.sh redis
reset:rabbitmq  → bash ./scripts/reset-seed/orchestration/reset-area.sh rabbitmq
reset:all       → bash ./scripts/reset-seed/orchestration/reset-area.sh all
migration:apply:keycloak      → bash ./scripts/reset-seed/keycloak/db-migrate-keycloak.sh
migration:generate:pseudonymity → bash ./scripts/add-migration.sh src/BindingChaos.Pseudonymity PseudonymityDbContext
migration:generate:identity     → bash ./scripts/add-migration.sh src/BindingChaos.IdentityProfile IdentityProfileDbContext
frontend:generate → bash ./scripts/generate-api-clients.sh
```

## Out of Scope

- The `.ps1` files generated under `node_modules/.bin/` — these are npm shims for Windows CLI tools, not our scripts, and should not be touched.
- No new dependencies introduced.
