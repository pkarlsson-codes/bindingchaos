using BindingChaos.IdentityProfile.Domain.Entities;

namespace BindingChaos.IdentityProfile.Application.Services;

/// <summary>
/// Contract for resolving and managing identity mappings and trust state.
/// </summary>
public interface IIdentityProfileService
{
    Task<string> LinkOrGetUserIdAsync(string provider, string subject, CancellationToken cancellationToken = default);

    Task<IdentityMap?> GetIdentityMapAsync(string userId, CancellationToken cancellationToken = default);

    Task<UserTrust> SetPersonhoodAsync(string userId, bool verified, CancellationToken cancellationToken = default);

    Task<UserTrust> GetOrCreateTrustAsync(string userId, CancellationToken cancellationToken = default);
}


