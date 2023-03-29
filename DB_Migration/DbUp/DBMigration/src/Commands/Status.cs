using DBMigration.DatabaseConnection.ConnectionStrings;
using DBMigration.Consts;
using DBMigration.JsonOperations;
using DBMigration.SqlScripts.Factories;
using DbUp;
using Sharprompt;
using DBMigration.DatabaseConnection.Factories;

namespace DBMigration.Commands;

/// <summary>
/// 状態の確認を司るクラス
/// </summary>
[Command(nameof(Status), "状態に関するコマンドです。")]
internal class Status : ConsoleAppBase
{
    /// <summary>
    /// 保持しているファイル一覧と対象データベースとの差分を表示します。
    /// </summary>
    /// <param name="path"></param>
    [Command(nameof(Status.Diff), "保持しているマイグレーションファイルの一覧と対象データベースとの差分を表示します。")]
    public void Diff([Option("", OptionDescription.PATH)] string path = "")
    {
        Console.WriteLine("+---------------------------------------------------------------------------------");
        Console.WriteLine("|保持しているマイグレーションファイルの一覧と対象データベースとの差分表示");
        Console.WriteLine("|「○」適応済み、「×」未適応");
        Console.WriteLine("+---------------------------------------------------------------------------------");
        // 保持しているファイル一覧
        var sqlScripts = SqlScriptFactory.SqlScripts;
        // 保持しているファイル一覧の最大文字列長を取得する
        var maxLength = sqlScripts.Max(s => s.Name.Length);
        // 区切り文字列を生成する
        var bar = $"+{string.Concat(Enumerable.Repeat("-", maxLength))}+--+";

        foreach (var jsonFilePath in new JsonFileManager(path).JsonFilePathList)
        {
            // Jsonの内容を読み込んで各種情報を生成
            var configuration = new ConfigurationBuilderFactory(jsonFilePath).Configuration;
            var factory = new DatabaseConnectionFactory(configuration);
            var datConnectionString = factory.CreateDatDatabaseConnection().ConnectionString;
            var sysConnectionString = factory.CreateSysDatabaseConnection().ConnectionString;

            // dat一覧生成
            var datList = string.Join(
                Environment.NewLine,
                sqlScripts.Where(w => w.Name.StartsWith(DatabaseConstants.DatPrefix))
                    .GroupJoin(
                        GetExecutedScripts(datConnectionString),
                        src => src.Name,
                        dst => dst,
                        (src, dst) => new { src.Name, src.Contents, dst = dst.FirstOrDefault() }
                    )
                    .Select(s => $"|{s.Name}{string.Concat(Enumerable.Repeat(" ", maxLength - s.Name.Length))}|{(string.IsNullOrEmpty(s.dst) ? "×" : "○")}|")
            );
            // sys一覧生成
            var sysList = string.Join(
                Environment.NewLine,
                sqlScripts.Where(w => w.Name.StartsWith(DatabaseConstants.SysPrefix))
                    .GroupJoin(
                        GetExecutedScripts(sysConnectionString),
                        src => src.Name,
                        dst => dst,
                        (src, dst) => new { src.Name, src.Contents, dst = dst.FirstOrDefault() }
                    )
                    .Select(s => $"|{s.Name}{string.Concat(Enumerable.Repeat(" ", maxLength - s.Name.Length))}|{(string.IsNullOrEmpty(s.dst) ? "×" : "○")}|")
            );

            Console.WriteLine($"設定ファイル:{jsonFilePath.FullPath}");
            Console.WriteLine(bar);
            Console.WriteLine(datList);
            Console.WriteLine(bar);
            Console.WriteLine(sysList);
            Console.WriteLine(bar);
        }
    }


    /// <summary>
    /// 保持しているマイグレーションファイルの一覧を表示します。
    /// </summary>
    /// <param name="select"></param>
    /// <exception cref="ArgumentNullException"></exception>
    [Command(nameof(Status.Scripts), "保持しているマイグレーションファイルの一覧を表示します。")]
    public void Scripts([Option("", "選択したsqlファイルの内容を表示するようにします。")] bool select = false)
    {
        Console.WriteLine("+---------------------------------------------------------------------------------");
        Console.WriteLine("|保持しているマイグレーションファイル一覧                                         ");
        Console.WriteLine("+---------------------------------------------------------------------------------");

        var migrationFiles = SqlScriptFactory.SqlScripts;
        var migrationFileNames = migrationFiles.Select(s => s.Name).ToList();

        // selectモードでなければ一覧表示して処理終了
        if (!select)
        {
            Console.WriteLine(string.Join(Environment.NewLine, migrationFileNames));
            return;
        }

        while (true)
        {
            var selected = Prompt.Select(
                "確認したいスクリプトを選択してください。↑↓:選択,←→:ページ,End or ctrl+c:終了",
                migrationFileNames.Concat(new List<string>() { "End" }),
                pageSize: 10
            ) ?? "End";

            if (selected == "End")
                break;

            Console.WriteLine(
                migrationFiles.FirstOrDefault(f => f.Name == selected)?.Contents
                    ?? throw new ArgumentNullException("選択した情報が存在しません。")
            );
        }
    }


    /// <summary>
    /// 対象データベースに適応されているマイグレーションを取得します。
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    private static IEnumerable<string> GetExecutedScripts(IConnectionString connectionString)
    {
        var executedScripts = DeployChanges.To
            .SqlDatabase(connectionString.Value)
            .JournalToSqlTable("dbo", Journal.TableName)
            .WithScript(SqlScriptFactory.GetDummySqlScripts())
            .LogToNowhere()
            .Build()
            .GetExecutedScripts();
        return executedScripts;
    }
}
