# Error Handling Strategy

## Overview

All unhandled exceptions are caught by a chain of `IExceptionHandler` implementations registered in `CorePlatform.API`. Errors are returned as **RFC 7807 ProblemDetails** responses, enriched with a `correlationId` on every response and a `stackTrace` in development.

## Exception Hierarchy

Domain exception types live in `BindingChaos.SharedKernel`.

```
Exception
├── AggregateNotFoundException           → 404
└── DomainException (abstract)
    ├── BusinessRuleViolationException   → 422
    └── InvariantViolationException      → 422
```

`AggregateNotFoundException` is intentionally **not** a subclass of `DomainException`. It is an infrastructure concern (the repository could not locate a record), not a violated business rule.

## Handler Chain

Handlers are evaluated in registration order. The first one that returns `true` wins; the rest are skipped.

| Handler | Catches | Status |
|---|---|---|
| `AggregateNotFoundExceptionHandler` | `AggregateNotFoundException` | 404 Not Found |
| `DomainExceptionHandler` | Any remaining `DomainException` | 422 Unprocessable Entity |
| `UnhandledExceptionHandler` | Everything else | 500 Internal Server Error |

Only `UnhandledExceptionHandler` has a fixed position — it must be last.

## Response Shape

```json
{
  "title": "Business rule violation",
  "status": 422,
  "detail": "Originator cannot amplify their own signal.",
  "correlationId": "0HN8...",
  "stackTrace": "..."  // development only
}
```

## Adding New Exception Types

1. Define the exception in `BindingChaos.SharedKernel.Domain.Exceptions`, extending `DomainException`.
2. Create a new `IExceptionHandler` in `CorePlatform.API/Infrastructure/ExceptionHandling/`.
3. Register it in `AddCorePlatformExceptionHandling()` before `UnhandledExceptionHandler`.
