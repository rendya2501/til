using DBMigration.DatabaseConnection.ConnectionStrings;

namespace DBMigration.DatabaseConnection;

internal class DatDatabaseConnection : IDatabaseConnection
{
    public IConnectionString? ConnectionString { get; init; }
    public int ConnectionTimeout { get; init; }
}