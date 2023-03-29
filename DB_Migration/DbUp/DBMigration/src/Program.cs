using DBMigration.Commands;
using System.Reflection;

var app = ConsoleApp.Create(
    args,
    options =>
    {
        // linuxでコマンド名が表示されないのでその対策。 表示される値は「Bundle」となる。
        options.ApplicationName = Assembly.GetExecutingAssembly().GetName().Name;
        // コマンドを名前順で並び替える
        options.HelpSortCommandsByFullName = true;
        // --connection-String を --connectionString にするための設定
        options.NameConverter = new Func<string, string>(str => str);
    }
);
app.AddSubCommands<Migration>();
app.AddSubCommands<Status>();
app.AddCommands<Connection>();
app.Run();
