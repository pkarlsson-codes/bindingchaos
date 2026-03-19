# Base API Controller Usage Guide

Use this guide when implementing API controllers that return standard envelopes.

## Scope

- Includes: response wrapping, inheritance rules, route/action patterns.
- Excludes: authorization policy design, command/query design.

## How It Works

1. Controllers inherit `BaseApiController` (`src/BindingChaos.Infrastructure/API/BaseApiController.cs`).
2. `Ok(...)`, `Created(...)`, and `CreatedAtAction(...)` automatically wrap payloads with `ApiResponse.CreateSuccess(data)`.
3. Consumer-facing actions return wrapped success responses without manual envelope code.

## Implementation Steps

1. Inherit from `BaseApiController` for controllers returning domain/view payloads.
2. Keep standard attributes on concrete controllers (`[ApiController]`, `[Route(...)]`, endpoint attributes).
3. Return wrapped results via base helpers (`Ok`, `Created`, `CreatedAtAction`).
4. Use explicit non-success responses where needed (`Unauthorized`, `NotFound`, `BadRequest`, `Problem`).

## Usage Rules

- Do not manually construct success envelopes in actions already using base helpers.
- Keep transport logic in controller; move orchestration to handlers/services.
- Use `CreatedAtAction` for resource creation when route lookup is available.
- Add `[AllowAnonymous]` only for endpoints that must bypass fallback authorization.

## Definition Of Done

- Controller inherits `BaseApiController`.
- Success responses use base helper methods.
- Response types in OpenAPI annotations match wrapped responses.
- Error paths return clear HTTP statuses.

## Common Mistakes

- Inheriting `ControllerBase` directly and drifting from response envelope conventions.
- Mixing wrapped and unwrapped success payloads in the same controller.
- Returning 200 for creation instead of `Created` or `CreatedAtAction`.
- Forgetting to mark public/read endpoints as `[AllowAnonymous]` when required.
