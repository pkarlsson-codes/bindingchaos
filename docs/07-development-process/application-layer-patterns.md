# Application Layer Patterns

**Command** — `sealed record` with strongly-typed ID parameters.

**Query** — same shape as commands.

**Handler** — `static class` with a static `Handle()` method. Returns a strongly-typed ID when the operation produces a new entity, otherwise `Task`.

**Domain events** — use `string` IDs, not typed IDs. Events cross serialization boundaries; the typed wrapper is applied when reconstituting the aggregate.

**HTTP boundary** — contracts (`Requests`/`Responses`) use `string` IDs. Controllers convert once at the boundary using `EntityId.Create()`, and extract `.Value` when building the response.

**Repository** — interfaces live in the Domain layer and extend `IRepository<TAggregate, TId>`. Implementations live in `Infrastructure/Persistence` and extend `MartenRepository<TAggregate, TId>`. Registered as `Scoped`.

**Persistence in handlers** — `repository.Stage(aggregate)` then `unitOfWork.CommitAsync(cancellationToken)`.

**Read models** — named `*View`. Projections live in `Infrastructure/Projections` and extend a Marten projection base class. Projection methods (`Create()`, `Apply()`) are static. Projections are registered as `ProjectionLifecycle.Async`.

**Controllers** — `sealed class` extending `BaseApiController`. Participant extracted via `HttpContext.GetParticipantIdOrAnonymous()`.

**Exception handling** — three global handlers, applied in order: `AggregateNotFoundException` → 404, `DomainException` → 422, unhandled → 500. All return ProblemDetails.

**Tests** — unit tests use a nested class structure (`TheHandleMethod` inside `HandlerTests`). Moq for mocking, FluentAssertions for assertions, xUnit `[Fact]`.