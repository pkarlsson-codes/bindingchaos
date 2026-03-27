using BindingChaos.IdentityProfile.Domain.Entities;
using BindingChaos.IdentityProfile.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BindingChaos.IdentityProfile.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for the Identity Profile module.
/// </summary>
public sealed class IdentityProfileDbContext : DbContext
{
    public IdentityProfileDbContext(DbContextOptions<IdentityProfileDbContext> options) : base(options)
    {
    }

    /// <summary>Gets the identity maps linking external provider subjects to internal user IDs.</summary>
    public DbSet<IdentityMap> IdentityMaps => Set<IdentityMap>();

    /// <summary>Gets the participant records storing stable pseudonyms.</summary>
    public DbSet<Participant> Participants => Set<Participant>();

    /// <summary>Gets the invite links created by participants.</summary>
    public DbSet<TrustInviteLink> TrustTrustInviteLinks => Set<TrustInviteLink>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("identity_profile");
        modelBuilder.ApplyConfiguration(new IdentityMapConfiguration());
        modelBuilder.ApplyConfiguration(new ParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new TrustInviteLinkConfiguration());
    }
}
