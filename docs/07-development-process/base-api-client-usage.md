# Base API Client

Gateway HTTP clients for CorePlatform inherit `BaseApiClient` (`src/BindingChaos.Infrastructure/API/BaseApiClient.cs`).

The base provides request helpers (`GetAsync`, `PostAsync`, `PutAsync`, `DeleteAsync`, `GetCollectionAsync`, `PostFormAsync`) that execute through a Polly resilience pipeline and deserialize `ApiResponse<T>`, returning only `Data`. `CorrelationIdHandler` is registered in `BindingChaos.CorePlatform.Clients/ServiceCollectionExtensions.cs`; `InternalGatewayAuthHandler` is attached by Gateway so outbound calls carry the internal JWT.

## Adding a client

1. Add interface `I<Feature>ApiClient` in `src/BindingChaos.CorePlatform.Clients`.
2. Add a class inheriting `BaseApiClient`; implement only endpoint-specific mapping using the base helpers rather than raw `HttpClient`.
3. Register with `Add<Feature>ApiClient(baseAddress)` in `BindingChaos.CorePlatform.Clients/ServiceCollectionExtensions.cs`.
4. Wire registration from `BindingChaos.Web.Gateway/Configuration/ServiceCollectionExtensions.cs` and attach `InternalGatewayAuthHandler`.

Throw on missing `Data` for single-resource calls; use `GetCollectionAsync` for nullable collections. Pass `CancellationToken` through from the caller. Keep client logic transport-only — no domain orchestration.
