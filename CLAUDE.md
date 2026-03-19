# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

### Infrastructure (required before running services)
```bash
npm run infra:start       # Start PostgreSQL, Redis, RabbitMQ, Keycloak, Minio via Docker
npm run infra:down        # Stop infrastructure
npm run reset:all         # Full reset and re-seed of all infrastructure
```

### Backend (.NET 10)
```bash
npm run dotnet:build      # Build entire solution
npm run dotnet:test       # Run all unit and integration tests
npm run dotnet:format     # Auto-format C# code
npm run dotnet:lint       # Verify formatting (no changes)
npm run api:start         # Start Core Platform API (port 5000)
npm run gateway:start     # Start Web Gateway/BFF (port 4000)

# Run a single test project
dotnet test src/BindingChaos.SignalAwareness.Tests
dotnet test src/BindingChaos.CorePlatform.API.IntegrationTests

# Generate EF migrations
npm run migration:generate:pseudonymity  # or :identity
npm run migration:apply:pseudonymity     # or :identity
```

### Frontend (React + Vite)
```bash
npm run frontend:start    # Start dev server (port 3000)
npm run frontend:test     # Run Jest tests
npm run frontend:lint     # ESLint
npm run frontend:format   # Format via prettier

# Regenerate TypeScript API client from Gateway OpenAPI spec (builds Gateway from source, no running process needed)
npm run frontend:generate
```

## Architecture Overview

### Three-Tier Service Architecture

```
Browser → Web (3000) → Web.Gateway/BFF (4000) → CorePlatform.API (5000)
```

- **BindingChaos.Web**: React + TypeScript + Vite frontend. Uses generated TypeScript client (`src/api/`) from the Gateway's OpenAPI spec. TanStack Query for server state. Tailwind + Radix UI + shadcn/ui components.
- **BindingChaos.Web.Gateway**: ASP.NET Core BFF. Handles OIDC auth with Keycloak, issues internal JWTs to Core API, aggregates/maps requests. Frontend proxies all API calls here.
- **BindingChaos.CorePlatform.API**: Main backend. Validates internal JWTs, hosts all bounded context controllers, dispatches commands/queries via Wolverine.
- **BindingChaos.DocumentProcessing**: Standalone worker service. Listens for Minio object-created events via RabbitMQ and generates attachment thumbnails.

### Bounded Context Structure

The solution is organized into domain-bounded contexts, each following the same internal pattern:

```
BindingChaos.<BoundedContext>/
  Domain/           # Aggregates, value objects, domain events, repository interfaces
  Application/
    Commands/       # Command records + static handler classes
    Queries/        # Query records + static handler classes
    DTOs/           # Data transfer objects
    ReadModels/     # Projections / read-side models
  Infrastructure/   # EF/Marten config, repository implementations, seeding

BindingChaos.<BoundedContext>.Contracts/   # Public events/contracts for cross-context integration
BindingChaos.<BoundedContext>.Tests/       # Unit tests mirroring Domain/ and Application/
```

**Bounded contexts:**
- **SignalAwareness** — signals, amplifications, suggested actions
- **Ideation** — ideas, amendments, amendment voting
- **CommunityDiscourse** — threaded discussion posts
- **Societies** — societies, social contracts, membership
- **Tagging** — tag management
- **Pseudonymity** — privacy-preserving participant pseudonyms (uses EF Core)
- **IdentityProfile** — identity storage (uses EF Core)

> **Note:** `Pseudonymity` and `IdentityProfile` are supporting service contexts, not DDD domain contexts. They use `Application/Services/` instead of the `Commands/`/`Queries/`/`ReadModels/` CQRS structure. This is intentional.

### Shared Kernel (`BindingChaos.SharedKernel`)

Contains CQRS primitives and DDD base types used across all bounded contexts:
- `AggregateRoot`, `Entity`, `ValueObject`, `EntityId`, `ParticipantId`, `SocietyId`
- `IUncommittedEvents` / `UncommittedEvents` for domain event tracking
- `IUnitOfWork` and repository patterns

### Message Handling (Wolverine)

Commands and queries are static handler classes dispatched via Wolverine (not MediatR). The `CorePlatform.API` discovers handlers by scanning bounded context assemblies:
```csharp
opts.Discovery.IncludeAssembly(typeof(SignalAwarenessAssemblyReference).Assembly);
```
Handler pattern: a static class with a `Handle(Command, Dependencies...)` method — Wolverine resolves parameters via DI.

### Persistence Strategy

- **Marten (PostgreSQL document/event store)**: SignalAwareness, Ideation, CommunityDiscourse, Tagging. Configured via `*MartenConfiguration` classes in each bounded context's Infrastructure.
- **Entity Framework Core (PostgreSQL relational)**: Pseudonymity, IdentityProfile, LocalityManagement. Migrations live under `Infrastructure/Persistence/Migrations/`.

### Auth Flow

1. Frontend authenticates with Keycloak via OIDC.
2. Web Gateway validates the Keycloak token, looks up the `ParticipantId`, and issues a short-lived internal JWT.
3. Core API validates this internal JWT (`InternalJwt:*` configuration). The `participant_id` claim is the name claim type.

### API Client Generation

The frontend TypeScript client (`src/BindingChaos.Web/src/api/`) is generated from the Gateway's OpenAPI spec. Regenerate when Gateway contracts change. The script builds the Gateway from source using NSwag — no running process needed:
```bash
npm run frontend:generate
```

## Code Conventions

### C# (Production Code)
- **Warnings are errors** (`TreatWarningsAsErrors=true`) in all non-test projects. Build will fail on any warning.
- All public members require XML doc comments (enforced by `GenerateDocumentationFile`).
- StyleCop analyzers are active. Run `npm run dotnet:format` before committing.
- Commands and queries are `sealed record` types. Handlers are `static` classes.
- Repository interfaces live in the `Domain` layer; implementations in `Infrastructure`.

### Testing
- Unit tests: xUnit + FluentAssertions in `*.Tests` projects.
- Integration tests: xUnit + Alba + `WebApplicationFactory` in `*.IntegrationTests` projects.
- Test projects have relaxed analyzer rules (warnings not errors, many CA rules suppressed).
- See [docs/testing-guide.md](docs/testing-guide.md) for structure, naming conventions, and what to test.

### Frontend
- Feature-based folder structure under `src/features/` (signals, ideas, amendments, auth, locality, etc.)
- Flat routing: `/signals`, `/ideas`, `/societies`, etc. (no locality prefix)
- React Query for data fetching; generated API client from `src/api/`
