# CLAUDE.md (BindingChaos)

## Quick Commands
- **Infra:** `npm run infra:start` | `npm run reset:all`
- **Backend:** `npm run dotnet:build` | `npm run dotnet:test` | `npm run dotnet:format`
- **Frontend:** `npm run frontend:start` | `npm run frontend:test` | `npm run frontend:generate`
- **Migrations:** `npm run migration:generate:<context>` (pseudonymity|identity)

## Architecture & Tech Stack
- **Flow:** Browser (3000) -> Web.Gateway/BFF (4000) -> CorePlatform.API (5000)
- **Backend:** .NET 10, Wolverine (CQRS), Marten (Doc/Event Store), EF Core (Relational).
- **Frontend:** React, Vite, TanStack Query, ShadcnUI. API client generated in `src/api/`.
- **Auth:** Keycloak (OIDC) -> Gateway -> Internal JWT -> Core API.

## Code Standards
- **C#:** `TreatWarningsAsErrors` is ON. Use `sealed record` for Cmds/Queries. Handlers are `static`.
- **Docs:** XML docs required for public members. Run `npm run dotnet:format` before commits.
- **Testing:** xUnit + FluentAssertions (Unit); Alba + WebApplicationFactory (Integration).

## Project Structure (Contexts)
Refer to these for domain logic:
- `SignalAwareness`, `Ideation`, `CommunityDiscourse`, `Societies`, `Tagging` (Marten)
- `Pseudonymity`, `IdentityProfile` (EF Core)

> **Note:** For deep architectural details or testing patterns, see `@docs/architecture.md` and `@docs/testing-guide.md`.