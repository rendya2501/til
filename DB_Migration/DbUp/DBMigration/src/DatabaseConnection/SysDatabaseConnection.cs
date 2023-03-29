using DBMigration.DatabaseConnection.ConnectionStrings;

namespace DBMigration.DatabaseConnection;

internal class SysDatabaseConnection : IDatabaseConnection
{
    public IConnectionString? ConnectionString { get; init; }
    public int ConnectionTimeout { get; init; }
}