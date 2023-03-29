namespace DBMigration.Executor;

internal enum ExecuteMode
{
    /// <summary>
    /// テストモード
    /// </summary>
    Test,
    /// <summary>
    /// テスト + IgnoreChangeモード
    /// </summary>
    TestIgnoreChange,
    /// <summary>
    /// 本番モード
    /// </summary>
    Production,
    /// <summary>
    /// 本番 + IgnoreChangeモード
    /// </summary>
    ProductionIgnoreChange,
}
