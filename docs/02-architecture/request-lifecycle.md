# BindingChaos Request Lifecycle (Gateway → API → Command/Query)

This document describes the implemented runtime request loop and the patterns it uses.

## Core Pattern Stack

- **BFF pattern**: Browser traffic enters `BindingChaos.Web.Gateway`.
- **Backend composition**: Gateway calls `BindingChaos.CorePlatform.API` through typed clients.
- **CQRS-style dispatch**: Core API controllers send command/query messages through Wolverine (`IMessageBus`).
- **Event-sourced write model + projected read model** (for event-sourced contexts):
  - Commands mutate aggregates by appending domain events.
  - Async projections build read models used by queries.
- **Module-to-module decoupling**: Domain events are mapped to integration events and consumed by other bounded contexts.

## End-to-End Entry Path

1. Client calls Gateway endpoint (for example, `/api/v1/signals`).
2. Gateway controller validates/normalizes request and calls a typed CorePlatform client.
3. Gateway auth handler injects an internal JWT (participant context/session info if available).
4. Typed client issues HTTP to Core API endpoint (for example, `/api/signals`).
5. Core API controller converts HTTP request to a command or query message.
6. Controller invokes Wolverine `IMessageBus.InvokeAsync<TResponse>(message)`.
7. Handler executes and returns result to controller.
8. Controller maps to contract response and returns to Gateway.
9. Gateway optionally reshapes to web view model and returns to caller.

## Query Path (read flow)

### What happens

1. Core API controller receives GET.
2. Controller constructs query message (example: `GetSignals`).
3. Wolverine routes query to query handler.
4. Query handler calls application service read method.
5. Application service reads from projected read models via `IQuerySession`.
6. Result is returned up the stack and mapped to API contracts.

### Characteristics

- No aggregate mutation.
- No new domain events appended.
- Fast read path over precomputed read models.
- Can include request-user context for response shaping (for example, amplification flags).

## Command Path (write flow)

### What happens

1. Core API controller receives POST/PUT/DELETE command endpoint.
2. Controller resolves participant identity and builds command message (example: `CaptureSignal`).
3. Wolverine routes command to command handler.
4. Command handler calls application service write method.
5. Application service loads/creates aggregate and invokes domain behavior.
6. Repository stages uncommitted domain events to Marten event stream.
7. Unit of Work commits (`SaveChangesAsync`) to persist events.
8. Marten async daemon processes committed events and updates read-model projections.
9. Domain-event handlers map/publish integration events via Wolverine for other modules.
10. Command response is returned (for example new aggregate id or updated count).

### Characteristics

- Write path is event-driven.
- Read models are updated asynchronously.
- Cross-context reactions are integration-event based, not direct in-process service calls.

## Why Query and Command Behave Differently

- **Query** requests prioritize read performance and shape data from projection tables/documents.
- **Command** requests prioritize invariant-safe domain mutation and event persistence.
- Because projections are async, there is an eventual-consistency window: a successful command may return before all read models are fully caught up.

## Security and Identity Through the Loop

- Gateway authenticates browser users (cookie + OIDC with Keycloak) and manages server-side token/session state.
- Redis is configured as token/session store infrastructure for the Gateway and is used when the token-store configuration is enabled.
- For Gateway → Core API calls, Gateway creates short-lived internal JWTs.
- Core API validates JWT and resolves participant id from claims for authorization/domain behavior.
- Anonymous vs authenticated behavior is enforced at command endpoints where needed.

## Practical Mental Model

Use this simplified model when reading or extending the system:

- **Gateway** = web-facing adapter + auth/session boundary.
- **Core API controllers** = transport mapping layer.
- **Wolverine messages** = application entry points (query/command).
- **Application services + aggregates** = business behavior.
- **Marten/EF persistence** = state storage.
- **Projections + integration events** = read optimization and bounded-context coordination.
