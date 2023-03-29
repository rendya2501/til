using DBMigration.Consts;
using DBMigration.DatabaseConnection.ConnectionStrings;
using Microsoft.Extensions.Configuration;

namespace DBMigration.DatabaseConnection.Factories;

internal class DatabaseConnectionFactory
{
    private readonly IConfiguration _configuration;

    public DatabaseConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DatDatabaseConnection CreateDatDatabaseConnection()
    {
        int connectionTimeout = GetConnectionTimeout();

        return new DatDatabaseConnection
        {
            ConnectionString = new DatConnectionString($"{_configuration!.GetConnectionString(DatabaseConstants.DatPrefix)};Connection Timeout={connectionTimeout}"),
            ConnectionTimeout = connectionTimeout
        };
    }

    public SysDatabaseConnection CreateSysDatabaseConnection()
    {
        int connectionTimeout = GetConnectionTimeout();

        return new SysDatabaseConnection
        {
            ConnectionString = new SysConnectionString($"{_configuration!.GetConnectionString(DatabaseConstants.SysPrefix)};Connection Timeout={connectionTimeout}"),
            ConnectionTimeout = connectionTimeout
        };
    }

    private int GetConnectionTimeout()
    {
        return int.Parse(_configuration?.GetSection("ConnectionTimeout").Value ?? "15");
    }
}
