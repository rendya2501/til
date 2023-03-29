using DBMigration.Consts;
using DBMigration.DatabaseConnection.Factories;
using DBMigration.JsonOperations;
using DBMigration.SqlScripts.Factories;
using DbUp;

namespace DBMigration.Commands;

/// <summary>
/// 接続を司るクラス
/// </summary>
internal class Connection : ConsoleAppBase
{
    /// <summary>
    /// 接続テストを行います。
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="Exception"></exception>
    [Command(nameof(Connection.ConnectionTest), "接続テストを行います。")]
    public void ConnectionTest([Option("", OptionDescription.PATH)] string path = "")
    {
        Console.WriteLine("+---------------------------------------------------------------------------------");
        Console.WriteLine("|接続テスト                                                                       ");
        Console.WriteLine("+---------------------------------------------------------------------------------");

        // Jsonパス生成
        foreach (var jsonFilePath in new JsonFileManager(path).JsonFilePathList)
        {
            // Jsonの内容を読み込んで各種情報を生成
            var configuration = new ConfigurationBuilderFactory(jsonFilePath).Configuration;
            var factory = new DatabaseConnectionFactory(configuration);
            var datConnectionString = factory.CreateDatDatabaseConnection().ConnectionString.Value;
            var sysConnectionString = factory.CreateSysDatabaseConnection().ConnectionString.Value;

            Console.WriteLine();
            Console.WriteLine($"接続テスト中... : {jsonFilePath.FullPath}");
            Console.WriteLine(datConnectionString);
            Console.WriteLine(sysConnectionString);
            Console.WriteLine();

            if (!DeployChanges.To.SqlDatabase(datConnectionString).WithScript(SqlScriptFactory.GetDummySqlScripts()).Build().TryConnect(out var datErrMsg))
                throw new Exception(datErrMsg);
            if (!DeployChanges.To.SqlDatabase(sysConnectionString).WithScript(SqlScriptFactory.GetDummySqlScripts()).Build().TryConnect(out var sysErrMsg))
                throw new Exception(sysErrMsg);
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("接続成功!!");
        Console.ResetColor();
    }
}
