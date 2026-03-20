# Base API Controller

Controllers returning domain/view payloads inherit `BaseApiController` ([src/BindingChaos.Infrastructure/API/BaseApiController.cs](../../src/BindingChaos.Infrastructure/API/BaseApiController.cs)).

The base overrides `Ok(...)`, `Created(...)`, and `CreatedAtAction(...)` to automatically wrap payloads in `ApiResponse.CreateSuccess(data)`. Keep `[ApiController]`, `[Route(...)]`, and endpoint attributes on the concrete controller.

For non-success responses use `Unauthorized`, `NotFound`, `BadRequest`, or `Problem` directly. Use `CreatedAtAction` for resource creation when a route lookup is available.
