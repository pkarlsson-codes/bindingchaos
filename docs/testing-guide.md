# Unit Testing Guide

## File and Folder Layout

Mirror the production structure. One test file per production class:

```
Domain/
  SignalTests.cs
Application/Commands/
  CreateSignalHandlerTests.cs
Application/Queries/
  GetSignalHandlerTests.cs
```

## Outer Class and TestBed

The outer class holds a shared `TestBed` inner class that constructs all dependencies and exposes the `Sut`. Each method group is a nested class that instantiates its own `testBed`.

```csharp
public class CreateSignalHandlerTests
{
    public class TestBed
    {
        public Mock<ISignalRepository> Repository { get; } = new();
        public CreateSignalHandler Sut { get; }

        public TestBed()
        {
            Sut = new CreateSignalHandler(Repository.Object);
        }
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenSignalIsSaved()
        {
            var command = new CreateSignalCommand(...);

            await testBed.Sut.Handle(command);

            testBed.Repository.Verify(r => r.Save(It.IsAny<Signal>()), Times.Once);
        }
    }
}
```

## Naming Conventions

- **Nested classes**: `TheHandleMethod`, `TheAmplifyMethod`, `TheConstructor`
- **Test methods**: `Given<context>_When<action>_Then<expectation>` — underscores only between the three parts, not within them
- **System under test**: always named `sut` when referenced directly in a test body, or `Sut` when exposed on a TestBed

## Domain / Aggregate Tests

Aggregates are pure — no TestBed needed, just construct and call:

```csharp
public class SignalTests
{
    public class TheAmplifyMethod
    {
        [Fact]
        public void GivenOpenSignal_WhenAmplified_ThenRaisesAmplifiedEvent()
        {
            var sut = Signal.Create(...);

            sut.Amplify(participantId);

            sut.UncommittedEvents.Should().ContainSingle(e => e is SignalAmplified);
        }

        [Fact]
        public void GivenClosedSignal_WhenAmplified_ThenThrows()
        {
            var sut = Signal.CreateClosed(...);

            var act = () => sut.Amplify(participantId);

            act.Should().Throw<DomainException>();
        }
    }
}
```

## What to Test

- **Aggregate behavior** — what domain events are emitted, what exceptions are thrown
- **Value object invariants** — valid/invalid construction, equality semantics
- **Handler decisions** — branching logic, what gets passed to dependencies
- **Failure cases explicitly** — every domain rule should have a test for the case where it's violated

## What Not to Test

- Infrastructure wiring (Marten/EF config, DI registrations, Wolverine routing)
- Trivial constructors that only assign properties with no logic
- Handlers with no logic (just `Get` then `Save` with no decisions — write an integration test instead)
- Framework behavior — trust Wolverine, EF, and Marten to work

**The rule:** if the test would still pass after deleting the production logic, it is not testing behavior.

## Other Rules

- **FluentAssertions everywhere** — no raw `Assert.Equal`. Prefer `.ContainSingle(...)` or `.BeEquivalentTo(...)` over checking `.Count` separately.
- **No section comments** — use blank lines to separate arrange, act, and assert. Do not write `// Arrange`, `// Act`, or `// Assert`.
- **One behavior per test** — multiple `.Should()` calls are fine when they describe the same outcome. Use separate tests for separate behaviors.
- **No shared mutable state** — each nested class gets its own `new TestBed()`. Never share a `testBed` instance across method group classes.
- **Setup helpers stay local** — helper methods that configure mocks or build inputs belong inside the nested class that uses them, not on the outer class or TestBed.
- **No mocking persistence** — mock repository interfaces freely, but never substitute a real Marten or EF store with an in-memory fake. If a test needs real persistence, it is an integration test.
