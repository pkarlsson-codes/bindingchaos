# Base API Client Usage Guide

Use this guide when adding or updating CorePlatform HTTP clients in Gateway.

## Scope

- Includes: client inheritance, method patterns, DI registration, handler pipeline.
- Excludes: endpoint design, controller contracts, frontend client generation.

## How It Works

1. Clients inherit `BaseApiClient` (`src/BindingChaos.Infrastructure/API/BaseApiClient.cs`).
2. Request helpers (`GetAsync`, `PostAsync`, `PutAsync`, `DeleteAsync`, `GetCollectionAsync`, `PostFormAsync`) execute via Polly resilience pipeline.
3. Responses are deserialized from `ApiResponse<T>` and return `Data`.
4. `CorrelationIdHandler` is attached in `BindingChaos.CorePlatform.Clients/ServiceCollectionExtensions.cs`.
5. Gateway adds `InternalGatewayAuthHandler` so outbound calls include internal JWT.

## Implementation Steps

1. Add interface in `src/BindingChaos.CorePlatform.Clients` (`I<Feature>ApiClient`).
2. Add class inheriting `BaseApiClient` and implement only endpoint-specific mapping logic.
3. Use base helpers instead of raw `HttpClient` methods.
4. Register client with `Add<Feature>ApiClient(baseAddress)` in `BindingChaos.CorePlatform.Clients/ServiceCollectionExtensions.cs`.
5. Wire registration from `BindingChaos.Web.Gateway/Configuration/ServiceCollectionExtensions.cs` and attach `InternalGatewayAuthHandler`.

## Usage Rules

- Expect wrapped payloads: all successful responses should be `ApiResponse<T>`.
- Throw on missing `Data` for single-resource calls; use `GetCollectionAsync` for nullable collections.
- Pass through `CancellationToken` from caller.
- Keep API client logic transport-only; no domain orchestration.

## Definition Of Done

- Client compiles and is DI-registered.
- Calls use `BaseApiClient` helper methods.
- Correlation id and internal auth handlers are attached.
- Consumer controller/service uses interface, not concrete client.

## Common Mistakes

- Calling `HttpClient` directly and bypassing base retry/timeout behavior.
- Returning raw `ApiResponse<T>` from client methods instead of unwrapped `Data`.
- Forgetting client registration in both clients package and gateway composition.
- Putting business logic in API client implementations.
