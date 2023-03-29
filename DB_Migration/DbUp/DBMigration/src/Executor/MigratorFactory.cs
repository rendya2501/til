using DBMigration.Consts;
using DBMigration.DatabaseConnection;
using DBMigration.SqlScripts;
using DBMigration.SqlScripts.Base;
using DbUp;
using DbUp.Engine;

namespace DBMigration.Executor;

/// <summary>
/// マイグレーション実行インスタンスを作成するためのファクトリークラス。
/// 実行モードとJSONコンテンツに応じて、適切なマイグレーション実行インスタンスを生成します。
/// </summary>
internal class MigratorFactory
{
    public IEnumerable<(IDatabaseConnection Dat, IDatabaseConnection Sys)> DatabaseConnections { get; }

    internal MigratorFactory(IEnumerable<(IDatabaseConnection Dat, IDatabaseConnection Sys)> databaseConnections)
    {
        DatabaseConnections = databaseConnections;
    }

    /// <summary>
    /// 戦略に応じたマイグレーション実行インスタンス(UpgradeEngine)を生成します
    /// </summary>
    internal IEnumerable<(UpgradeEngine Dat, UpgradeEngine Sys)> Create(ExecuteMode executeMode)
    {
        return DatabaseConnections.Select(s =>
            (
                CreateUpgradeEngine(s.Dat, new DatSqlScriptManager()),
                CreateUpgradeEngine(s.Sys, new SysSqlScriptManager())
            )
        );

        UpgradeEngine CreateUpgradeEngine(IDatabaseConnection databaseSettings, SqlScriptManager sqlScriptManager)
        {
            var upgradeConfig = DeployChanges.To
                .SqlDatabase(databaseSettings.ConnectionString.Value)
                .JournalToSqlTable("dbo", Journal.TableName)
                .LogToConsole()
                .WithExecutionTimeout(TimeSpan.FromSeconds(databaseSettings.ConnectionTimeout));

            switch (executeMode)
            {
                case ExecuteMode.Production:
                    upgradeConfig.WithScripts(sqlScriptManager.GetSqlScripts());
                    upgradeConfig.WithTransaction();
                    break;
                case ExecuteMode.ProductionIgnoreChange:
                    upgradeConfig.WithScripts(sqlScriptManager.GetSqlScriptsIgnoreChange());
                    upgradeConfig.WithTransaction();
                    break;
                case ExecuteMode.Test:
                    upgradeConfig.WithScripts(sqlScriptManager.GetSqlScripts());
                    upgradeConfig.WithTransactionAlwaysRollback();
                    break;
                case ExecuteMode.TestIgnoreChange:
                    upgradeConfig.WithScripts(sqlScriptManager.GetSqlScriptsIgnoreChange());
                    upgradeConfig.WithTransactionAlwaysRollback();
                    break;
                default:
                    throw new NotSupportedException($"Unknown strategy type: {executeMode}");
            }

            return upgradeConfig.Build();
        }
    }
}
