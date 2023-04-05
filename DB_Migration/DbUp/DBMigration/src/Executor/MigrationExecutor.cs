using DbUp.Engine;

namespace DBMigration.Executor;

/// <summary>
/// マイグレーション実行のエントリーポイント。
/// 指定された実行モードに応じて、適切なマイグレーション戦略を選択して実行します。
/// </summary>
internal class MigrationExecutor
{
    /// <summary>
    /// マイグレーション実行クラスのインスタンスを生成するファクトリー
    /// </summary>
    private readonly MigratorFactory _MigrationStrategy;

    public MigrationExecutor(MigratorFactory migrationStrategy)
    {
        _MigrationStrategy = migrationStrategy;
    }

    internal void Execute(ExecuteMode executeMode)
    {
        foreach (var (Dat, Sys) in _MigrationStrategy.Create(executeMode))
        {
            ThrowWhenFailed(Dat.PerformUpgrade());
            ThrowWhenFailed(Sys.PerformUpgrade());
        }
    }

    /// <summary>
    /// 結果が失敗の場合、スローします。
    /// </summary>
    /// <param name="result"></param>
    /// <exception cref="Exception"></exception>
    private static void ThrowWhenFailed(DatabaseUpgradeResult result)
    {
        if (!result.Successful)
            throw new Exception(
                result?.ErrorScript?.Name +
                Environment.NewLine +
                // あまりに長いとコンソールのエラーが見切れてしまうため、先頭100行までを表示する
                string.Join(Environment.NewLine, result?.ErrorScript?.Contents?.Split(Environment.NewLine, StringSplitOptions.None)?.Take(100) ?? Enumerable.Empty<string>())
            );
    }
}
