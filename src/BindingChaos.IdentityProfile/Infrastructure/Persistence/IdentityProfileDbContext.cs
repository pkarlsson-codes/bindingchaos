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

    public DbSet<IdentityMap> IdentityMaps => Set<IdentityMap>();

    public DbSet<UserTrust> UserTrusts => Set<UserTrust>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("identity_profile");
        modelBuilder.ApplyConfiguration(new IdentityMapConfiguration());
        modelBuilder.ApplyConfiguration(new UserTrustConfiguration());
    }
}


