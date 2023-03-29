namespace DBMigration.DatabaseConnection.ConnectionStrings;

/// <summary>
/// Sysの接続文字列を表す値オブジェクト
/// </summary>
internal class SysConnectionString : IConnectionString
{
    /// <summary>
    /// Sys接続文字列
    /// </summary>
    public string Value { get; }

    internal SysConnectionString(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("Sysの接続文字列を設定してください。");

        Value = connectionString;
    }
}
