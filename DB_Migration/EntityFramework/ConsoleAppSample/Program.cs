using System.Reflection;
using ConsoleAppSample.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"));
            });
    })
    .Build();



const string menu = @"
------------------------
選択してください。
1. 移行実行
2. 適応状況一覧
3. 保持ファイル一覧
q. 処理終了
------------------------
";

try
{
    while (true)
    {
        Console.WriteLine(menu);

        switch (Console.ReadLine())
        {
            case "1":
                Console.WriteLine("未実装");
                break;
            case "2":
                var _datContext = host.Services.GetService<AppDbContext>();
                if (_datContext != null)
                {
                    var appliedMigratiosn = await _datContext.Database.GetAppliedMigrationsAsync();
                    foreach (var item in appliedMigratiosn)
                        Console.WriteLine(item);
                }
                break;
            case "3":
                // var appliedMigratiosn = Directory.EnumerateFiles(@$"{Directory.GetCurrentDirectory()}\Migrations", "*.cs")
                //     .Select(s => Path.GetFileNameWithoutExtension(s))
                //     .Where(w => !w.EndsWith("Designer") && !w.EndsWith("Snapshot"))
                //     .ToList();
                Assembly assembly = Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceNames();
                foreach (var item in stream)
                    Console.WriteLine(item);
                break;
            case "q":
            case "Q":
                return;
        }
    }
}
finally
{
    host.Dispose();
}
