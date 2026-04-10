using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BindingChaos.Reputation.Infrastructure.Persistence;

/// <summary>
/// Design-time factory used by EF Core tooling (migrations) when no startup project is provided.
/// </summary>
internal sealed class CompetenceDbContextFactory : IDesignTimeDbContextFactory<CompetenceDbContext>
{
    /// <inheritdoc />
    public CompetenceDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Database")
            ?? "Host=localhost;Port=5432;Database=bindingchaos;Username=bindingchaos;Password=bindingchaos";

        var options = new DbContextOptionsBuilder<CompetenceDbContext>()
            .UseNpgsql(connectionString, b => b.MigrationsAssembly(typeof(CompetenceDbContext).Assembly.FullName))
            .Options;

        return new CompetenceDbContext(options);
    }
}
