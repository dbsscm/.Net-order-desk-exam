using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace OrderDesk.Data;

public static class DatabaseSetup
{
    public static IServiceCollection AddAppDatabase(
        this IServiceCollection services, IConfiguration configuration, string contentRootPath)
    {
        var provider = configuration["Database:Provider"] ?? "Sqlite";
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is not set in appsettings.json.");

        if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            return services;
        }

        // Resolve the SQLite file relative to the project folder, not the current directory.
        var sqlite = new SqliteConnectionStringBuilder(connectionString);
        sqlite.DataSource = Path.GetFullPath(Path.Combine(contentRootPath, sqlite.DataSource));
        Directory.CreateDirectory(Path.GetDirectoryName(sqlite.DataSource)!);

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(sqlite.ToString()));

        return services;
    }
}
