# Bounded Context Setup Guide

Use this guide to create a new bounded context project skeleton.

## Scope

- Includes: project scaffold, references, context DI, context Marten config, host wiring.
- Excludes: domain design, aggregate strategy, endpoint design.

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

## Setup Steps

1. Create `src/BindingChaos.<Context>/BindingChaos.<Context>.csproj` and target `net10.0`.
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

## Reference Rules

- Keep `.csproj` lean.
- Add `BindingChaos.IntegrationEvents` only when needed.
- Add `Marten` package only when direct APIs in this project require it.
- Do not register host-level Marten services in context DI.

## Definition Of Done

- Project builds.
- Context is referenced by `BindingChaos.CorePlatform.API`.
- `Add<Context>(...)` exists and is called by host composition.
- `<Context>MartenConfiguration.Configure(...)` is called by host composition.
- Wolverine discovery includes `<Context>AssemblyReference.Assembly`.
- Optional integration-event and seeding wiring exists only when required.

## Common Mistakes

- Registering host-level services inside context DI.
- Forgetting Wolverine assembly discovery registration.
- Adding packages before they are needed.
