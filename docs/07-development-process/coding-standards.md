# Coding Standards

## C# Language Features

### 1. Collection Initialization
Use collection expressions `[]`:
```csharp
public List<string> Tags { get; internal set; } = [];
```

### 2. Constructor Syntax
Use `new()` when type is inferred:
```csharp
private static readonly object _lock = new();
```

### 3. Namespace Organization
Use file-level namespaces:
```csharp
namespace BindingChaos.Ideation.Infrastructure.Persistence.Entities;
```

### 4. Nullable Reference Types
Prefer non-nullable; use nullable only when value can be null:
```csharp
public string Value { get; }                      // Required
public string Title { get; set; } = string.Empty; // Required, initialized
public string? Description { get; set; }          // Optional
```

### 5. DateTime Naming
Use "At" for timestamps, "On" for dates:
```csharp
public DateTimeOffset CreatedAt { get; set; }
public DateTimeOffset DueOn { get; set; }
```

### 6. ConfigureAwait
Use `ConfigureAwait(false)` in library code:
```csharp
var result = await _repository.GetAsync(id).ConfigureAwait(false);
```

### 7. Curly Braces
Always use braces for control structures:
```csharp
if (condition)
{
    DoSomething();
}
```

## Code Organization

### 8. Using Statements
Group by namespace type:
```csharp
using BindingChaos.SharedKernel.Domain;

using Microsoft.Extensions.DependencyInjection;

using System;
```

### 9. Object Initialization
Use object initializers:
```csharp
var signal = new InternalSignal
{
    Id = Guid.NewGuid().ToString(),
    Title = signalViewModel.Title ?? string.Empty
};
```

### 10. Immutable Collections
Use `IReadOnlyCollection<T>` for public collections:
```csharp
public IReadOnlyCollection<Item> Items => _items.AsReadOnly();
```

## Naming Conventions

### 11. Method Naming
Use verb-noun pattern:
```csharp
public static Signal Create(SignalContent content, ParticipantId originatorId)
public void UpdateContent(SignalContent newContent)
```

### 12. Value Object Patterns
Use factory methods for value object creation:
```csharp
public static SignalId Generate() => new SignalId($"signal-{Guid.NewGuid():N}");
```

## Error Handling

### 13. Exception Handling
Use specific exception types and structured logging:
```csharp
catch (HttpRequestException ex)
{
    _logger.LogError(ex, "HTTP request failed for endpoint: {Endpoint}", endpoint);
    throw;
}
```

### 14. Warning Suppression
Use pragma directives with comments:
```csharp
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
private Signal(SignalId id) : base(id) { }
#pragma warning restore CS8618
```

## LINQ and Data Access

### 15. LINQ Patterns
Prefer method syntax:
```csharp
var signals = _signals.Select(s => MapToViewModel(s)).ToList();
```

### 16. Specification Pattern
Use specifications for complex queries:
```csharp
var spec = new ActiveSignalsSpecification()
    .And(new SignalsInLocalitySpecification(localityId));
```

## Async/Await

### 17. Async Consistency
Use async/await; never use `.Result` or `.Wait()`:
```csharp
public async Task<TResponse> HandleAsync(TRequest request)
{
    return await _repository.GetAsync(id);
}
```

## Domain-Driven Design

### 18. Aggregate Invariants
Implement as named methods passed to `AddInvariants`:
```csharp
AddInvariants(
    CreatorMustBeSpecified,
    StatusMustBeDefined);

private bool CreatorMustBeSpecified() => CreatorId != null;
```

### 19. Domain Event Naming
Use past tense:
```csharp
public class SignalCaptured : IDomainEvent
public class IdeaCreated : IDomainEvent
```

## API Design

### 20. Handler Naming
Use `HandleAsync` consistently:
```csharp
public async Task<Result<SignalId>> HandleAsync(CreateSignalCommand command)
```

### 21. API Response Envelopes
Use consistent response patterns:
```csharp
public class ApiResponse<T>
{
    public T Data { get; set; } = default!;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
```

## Testing

### 22. Test Naming
Use descriptive names: `MethodName_Scenario_ExpectedResult`:
```csharp
[Fact]
public void CreateSignal_WithValidContent_ShouldReturnSignalId()
```

## Performance

### 23. String Interpolation
Use interpolation over concatenation:
```csharp
var message = $"Signal {signalId} was created at {timestamp}";
```

### 24. Collection Types
Use appropriate types for the use case:
```csharp
public HashSet<string> UniqueTags { get; set; } = [];     // Unique items
public List<string> OrderedItems { get; set; } = [];      // Ordered items
public Dictionary<string, int> Counts { get; set; } = []; // Key-value pairs
```
