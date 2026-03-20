# CLAUDE.md (BindingChaos)

## Quick npm Commands
- **Infra:** `infra:start` | `reset:all`
- **Backend:** `dotnet:build` | `dotnet:test` | `dotnet:format`
- **Frontend:** `frontend:start` | `frontend:test` | `frontend:generate`
- **Migrations:** `migration:generate:identity`

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

## Additional documentation
- docs/02-architecture/high-level.md
- docs/07-development-process/adding-a-bounded-context.md
- docs/07-development-process/adding-a-functional-slice.md
- docs/07-development-process/authorization-flow.md
- docs/07-development-process/coding-standards.md