using Microsoft.Extensions.Configuration;
using System.Reflection;

var app = ConsoleApp.Create(
    args,
    options =>
    {
        options.ApplicationName = Assembly.GetExecutingAssembly().GetName().Name; // FolderRead
        options.HelpSortCommandsByFullName = true;
        // --connection-String を --connectionString にするための設定
        options.NameConverter = new Func<string, string>(str => str);
    }
);

app.AddCommand(
    "Hoge",
    ([Option("f", "Message to display.")] string folderPath) =>
    {
        // 引数の対象フォルダのパスから、フォルダ内のjsonファイルの絶対パスを取得する。
        var filePath = Directory.GetFiles(folderPath, "*.json");
        // ファイルの数分ループ
        foreach (string path in filePath)
        {
            // 絶対パスが表示される
            Console.WriteLine(path);
            // ファイル名だけが表示される
            Console.WriteLine(Path.GetFileName(path));

            // ConfigurationBuilderを使ってjsonを読みだす。
            // パスとファイル名を指定してbuildする。
            var configuration = new ConfigurationBuilder()
                .SetBasePath(folderPath)
                .AddJsonFile(Path.GetFileName(path))
                .Build();
            // GetConnectionStringのDatとSysの情報を出力する
            Console.WriteLine($"Dat: {configuration.GetConnectionString("Dat")}");
            Console.WriteLine($"Sys: {configuration.GetConnectionString("Sys")}");
            Console.WriteLine();
        }
    }
);
app.Run();

