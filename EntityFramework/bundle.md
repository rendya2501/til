# bundle

## bundle概要

- 移行バンドルは、移行を実行するために必要なすべてのものを含む自己充足的な実行ファイル(exe)  
- 移行バンドルはコマンドラインインターフェースと同じアクションを実行する。  
- CLIはツールの依存関係（.NET SDK、モデルをコンパイルするためのソースコード、ツール自体）が本番サーバーにインストールされている必要がある。  
- バンドルは実行環境に影響を受けずにマイグレーションを実行することができる。  
- パラメータとして接続文字列を受け取る。  
- 主要なツール (Docker、SSH、PowerShell 等) で動作する。  

- 移行を実行するだけの機能なので、細かい制御はできない。  
- 実質的にdotnet ef database updateと同等の動作を提供する。  

>移行バンドルは、データベースに移行を適用するために使用できる単一ファイルの実行可能ファイルです。  
>これらは、次のような SQL スクリプトとコマンドライン ツールの欠点の一部に対処します。  
>
>- SQL スクリプトを実行するには、追加のツールが必要です。  
>- これらのツールによるトランザクション処理とエラー時の続行動作には一貫性がなく、予期できない場合もあります。  
>  そのため、移行の適用時にエラーが発生した場合に、データベースが未定義の状態になる可能性があります。  
>- バンドルは、CI プロセスの一部として生成することができ、後で配置プロセスの一部として簡単に実行することができます。  
>- バンドルは、.NET SDK または EF ツールを (または、自己完結型の場合は .NET ランタイムさえも) インストールせずに実行でき、プロジェクトのソース コードは必要ありません。  
>[移行の適用](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli#bundles)  

<!--  -->
>生成される実行可能ファイルは、既定では efbundle という名前になります。  
>これは、データベースを最新の移行に更新するために使用できます。  
>これは、dotnet ef database update または Update-Database を実行するのと同等です。  
>[移行の適用](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli#efbundle)

---

## bundle作成手順

1. dotnet-efのインストール  
2. コンソールプロジェクトでマイグレーションファイルを作成  
3. `dotnet ef migrations bundle --configuration Bundle`コマンドを実行  

バンドル作成時はVSCodeおよび、VisualStudioを閉じて実行すること。  
dllへの参照がある状態ではバンドルは作成できない模様。  

[Entity Framework Core ツールのリファレンス - .NET Core CLI](https://learn.microsoft.com/ja-jp/ef/core/cli/dotnet)  

---

## bundleの使い方

ダブルクリックで直接実行 or CUIからオプションを指定して実行  

直接実行で接続情報内部埋め込み式ではない場合、appsetting.jsonの接続情報を元に移行だけを実行。  
appsettings.jsonが存在しない場合はエラーとなり、移行は実行されない。  

接続情報内部埋め込み式の場合、そのまま移行が実行される。  

CUIからの場合はオプションの指定が可能。  

`efbundle --connection "Server=.\SQLEXPRESS;Database=<db_name>;User ID=<user_id>;Password=<passwd>;`  

ヘルプ :  
`efbandle --help`  

[移行の適用](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli)  

---

## bundleでロールバック

実質的なDownコマンドは存在しない。  
マイグレーションファイルを指定することで、その時点そこまで戻すことができる。  

例:  
`efbandle 20221110062826_Second`

---

## EFCore + bundle

[Introduction to Migration Bundles - What can they do the migration scripts don't?](https://www.youtube.com/watch?v=mBxSONeKbPk)  
[EF Core 6  - Apresentando Migration Bundles](https://macoratti.net/21/09/efc6_migbndl1.htm)  
[EF Core 6.0: Introducing Migration Bundles](https://jaliyaudagedara.blogspot.com/2021/08/ef-core-60-introducing-migration-bundles.html?spref=tw)  

---

## bundle作成のスタートアップ

dotnet ef migrations bundleでバンドルを作成する時のMainメソッドは何がOKなのか調査。  
DBContextクラスで直接、接続文字列を書くくらいなら、DIしたいとも思った。  

結論から言うと、公式の方法の通りにやれば良い。  

ASP.NET Core 2.2 アプリで dotnet ef コマンドを実行する場合は、 Program.cs に CreateWebHostBuilder メソッドが必要な模様。  

``` cs : ○ bundle作成できた
string connectionString = @"Server=.\SQLEXPRESS;Database=BundleDB2;Integrated Security=True";

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

---

## トラブルシューティング

### dotnet ef migrations bundle のエラー

バンドルを作成しようとした時にエラーが発生。  
バンドルはEF Core 6.0からの機能。  
5.0では当然エラーになるというわけです。  

``` txt
Build started...
Build succeeded.
Specify --help for a list of available options and commands.
Unrecognized command or argument 'bundle'.
```

[Unrecognized command or argument 'optimize' on Entity Framework Core .NET Command-line Tools 5.0.7](https://github.com/dotnet/efcore/issues/25135)  
