using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BindingChaos.IdentityProfile.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for creating IdentityProfileDbContext instances during EF migrations.
/// </summary>
public class IdentityProfileDbContextFactory : IDesignTimeDbContextFactory<IdentityProfileDbContext>
{
    /// <summary>
    /// Creates a new instance of IdentityProfileDbContext for design-time operations.
    /// </summary>
    /// <param name="args">Command line arguments passed to the EF tools.</param>
    /// <returns>A configured IdentityProfileDbContext instance.</returns>
    public IdentityProfileDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityProfileDbContext>();

        var connectionString = Environment.GetEnvironmentVariable("BINDINGCHAOS_DATABASE_CONNECTION_STRING")
            ?? "Host=localhost;Database=bindingchaos;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString, options =>
        {
            options.MigrationsAssembly(typeof(IdentityProfileDbContext).Assembly.FullName);
            options.MigrationsHistoryTable("__EFMigrationsHistory", "identity_profile");
        });

        return new IdentityProfileDbContext(optionsBuilder.Options);
    }
}


