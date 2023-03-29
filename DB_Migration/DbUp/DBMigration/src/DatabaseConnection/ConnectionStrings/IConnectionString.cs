namespace DBMigration.DatabaseConnection.ConnectionStrings;

/// <summary>
/// 接続文字列インターフェース
/// </summary>
internal interface IConnectionString
{
    /// <summary>
    /// 接続文字列
    /// </summary>
    internal string Value { get; }
}
