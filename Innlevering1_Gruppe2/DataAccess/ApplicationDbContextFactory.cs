using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Reachout.DataAccess;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var directory = Directory.GetCurrentDirectory();
        if (File.Exists(Path.Combine(directory, "Reachout", "Reachout.csproj")))
        {
            directory = Path.Combine(directory, "Reachout");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(directory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Local.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        var connection = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connection))
        {
            throw new InvalidOperationException("Konfigurer DefaultConnection før du bruker databaseverktøyene.");
        }

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySql(connection, new MariaDbServerVersion(new Version(11, 8, 0)))
            .Options;
        return new ApplicationDbContext(options);
    }
}
