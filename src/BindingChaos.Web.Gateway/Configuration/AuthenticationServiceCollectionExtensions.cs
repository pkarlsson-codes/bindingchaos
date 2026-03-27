using BindingChaos.Web.Gateway.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace BindingChaos.Web.Gateway.Configuration;

/// <summary>
/// Authentication registration helpers for cookie + OpenID Connect.
/// </summary>
public static class AuthenticationServiceCollectionExtensions
{
    /// <summary>
    /// Registers cookie auth and OIDC (Keycloak) with gateway-specific defaults.
    /// </summary>
    /// <param name="services">The DI service collection.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCookieAndOidcAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = GatewayDefaults.Cookies.AuthenticationCookie;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
            })
            .AddOpenIdConnect(options =>
            {
                options.Authority = configuration["Authentication:OIDC:Authority"];
                options.ClientId = configuration["Authentication:OIDC:ClientId"];
                options.ClientSecret = configuration["Authentication:OIDC:ClientSecret"];
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.UsePkce = true;
                options.SaveTokens = false;
                options.CallbackPath = "/auth/callback";
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("offline_access");
                options.TokenValidationParameters.NameClaimType = "preferred_username";
                options.TokenValidationParameters.RoleClaimType = "roles";
                options.RequireHttpsMetadata = false;
                options.SignedOutCallbackPath = "/auth/signedout-callback";
                options.SignedOutRedirectUri = configuration["Authentication:OIDC:PostLogoutRedirect"]
                    ?? configuration["Authentication:OIDC:PostLoginRedirect"]
                    ?? "/";
            });

        services.AddSingleton<OidcEventsFactory>();
        services.AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
            .Configure<OidcEventsFactory>((options, factory) => options.Events = factory.Create());

        services.AddHttpClient();
        return services;
    }
}
