using DBMigration.DatabaseConnection.ConnectionStrings;

namespace DBMigration.DatabaseConnection;

internal interface IDatabaseConnection
{
    internal IConnectionString? ConnectionString { get; init; }
    internal int ConnectionTimeout { get; init; }
}