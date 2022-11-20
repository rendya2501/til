# bundle

## EFCore + Console

[EntityFrameworkCoreを.NET Core コンソールアプリでCodeFirstに使う](https://qiita.com/namoshika/items/7d1bf911bc03ed03e17d)  

---

## EFCore + bundle

[Introduction to Migration Bundles - What can they do the migration scripts don't?](https://www.youtube.com/watch?v=mBxSONeKbPk)  
[EF Core 6  - Apresentando Migration Bundles](https://macoratti.net/21/09/efc6_migbndl1.htm)  

## dotnet ef migrations bundle のエラー

バンドルはEF Core 6.0からの機能。  
5.0では当然エラーになるというわけです。  

``` txt
Build started...
Build succeeded.
Specify --help for a list of available options and commands.
Unrecognized command or argument 'bundle'.
```

[Unrecognized command or argument 'optimize' on Entity Framework Core .NET Command-line Tools 5.0.7](https://github.com/dotnet/efcore/issues/25135)  

---

## bundle作成のスタートアップ

dotnet ef migrations bundleでバンドルを作成する時のMainメソッドは何がOKなのか調査。  
DBContextクラスで直接、接続文字列を書くくらいなら、DIしたいとも思った。  

結論から言うと、公式の方法の通りにやれば良い。  

ASP.NET Core 2.2 アプリで dotnet ef コマンドを実行する場合は、 Program.cs に CreateWebHostBuilder メソッドが必要な模様。  

``` cs : ○ bundle作成できた
string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=BundleDB2;Integrated Security=True";

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddDbContext<DatContext>(options =>
            {
                var appsettings = hostContext.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(appsettings ?? connectionString);
            });
    })
    .Build();
// Runまでせずともbundleを作成する事ができた。  
// host.Run();
```

[デザイン時 DbContext 作成](https://learn.microsoft.com/ja-jp/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli)  
[dotnet ef migrations でエラーになった話](https://qiita.com/wukann/items/53462f4b21104ed75c31)  

中途半端にDIするだけだとダメだった結果がこちら。  

``` cs : × bundle作成出来ず
var services = new ServiceCollection();
services.AddDbContext<DatContext>(options => options.UseSqlServer(connectionString));
ServiceProvider serviceProvider = services.BuildServiceProvider();
_datContext = serviceProvider.GetService<DatContext>();
```

[Accessing dbContext in a C# console application](https://stackoverflow.com/questions/49972591/accessing-dbcontext-in-a-c-sharp-console-application)  
[How to Add Entity Framework Core DBContext in .NET Core Console Application](http://www.techtutorhub.com/article/How-to-Add-Entity-Framework-Core-DBContext-in-Dot-NET-Core-Console-Application/86)  

検索文字列 : dependency injection dbcontext console app  
