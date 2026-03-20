# Authorization Flow

## End-to-End Flow

1. Browser calls `GET /auth/login` on Gateway.
2. Gateway challenges OIDC (`AddCookieAndOidcAuth`), using Authorization Code + PKCE.
3. On token validation (`OidcEventsFactory`):
   - resolve IdP subject (`sub`),
   - link/create internal user via CorePlatform `api/identity/users/link`,
   - store provider tokens in `ITokenStore` (Redis),
   - issue cookies: `bc_session` (HttpOnly) and `bc_csrf` (readable).
4. Browser calls Gateway APIs with session cookie.
5. For mutating requests, `UseCsrfProtection` requires:
   - allowed origin/referer,
   - `X-CSRF-Token` matching `bc_csrf` cookie.
6. Gateway API clients call CorePlatform through `InternalGatewayAuthHandler`.
7. Handler resolves participant id from claims or token store and mints short-lived internal JWT (`InternalJwtService`).
8. CorePlatform validates JWT (`AddJwtBearer`) and applies fallback authorization policy (authenticated by default).
9. Controllers read participant from `participant_id` via `GetParticipantIdOrAnonymous()`; endpoints explicitly marked `[AllowAnonymous]` are public.

## Key Components

- Gateway auth registration: `src/BindingChaos.Web.Gateway/Configuration/AuthenticationServiceCollectionExtensions.cs`
- Gateway pipeline order: `src/BindingChaos.Web.Gateway/Configuration/Extensions/WebApplicationExtensions.cs`
- CSRF middleware: `src/BindingChaos.Web.Gateway/Middleware/CsrfProtectionMiddleware.cs`
- Token store: `src/BindingChaos.Web.Gateway/Services/ITokenStore.cs`, `src/BindingChaos.Web.Gateway/Services/RedisTokenStore.cs`
- Internal JWT forwarding: `src/BindingChaos.Web.Gateway/Services/InternalGatewayAuthHandler.cs`, `src/BindingChaos.Web.Gateway/Services/InternalJwtService.cs`
- CorePlatform auth/authorization: `src/BindingChaos.CorePlatform.API/Infrastructure/DependencyInjection.cs`
