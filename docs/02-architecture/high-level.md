# BindingChaos Architecture (High-Level)

BindingChaos is implemented as a modular monolith backend with clear bounded contexts, plus a separate gateway and web frontend.

## Top-Level Shape

- **CorePlatform API** is the main backend host that composes multiple bounded-context modules behind one unified API surface.
- **Web Gateway** acts as a BFF for browser-facing concerns (auth/session/security) and forwards calls to CorePlatform through typed clients.
- **Web Frontend** is a separate SPA host.

## Bounded Contexts (implemented)

Core business capabilities are split into modules such as:

- SignalAwareness
- Ideation
- Tagging
- CommunityDiscourse
- LocalityManagement
- IdentityProfile
- Pseudonymity

## Bounded Contexts (planned)

- **Reputation** — trust graph, shunning, and societal approval standing. The primary sybil-resistance mechanism. See [trust-graph-sybil-resistance.md](../07-development-process/trust-graph-sybil-resistance.md).

These modules are developed as separate projects and then wired together in the CorePlatform host.

## Integration Style

Modules integrate primarily through an internal event-driven approach:

- Commands/queries are dispatched through Wolverine from API endpoints into application handlers.
- Domain events are mapped to shared integration events.
- Other modules consume those integration events via handlers/subscriptions.

This keeps cross-context dependencies looser than direct service-to-service calls.

## Persistence Strategy

The solution uses a hybrid persistence model on PostgreSQL:

- **Marten** for event/document-oriented modules and projections.
- **EF Core (Npgsql)** for selected relational contexts (notably IdentityProfile and LocalityManagement).

## Cross-Cutting Foundations

Shared technical concerns are centralized in shared projects:

- Shared domain/kernel primitives
- API base abstractions and response types
- Querying/pagination/sorting helpers
- Correlation ID and other infrastructure middleware

## Security & Runtime Boundaries

- The Gateway handles browser/OIDC/cookie/session concerns (Keycloak as IdP) and token storage.
- Gateway issues short-lived internal JWTs when calling CorePlatform APIs.
- CorePlatform validates internal JWTs and applies API authorization policies.

## Infrastructure (at a glance)

- **Hosting/runtime**: ASP.NET Core (.NET 10) services for Gateway and CorePlatform.
- **Data**: PostgreSQL as the main datastore, used via Marten and EF Core (Npgsql). Neo4j for graph data (trust relationships in the Reputation bounded context).
- **Messaging/dispatch**: Wolverine for in-process command/query dispatch and message handling.
- **Observability**: Serilog-based structured logging; health-check endpoints on hosts.
- **Identity/session infra**: Keycloak (OIDC) integration at Gateway; Redis-backed token/session storage is configured infrastructure used by the Gateway token store when enabled.
- **Document storage integration**: Minio-backed document management used by CorePlatform.

