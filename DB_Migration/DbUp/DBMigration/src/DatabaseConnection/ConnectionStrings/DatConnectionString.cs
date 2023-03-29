namespace DBMigration.DatabaseConnection.ConnectionStrings;

/// <summary>
/// Datの接続文字列を表す値オブジェクト
/// </summary>
internal class DatConnectionString : IConnectionString
{
    /// <summary>
    /// Dat接続文字列
    /// </summary>
    public string Value { get; }

    internal DatConnectionString(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("Datの接続文字列を設定してください。");

        Value = connectionString;
    }
}
