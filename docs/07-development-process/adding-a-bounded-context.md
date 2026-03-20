# Bounded Context Setup

## Scaffold

```text
src/BindingChaos.<Context>/
  Application/
    Commands/
    Queries/
    ReadModels/
    Services/
  Domain/
  Infrastructure/
    AssemblyReference.cs
    DependencyInjection.cs
    Persistence/
      <Context>MartenConfiguration.cs
    Projections/
    IntegrationEventMappers/  (optional)
    IntegrationEventHandlers/ (optional)
    Seeding/                  (optional, dev-only)
```

## Steps

1. Create `src/BindingChaos.<Context>/BindingChaos.<Context>.csproj` targeting `net10.0`.
2. Add only required references/packages.
3. Add `Infrastructure/AssemblyReference.cs` with `<Context>AssemblyReference.Assembly` for Wolverine discovery.
4. Add `Infrastructure/DependencyInjection.cs` with `Add<Context>(IServiceCollection, IConfiguration)` for context-local registrations.
5. Add `Infrastructure/Persistence/<Context>MartenConfiguration.cs` and register schema, read models, and async projections.
6. Wire context into `BindingChaos.CorePlatform.API`:
   - add project reference
   - call `services.Add<Context>(configuration)`
   - call `<Context>MartenConfiguration.Configure(options)`
   - include `<Context>AssemblyReference.Assembly` in Wolverine discovery
   - add dev seeding only if needed
7. Add integration-event mappers/handlers only when the context publishes or consumes integration events.

## References

- Keep `.csproj` lean; add packages only when required by this project directly.
- Add `BindingChaos.IntegrationEvents` only when publishing or consuming integration events.
- Add `Marten` package only when this project uses Marten APIs directly.
- Register only context-local services in `Add<Context>`; host-level services (e.g. Marten store) belong in host composition.
