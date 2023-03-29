using DBMigration.Consts;
using DBMigration.DatabaseConnection;
using DBMigration.DatabaseConnection.Factories;
using DBMigration.Executor;
using DBMigration.JsonOperations;
using Sharprompt;

namespace DBMigration.Commands;

/// <summary>
/// 移行を司るクラス
/// </summary>
[Command(nameof(Migration), "移行に関するコマンドです。")]
internal class Migration : ConsoleAppBase
{
    const string IGNORE_CHANGE = "FirstMigrationを実行せず、履歴にだけ残すオプションです。";
    const string NO_CONFIRM = "事前テスト成功後の実行確認を行いません。";


    /// <summary>
    ///  本番移行を実行します。
    /// </summary>
    /// <param name="ignoreChange"></param>
    /// <param name="noConfirm"></param>
    /// <param name="path"></param>
    [Command(nameof(Migration.Production), "本番移行を実行します。")]
    public void Production(
        [Option("i", IGNORE_CHANGE)] bool ignoreChange = false,
        [Option("", NO_CONFIRM)] bool noConfirm = false,
        [Option("", OptionDescription.PATH)] string path = "")
    {
        // 実行できる状態まで初期化
        var executor = InitializeExecutor(path);

        Console.WriteLine("+---------------------------------------------------------------------------------");
        Console.WriteLine("|事前テスト実行中...                                                              ");
        Console.WriteLine("+---------------------------------------------------------------------------------");
        executor.Execute(ignoreChange ? ExecuteMode.TestIgnoreChange : ExecuteMode.Test);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("事前テスト成功!!");
        Console.ResetColor();


        if (!noConfirm && !Prompt.Confirm("実行します。よろしいですか?"))
        {
            Console.WriteLine("中止しました。");
            return;
        }


        Console.WriteLine("+---------------------------------------------------------------------------------");
        Console.WriteLine("|本番移行実行中...                                                                ");
        Console.WriteLine("+---------------------------------------------------------------------------------");
        executor.Execute(ignoreChange ? ExecuteMode.ProductionIgnoreChange : ExecuteMode.Production);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("本番移行成功!!");
        Console.ResetColor();
    }


    /// <summary>
    /// テスト移行を実行します。
    /// </summary>
    /// <param name="ignoreChange"></param>
    /// <param name="path"></param>
    [Command(nameof(Migration.Test), "テスト移行を実行します。")]
    public void Test(
        [Option("i", IGNORE_CHANGE)] bool ignoreChange = false,
        [Option("", OptionDescription.PATH)] string path = "")
    {
        // 実行できる状態まで初期化
        var executor = InitializeExecutor(path);

        Console.WriteLine("+---------------------------------------------------------------------------------");
        Console.WriteLine("|移行テスト実行中...                                                              ");
        Console.WriteLine("+---------------------------------------------------------------------------------");
        executor.Execute(ignoreChange ? ExecuteMode.TestIgnoreChange : ExecuteMode.Test);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("移行テスト成功!!");
        Console.ResetColor();
    }



    /// <summary>
    /// マイグレーション実行インスタンス初期化処理
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static MigrationExecutor InitializeExecutor(string path)
    {
        // パスを生成
        var files = new JsonFileManager(path).JsonFilePathList;
        // Jsonの内容を読み込んで各種情報を生成
        var databaseConnections = files.Select(s =>
        {
            var fac = new DatabaseConnectionFactory(new ConfigurationBuilderFactory(s).Configuration);
            return ((IDatabaseConnection)fac.CreateDatDatabaseConnection(), (IDatabaseConnection)fac.CreateSysDatabaseConnection());
        });
        // 実行インスタンス生成ファクトリー
        var factory = new MigratorFactory(databaseConnections);
        // マイグレーション実行インスタンス
        return new MigrationExecutor(factory);
    }
}
